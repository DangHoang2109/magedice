using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class StoreBagItem : StoreItem
{
    [Header("Bag")]
    public TextMeshProUGUI tmpBagName;
    public TextMeshProUGUI tmpTour;
    public TextMeshProUGUI tmpAmount;

    [Header("Image")]
    public Image imgBag;

    private StoreBoosterConfig Config;
    private string nameTour;

    public override void ParseConfig(StoreItemConfig config)
    {
        base.ParseConfig(config);

        this.Config = config as StoreBoosterConfig;
        if (this.Config != null)
        {
            //parse config bag
            ParseBag(Config.bagAmounts[0]);
            ShowPrice(Config.price);
        }
    }

    private void ParseBag(BagAmount bagAmount)
    {
        BagAssetConfig bagAsset = BagAssetConfigs.Instance.GetBagAsset(bagAmount.bagType);
        if (bagAsset != null)
        {
            this.imgBag.sprite = bagAsset.sprBag;
            this.tmpBagName.SetText(bagAsset.name);
            this.nameTour = string.Format("TOUR {0}", GiftBagConfigs.Instance.GetCurrentRoomForBag(bagAmount.bagType));
            this.tmpTour.SetText(this.nameTour);
            
        }
        this.tmpAmount.SetText(string.Format("X{0}", bagAmount.amount));
    }

    public override void OnClickBuy()
    {
        //Debug.LogError("Click buy bag");
        if (this.Config != null)
        {
            if (UserBehaviorDatas.Instance.IsCheater())
            {
                MessageBox.Instance.ShowMessageBox("Error", "You are cheater!").SetEvent(() => { }, null)
                    .SetButtonNo("No").SetButtonYes("Contact");
                return;
            }
            if (UserProfile.Instance.UseBooster(this.Config.price, string.Format("Shop_Bag_{0}", this.Config.bagAmounts[0].bagType), LogSinkWhere.SHOP_BUY_BAG))
            {
                BuySuccess();    
            }
            else
            {
                GameUtils.ShowNeedMoreBooster(this.Config.price);

                //NeedMoreGemDialog dialog =
                //    GameManager.Instance.OnShowDialogWithSorting<NeedMoreGemDialog>("Home/GUI/Dialogs/NeedMoreGem/NeedMoreGemDialog",
                //        PopupSortingType.CenterBottomAndTopBar);
                //dialog?.ParseData(this.Config.price);
            }

            LogGameAnalytics.Instance.LogEvent(LogAnalyticsEvent.CLICK_SHOP_ITEM, LogParams.SHOP_ITEM_ID, Config.id);
        }
        base.OnClickBuy();
    }

    protected override void BuySuccess()
    {
        LogGameAnalytics.Instance.LogEvent(LogAnalyticsEvent.COMPLETE_BUY_SHOP_ITEM, LogParams.SHOP_ITEM_ID, Config.id);
        base.BuySuccess();

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

        if (this.Config!=null) MainBagSlots.OpenBagNow(trulyBagGiftTour, "Shop");
    }
}
