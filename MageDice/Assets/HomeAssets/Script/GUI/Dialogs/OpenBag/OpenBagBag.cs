using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Cosina.Components;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;
using UnityEngine.Serialization;

public class OpenBagBag : MonoBehaviour
{
    public enum BagAppearStep
    {
        None,
        FallingDown,
        Opening,
        Waiting,
        Disappear,
    }

    [Header("Linker")]
    public BGTexScroller texScroller;
    
    public Image imgBox;
    public Image imgBoxUpper;
    
    public OpenBagBall ball;
    public Transform firePlace;

    public GameObject goCounter;

    //[Header("Data")]
    [Header("Fall down animation")]
    public float durationFall;
    public PairTweenVector[] scalesLandedEachPhase;
    
    [Header("Open bag animation")]
    public PairTweenVector[] scalesOpenEachPhase;
    public PairTweenVector[] movesOpenEachPhase;
    
    private Tween tween;
    private Transform cachedTransform;
    public Transform CachedTransform => cachedTransform;

    private Transform transTop;
    private Transform transMid;

    private System.Action callback;

    private BagAppearStep step;
    public BagAppearStep Step => this.step;

    [Header("fx")]
    public ParticleSystem fxLanded;
    //
    // public RectTransform transFxLanded;
    // public Image imgFxLanded;
    
    public void Init(Transform transformTop, Transform transformMid)
    {
        this.tween = null;
        this.cachedTransform = this.transform;
        this.step = BagAppearStep.None;

        this.transTop = transformTop;
        this.transMid = transformMid;
        
        this.ball.Init();
        this.ball.OnHideBG(this.HideBagAndCounter);
    }

    public OpenBagBag ParseData(BagType t)
    {
        this.imgBox.sprite = GameAssetsConfigs.Instance.bagAsset.GetBagAsset(t)?.sprBag;
        return this;
    }

    public OpenBagBag StartFallDown(Vector3 posStart, Vector3 posEnd)
    {
        this.Clear();
        this.step = BagAppearStep.FallingDown;
        this.cachedTransform.position = posStart;
        this.texScroller.speed.y = -2f;
        this.tween = this.cachedTransform.DOMove(posEnd, this.durationFall)
            .OnComplete(this.OnLanded).SetEase(Ease.InCubic);

        return this;
    }

    private void OnLanded()
    {
        this.Clear();
        
        this.texScroller.speed.y = -0.2f;
        Sequence s = this.scalesLandedEachPhase.ToScaleSequence(this.cachedTransform);
        s?.OnComplete(this.OnAnimateComplete);
        this.tween = s;

        this.fxLanded.time = 0f;
        this.fxLanded.Play(false);
    }

    public OpenBagBag StartOpenBag(OpenBagItemUpper.CardDisplayType animationType)
    {
        this.Clear();
        this.step = BagAppearStep.Opening;


        Sequence s = null;
        switch (animationType)
        {
            case OpenBagItemUpper.CardDisplayType.Common:
            case OpenBagItemUpper.CardDisplayType.Card:
            case OpenBagItemUpper.CardDisplayType.CardNewCue:
                s = this.scalesOpenEachPhase.ToScaleSequence(this.cachedTransform, OnFireCard);
                break;
            case OpenBagItemUpper.CardDisplayType.Cue:
                s = this.scalesOpenEachPhase.ToScaleSequence(this.cachedTransform, OnFireCue);
                break;
            case OpenBagItemUpper.CardDisplayType.Box:
                break;
        }
        
        this.tween = s;

        return this;
    }
    

    /// <summary>
    /// must be called after animation
    /// </summary>
    public OpenBagBag OnComplete(System.Action c)
    {
        
        if (this.tween is null)
        {
            Invoker.Invoke(c);
        }
        else
        {
            this.callback = c;
        }
        return this;
    }

    public void SkipAnimation()
    {
        if (this.step != BagAppearStep.None)
            this.Clear(true);
        this.ball.StopBallEmitting();
    }

    public void Done()
    {
        this.step = BagAppearStep.None;
    }
    
    private void OnFireCard()
    {
        this.ball.StartFlyUp(this.firePlace.position, this.transTop.position)
            .OnComplete(this.OnFireReachTarget);
    }

    private void OnFireCue()
    {
        this.ball.StartFlyUpSpecial(this.firePlace.position, this.transMid.position)
            .OnComplete(this.OnFireReachTarget);
    }
    private void OnFireReachTarget()
    {
        if (this.ball.gameObject.activeSelf)
            this.ball.Hide();
        this.OnAnimateComplete();
    }
    
    private void OnAnimateComplete()
    {
        
        switch (this.step)
        {
            case BagAppearStep.FallingDown:
                
            case BagAppearStep.Opening:
                this.step = BagAppearStep.Waiting;
                break;
            case BagAppearStep.Waiting:
                this.step = BagAppearStep.Opening;
                break;
        }
        Debug.Log( $"{"OpenBag_Bag".WrapColor("cyan")} OnAnimateComplete -step {this.step}");
        
        if(!(this.callback is null))
        {
            // prevent nested callback -> cause null callback afterward
            var backupCall = this.callback;
            this.callback = null;
            backupCall.Invoke();
        }
        
    }

    public void HideBagAndCounter()
    {
        this.gameObject.SetActive(false);
        this.goCounter.SetActive(false);
    }
    private void ShowBagAndCounter()
    {
        this.gameObject.SetActive(true);
        this.goCounter.SetActive(true);
    }
    
    private void Clear(bool complete = false)
    {
        if (this.tween != null)
            this.tween.Kill(complete);
        this.tween = null;
        
        this.ball.Hide();
        this.cachedTransform.localScale = Vector3.one;
        this.ShowBagAndCounter();
        if (complete)
        {
            this.OnAnimateComplete();
        }
        else
        {
            this.callback = null;
        }
    }
}
