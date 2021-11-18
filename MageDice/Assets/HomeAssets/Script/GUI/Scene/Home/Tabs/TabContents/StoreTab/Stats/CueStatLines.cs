using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;

public class CueStatLines : MonoBehaviour
{
    [SerializeField]
    private Image imgBG;
    
    [Header("Stat of 4 lines")]
    [SerializeField]
    private StatItemStatLine linePow;
    [SerializeField]
    private StatItemStatLine lineAim;
    [SerializeField]
    private StatItemStatLine lineSpin;
    [SerializeField]
    private StatItemStatLine lineTime;

    private Color colBg;
    private Tween tween;
    
    private void Awake()
    {
        this.colBg = this.imgBG.color;
    }

    public void ParseStats(StatItemStats s)
    {
        Debug.Log("edit");
        //ShopCueRef @ref = ShopCueRef.Instance;
        //this.linePow.ParseData(0f, @ref.transparent, s.damageStrength);
        //this.lineAim.ParseData(0f, @ref.transparent, s.speedStrength);
        //this.lineSpin.ParseData(0f, @ref.transparent, s.rangeStrength);
        //this.lineTime.ParseData(0f, @ref.transparent, s.timeEffectStrength);
    }
    public void ParseStatsCurrent(StatData c)
    {
        StatItemStats cur = c.CurrentStats;
        ParseStats(cur);
    }

    public void ParseStatsCurrentNMax(StatData c)
    {
        StatItemStats cur = c.CurrentStats;
        StatItemStats max = c.FullStats;
        Debug.Log("edit");
        ShopCueRef @ref = ShopCueRef.Instance;
        this.linePow.ParseData(max.damageStrength, @ref.colStatMax, cur.damageStrength);
        this.lineAim.ParseData(max.speedStrength, @ref.colStatMax, cur.speedStrength);
        this.lineSpin.ParseData(max.rangeStrength, @ref.colStatMax, cur.rangeStrength);
        this.lineTime.ParseData(max.timeEffectStrength, @ref.colStatMax, cur.timeEffectStrength);
    }

    public void ParseStatsCurrentNNext(StatData c)
    {
        StatItemStats cur = c.CurrentStats;
        StatItemStats next = c.NextStats;
        Debug.Log("edit");

        ShopCueRef @ref = ShopCueRef.Instance;
        this.linePow.ParseData(next.damageStrength, @ref.colStatNext, cur.damageStrength);
        this.lineAim.ParseData(next.speedStrength, @ref.colStatNext, cur.speedStrength);
        this.lineSpin.ParseData(next.rangeStrength, @ref.colStatNext, cur.rangeStrength);
        this.lineTime.ParseData(next.timeEffectStrength, @ref.colStatNext, cur.timeEffectStrength);
    }

    public void ParseStatsNext(StatData c)
    {
        StatItemStats next = c.NextStats;
        Debug.Log("edit");

        ShopCueRef @ref = ShopCueRef.Instance;
        this.linePow.ParseData(0f, @ref.transparent, next.damageStrength);
        this.lineAim.ParseData(0f, @ref.transparent, next.speedStrength);
        this.lineSpin.ParseData(0f, @ref.transparent, next.rangeStrength);
        this.lineTime.ParseData(0f, @ref.transparent, next.timeEffectStrength);
    }
    // public void ParseStatsNextAlt(StatData c)
    // {
    //     StatItemStats next = c.NextStats;
    //     ShopCueRef @ref = ShopCueRef.Instance;
    //     this.linePow.ParseData(next.damageStrength, @ref.colStatNext, 0f);
    //     this.lineAim.ParseData(next.speedStrength, @ref.colStatNext, 0f);
    //     this.lineSpin.ParseData(next.rangeStrength, @ref.colStatNext, 0f);
    //     this.lineTime.ParseData(next.timeEffectStrength, @ref.colStatNext, 0f);
    // }
    public void ParseStatsNextNMax(StatData c)
    {
        StatItemStats next = c.NextStats;
        StatItemStats max = c.FullStats;
        Debug.Log("edit");

        ShopCueRef @ref = ShopCueRef.Instance;
        this.linePow.ParseData(max.damageStrength, @ref.colStatMax, next.damageStrength);
        this.lineAim.ParseData(max.speedStrength, @ref.colStatMax, next.speedStrength);
        this.lineSpin.ParseData(max.rangeStrength, @ref.colStatMax, next.rangeStrength);
        this.lineTime.ParseData(max.timeEffectStrength, @ref.colStatMax, next.timeEffectStrength);
    }


    /// <summary>
    /// animate upgrade the current stats to the next stats s
    /// </summary>
    /// <param name="s">the next stats</param>
    public void AnimateUpgrade(StatItemStats s/*, TweenCallback callback*/)
    {
        this.ClearAnim();

        if (s == null)
        {
            Debug.LogException(new System.Exception(
                "CueStatLines AnimateUpgrade error: s NULL!"));
            return;
        }

        
        
        SoundManager.Instance.Play("sfx_pellet_start");

        Debug.Log("edit");

        ShopCueRef @ref = ShopCueRef.Instance;
        this.AnimateUnlockLine(this.linePow, s.damageStrength, @ref);
        this.AnimateUnlockLine(this.lineAim, s.speedStrength, @ref);
        this.AnimateUnlockLine(this.lineSpin, s.rangeStrength, @ref);
        this.AnimateUnlockLine(this.lineTime, s.timeEffectStrength, @ref);
    }

    /// <summary>
    /// animate upgrade the current stats to the next stats s
    /// </summary>
    /// <param name="s">the next stats</param>
    public void AnimateUnlock(StatItemStats s)
    {
        this.ClearAnim();

        Debug.Log("edit");

        ShopCueRef @ref = ShopCueRef.Instance;

        this.imgBG.color = @ref.colStatCurrent;
        this.tween = this.imgBG.DOColor(this.colBg, 1f)
            .SetId(this);
        this.AnimateUnlockLine(this.linePow, s.damageStrength, @ref);
        this.AnimateUnlockLine(this.lineAim, s.speedStrength, @ref);
        this.AnimateUnlockLine(this.lineSpin, s.rangeStrength, @ref);
        this.AnimateUnlockLine(this.lineTime, s.timeEffectStrength, @ref);
    }

    public void AnimateFlashingStatsNext(float cycle)
    {
        this.AnimateFlashingStatsNextLine(this.linePow, cycle);
        this.AnimateFlashingStatsNextLine(this.lineAim, cycle);
        this.AnimateFlashingStatsNextLine(this.lineSpin, cycle);
        this.AnimateFlashingStatsNextLine(this.lineTime, cycle);
    }
    
    private void AnimateUnlockLine(StatItemStatLine line, float nextStat, in ShopCueRef @ref)
    {
        line.ChangeValueFront(nextStat, 0f);
        line.ChangeColorFront(@ref.colStatNext, 0f);
        line.ChangeColorFront(@ref.colStatCurrent, 1f).SetId(this);
    }
    private void AnimateUpgradeLine(StatItemStatLine line, float nextStat, in ShopCueRef @ref)
    {
        line.ChangeValueBack(nextStat, 0f);
        line.ChangeColorBack(@ref.colStatNext, 0f);
        line.ChangeColorBack(@ref.colStatCurrent, 2f).SetId(this);
    }
    private void AnimateUnlockLine(StatItemStatLine line, in ShopCueRef @ref)
    {
        line.ChangeColorBack(@ref.colStatCurrent, 1f).SetId(this);
    }

    private void AnimateFlashingStatsNextLine(StatItemStatLine line, float cycle)
    {
        this.tween = line.FlashingBack(cycle).SetId(this);
    }

    public void ClearAnim()
    {
        if (tween != null)
        {
            DOTween.Kill(this, true);
        }
    }
}
