using System;
using System.Collections;
using System.Collections.Generic;
using Coffee.UIEffects;
using DG.Tweening;
using UnityEngine;

public class UIHsvPlayer : MonoBehaviour
{
    public float durationCycle;
    public float timeDarken;
    public UIHsvModifier hsvModifier;

    // private Tween tweenLoop;
    // private Tween tweenDarken;

    private float speed;
    private delegate void CallUpdate(float dt);
    private CallUpdate callUpdate;
#if UNITY_EDITOR
    private void OnValidate()
    {
        this.hsvModifier = this.GetComponent<UIHsvModifier>();
    }
#endif

    private void Awake()
    {
        this.hsvModifier.enabled = false;
        // this.tweenLoop = DOTween.To(() => -0.5f, this.SetHue, 0.5f, this.durationCycle)
        //     .SetLoops(-1, LoopType.Restart).SetAutoKill(false).Pause().SetTarget(this);
        // this.tweenDarken = DOTween.To(() => 0f, this.SetSaturationAndValue, -0.5f, this.timeDarken)
        //     .SetAutoKill(false).Pause().SetTarget(this);
    }

    private void Update()
    {
        if (this.callUpdate != null)
        {
            this.callUpdate.Invoke(Time.deltaTime);
        }
    }

    private void UpdateHue(float dt)
    {
        this.hsvModifier.hue += dt * this.speed;
        if (this.hsvModifier.hue >= 0.5f)
            this.hsvModifier.hue -= 1f;
    }

    private void UpdateDark(float dt)
    {
        if (this.hsvModifier.saturation > -0.5f)
        {
            this.hsvModifier.saturation -= dt * this.speed;
            this.hsvModifier.value -= dt * this.speed;
        }
    }

    private void SetHue(float hue)
    {
        this.hsvModifier.hue = hue;
    }
    private void SetSaturationAndValue(float v)
    {
        this.hsvModifier.saturation = v;
        this.hsvModifier.value = v;
    }

    public void StartAnimate()
    {
        //this.tweenDarken.Pause();
        
        this.hsvModifier.saturation = 0f;
        this.hsvModifier.value = 0f;
        this.hsvModifier.enabled = true;
        //this.tweenLoop.Play();

        this.speed = 1f / durationCycle;
        this.callUpdate = this.UpdateHue;
    }
    public void StartDarken()
    {
        //this.tweenLoop.Pause();
        
        this.hsvModifier.enabled = true;
        this.hsvModifier.saturation = 0f;
        this.hsvModifier.value = 0f;
        //this.tweenDarken.Restart();

        this.speed = 0.5f / timeDarken;
        this.callUpdate = this.UpdateDark;
    }
    public void StopAnimate()
    {
        this.hsvModifier.enabled = false;
        //this.tweenLoop.Pause();
        //this.tweenDarken.Pause();

        this.callUpdate = null;
    }

    private void OnDisable()
    {
        this.StopAnimate();
    }
}
