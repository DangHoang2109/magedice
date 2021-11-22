using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StoreDealCardItem : StoreItem
{
    public StoreDealItemUI ui;

    public StatCardItemDisplay cardDisplay;

    private StoreDealCardData dealCardData;

    private StatData StatData;

    StoreDealSlotConfig dealSlot;

    private bool isLock;
    private int countBuy;

    public void ParseData(StoreDealCardData dealCardData)
    {
        this.dealCardData = dealCardData;
        this.countBuy = 1; //mỗi lần mua 1 card

        //kiểm tra lock
        this.dealSlot = StoreConfigs.Instance.GetDealSlot_ByID(dealCardData.idSlot);
        if (dealSlot != null)
        {
            if (dealSlot.tourUnlock > RoomDatas.Instance.GetRoomUnlockedMax())
            {
                this.isLock = true;
                ui.ShowLock(true);
                ui.SetTextLock(string.Format(LanguageManager.GetString("SHOP_DAILY_UNLOCK", LanguageCategory.Feature), dealSlot.tourUnlock),
                    LanguageManager.GetString("TITLE_LOCKED")); //"Unlock at tour {0}" , LOCKED
                this.bstPrice.gameObject.SetActive(false);
                this.ShowPriceText(LanguageManager.GetString("DES_LOCKED")); //locked
            }
            else
            {
                //check max max
                if (dealCardData.IsMaxCanBuy())
                {
                    this.isLock = true;
                    ShowCompleted();
                }
                else
                {
                    this.isLock = false;
                    ui.ShowLock(false);
                    this.bstPrice.imgBooster.enabled = true;
                    this.bstPrice.gameObject.SetActive(true);
                    StatData = StatDatas.Instance.GetStat(dealCardData.cueID);
                    if (StatData != null)
                    {
                        cardDisplay.ParseStatDataAndShowTag(StatData, this.countBuy); //mỗi lần mua 1 card
                        this.bstPrice.ParseBooster(dealCardData.GetCurrentPrice());
                    }
                    else
                    {
                        Debug.LogError("Stats config is NULL");
                        ShowCompleted();
                    }
                }

            }
        }
        else
        {
            Debug.LogError("Deal slot is NULL");
            ShowCompleted();
        }
    }

    private void ShowCompleted()
    {      
        ui.ShowLock(true);
        ui.SetTextLock(LanguageManager.GetString("DES_WAITFORNEXTTIME"),
            LanguageManager.GetString("TITLE_COMPLETED"));
        this.bstPrice.gameObject.SetActive(false);
    }

    public override void OnClickBuy()
    {
        base.OnClickBuy();

        if (this.dealCardData != null)
        { 
            if (this.isLock)
            {          
                if (this.dealCardData.IsMaxCanBuy())
                {
                    Notification.Instance.ShowNotificationIcon(LanguageManager.GetString("DES_WAITFORNEXTTIME"));
                }
                else
                {
                    Notification.Instance.ShowNotificationIcon(string.Format(LanguageManager.GetString("SHOP_DAILY_UNLOCK", LanguageCategory.Feature), this.dealSlot.tourUnlock));
                }
                return;
            }

            if (this.StatData != null)
            {
                BoosterCommodity price = this.dealCardData.GetCurrentPrice();
                if (price == null)
                {
                    Debug.Log("Price is NULL");
                    return;
                }
                LogGameAnalytics.Instance.LogEvent(LogAnalyticsEvent.CLICK_SHOP_ITEM, LogParams.SHOP_ITEM_ID, StatData.id.ToString());

                if (UserProfile.Instance.IsCanUseBooster(price.type, price.GetValue()))
                {
                    MessageBox message = MessageBox.Instance.ShowMessageBox(LanguageManager.GetString("TITLE_SURE"),
                       LanguageManager.GetString("CUE_BUYCARDCONFIRM", LanguageCategory.Feature) + string.Format(" {0} {1} ?", price.GetValue(), GameAssetsConfigs.Instance.boosters.GetBooster(price.type).name));
                    message.SetEvent(
                        ()=>
                        {
                        if (UserProfile.Instance.UseBooster(price, string.Format("Shop_Card_{0}", this.dealCardData.cueID), LogSinkWhere.SHOP_BUY_CARD))
                        {
                            BuySuccess();
                        }
                    }, null);
                }
                else
                {
                    if (price.type == BoosterType.CASH)
                    {
                        NeedMoreGemDialog dialog =
                        GameManager.Instance.OnShowDialogWithSorting<NeedMoreGemDialog>("Home/GUI/Dialogs/NeedMoreGem/NeedMoreGemDialog",
                            PopupSortingType.CenterBottomAndTopBar);
                        dialog?.ParseData(price);
                    }
                    else
                    {
                        NeedMoreCoinDialogs dialog =
                        GameManager.Instance.OnShowDialogWithSorting<NeedMoreCoinDialogs>("Home/GUI/Dialogs/NeedMoreCoin/NeedMoreCoinDialog",
                            PopupSortingType.CenterBottomAndTopBar);
                        dialog?.ParseData(price.GetValue(), "DEAL_CARD");
                    }
                }
            }
            else
            {
                Debug.LogError("Cue Data is NULL");
            }
        }
    }

    protected override void BuySuccess()
    {      
        base.BuySuccess();

        //MissionDatas.Instance.DoStep(MissionID.BUY_CARD_SHOP);
        if (this.StatData != null)
        {
            StatDatas.Instance.AddCard(this.StatData.id, this.countBuy);
            StoreDealCardsData.Instance.OnBuyDealCard(this.dealCardData.cueID);
            ParseData(this.dealCardData); //parse lại data
        }
        LogGameAnalytics.Instance.LogEvent(LogAnalyticsEvent.COMPLETE_BUY_SHOP_ITEM, LogParams.SHOP_ITEM_ID, StatData.id.ToString());
    }
}
