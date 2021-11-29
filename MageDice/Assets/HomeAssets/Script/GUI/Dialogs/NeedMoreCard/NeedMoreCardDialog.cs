using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using TMPro;
//Call like this
//GameManager.Instance.OnShowDialogWithSorting<NeedMoreCardDialog>(
//    "Home/GUI/Dialogs/NeedMoreCard/NeedMoreCardDialog",
//    PopupSortingType.OnTopBar)
//    .ParseData((int)(this.Data.RequirementCard - this.Data.cards), this.Data, "ShopCueDialog");
public class NeedMoreCardDialog : BaseSortingDialog
{
    public StatCardItemDisplay cardDisplayer;
    public TextCurrencyResize txtCash;
    public TextMeshProUGUI tmpAmount;

    private StatData _cueData;
    private long _cardNeed;

    private BoosterCommodity gem;
    private string from;

    private UnityAction callback;
    public void ParseData(int cardNeed, StatData cueData, string _from, UnityAction callback = null)
    {
        this._cueData = cueData;
        this._cardNeed = cardNeed;
        this.callback = callback;
        this.from = _from;

        this.cardDisplayer.ParseData(cueData.config);
        tmpAmount.SetText($"x{cardNeed}");

        this.gem = StoreConfigs.GetPriceCueCardNeed(cueData);
        this.txtCash.ParseData(this.gem.GetValue());
    }

    public void ClickBuy()
    {
        if (this.gem == null || this._cueData == null)
        {
            return;
        }

        if (UserProfile.Instance.UseBooster(this.gem, this.from, LogSinkWhere.NEED_MORE_CARD))
        {
            StatManager.Instance.AddCard(_cueData, _cardNeed);

            this.callback?.Invoke();
            this.callback = null;

            this._cueData = null;
            this.gem = null;

            this.from = string.Empty;
            this.OnCloseDialog();
        }
        else
        {
            GameUtils.ShowNeedMoreBooster(this.gem);

            //NeedMoreGemDialog dialog =
            //    GameManager.Instance.OnShowDialogWithSorting<NeedMoreGemDialog>("Home/GUI/Dialogs/NeedMoreGem/NeedMoreGemDialog",
            //        PopupSortingType.CenterBottomAndTopBar);
            //dialog?.ParseData(this.gem);
        }
    }



}
