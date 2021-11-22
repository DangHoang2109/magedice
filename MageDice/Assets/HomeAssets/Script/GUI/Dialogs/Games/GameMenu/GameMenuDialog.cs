using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using UnityEngine.Events;

public class GameMenuDialog : BaseDialog
{
    public override void OnShow(object data = null, UnityAction callback = null)
    {
        MageDiceGameManager.Instance.OnPauseGame(true);
        base.OnShow(data, callback);
    }
    protected override void OnCompleteHide()
    {
        base.OnCompleteHide();
        MageDiceGameManager.Instance.OnPauseGame(false);

    }
    public void OnClickBackHome()
    {
        SoundManager.Instance.PlayButtonClick();

        MessageBox.Instance.ShowMessageBox("Quit Game!?", "You will not receive prize if you quit! Are you sure?")
            .SetButtonYes("Stay")
            .SetButtonNo("Quit")
            .SetEvent(
            callbackYes: null,
            callbackNo: LoadHome);
    }
    private void LoadHome()
    {
        JoinGameHelper.Instance.BackHomeScene();
    }

}
