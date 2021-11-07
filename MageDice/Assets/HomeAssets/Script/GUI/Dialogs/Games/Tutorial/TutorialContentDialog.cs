using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class TutorialContentDialog : TutorialBaseDialog
{
    [SerializeField]
    private Text txtContent, txtTapToContinue, txtTime;

    [SerializeField]
    private Transform tranTap, tranBlockRaycast;

    [SerializeField]
    private CanvasGroup canContinue;

    private bool needTapped, isCloseOnTapped, doneAnim, isWaitting;

    //private Coroutine ieWaitting;

    private float timeWaitContinue = 2.8f;

    private int timeShow;

    private void OnEnable()
    {
        this.doneAnim = false;
        this.canContinue.alpha = 0f;
        //this.txtTapToContinue.color = new Vector4(this.txtTapToContinue.color.r, this.txtTapToContinue.color.g, this.txtTapToContinue.color.b, 0);
        this.txtContent.text = "";
    }

    public TutorialContentDialog OnTutorial(string content, Vector3 pos, bool blockRaycast = false)
    {
        //this.txtContent.text = content;
        this.AnimShowText(content);
        this.panel.localPosition = pos;

        this.needTapped = isCloseOnTapped = false;
        this.tranTap.gameObject.SetActive(false);
        this.txtTapToContinue.gameObject.SetActive(false);
        this.txtTime.gameObject.SetActive(false);
        this.tranBlockRaycast.gameObject.SetActive(blockRaycast);
        return this;
    }


    private void AnimShowText(string content)
    {
        this.txtContent.text = content;
        this.txtContent.color = new Vector4(1, 1, 1, 0);
        this.txtContent.DOFade(1, 0.4f);
    }

    private void DoneWatting()
    {
        if(this.callbackTut != null)
        {
            var bk = this.callbackTut;
            this.callbackTut = null;
            bk.Invoke();
        }
    }

    public void HideAllRayCast()
    {
        this.tranBlockRaycast.gameObject.SetActive(false);
        this.tranTap.gameObject.SetActive(false);
    }

    public TutorialContentDialog SetNeedTap(bool needTapped, bool isCloseOnTapped = true, UnityAction callback = null, bool blockRaycast = false)
    {
        this.needTapped = needTapped;
        this.tranTap.gameObject.SetActive(needTapped);
        this.txtTapToContinue.gameObject.SetActive(needTapped);
        this.txtTime.gameObject.SetActive(needTapped);
        this.tranBlockRaycast.gameObject.SetActive(blockRaycast);

        this.isCloseOnTapped = isCloseOnTapped;
        this.callbackTut = callback;

        return this;
    }

    protected override void OnCompleteShow()
    {
        base.OnCompleteShow();
        this.doneAnim = false;
        this.timeWaitContinue = 2.8f;
        //this.canContinue.DOFade(1, 0.4f).OnComplete;
    }

    private void Update()
    {
        if (!this.doneAnim && this.needTapped)
        {
            this.timeWaitContinue -= Time.deltaTime;
            if (this.timeWaitContinue > 0)
            {
                this.txtTime.text = string.Format("({0})", (int)(this.timeWaitContinue) + 1);

                if (this.timeShow != (int)this.timeWaitContinue)
                {
                    this.timeShow = (int)this.timeWaitContinue;
                    this.AnimFlashContinue();
                }
            }
            else
            {
                this.doneAnim = true;
                this.txtTime.text = "> ";
                this.AnimContinue();
            }
        }
    }

    private void AnimContinue()
    {
        DOTween.Kill(this + "continue");
        Sequence seq = this.AnimFlashContinue();
        seq.AppendInterval(0.5f);
        seq.SetLoops(-1);
    }

    private Sequence AnimFlashContinue()
    {
        Sequence seq = DOTween.Sequence();
        seq.Append(this.canContinue.DOFade(0.8f, 0.2f).SetEase(Ease.Linear));
        seq.Append(this.canContinue.DOFade(1f, 0.2f).SetEase(Ease.Linear));
        seq.SetId(this + "continue");
        return seq;
    }



    public void OnClickTap()
    {
        if (this.doneAnim)
        {
            //if (this.ieWaitting != null) this.StopCoroutine(this.ieWaitting);
            if(this.callbackTut != null)
            {
                var bk = this.callbackTut;
                this.callbackTut = null;
                bk.Invoke();
            }
            if(this.isCloseOnTapped)
            {
                this.OnCloseDialog();
            }
        } 
    }  
}
