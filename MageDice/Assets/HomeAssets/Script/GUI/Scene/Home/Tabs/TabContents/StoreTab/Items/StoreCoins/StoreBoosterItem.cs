using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class StoreBoosterItem : StoreItem
{
    public BoosterUIItem boosterUI;

    private StoreBoosterConfig Config;
    public override void ParseConfig(StoreItemConfig config)
    {
        base.ParseConfig(config);

        this.Config = config as StoreBoosterConfig;
        if (this.Config != null)
        {
            ShowBooster(this.Config.boosters[0]);
            ShowPrice(this.Config.price);
        }
    }

    public virtual void ShowBooster(BoosterCommodity booster)
    {
        this.boosterUI.ShowBooster(booster);
    }

    public override void OnClickBuy()
    {
        //Debug.LogError("Click buy booster ");
        if (this.Config != null)
        {
            if (UserBehaviorDatas.Instance.IsCheater())
            {
                MessageBox.Instance.ShowMessageBox("Error", "You are cheater!").SetEvent(() => { }, null)
                    .SetButtonNo("No").SetButtonYes("Contact");
                return;
            }
            if (UserProfile.Instance.UseBooster(this.Config.price, string.Format("Shop_{0}", this.Config.id),LogSinkWhere.SHOP_BUY ))
            {
                BuySuccess();
            }
            else
            {
                GameUtils.ShowNeedMoreBooster(this.Config.price, OnClickBuy);
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
        LogGameAnalytics.Instance.LogEvent(LogAnalyticsEvent.CLICK_SHOP_ITEM, LogParams.SHOP_ITEM_ID, Config.id);
        base.BuySuccess();
        if (this.Config != null)
        {
            UserProfile.Instance.AddBoosters(this.Config.boosters, string.Format("Shop_{0}", +this.Config.id), LogSourceWhere.SHOP_BUY, false);
            FxHelper.Instance.ShowFxCollectBoosters(this.Config.boosters, this.transform);
        }  
    }
}
