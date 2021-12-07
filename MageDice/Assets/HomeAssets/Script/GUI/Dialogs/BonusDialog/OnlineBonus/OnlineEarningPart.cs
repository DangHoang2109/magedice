using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;
[System.Serializable]
public class ProgressBarDot
{
    public DotOnlineItem[] dots;

    public void ParseData(List<OnlineRewardConfigItem> config)
    {
        for (int i = 0; i < dots.Length; i++)
        {
            if (config[i].IsRewardBag)
                dots[i].Reward.ShowBag(config[i].Bag, "");
            else
                dots[i].Reward.ShowBooster(config[i].Booster, true);

        }
    }
    public void DoDot(int currentIndex)
    {
        Sequence seq = DOTween.Sequence();
        for (int i = 0; i < dots.Length; i++)
        {
            dots[i].ActiveIndex(i == currentIndex);

            if (i < currentIndex)
                seq.Append(dots[i].DoDot(i == currentIndex - 1));
        }
    }
}
public class OnlineEarningPart : MonoBehaviour
{
    [Header("Lock")]
    public TMPro.TextMeshProUGUI tmpTimeWait;

    [Header("Reward")]
    public ProgressBarDot progress;

    public GameObject gBtnClaim;

    public GameObject gProgressDot;
    public GameObject gPrizeOutRange;


    [Header("Reward")]
   
    UserOnlineBonusData dataInstance;
    double timeRemain;
    bool isActive;
    UserOnlineBonusData DataInstance
    {
        get
        {
            if (dataInstance == null)
                dataInstance = UserOnlineBonusData.Instance;

            return dataInstance;
        }
    }
    protected void OnEnable()
    {
        OnShow();

        DataInstance.callbackChangeUserMode += ParsePrize;
    }
    protected void OnDisable()
    {
        DataInstance.callbackChangeUserMode -= ParsePrize;
    }
    public void OnShow()
    {
        progress.ParseData(DataInstance.CurrentModeConfig.config);
        ParseNextPrize();

    }
    private void Update()
    {
        CustomUpdate();
    }
    protected void CustomUpdate()
    {
        this.isActive = !DataInstance.IsClaimAllReward && DataInstance.IsWaitEnough(ref this.timeRemain);

        if (this.timeRemain <= 0)
        {
            this.timeRemain = 0;
            ActiveBottom();
        }
        else
        {
            if (!DataInstance.IsClaimAllReward)
                this.tmpTimeWait.SetText($"Online {GameUtils.ConvertFloatToTime((float)this.timeRemain)} to claim");
        }

    }

    private void ActiveBottom()
    {
        this.tmpTimeWait.gameObject.SetActive(!isActive);
        this.gBtnClaim.gameObject.SetActive(isActive);
    }
    public void OnClickButtonClaim()
    {
        if (!isActive)
        {
            if (DataInstance.IsClaimAllReward)
                Notification.Instance.ShowNotificationIcon($"Come back tomorrow to claim bigger prizes");
            else
                Notification.Instance.ShowNotificationIcon($"Online {GameUtils.ConvertFloatToTime((float)this.timeRemain)} to claim");
        }
        else
        {
            OnlineRewardConfigItem CurrentRewardData = DataInstance.CurrentRewardConfig;
            if (CurrentRewardData != null)
            {
                if (CurrentRewardData.IsRewardBag)
                    MainBagSlots.OpenBagNow(CurrentRewardData.Bag, "OnlineBonusIcon");
                else
                {
                    UserProfile.Instance.AddBooster(CurrentRewardData.Booster, "OnlineBonusIcon", "OnlineBonusIcon");
                    FxHelper.Instance.ShowFxCollectBooster(CurrentRewardData.Booster, this.transform);
                }
            }

            DataInstance.OnClaimReward();
            ParseNextPrize();
        }
    }

    private void ParseNextPrize()
    {
        this.isActive = !DataInstance.IsClaimAllReward && DataInstance.IsWaitEnough(ref this.timeRemain);
        ParsePrize();
    }

    private void ParsePrize()
    {
        if (DataInstance.IsClaimAllReward)
        {
            gProgressDot.SetActive(false);
            gPrizeOutRange.SetActive(true);

            ActiveBottom();
            this.tmpTimeWait.SetText($"Come Back Tomorrow to Claim Huge Prizes");
        }
        else
        {
            ActiveBottom();
            gPrizeOutRange.SetActive(false);

            OnlineRewardConfigItem CurrentRewardData = DataInstance.CurrentRewardConfig;
            if (CurrentRewardData != null)
            {
                this.progress.DoDot(DataInstance._nextRewardIndex);
            }

        }

    }
}
