using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class MainFreeCoinIcon : BaseIcon
{
    [Header("Time")]
    public TextMeshProUGUI tmpTime;
    public Animator animClock;

    private double timeRemain;
    private bool isFree, isFreeNew;
    private double timeWait;

    private void Update()
    {
        //this.isFreeNew = StoreFreeCoinData.Instance.IsFreeCoin(ref this.timeRemain);
        //if (this.isFreeNew != this.isFree)
        //{     
        //    if (this.isFreeNew)
        //    {
        //        this.tmpTime.text = LanguageManager.GetString("TITLE_FREE");
        //    } 
        //    this.isFree = this.isFreeNew;
        //    this.animClock.SetBool("Stop", this.isFree);
        //}

        //if (!this.isFree)
        //{
        //    this.timeWait = StoreFreeCoinData.TOTAL_TIME_WAIT - this.timeRemain;
        //    //if (this.timeWait < 60) this.timeWait = 60f; //để luôn hiển thị còn 1 phút
        //    this.tmpTime.text = GameUtils.ConvertFloatToTime(this.timeWait, "mm'm'ss's'");
        //}
    }

    public override void OnClickIcon()
    {
        Debug.LogError("Click free coin");

        if (this.isFree)
        {
            AdsManager.Instance.ShowVideoReward(LogAdsVideoWhere.FREE_COIN_HOME, DoneWatchAdsFreeCoin);
        }
        else
        {
            Notification.Instance.ShowNotificationIcon(string.Format(LanguageManager.GetString("DES_WAIT"), GameUtils.ConvertFloatToTime(this.timeWait, "mm'm'ss's'")));
        }
        base.OnClickIcon();
    }

    private void DoneWatchAdsFreeCoin(bool done)
    {
        if (done)
        {
            CollectCoin();
        }
    }

    private void CollectCoin()
    {
        //StoreFreeCoinData.Instance.CollectFreeCoin();
        BoosterCommodity booster = new BoosterCommodity(BoosterType.COIN, GameDefine.COIN_WATCH_FREE);
        UserProfile.Instance.AddBooster(booster, "Free_Coin", LogSourceWhere.COIN_FREE_COIN, false);
        FxHelper.Instance.ShowFxCollectBooster(booster, this.transform);
    }
}
