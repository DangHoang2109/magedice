using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Events;

public class NeedMoreCoinDialogs : BaseSortingDialog
{
    public TextCurrencyResize txtCoin;
    public TextCurrencyResize txtCash;

    private long coin;
    private long gem;
    private string from;
    private UnityAction callback;
    public void ParseData(long coinNeed, string _from, UnityAction callback=null )
    {
        this.callback = callback;
        this.from = _from;
        this.coin = coinNeed;
        this.txtCoin.ParseData(this.coin);
        this.gem = GameUtils.ConvertValue_Coin_To_Cash(this.coin);
        this.txtCash.ParseData(this.gem);
    }

    public void ClickBuy()
    {
        if (this.gem == -1)
        {
            return;
        }
        if (UserProfile.Instance.UseBooster(BoosterType.CASH, this.gem, this.from, LogSinkWhere.NEED_MORE_COIN))
        {
            UserProfile.Instance.AddBooster(BoosterType.COIN, this.coin, this.from, LogSourceWhere.NEED_MORE_COIN);
            this.callback?.Invoke();
            this.callback = null;
            this.coin = -1;
            this.gem = -1;
            this.from = String.Empty;
            this.OnCloseDialog();
        }
        else
        {
            NeedMoreGemDialog dialog =
                GameManager.Instance.OnShowDialogWithSorting<NeedMoreGemDialog>("Home/GUI/Dialogs/NeedMoreGem/NeedMoreGemDialog",
                    PopupSortingType.CenterBottomAndTopBar);
            dialog?.ParseData(new BoosterCommodity(BoosterType.CASH, this.gem));
        }
    }
}
