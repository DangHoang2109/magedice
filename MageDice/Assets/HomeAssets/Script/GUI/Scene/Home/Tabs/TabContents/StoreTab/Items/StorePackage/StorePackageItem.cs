using Cosina.Components;
using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class StorePackageItem : StoreItem
{
    private StorePackageConfig Config;

    [Header("Layout")]
    public StorePackageLayoutType layoutType;

    [Header("Package")]
    public TextMeshProUGUI tmpName;     
    public TextMeshProUGUI tmpContains;
    public TextMeshProUGUI tmpBonus;
    public Image imgBG;

    [Header("Cards")]
    public BagUIItem[] bags;
    public StoreCardBoosterItem cardBoosterItem; //dùng dể parse coin, cash

    [Header("Cue")]
    public Transform tranCueItem;
    public CueCardDisplay cueDisplay;

    //OLD
    //public StoreCardValueItem cardValueItem;     //dùng để parse string

    private string strContains;


#if UNITY_EDITOR
    private void OnValidate()
    {
        this.bags = this.GetComponentsInChildren<BagUIItem>();
    }
#endif

    public override void ParseConfig(StoreItemConfig config)
    {
        base.ParseConfig(config);

        this.Config = config as StorePackageConfig;
        if (this.Config != null)
        {
            this.bstPrice.tmpValue.SetText(string.Format("{0}", IAPManager.Instance.FormatMoneyLocal(this.Config.key_iap)));
            this.tmpName.SetText(string.Format(LanguageManager.GetString(string.Format("SHOP_PACKAGE_OFFER_NAME"), LanguageCategory.Feature))); //this.Config.name
            this.tmpBonus.SetText(string.Format("x{0}", this.Config.xBonus));

            this.strContains = "";
            ParseBags();
            ParseBoosterCard();
            ParseCue();
            this.tmpContains.SetText(this.strContains);
        }
    }
    
    private void ParseBags()
    {
        if (this.Config != null)
        {
            if (this.Config.bagAmounts != null)
            {
                for (int iBag = 0; iBag < this.bags.Length; iBag++)
                {
                    if (iBag < this.Config.bagAmounts.Count)
                    {
                        this.bags[iBag].gameObject.SetActive(true);

                        BagAmount bagAmount = this.Config.bagAmounts[iBag];

                        string nameTour = string.Format("TOUR {0}", GiftBagConfigs.Instance.GetCurrentRoomForBag(bagAmount.bagType));
                        this.bags[iBag].ShowBag(bagAmount, nameTour);

                        //parse name bag to text contains
                        BagAssetConfig bagAsset = BagAssetConfigs.Instance.GetBagAsset(bagAmount.bagType);
                        if (bagAsset != null)
                        {
                            string nameBag = string.Format("{0} - {1}", bagAsset.name, nameTour);
                            this.strContains = string.Format("{0}", nameBag);
                        }
                    }
                    else
                    {
                        this.bags[iBag].gameObject.SetActive(false);
                    }
                }
            }
        }      
    }

    private void ParseBoosterCard()
    {
        if (this.Config != null)
        {
            if (this.Config.boosters != null)
            {
                if (this.Config.boosters.Count > 0)
                {
                    this.cardBoosterItem.gameObject.SetActive(true);
                    this.cardBoosterItem.ParseBooster(this.Config.boosters[0]);

                    //parse booster text to contains
                    BoosterConfig booster = BoosterConfigs.Instance.GetBooster(this.Config.boosters[0].type);
                    if (booster != null)
                    {
                        string nameBst = booster.name;
                        this.strContains = string.Format("{0}+ {1}", this.strContains, nameBst);
                    }
                }
                else this.cardBoosterItem.gameObject.SetActive(false);
            }
            else this.cardBoosterItem.gameObject.SetActive(false);
        }
    }
    
    private void ParseCue()
    {
        this.tranCueItem.gameObject.SetActive(false);
        if (this.Config != null)
        {
            if (this.Config.diceID != DiceID.NONE)
            {
                this.tranCueItem.gameObject.SetActive(true);
                this.cueDisplay.ParseCueFullStats(this.Config.diceID);
            }
        }
    }

    //OLD
    //private void ParseStringCard()
    //{
    //    //HOLDING TO STRING
    //    //this.cardValueItem.gameObject.SetActive(false);
    //}

    public override void OnClickBuy()
    {
        LogGameAnalytics.Instance.LogEvent(LogAnalyticsEvent.CLICK_SHOP_ITEM, LogParams.SHOP_ITEM_ID, Config.id);
        base.OnClickBuy();

        //TODO buy iap
        StoreConfigs.Instance.BuyIAP(this.Config.id, this.Config.key_iap, string.Format("Shop_Buy_Package_{0}",this.Config.id),
        (success) =>
        {
            if (success)
            {
                this.BuySuccess();
            }
            else
            {
                //TODO: Language
                Notification.Instance.ShowNotification("Purchase fail");
            }
        });
    }

    protected override void BuySuccess()
    {
        LogGameAnalytics.Instance.LogEvent(LogAnalyticsEvent.CLICK_SHOP_ITEM, LogParams.SHOP_ITEM_ID, Config.id);
        base.BuySuccess();
        if (this.Config != null)
        {
            if (this.Config.boosters != null)
            {
                UserProfile.Instance.AddBoosters(this.Config.boosters, string.Format("Shop_Package_{0}", this.Config.id), LogSourceWhere.SHOP_BUY_PACKAGE, false);
                FxHelper.Instance.ShowFxCollectBoosters(this.Config.boosters, this.transform);
            }

            if (this.Config.diceID != DiceID.NONE)
            {
                StatData StatData = StatDatas.Instance.GetStat(this.Config.diceID);
                StatManager.Instance.WinCue(StatData);
                
                LogGameAnalytics.Instance.LogEvent(LogAnalyticsEvent.CUE_UNLOCKED, LogParams.STAT_ITEM_ID, StatData.id.ToString());//unlock từ package
            }

            Invoker.Invoke(CollectCueAndOpenBags, 1f);
        }
    }

    private void CollectCueAndOpenBags()
    {
        //collect cue, open bag
        if (this.Config.diceID != DiceID.NONE)
        {
            //TODO add cue
            Debug.Log(string.Format("<color=yellow>Collect cue </color>" + this.Config.diceID));

            StatData StatData = StatDatas.Instance.GetStat(this.Config.diceID);

            CollectCueDialog dialog = GameManager.Instance.OnShowDialogWithSorting<CollectCueDialog>(
            "Home/GUI/Dialogs/OpenBag/CollectCue",
            PopupSortingType.CenterBottomAndTopBar);
            dialog.ParseData(StatData, false);
            dialog.OnClosed += OpenBags;
        }
        else
        {
            OpenBags();
        }
    }

    private void OpenBags()
    {
        if (this.Config.bagAmounts != null)
        {
            GiftBagConfigs BagConfigs = GiftBagConfigs.Instance;

            List<BagAmount> trulyBagGiftTour = new List<BagAmount>();
            foreach (BagAmount config in this.Config.bagAmounts)
            {
                trulyBagGiftTour.Add(new BagAmount()
                {
                    bagType = config.bagType,
                    amount = config.amount,
                    tour = BagConfigs.GetCurrentRoomForBag(config.bagType)
                });
            }

            MainBagSlots.OpenBagNow(trulyBagGiftTour, "Shop");
        }
    }
}


public enum StorePackageLayoutType
{
    Hor_Middle = 0, //nằm ngang, căn giữa
    Hor_Left = 1,   // nằm ngang, string ở bên trái
    Ver_Middle = 2  // nằm dọc, căn giữa
}