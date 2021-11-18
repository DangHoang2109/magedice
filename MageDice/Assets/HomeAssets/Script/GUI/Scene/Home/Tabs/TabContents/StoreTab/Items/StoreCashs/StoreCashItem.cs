using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class StoreCashItem : StoreItem
{
    [Header("Booster")]
    public IBooster booster;
    public Image icon;

    private StoreCashConfig Config;
    public override void ParseConfig(StoreItemConfig config)
    {
        base.ParseConfig(config);

        this.Config = config as StoreCashConfig;
        if (this.Config != null)
        {
            this.booster.ParseBooster(this.Config.boosters[0]);
            Sprite sprIcon = SpriteIconValueConfigs.Instance.GetSprite(this.Config.boosters[0].type, this.Config.boosters[0].GetValue());
            if (sprIcon != null) this.icon.sprite = sprIcon;
            ShowPriceText(string.Format("{0}",IAPManager.Instance.FormatMoneyLocal(this.Config.key_iap)));
        }
    }

    public override void OnClickBuy()
    {
        //Debug.LogError("Click buy cash");     
        LogGameAnalytics.Instance.LogEvent(LogAnalyticsEvent.CLICK_SHOP_ITEM, LogParams.SHOP_ITEM_ID, Config.id);

        base.OnClickBuy();
        BuySuccess(); //TODO IAP manager
    }

    protected override void BuySuccess()
    {
        LogGameAnalytics.Instance.LogEvent(LogAnalyticsEvent.CLICK_SHOP_ITEM, LogParams.SHOP_ITEM_ID, Config.id);
        base.BuySuccess();
        if (this.Config != null)
        {    
            StoreConfigs.Instance.BuyIAP(this.Config.id, this.Config.key_iap, string.Format("Shop_Buy_Cash_{0}",this.Config.id), (success) =>
            {
                if (success)
                {
                    UserProfile.Instance.AddBoosters(this.Config.boosters, string.Format("Shop_{0}", this.Config.id), LogSourceWhere.SHOP_BUY, false);
                    FxHelper.Instance.ShowFxCollectBoosters(this.Config.boosters, this.transform);
                }
                else
                {
                    //TODO: Language
                    Notification.Instance.ShowNotification("Purchase fail");
                }
            });
            
        }
    }
}
