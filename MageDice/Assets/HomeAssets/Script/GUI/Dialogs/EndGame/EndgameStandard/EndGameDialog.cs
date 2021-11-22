using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;
using UnityEngine.Events;

public class EndGameDialog : BaseDialog
{
    [Header("Base Button")]
    public CanvasGroup canvasInterractButton;
    public CanvasGroup canvasRewardButton;

    public override void OnShow(object data = null, UnityAction callback = null)
    {
        Clear();
        base.OnShow(data, callback);
    }
    public virtual void ParseReward()
    {

    }
    public virtual void ParseData(StandardPlayer player)
    {
        this.Clear();
        this.ShowInfo(player);
    }
    public virtual void ShowResult()
    {

    }
    protected virtual void ShowInfo(PlayerModel player)
    {
    }
    protected virtual void Clear()
    {

    }

    #region Button Click
    public void OnClickHome()
    {
        JoinGameHelper.Instance.BackHomeScene();
    }
    public void OnClickAds()
    {
        AdsManager.Instance.ShowVideoReward(LogAdsVideoWhere.DOUBLE_WIN_GAME, (success) =>
        {
            if (success)
            {
                DOTween.Kill(this.GetInstanceID());
                this.canvasRewardButton.alpha = 0; ;
                OnAdsSuccess();
            }
        });
    }
    protected virtual void OnAdsSuccess()
    {

    }
    #endregion
}
