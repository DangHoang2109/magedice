using System.Collections;
using System.Collections.Generic;
using Cosina.Components;
using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;

public class StatItemStatLine : MonoBehaviour
{
    public Image imgBack;
    public Image imgFront;

    /// <summary>
    /// value must be 0 ~ 10
    /// </summary>
    public void ParseData(float back, in Color colBack, float front)
    {
        this.imgBack.fillAmount = back / 10f;
        this.imgFront.fillAmount = front / 10f;
        this.imgBack.color = colBack;
    }

    public Tween ChangeColorBack(in Color colBackNew, float time)
    {
        if (time <= CosinaMathf.ZERO_BUT_GREATER)
        {
            this.imgBack.color = colBackNew;
            return null;
        }
        else
        {
            return this.imgBack.DOColor(colBackNew, time)
                .SetEase(Ease.InCubic).SetId(this);
        }
    }

    public Tween ChangeValueBack(float newBack, float time)
    {
        if (time <= CosinaMathf.ZERO_BUT_GREATER)
        {
            this.imgBack.fillAmount = newBack / 10f;
            return null;
        }
        else
        {
            return this.imgBack.DOFillAmount(newBack, time);
        }
    }

    public Tween ChangeColorFront(in Color colFrontNew, float time)
    {
        if (time <= CosinaMathf.ZERO_BUT_GREATER)
        {
            this.imgFront.color = colFrontNew;
            return null;
        }
        else
        {
            return this.imgFront.DOColor(colFrontNew, time)
                .SetEase(Ease.InCubic)
                .SetId(this);
        }
    }

    
    public Tween ChangeValueFront(float newFront, float time)
    {
        if (time <= CosinaMathf.ZERO_BUT_GREATER)
        {
            this.imgFront.fillAmount = newFront / 10f;
            return null;
        }
        else
        {
            return this.imgFront.DOFillAmount(newFront, time);
        }
    }

    public Tween FlashingBack(float cycle)
    {
        Color c = this.imgBack.color;
        c.a = 0f;
        this.imgBack.color = c;
        return this.imgBack.DOFade(1f, cycle / 2f).SetLoops(-1, LoopType.Yoyo);
    }
}
