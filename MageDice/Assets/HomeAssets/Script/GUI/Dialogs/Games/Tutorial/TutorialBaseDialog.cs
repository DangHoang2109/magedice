using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class TutorialBaseDialog : BaseSortingDialog
{
    protected UnityAction callbackTut;

    protected override void AnimationShow()
    {
        //base.AnimationShow();
        DOTween.Kill(this);
        this.canvasGroup.alpha = 0;
        this.canvasGroup.DOFade(1, 0.8f).OnComplete(this.OnCompleteShow).SetId(this);
    }

    protected override void AnimationHide()
    {
        //base.AnimationHide();
        DOTween.Kill(this);
        this.canvasGroup.DOFade(0, 0.8f).OnComplete(this.OnCompleteHide).SetId(this);
    }
}
