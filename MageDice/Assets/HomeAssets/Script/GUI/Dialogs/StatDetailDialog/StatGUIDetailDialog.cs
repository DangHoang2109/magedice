using DG.Tweening;
using TMPro;
using UnityEngine;
using UnityEngine.Events;

#if UNITY_EDITOR
using UnityEditor;
#endif

// demo at the bottom
public class StatGUIDetailDialog : BaseSortingDialog
{
    // stats
    [Header("part")]
    public StatItemGUI partStatGUI;

    public StatGUIStatLines partStatLines;
    public StatGUIUpgrade partUpgrade;
    public StatGUIBtnBuyOrUse partBuyOrUse;


    private StatData _data;
    public StatData Data
    {
        get { return this._data; }
    }

    public override void OnShow(object data = null, UnityAction callback = null)
    {
        base.OnShow(data, callback);
        
        this._data = this.data as StatData;
        if(this._data != null)
            this.ParseData();
    }

    
    #region animation

    protected override void AnimationShow()
    {
        if (this.canvasGroup != null)
        {
            this.canvasGroup.alpha = 0f;
            this.canvasGroup.DOFade(1f, this.transitionTime)
                .OnComplete(this.OnCompleteShow);
        }
        SoundManager.Instance.Play("snd_panel");
    }

    protected override void AnimationHide()
    {
        if (this.canvasGroup != null)
        {
            this.canvasGroup.DOFade(0f, this.transitionTime)
                .OnComplete(this.OnCompleteHide);
        }
    }

    #endregion
    

    private void ParseData()
    {
        var tier = this._data.config.tier;

        this.partStatGUI.ParseData(this._data);
        
        switch (this._data.kind)
        {
            case StatManager.Kind.NotUnlocked:
                
                this.partStatLines.ParseStatsNext(this._data);
                this.partUpgrade.ParseCueToBuy(this._data);
                
                this.partBuyOrUse.ParseToBuy(this._data.UpgradePrice, this._data.config.unlockType);
                break;
            case StatManager.Kind.UnlockedNonMaxed:
                
                this.partStatLines.ParseStatsCurrentNNext(this._data);
                this.partUpgrade.ParseCueBought(this._data);
                this.partBuyOrUse.ParseCueBought(StatManager.Instance.IsUsing(this._data.id));
                break;
            case StatManager.Kind.Maxed:
                this.partStatLines.ParseStatsCurrent(this._data);
                this.partUpgrade.ParseCueBought(this._data);
                this.partBuyOrUse.ParseCueBought(StatManager.Instance.IsUsing(this._data.id));
                break;
            default:
                Debug.LogException(new System.Exception("ShopCueItem Refresh: kind exception: " + this._data.kind.ToString()));
                break;
        }
        
#if CHEAT
        this.ShowCheatButtons();
#endif
    }
    
    
    public void OnClickBuyOrUse()
    {
        if (this._data.kind == StatManager.Kind.NotUnlocked)
            this.OnBuying();
        else
            this.OnUsing();
    }

    private StatItemStats _lastStats;
    private int _lastCardsCount;
    public void OnClickUpgrade()
    {

        this._lastStats = this._data.CurrentStats;
        this._lastCardsCount = (int)this._data.cards;

        LogGameAnalytics.Instance.LogEvent(LogAnalyticsEvent.CLICK_UPGRADE_STAT_ITEM, LogParams.STAT_ITEM_ID, this._data.id.ToString());

        StatManager.Instance.UpgradeCue(
            cue: this._data,
            onSuccess: this.OnUpgradeCueSuccess,
            onFail: this.OnUpgradeCueFailed
            );
    }

    private void OnUpgradeCueSuccess(string e)
    {
        if (this._data == null)
        {
            Debug.LogException(new System.Exception(
                "ShopCueItem OnUpgradeCueSuccess ERROR! _data NULL!"));
            return;
        }
        Debug.Log("edit");
        //var du = GameManager.Instance.OnShowDialogWithSorting<CueUpgradingDialog>(
        //    "Home/GUI/Dialogs/Cue/CueUpgrading",
        //    PopupSortingType.CenterBottomAndTopBar,
        //    this._data);

        //du.OnClosing = this.OnAnimateUpgradeComplete;
        this._lastStats = null;
    }

    private void OnAnimateUpgradeComplete()
    {
        this.ParseData();
    }

    private void OnUpgradeCueFailed(string reason)
    {
        if(!StatManager.Instance.IsHome)
            return;
        
        switch (reason)
        {
            case StatManager.Constant.CUE_NOT_ENOUGH_CARDS:
                this.ClickCloseDialog();
                break;
            default:
                return;
        }
        //SoundManager.Instance.PlayButtonClick();
    }
    
    private void OnBuying()
    {
        StatManager.Instance.BuyCue(this._data, this.OnBuyCueSuccess, this.OnBuyCueFail);
    }

    private void OnBuyCueSuccess(string _)
    {
        CanvasBlocker.Instance.SetActive(true);
    }

    private void OnHighLightCue()
    {
        SoundManager.Instance.Play("sfx_glare_mid");
    }

    private void OnAnimateBuyCueComplete()
    {
        CanvasBlocker.Instance.SetActive(false);
        this.ParseData();
    }
    
    private void OnBuyCueFail(string reason)
    {
        string langCode = $"CUE_{reason}";
        //Notification.Instance.ShowNotification(LanguageManager.GetString(langCode, LanguageCategory.Feature));
        //SoundManager.Instance.PlayButtonClick();
        
        this.ClickCloseDialog();
    }
    
    private void OnUsing()
    {
        if (StatManager.Instance.IsUsing(this._data.id))
        {
            SoundManager.Instance.Play("snd_noti");
        }
        else
        {
            Debug.Log("edit using");
            //StatManager.Instance.ChangeCue(this._data, OnUseCueSuccess);    
        }
    }

    private void OnUseCueSuccess(string _)
    {
        SoundManager.Instance.PlayButtonClick();
        this.partBuyOrUse.ParseCueBought(StatManager.Instance.IsUsing(this._data.id));
       
    }
    
}
