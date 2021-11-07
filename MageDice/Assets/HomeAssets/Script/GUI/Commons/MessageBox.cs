using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;
using DG.Tweening;
using TMPro;

public class MessageBox : MonoSingleton<MessageBox>
{
    public CanvasGroup canvasGroup;
    public Transform panel;
    public TextMeshProUGUI txtTitle;
    public TextMeshProUGUI txtContent;
    public TextMeshProUGUI txtOk;
    public TextMeshProUGUI txtYes;
    public TextMeshProUGUI txtNo;
    public GameObject btnOk;
    public GameObject btnLeft;
    public GameObject btnRight;
    private string title;
    private string content;

    private UnityAction callbackYes;
    private UnityAction callbackNo;

#if UNITY_EDITOR
    private void OnValidate()
    {
        //this.canvasGroup = this.GetComponentInChildren<CanvasGroup>();
    }
#endif

    public MessageBox ShowMessageBox(string title, string content)
    {
        OnShow();
        this.title = title;
        this.content = content;
        this.txtTitle.text = this.title;
        this.txtContent.text = this.content;
        this.btnLeft.gameObject.SetActive(false);
        this.btnRight.gameObject.SetActive(false);
        this.btnOk.gameObject.SetActive(true);

        return this;
    }
    public MessageBox SetEvent(UnityAction callback = null)
    {
        this.callbackYes = callback;
        this.btnLeft.gameObject.SetActive(false);
        this.btnRight.gameObject.SetActive(false);
        this.btnOk.gameObject.SetActive(true);
        return this;
    }
    public MessageBox SetEvent(UnityAction callbackYes = null, UnityAction callbackNo = null)
    {
        this.callbackYes = callbackYes;
        this.callbackNo = callbackNo;
        this.btnLeft.gameObject.SetActive(true);
        this.btnRight.gameObject.SetActive(true);
        this.btnOk.gameObject.SetActive(false);
        return this;
    }
    public MessageBox SetButtonOk(string msg = "")
    {
        if(!msg.Equals(""))
        {
            this.txtOk.text = msg;
            return this;
        }
        this.txtOk.text = "OK";
        return this;
    }
    public MessageBox SetButtonYes(string msg = "")
    {
        if (!msg.Equals(""))
        {
            this.txtYes.text = msg;
            return this;
        }
        this.txtYes.text = LanguageManager.GetString("TITLE_YES");
        return this;
    }
    public MessageBox SetButtonNo(string msg = "")
    {
        if (!msg.Equals(""))
        {
            this.txtNo.text = msg;
            return this;
        }
        this.txtNo.text = LanguageManager.GetString("TITLE_NO");

        return this;
    }
    public void CloseMessageBox()
    {
        this.callbackYes = null;
        Sequence seq = DOTween.Sequence();
        seq.Join(this.canvasGroup.DOFade(0, 0.3f));
        seq.Join(this.panel.DOScale(0, 0.3f));//.OnComplete(() => this.OnHide());
        seq.OnComplete(() => this.OnHide());
        seq.SetEase(Ease.Linear);
    }
    private void OnShow()
    {
        this.panel.localScale = Vector3.zero;
        this.canvasGroup.alpha = 0;
        this.canvasGroup.gameObject.SetActive(true);

        Sequence seq = DOTween.Sequence();
        seq.Join(this.canvasGroup.DOFade(1, 0.3f).SetEase(Ease.Linear));

        seq.Join(this.panel.DOScale(1, 0.3f).SetEase(Ease.Linear));
        seq.SetEase(Ease.Linear);
    }
    public void OnClickOk()
    {
        if (this.callbackYes != null)
        {
            this.callbackYes.Invoke();
            this.callbackYes = null;
        }
        this.CloseMessageBox();
        SoundManager.Instance.PlayButtonClick();
    }
    public void OnClickCancel()
    {
        if (this.callbackNo != null)
        {
            this.callbackNo.Invoke();
            this.callbackNo = null;
        }
        this.CloseMessageBox();
        SoundManager.Instance.PlayButtonClick();
    }
    public void OnClickYes()
    {
        if (this.callbackYes != null)
        {
            this.callbackYes.Invoke();
        }
        this.CloseMessageBox();
        SoundManager.Instance.PlayButtonClick();

    }
    private void OnHide()
    {
        this.canvasGroup.gameObject.SetActive(false);
    }

}
