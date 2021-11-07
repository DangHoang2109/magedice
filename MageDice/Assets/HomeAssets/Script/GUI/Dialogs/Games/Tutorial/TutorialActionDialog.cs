using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class TutorialActionDialog : TutorialBaseDialog
{
    [SerializeField]
    private Button btAction;

    [SerializeField]
    private Text txtTime;

    [SerializeField]
    private float timeShowButton;

    private float timeWait;
    private bool onWait;

    private void OnEnable()
    {
        this.btAction.interactable = false;
        this.onWait = false;
        this.timeWait = this.timeShowButton;
        this.txtTime.text = string.Format("({0})", (int)(this.timeWait) + 1);
#if UNITY_EDITOR
        this.timeShowButton = 1;
#endif
    }

    public override void OnCloseDialog()
    {
        base.OnCloseDialog();
    }

    public void OnTutorial(UnityAction callback)
    {
        this.callbackTut = callback;
    }

    protected override void OnCompleteShow()
    {
        base.OnCompleteShow();
        this.onWait = true;
    }

    private void Update()
    {
        if (onWait)
        {
            this.timeWait -= Time.deltaTime;
            if (this.timeWait > 0)
            {
                this.txtTime.text = string.Format("({0})",(int)(this.timeWait) + 1);
            }
            else
            {
                this.onWait = false;
                this.timeWait = this.timeShowButton;
                string taptocontinue = LanguageManager.GetString("TITLE_TAP");
                this.txtTime.text = $"<color=green>{taptocontinue}</color>";
                this.ShowBtAction();
            }
        }
    }

    private void ShowBtAction()
    {
        this.btAction.interactable = true;
    }


    public void OnClickTutorial()
    {
        if (this.callbackTut != null)
        {

            var backup = this.callbackTut;
            this.callbackTut = null;
            backup.Invoke();
        }
        this.OnCloseDialog();
    }
}
