using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class StoreDealFreeItem : StoreBoosterItem
{
    [Header("Tran booster")]
    public Transform tranBooster;

    [Header("bag")]
    public BagUIItem bagUI;

    [Header("Deal ui")]
    public StoreDealItemUI ui;

    //Data
    private StoreFreeDealConfig config;
    private StoreDealFreeData.FreeDealStatus status, tempStatus;
    private double timeRemain;
    private double timeWait;

    public void ParseData()
    {
        this.config = StoreDealFreeData.Instance.GetCurrentFreeDealConfig();
        if (this.config != null)
        {
            this.status = StoreDealFreeData.FreeDealStatus.NONE;
            if (this.config.bagAmounts != null)
            {
                if (this.config.bagAmounts.Count > 0)
                {
                    this.bagUI.gameObject.SetActive(true);
                    this.bagUI.ShowBagWithNameTour(this.config.bagAmounts[0]);
                    this.tranBooster.gameObject.SetActive(false);
                    return;
                }
            }
            
            if (this.config.boosters != null)
            {
                if (this.config.boosters.Count > 0)
                {
                    this.tranBooster.gameObject.SetActive(true);
                    this.ShowBooster(this.config.boosters[0]);
                    this.bagUI.gameObject.SetActive(false);
                    return;
                }
            }
        }
        
    }

    private void Update()
    {
        this.tempStatus = StoreDealFreeData.Instance.IsFreeDeal(ref timeRemain);
        if (this.tempStatus == StoreDealFreeData.FreeDealStatus.WAITTING)
        {
            this.timeWait = StoreDealFreeData.Instance.TOTAL_TIME_WAIT - this.timeRemain; ;
            this.ShowPriceText(GameUtils.ConvertFloatToTime(this.timeWait, "mm'm'ss's'"));
        }
        if (this.tempStatus != this.status)
        {
            UpdateStatus(this.tempStatus);
        }
    }

    private void UpdateStatus(StoreDealFreeData.FreeDealStatus status)
    {
        this.status = status;
        this.ui.ShowLock(this.status == StoreDealFreeData.FreeDealStatus.COMPLETED || this.status == StoreDealFreeData.FreeDealStatus.NONE);
        switch (this.status)
        {
            case StoreDealFreeData.FreeDealStatus.FREE:
                this.ShowPriceText(LanguageManager.GetString("TITLE_FREE"));
                break;
            case StoreDealFreeData.FreeDealStatus.WATCH:
                this.ShowPriceText(LanguageManager.GetString("TITLE_WATCH"));
                break;
            case StoreDealFreeData.FreeDealStatus.COMPLETED:
                this.ui.SetTextLock(LanguageManager.GetString("DES_WAITFORNEXTTIME"), LanguageManager.GetString("TITLE_LOCKED"));
                this.ShowPriceText(LanguageManager.GetString("TITLE_COMPLETED"));
                break;
            case StoreDealFreeData.FreeDealStatus.NONE:
                this.ShowPriceText(LanguageManager.GetString("TITLE_LOCKED"));
                break;
        }
    }

    public override void OnClickBuy()
    {
        base.OnClickBuy();

        //check state free, waitting, watch, full
        if (this.config != null)
        {
            switch (this.status)
            {
                case StoreDealFreeData.FreeDealStatus.FREE:
                    CollectDeal();
                    break;
                case StoreDealFreeData.FreeDealStatus.WAITTING:
                    //Debug.LogError(LanguageManager.GetString("DES_WAIT"));
                    Notification.Instance.ShowNotificationIcon(string.Format(LanguageManager.GetString("DES_WAIT"), GameUtils.ConvertFloatToTime(this.timeWait, "mm'm'ss's'")));
                    //show noti waitting
                    break;
                case StoreDealFreeData.FreeDealStatus.WATCH:
                    AdsManager.Instance.ShowVideoReward(LogAdsVideoWhere.WATCH_DEAL_SHOP, DoneWatchAdsCollectDeal);
                    //watch ad to collect
                    break;
                case StoreDealFreeData.FreeDealStatus.COMPLETED:
                    Notification.Instance.ShowNotificationIcon(LanguageManager.GetString("DES_GIFTRECEIVE"));
                    //show noti full, next time in: --h--m
                    break;
                default:
                    break;
            }

            LogGameAnalytics.Instance.LogEvent(LogAnalyticsEvent.CLICK_SHOP_ITEM, LogParams.SHOP_ITEM_ID, config.id);

        }
    }

    private void DoneWatchAdsCollectDeal(bool done)
    {
        if (done)
        {
            CollectDeal();
        }
    }

    private void CollectDeal()
    {
        _CollectReward();

        LogGameAnalytics.Instance.LogEvent(string.Format(LogAnalyticsEvent.CLAIM_SHOP_FREE, StoreDealFreeData.Instance.indexFree));

        //mission
        //MissionDatas.Instance.DoStep(MissionID.CLAIM_FREE_DEAL);
        StoreDealFreeData.Instance.CollectFreeDeal();
        ParseData();
    }

    private void _CollectReward()
    {
        if (this.config != null)
        {
            if (this.config.boosters != null)
            {
                foreach(BoosterCommodity booster in this.config.boosters)
                {
                    UserProfile.Instance.AddBooster(booster, "Shop_Free", LogSourceWhere.SHOP_FREE_COIN, false);
                    FxHelper.Instance.ShowFxCollectBooster(booster, this.tranBooster);
                }
            }

            if (this.config.bagAmounts != null)
            {
                GiftBagConfigs BagConfigs = GiftBagConfigs.Instance;

                foreach (BagAmount bag in this.config.bagAmounts)
                {
                    MainBagSlots.OpenBagNow(bag.bagType, BagConfigs.GetCurrentRoomForBag(bag.bagType), "ShopFree");

                    //BagSlotDatas.Instance.CollectBag(bag, "TOUR "+ bag.tour, "Collect free deal");
                }
            }
        }
    }
}

[System.Serializable]
public class StoreDealItemUI
{
    [Header("Next Time")]
    public Transform tranLock;
    public Transform tranCard;
    public TextMeshProUGUI tmpLockMid;
    public TextMeshProUGUI tmpLockBottom;

    [Header("Bg")]
    public Image imgBg;
    public Sprite[] sprBgs; //on 1; off 0
    
    public void ShowLock(bool isLock)
    {
        this.tranLock.gameObject.SetActive(isLock);
        this.tranCard.gameObject.SetActive(!isLock);
        this.imgBg.sprite = isLock ? sprBgs[0] : sprBgs[1];
    }

    public void SetTextLock(string textMid, string textBottom)
    {
        this.tmpLockBottom.SetText(textBottom);
        this.tmpLockMid.SetText(textMid);
    }
}
