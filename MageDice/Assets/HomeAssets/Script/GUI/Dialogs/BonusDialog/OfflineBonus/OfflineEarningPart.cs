using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OfflineEarningPart : MonoBehaviour
{
    public TMPro.TextMeshProUGUI tmpTimePassed;

    [Header("Button")]
    public GameObject gClaim;
    public GameObject gWait;
    public GameObject gSkipReward;

    public TMPro.TextMeshProUGUI tmpTimeWait;

    [Header("Reward")]
    public RewardItemUI[] rewards;


    private List<object> prizes;
    UserOfflineBonusData dataInstance;
    double timePassed;
    bool isActive;

    private Coroutine coroutineUpdatePrize;
    private YieldInstruction yieldWait1Minus;

    UserOfflineBonusData DataInstance
    {
        get
        {
            if (dataInstance == null)
                dataInstance = UserOfflineBonusData.Instance;

            return dataInstance;
        }
    }
    protected void OnEnable()
    {
        yieldWait1Minus = new WaitForSeconds(60);
        OnShow();
    }
    protected void OnDisable()
    {
        if(coroutineUpdatePrize != null)
            StopCoroutine(coroutineUpdatePrize);
    }

    public void OnShow()
    {
        isActive = DataInstance.IsCanCollect(ref this.timePassed);
        ActiveBottom();

        prizes = new List<object>();
        ParsePrize();
    }
    private IEnumerator ieLoopUpdatePrize()
    {
        yield return this.yieldWait1Minus;
        ParsePrize();
    }
    private void Update()
    {
        CustomUpdate();
    }
    protected void CustomUpdate()
    {
        bool state = DataInstance.IsCanCollect(ref this.timePassed);

        if (isActive != state)
        {
            isActive = state;
            ActiveBottom();
        }

        if(!isActive)
            this.tmpTimeWait.SetText(string.Format("{0} \n remaining", GameUtils.ConvertFloatToTime((float)(UserOfflineBonusData.TIME_MIN_ALLOW_COLLECT - timePassed))) );

        this.tmpTimePassed.SetText(GameUtils.ConvertFloatToTime(DataInstance.TimeOfflinePassed));
    }
    private void ActiveBottom()
    {
        this.gWait.gameObject.SetActive(!isActive);
        this.gClaim.gameObject.SetActive(isActive);

        this.gSkipReward.gameObject.SetActive(!DataInstance.isClaimSkip);
    }

    private void ParsePrize()
    {
        if (coroutineUpdatePrize != null)
            StopCoroutine(coroutineUpdatePrize);

        DataInstance.UpdateListPrize(ref this.prizes);
        for (int i = 0; i < this.rewards.Length; i++)
        {
            this.rewards[i].gameObject.SetActive(i < prizes.Count);
            if (i < prizes.Count)
                this.rewards[i].Show(this.prizes[i], "");
        }

        this.coroutineUpdatePrize = StartCoroutine(ieLoopUpdatePrize());
    }

    public void OnClickClaim()
    {
        DataInstance.UpdateListPrize(ref this.prizes);
        ClaimReward(this.prizes);

        DataInstance.ClaimReward();
        ParsePrize();
    }

    private void ClaimReward(List<object> prizes)
    {

        List<CardAmount> cards = new List<CardAmount>();
        List<BagAmount> bags = new List<BagAmount>();
        List<BoosterCommodity> bs = new List<BoosterCommodity>();


        foreach (object data in prizes)
        {
            if (data is CardAmount)
                cards.Add(data as CardAmount);

            else if (data is BagAmount)
                bags.Add(data as BagAmount);

            else if (data is BoosterCommodity)
                bs.Add(data as BoosterCommodity);
        }

        if (bs.Count > 0)
        {
            UserProfile.Instance.AddBoosters(bs, "OfflineEarning", "OfflineEarning");
            FxHelper.Instance.ShowFxCollectBoosters(bs, gSkipReward.activeInHierarchy ? gSkipReward.transform : gClaim.transform);
        }

        if (bags.Count > 0 || cards.Count > 0)
            MainBagSlots.OpenCardsAndBagNow(cards, bags, "OfflineEarning");


        isActive = DataInstance.IsCanCollect(ref this.timePassed);
        ActiveBottom();
    }
    public void OnClickClaimSkip()
    {
        
        List<object> prizes = new List<object>();

        prizes = DataInstance.GetPrizesByTime(3600, ref prizes);
        Debug.Log("reward skip" + prizes.Count);
        if (prizes.Count > 0)
            AdsManager.Instance.ShowVideoReward("OfflineEarning Skip", (r) =>
            {
                if (r)
                {
                    DataInstance.ClaimRewardSkip();
                    ClaimReward(prizes);
                }
            });
    }
    
}
