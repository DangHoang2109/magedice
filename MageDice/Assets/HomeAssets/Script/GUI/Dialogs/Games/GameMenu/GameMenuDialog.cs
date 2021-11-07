using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class GameMenuDialog : BaseSortingDialog
{
    [SerializeField]
    private Transform panelMenu;
    protected override void AnimationShow()
    {
        //PoolGame.Instance.OnPauseGame();
        this.panelMenu.DOLocalMoveX(80, 0.2f);
    }
    protected override void AnimationHide()
    {
        this.panelMenu.DOLocalMoveX(-300, 0.2f).OnComplete(this.OnCompleteHide);
    }
    public override void OnCloseDialog()
    {
        //PoolGame.Instance.OnResumeGame();
        base.OnCloseDialog();
    }
    public void ClickClose()
    {
        this.ClickCloseDialog();
    }

    public void ClickOption()
    {
        GameManager.Instance.OnShowDialog<UserProfileDialog>("Home/GUI/Dialogs/Profiles/UserProfileDialog");
    }
    public void ClickLeave()
    {
        //PoolGame.Instance.LeaveGame();
        SoundManager.Instance.PlayButtonClick();

    }
}
