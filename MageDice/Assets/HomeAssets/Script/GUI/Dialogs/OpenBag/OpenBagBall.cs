using System.Collections;
using System.Collections.Generic;
using Cosina.Components;
using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;

public class OpenBagBall : MonoBehaviour
{
    private static readonly Vector3 VEC_ROLL_4_ROUND = new Vector3(0f, 0f, 360f * 4f);
    
    [Header("Linkers")]   
    public Image imgBall;
    /// <summary>
    /// front of the ball
    /// </summary>
    public Image imgFlash;

    // white card
    public Image imgCardFore;

    public float durationFly;
    private System.Action callback;
    private Tween tween;

    private Transform transBall;
    private Transform transCardFore;

    public ParticleSystem psTrail;
    private Transform transTrail;
    private ParticleSystem.ShapeModule psShape;
    
    [Header("Stats")]
    public Color[] colsOfFlashy;
    public Vector3 scaleCardForeBig;
    //
    // private Transform cachedTransform;
    // public Transform CachedTransform => cachedTransform;
    
    private System.Action callHideBG;
    private System.Action callInInvoker;
    
    public void Init()
    {
        //this.cachedTransform = this.transform;
        this.transBall = this.imgBall.transform;
        this.transCardFore = this.imgCardFore.transform;
        this.transTrail = this.psTrail.transform;
        this.transTrail.localPosition = Vector3.zero;
        //this.psTrail.Stop(false);
        this.psShape = this.psTrail.shape;
    }

    public void OnHideBG(System.Action call)
    {
        this.callHideBG = call;
    }
    
    public OpenBagBall StartFlyUp(Vector3 posStart, Vector3 posEnd)
    {
        if (!this.transBall.gameObject.activeSelf)
        {
            this.transBall.gameObject.SetActive(true);
        }
        
        this.Clear(false);
        
        
        this.transBall.position = posStart;
        this.psShape.position = Vector3.zero;
        //this.transTrail.gameObject.SetActive(true);
        //this.psTrail.emission.rateOverTime = 1f;
        var emission = this.psTrail.emission;
        emission.rateOverTime = 300;
        this.psTrail.Play();
        
        this.tween = this.transBall.DOMove(posEnd, this.durationFly)
            .OnUpdate(this.UpdateTrailPos)
            /*.OnComplete(this.OnAnimateComplete)*/.SetEase(Ease.OutCubic);
        this.transBall.DOLocalRotate(VEC_ROLL_4_ROUND, this.durationFly,
            RotateMode.LocalAxisAdd).SetEase(Ease.Linear);
        this.transBall.localScale = Vector3.one * 0.5f;
        this.transBall.DOScale(1.5f, this.durationFly * 0.8f).SetEase(Ease.InQuad);
        //this.transBall.DOShakePosition(this.durationFly, fadeOut: false);
        
            
        this.callInInvoker = this.OnAnimateComplete;
        Invoker.Invoke(this.callInInvoker, this.durationFly * 0.8f);
        
        
        return this;
    }
    
    /// <summary>
    /// for cue
    /// </summary>
    public OpenBagBall StartFlyUpSpecial(Vector3 posStart, Vector3 posEnd)
    {
        if (!this.transBall.gameObject.activeSelf)
        {
            this.transBall.gameObject.SetActive(true);
        }
        
        this.Clear(false);
        this.transBall.position = posStart;
        this.imgFlash.color = this.colsOfFlashy[0];
        //this.imgFlash.gameObject.SetActive(true);
        
        // var seq = DOTween.Sequence();
        // seq.Append(this.transBall.DOMove(posEnd, this.durationFly * 2f).SetEase(Ease.OutExpo))
        //     .Join(this.imgFlash.DOColor(this.colsOfFlashy[1], this.durationFly * 1.5f)
        //         .SetId(this.imgFlash))
        //     .OnComplete(this.OnDarkenSpecial);
        // this.tween = seq;

        this.psShape.position = Vector3.zero;
        //this.transTrail.gameObject.SetActive(true);
        var emission = this.psTrail.emission;
        emission.rateOverTime = 100;
        this.psTrail.Play();
        
        this.tween = this.transBall.DOMove(posEnd, this.durationFly * 2f)
            .OnUpdate(this.UpdateTrailPos)
            .SetEase(Ease.OutExpo);
        this.transBall.DOLocalRotate(VEC_ROLL_4_ROUND * 2f, this.durationFly * 2f,
                RotateMode.LocalAxisAdd).SetEase(Ease.Linear);
        this.transBall.localScale = Vector3.one * 0.5f;
        this.transBall.DOScale(1.5f, this.durationFly * 1.5f).SetEase(Ease.InQuad);
        
        this.imgFlash.DOColor(this.colsOfFlashy[1], this.durationFly * 1.5f)
            .SetEase(Ease.InQuad).SetId(this.imgFlash);
        
        this.callInInvoker = this.OnDarkenSpecial;
        Invoker.Invoke(this.callInInvoker, this.durationFly * 1.5f);

        return this;
    }

    private void UpdateTrailPos()
    {
        this.psShape.position = this.transBall.localPosition;
    }

    private void OnDarkenSpecial()
    {
        DOTween.Kill(this.imgFlash);
        this.StopBallEmitting();
        //this.psTrail.Stop(false, ParticleSystemStopBehavior.StopEmittingAndClear);
        //this.imgFlash.gameObject.SetActive(false);
        this.imgFlash.color = Color.clear;

        this.tween = this.imgFlash.DOColor(this.colsOfFlashy[2], 0.1f)
            .OnComplete(this.OnWhiteSpecial).SetId(this);
            
        // this.callInInvoker = this.OnWhiteSpecial;
        // Invoker.Invoke(this.callInInvoker, 0.01f);
    }

    private void OnWhiteSpecial()
    {
        //this.imgFlash.gameObject.SetActive(true);
        //this.imgFlash.color = Color.white;
        //this.imgFlash.color = this.colsOfFlashy[2];
        this.tween = this.imgFlash.DOColor(
                this.colsOfFlashy[3], this.durationFly * 1.5f)
            .SetEase(Ease.InQuart);
            //.OnComplete(this.OnCardAppearSpecial);
        
        this.callHideBG?.Invoke();
        this.imgBall.gameObject.SetActive(false);

        this.OnAnimateComplete();
        this.callInInvoker = null;
        // this.callInInvoker = OnAnimateComplete;
        // Invoker.Invoke(this.callInInvoker, this.durationFly);
    }

    // private void OnCardAppearSpecial()
    // {
    //     this.callInInvoker = null;
    //     
    //     this.imgFlash.gameObject.SetActive(false);
    //     this.transCardFore.gameObject.SetActive(true);
    //     this.transCardFore.localScale = this.scaleCardForeBig;
    //     this.imgCardFore.color = this.colsOfFlashy[3];
    //
    //     this.tween = this.transCardFore.DOScale(Vector3.one, this.durationFly / 2f)
    //         .OnComplete(this.OnAnimateComplete);
    //     this.imgCardFore.DOColor(Color.white, this.durationFly / 2f);
    // }
    
    /// <summary>
    /// must be called after animation
    /// </summary>
    public OpenBagBall OnComplete(System.Action c)
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
        this.Clear(false, true);
    }
    
    private void OnAnimateComplete()
    {
        this.StopBallEmitting();
            
        this.callInInvoker = null;
        if(!(this.callback is null))
        {
            // prevent nested callback -> cause null callback afterward
            var backupCall = this.callback;
            this.callback = null;
            backupCall.Invoke();
        }
        
    }

    public void StopBallEmitting()
    {
        if (this.psTrail.isEmitting)
        {
            //this.psTrail.Clear();
            this.psTrail.Stop(false, ParticleSystemStopBehavior.StopEmittingAndClear);
            //this.transTrail.gameObject.SetActive(false);
        }
    }
    
    private void Clear(bool isSmooth, bool complete = false)
    {
        if (this.tween != null)
            this.tween.Kill(complete);
        this.tween = null;
        if (this.callInInvoker != null)
        {
            Invoker.CancelInvoke(this.callInInvoker);
            this.callInInvoker = null;    
        }
        
        this.transCardFore.gameObject.SetActive(false);
        //this.imgFlash.gameObject.SetActive(false);
        
        if(isSmooth)
        {
            DOTween.Kill(this.imgFlash);
            this.imgFlash.DOFade(0f, 0.7f).SetEase(Ease.InCubic);
        }
        else
        {
            DOTween.Kill(this.imgFlash);
            this.imgFlash.color = Color.clear;
        }
        this.transBall.localScale = Vector3.one;
        if (complete)
        {
            this.OnAnimateComplete();
        }
        else
        {
            this.callback = null;
        }
    }

    public void Hide()
    {
        this.transBall.gameObject.SetActive(false);
        this.Clear(true, false);
    }
}
