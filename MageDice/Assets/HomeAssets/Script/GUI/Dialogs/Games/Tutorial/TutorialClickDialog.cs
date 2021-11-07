using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class TutorialClickDialog : TutorialBaseDialog
{
    [SerializeField]
    protected RectTransform rectBt, rectBorder;

    [SerializeField]
    protected Image imgboder;

    [SerializeField]
    protected BoxCollider2D boxBt;
    protected bool isNeedClose = true;

    public virtual void OnTutorial(Vector3 pos, Vector3 size, UnityAction callback = null)
    {
        this.callbackTut = callback;
        this.rectBt.position = pos;
        this.rectBt.sizeDelta = size;
        this.boxBt.size = size;
        this.rectBorder.sizeDelta = 1.5f * size;
    }

    public void SetNeedClose(bool needClose)
    {
        this.isNeedClose = needClose;
    }

    protected virtual void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            Vector3 mousPosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            GameObject obj = TouchChecker(mousPosition);

            if (obj)
            {
                if (obj == this.rectBt.gameObject)
                {
                    this.OnClickTutorial();
                }
            } 
        }
    }

    GameObject TouchChecker(Vector3 wp)
    {
        Vector2 touchPos = new Vector2(wp.x, wp.y);
        GameObject obj;
        if (Physics2D.OverlapPoint(touchPos))
        {
            obj = Physics2D.OverlapPoint(touchPos).gameObject;
            return obj;
        }
        return null;
    }

    private void OnClickTutorial()
    {
        bool needClose = this.isNeedClose;

        if (this.callbackTut != null)
        {
            // "callback tut can be changed inside callback tut itself
            var backupCallback = this.callbackTut;
            this.callbackTut = null;
            backupCallback.Invoke();
        }
        if (needClose)
            this.OnCloseDialog();
    }

    protected override void OnCompleteShow()
    {
        base.OnCompleteShow();
        //animation tutorial

        Sequence seqBorder = DOTween.Sequence();
        seqBorder.AppendCallback(() => this.imgboder.color = new Vector4(1, 1, 1, 0.6f));
        seqBorder.Append(this.imgboder.transform.DOScale(0.7f, 1f).SetEase(Ease.Linear));
        seqBorder.Join(this.imgboder.DOFade(0f, 1f).SetEase(Ease.Linear));
        seqBorder.AppendCallback(()=>
        {
            this.imgboder.transform.localScale = Vector3.one;
            this.imgboder.color = new Vector4(1, 1, 1, 0);
        });
        seqBorder.SetLoops(-1);
    }
}
