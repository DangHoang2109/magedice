using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UpgradeCardStats : MonoBehaviour
{
    private RectTransform rectTrans;

    private RectTransform RectTrans
    {
        get
        {
            if (ReferenceEquals(this.rectTrans, null))
            {
                this.rectTrans = this.GetComponent<RectTransform>();
            }
            return this.rectTrans;
        }
    }
    public UpgradeCardLine[] lines;
    public float interval;


//#if UNITY_EDITOR
//    private void OnValidate()
//    {
//        this.lines = new List<UpgradeCardLine>().ToArray();
//        this.lines = this.GetComponentsInChildren<UpgradeCardLine>();
//    }
//#endif

    public void ParseData(StatItemStats before, StatItemStats after, StatItemStats max)
    {
        float maxWidth = this.RectTrans.rect.width;

        if(after.damageStrength > before.damageStrength)
            this.lines[0].Parse(before.damageStrength, after.damageStrength, maxWidth, max.damageStrength);
        else
            this.lines[0].Show(false);

        if (after.speedStrength > before.speedStrength)
            this.lines[1].Parse(before.speedStrength, after.speedStrength, maxWidth, max.speedStrength);
        else
            this.lines[1].Show(false);

        if (after.rangeStrength > before.rangeStrength)
            this.lines[2].Parse(before.rangeStrength, after.rangeStrength, maxWidth, max.rangeStrength);
        else
            this.lines[2].Show(false);

        if (after.timeEffectStrength > before.timeEffectStrength)
            this.lines[3].Parse(before.timeEffectStrength, after.timeEffectStrength, maxWidth, max.timeEffectStrength);
        else
            this.lines[3].Show(false);
    }

    public void StartAnimate()
    {
        for (int i = 0; i < this.lines.Length; ++i)
        {
            if(this.lines[i].gameObject.activeSelf)
                this.lines[i].StartAnimate(interval * i);
        }
    }

    public void Clear()
    {
        for (int i = 0; i < this.lines.Length; ++i)
        {
            if(this.lines[i].gameObject.activeSelf)
            {
                this.lines[i].Clear(false);
                this.lines[i].Show(false);
            }
        }
    }
}
