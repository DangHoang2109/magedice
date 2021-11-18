using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class BagInfoDialog : BaseSortingDialog
{
    [Header("Bag")]
    public BagInfoPanel bagInfo;

    [Header("Anim - time")]
    public Animator animClock; 
    public TextMeshProUGUI tmpTime;


    [Header("Open")]
    public GameObject goBstOpen;
    public IBooster bstOpen;
    public Transform tranOpen;
    public Transform tranStart;

    [Header("Reduce")]
    public Transform tranReduce;
    public TextMeshProUGUI tmpTimeReduce;

    [Header("Another being openned")]
    public Transform tranAnother;


    private BagSlotData bagSlotData;
    private BagConfig bagConfig;
    private GiftBagPerTourConfig giftBagConfig;

    private bool isWaitting;
    private double timeRemain;

    public event System.Action OnButtonStartUnlockClicked;
    public event System.Action<bool> OnButtonOpenNowClicked;

    public BagSlotState StateBag
    {
        get
        {
            if (this.bagSlotData != null)
            {
                return this.bagSlotData.state;
            }

            return BagSlotState.NONE;
        }
    }

    public void ParseData(BagSlotData data)
    {
        this.bagSlotData = data;
        if (this.bagSlotData != null)
        {
            this.bagConfig = BagConfigs.Instance.GetBag(data.type);
            this.giftBagConfig = GiftBagConfigs.Instance.GetGiftBag(data.type, data.tourID);
                //
            if (this.giftBagConfig != null)
            {
                this.bagInfo.ParseBag(this.bagSlotData.type, data.tourID, this.bagSlotData.name);

                this.tranStart.gameObject.SetActive(false);
                this.tranReduce.gameObject.SetActive(false);
                this.tranAnother.gameObject.SetActive(false);
                this.tranOpen.localPosition = new Vector3(-130, 0);
                this.goBstOpen.SetActive(true);
                this.bstOpen.ParseBooster(data.bstUnlock);


                //check có thằng nào đang openned ngoài nó không
                if (BagSlotDatas.Instance.IsAnotherBeingOpenned(this.bagSlotData.id))
                {
                    this.tranAnother.gameObject.SetActive(true);

                    if (data.name.Equals("TUTORIAL"))
                        this.tmpTime.SetText(GameUtils.ConvertFloatToTime(data.TOTAL_TIME_WAIT, GetTimeFormat(data.TOTAL_TIME_WAIT)));
                    else
                        this.tmpTime.SetText(GameUtils.ConvertFloatToTime(bagConfig.totalTimeWait, GetTimeFormat(bagConfig.totalTimeWait)));
                }
                else
                {
                    switch (data.state)
                    {
                        case BagSlotState.EXIST:
                            this.tranStart.gameObject.SetActive(true);

                            if(data.name.Equals("TUTORIAL"))
                                this.tmpTime.SetText(GameUtils.ConvertFloatToTime(data.TOTAL_TIME_WAIT, GetTimeFormat(data.TOTAL_TIME_WAIT)));
                            else
                                this.tmpTime.SetText(GameUtils.ConvertFloatToTime(bagConfig.totalTimeWait, GetTimeFormat(bagConfig.totalTimeWait)));

                            this.animClock.SetBool("Stop", true);
                            break;
                        case BagSlotState.WAITTING:
                            this.tranReduce.gameObject.SetActive(true);
                            this.isWaitting = true;
                            this.animClock.SetBool("Stop", false);
                            this.tmpTimeReduce.SetText(string.Format("- {0}", GameUtils.ConvertFloatToTime(bagConfig.timeReduce, "mm'Min'")));
                            break;
                        case BagSlotState.READY_OPEN:
                            this.tranOpen.localPosition = Vector3.zero;
                            this.goBstOpen.SetActive(false); //tắt booster open
                            this.animClock.SetBool("Stop", true);
                            break;
                        default:
                            break;
                    }
                }

                LogGameAnalytics.Instance.LogEvent(LogAnalyticsEvent.CLICK_BAG_INFO, LogParams.BAG_TYPE, data.type.ToString());
            }

        }
    }


    private string GetTimeFormat(double time)
    {
        if (time <= 60)
            return "mm'm'ss's'";
        else
            return "hh'h'mm'm'";
    }

    private void Update()
    {
        if (this.bagSlotData != null)
        {
            if (this.isWaitting)
            {
                if (!this.bagSlotData.IsReadyToOpen(ref timeRemain))
                {
                    //Debug.LogError(this.data.TOTAL_TIME_WAIT - this.timeRemain);
                    double remain = this.bagSlotData.TOTAL_TIME_WAIT - this.timeRemain;
                    this.tmpTime.SetText(GameUtils.ConvertFloatToTime(remain, GetTimeFormat(remain)));
                    this.bstOpen.ParseBooster(this.bagSlotData.bstUnlock);
                }
                else
                {
                    this.isWaitting = false;
                    ParseData(this.bagSlotData);
                }
            }
        }
    }

    public void OnClickOpenNow()
    {
        LogGameAnalytics.Instance.LogEvent(LogAnalyticsEvent.CLICK_SPEEDUP_BAGINFO, LogParams.BAG_TYPE, this.bagSlotData.type.ToString());

        //TODO use booster => open now
        if (UserProfile.Instance.UseBooster(this.bagSlotData.bstUnlock,string.Format("Bag_{0}", this.bagSlotData.type.ToString()),LogSinkWhere.OPEN_BAG))
        {
            Debug.LogError("Open bag: " + this.bagSlotData.tourID);
            MainBagSlots.OpenBagNow(this.bagSlotData.type, this.bagSlotData.tourID, "BagSlot");
            this.bagSlotData.OpenBag();
            this.OnCloseDialog();
            
            this.OnButtonOpenNowClicked?.Invoke(true);

            LogGameAnalytics.Instance.LogEvent(LogAnalyticsEvent.COMPLETE_SPEEDUP_BAGINFO, LogParams.BAG_TYPE, this.bagSlotData.type.ToString());

        }
        else
        {
            NeedMoreGemDialog dialog =
                GameManager.Instance.OnShowDialogWithSorting<NeedMoreGemDialog>("Home/GUI/Dialogs/NeedMoreGem/NeedMoreGemDialog",
                    PopupSortingType.CenterBottomAndTopBar);
            dialog?.ParseData(this.bagSlotData.bstUnlock);
            Debug.LogError("Not enought cash, go to shop");
            
            this.OnButtonOpenNowClicked?.Invoke(false);
        }
        SoundManager.Instance.PlayButtonClick();
    }

    public void OnClickStartOpen()
    {
        //TODO start waitting
        ParseData(this.bagSlotData.ChangeState(BagSlotState.WAITTING));
        ReloadSlotInMain();
        SoundManager.Instance.PlayButtonClick();

        this.OnButtonStartUnlockClicked?.Invoke();
    }

    public void OnClickReduceTime()
    {
        //TODO show ads => reduce time
        AdsManager.Instance.ShowVideoReward(LogAdsVideoWhere.REDUCE_TIME_BAG, DoneWatchAdsReduceTime);
        SoundManager.Instance.PlayButtonClick();
    }

    private void DoneWatchAdsReduceTime(bool success)
    {
        if (success)
        {
            if (this.bagConfig != null) ParseData(this.bagSlotData.ReduceTime(this.bagConfig.timeReduce));
            ReloadSlotInMain();
        }
    }

    public void OnClickAnotherBeingOpenned()
    {
        Notification.Instance.ShowNotificationIcon(LanguageManager.GetString("BAGINFO_ANOTHEROPENING", LanguageCategory.Feature));
        SoundManager.Instance.PlayButtonClick();
    }

    public override void OnCloseDialog()
    {
        base.OnCloseDialog();
        ReloadSlotInMain();
    }

    private void ReloadSlotInMain()
    {
        if (this.bagSlotData != null) MainBagSlots.Instance.ReloadSlot(this.bagSlotData.id);
    }
}
