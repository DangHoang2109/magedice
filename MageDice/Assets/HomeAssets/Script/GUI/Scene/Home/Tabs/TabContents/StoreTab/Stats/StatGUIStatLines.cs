using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;

public class StatGUIStatLines : MonoBehaviour
{
    //[SerializeField]
    //private Image imgBG;
    
    [Header("Stat of 4 lines")]
    [SerializeField]
    private StatItemStatLine lineDamane;
    [SerializeField]
    private StatItemStatLine lineRange;
    [SerializeField]
    private StatItemStatLine lineSpeed;
    [SerializeField]
    private StatItemStatLine lineTime;
    
    private void Awake()
    {
        //this.colBg = this.imgBG.color;
    }

    public void ParseStats(StatItemStats s)
    {
        this.lineDamane.ParseData(s.damageStrength, s.damageStrength);
        this.lineRange.ParseData(s.rangeStrength, s.rangeStrength);
        this.lineSpeed.ParseData(s.speedStrength, s.speedStrength);
        this.lineTime.ParseData(s.timeEffectStrength, s.timeEffectStrength);
    }
    public void ParseStatsCurrent(StatData c)
    {
        StatItemStats cur = c.CurrentStats;
        ParseStats(cur);
    }

    public void ParseStatsCurrentNNext(StatData c)
    {
        StatItemStats cur = c.CurrentStats;
        StatItemStats next = c.NextStats;

        this.lineDamane.ParseData(cur.damageStrength, next.damageStrength);
        this.lineRange.ParseData(cur.rangeStrength, next.rangeStrength);
        this.lineSpeed.ParseData(cur.speedStrength, next.speedStrength);
        this.lineTime.ParseData(cur.timeEffectStrength, next.timeEffectStrength);
    }

    public void ParseStatsNext(StatData c)
    {
        StatItemStats next = c.NextStats;
        Debug.Log("ParseStatsNext");

        this.lineDamane.ParseData(next.damageStrength, next.damageStrength);
        this.lineRange.ParseData( next.rangeStrength, next.rangeStrength);
        this.lineSpeed.ParseData(next.speedStrength, next.speedStrength);
        this.lineTime.ParseData(next.timeEffectStrength, next.timeEffectStrength);
    }

    public void ParseStatsNextNMax(StatData c)
    {
        StatItemStats next = c.NextStats;
        StatItemStats max = c.FullStats;
        Debug.Log("edit");


        this.lineDamane.ParseData(next.damageStrength, max.damageStrength);
        this.lineRange.ParseData(next.rangeStrength, max.rangeStrength);
        this.lineSpeed.ParseData(next.speedStrength, max.speedStrength);
        this.lineTime.ParseData(next.timeEffectStrength, max.timeEffectStrength);
    }


    ///// <summary>
    ///// animate upgrade the current stats to the next stats s
    ///// </summary>
    ///// <param name="s">the next stats</param>
    //public void AnimateUpgrade(StatItemStats s/*, TweenCallback callback*/)
    //{
    //    this.ClearAnim();

    //    if (s == null)
    //    {
    //        Debug.LogException(new System.Exception(
    //            "CueStatLines AnimateUpgrade error: s NULL!"));
    //        return;
    //    }

        
        
    //    SoundManager.Instance.Play("sfx_pellet_start");

    //    Debug.Log("edit");

    //    ShopCueRef @ref = ShopCueRef.Instance;
    //    this.AnimateUnlockLine(this.lineDamane, s.damageStrength, @ref);
    //    this.AnimateUnlockLine(this.lineRange, s.speedStrength, @ref);
    //    this.AnimateUnlockLine(this.lineSpeed, s.rangeStrength, @ref);
    //    this.AnimateUnlockLine(this.lineTime, s.timeEffectStrength, @ref);
    //}

    ///// <summary>
    ///// animate upgrade the current stats to the next stats s
    ///// </summary>
    ///// <param name="s">the next stats</param>
    //public void AnimateUnlock(StatItemStats s)
    //{
    //    this.ClearAnim();

    //    Debug.Log("edit");

    //    ShopCueRef @ref = ShopCueRef.Instance;

    //    this.imgBG.color = @ref.colStatCurrent;
    //    this.tween = this.imgBG.DOColor(this.colBg, 1f)
    //        .SetId(this);

    //    this.AnimateUnlockLine(this.lineDamane, s.damageStrength, @ref);
    //    this.AnimateUnlockLine(this.lineRange, s.speedStrength, @ref);
    //    this.AnimateUnlockLine(this.lineSpeed, s.rangeStrength, @ref);
    //    this.AnimateUnlockLine(this.lineTime, s.timeEffectStrength, @ref);
    //}

    //public void AnimateFlashingStatsNext(float cycle)
    //{
    //    this.AnimateFlashingStatsNextLine(this.lineDamane, cycle);
    //    this.AnimateFlashingStatsNextLine(this.lineRange, cycle);
    //    this.AnimateFlashingStatsNextLine(this.lineSpeed, cycle);
    //    this.AnimateFlashingStatsNextLine(this.lineTime, cycle);
    //}
    
    //private void AnimateUnlockLine(StatItemStatLine line, float nextStat, in ShopCueRef @ref)
    //{
    //    line.ChangeValueFront(nextStat, 0f);
    //    line.ChangeColorFront(@ref.colStatNext, 0f);
    //    line.ChangeColorFront(@ref.colStatCurrent, 1f).SetId(this);
    //}
    //private void AnimateUpgradeLine(StatItemStatLine line, float nextStat, in ShopCueRef @ref)
    //{
    //    line.ChangeValueBack(nextStat, 0f);
    //    line.ChangeColorBack(@ref.colStatNext, 0f);
    //    line.ChangeColorBack(@ref.colStatCurrent, 2f).SetId(this);
    //}
    //private void AnimateUnlockLine(StatItemStatLine line, in ShopCueRef @ref)
    //{
    //    line.ChangeColorBack(@ref.colStatCurrent, 1f).SetId(this);
    //}

    //private void AnimateFlashingStatsNextLine(StatItemStatLine line, float cycle)
    //{
    //    this.tween = line.FlashingBack(cycle).SetId(this);
    //}

    //public void ClearAnim()
    //{
    //    if (tween != null)
    //    {
    //        DOTween.Kill(this, true);
    //    }
    //}
}
