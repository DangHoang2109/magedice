using System.Collections;
using System.Collections.Generic;
using Coffee.UIEffects;
using Cosina.Components;
using DG.Tweening;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class StatsCardLevelV2 : MonoBehaviour
{
    [Header("Linkers")]
    public GameObject panel;
    public TextMeshProUGUI txtLevel;
    public TextMeshProUGUI txtProgress;
    public Image imgProgress;
    
    public Image imgFill;
    public GameObject panelUpgrade;

    public GameObject goEXP;
    
    private long cur;
    private long newCur;
    private long tot;

    private System.Action callback;
    
    public void Show(bool isShow)
    {
        this.panel.SetActive(isShow);
    }
    /// <summary>
    /// 
    /// </summary>
    /// <param name="level">level hien tai</param>
    /// <param name="current">card hien tai</param>
    /// <param name="total">tong card can upgarde</param>
    public StatsCardLevelV2 ParseData(int level, long current, long total, in Color col,  bool haveUpgrade = true)
    {
        DOTween.Kill(this);
        
        this.Show(true);
        this.goEXP.SetActive(true);
        
        this.cur = current;
        this.tot = total;

        this.OnUpdateFill(current);
        // ensure no miscalculation from rounding float
        this.txtProgress.text = string.Format("{0}/{1}", current, total);
        this.txtLevel.text = string.Format("{0}", level + 1);
        this.txtLevel.color = col;

        if (current >= total)
        {
            this.imgProgress.color = ColorCommon.ColorProgressLightGreen;
            if (this.panelUpgrade != null)
            {
                this.panelUpgrade.SetActive(haveUpgrade);    
            }
        }
        else
        {
            this.imgProgress.color = ColorCommon.ColorProgressLightBlue;
            if (this.panelUpgrade != null)
            {
                this.panelUpgrade.SetActive(false);    
            }
        }

        return this;
    }

    public StatsCardLevelV2 ParseDataMaxed(int level, int current, in Color col)
    {
        DOTween.Kill(this);
        this.Show(true);
        this.goEXP.SetActive(true);
        
        this.cur = current;
        this.tot = 0;
        
        this.imgFill.fillAmount = 1f;
        this.txtProgress.text = string.Format("{0}/{1}", current, "MAX");
        this.txtLevel.text = string.Format("{0}", level + 1);
        this.txtLevel.color = col;

        this.imgProgress.color = ColorCommon.ColorProgressLightGreen;
        this.panelUpgrade.SetActive(false);
        
        return this;
    }

    public StatsCardLevelV2 ParseData(int level, in Color col)
    {
        DOTween.Kill(this);
        this.Show(true);
        this.goEXP.SetActive(false);
        
        this.txtLevel.text = string.Format("{0}", level + 1);
        this.txtLevel.color = col;

        return this;
    }

    public void SetLevel(string level)
    {
        this.txtLevel.text = level;
    }

    public StatsCardLevelV2 DoFillTo(in long newCurrent, in float duration, in Ease ease = Ease.Unset)
    {
        this.newCur = newCurrent;
        
        if (ease == Ease.Unset)
        {
            DOTween.To(this.GetCurrent, this.OnUpdateFill, newCurrent, duration)
                .OnComplete(this.OnFillCompleted).SetId(this);
        }
        else
        {
            DOTween.To(this.GetCurrent, this.OnUpdateFill, newCurrent, duration)
                .OnComplete(this.OnFillCompleted).SetEase(ease).SetId(this);
        }

        return this;
    }

    public void SkipFill(bool isComplete = false)
    {
        var tws = DOTween.TweensById(this);
        if (tws is null || tws.Count == 0)
            return;

        DOTween.Kill(this, isComplete);
        if (isComplete)
        {
            this.OnFillCompleted();
        }
    }

    public StatsCardLevelV2 OnComplete(System.Action call)
    {
        this.callback = call;
        return this;
    }

    private float GetCurrent()
    {
        return (float)this.cur;
    }

    private void OnUpdateFill(float current)
    {
        this.imgFill.fillAmount = current / (float)this.tot;
        this.txtProgress.text = string.Format("{0}/{1}", Mathf.RoundToInt(current), this.tot);
    }

    private void OnFillCompleted()
    {
        this.OnUpdateFill(this.cur = this.newCur);
        // ensure no miscalculation from rounding float
        this.txtProgress.text = string.Format("{0}/{1}", this.cur, this.tot);
        
        if (this.cur >= this.tot)
        {
            this.imgProgress.color = ColorCommon.ColorProgressLightGreen;
            if (this.panelUpgrade != null)
            {
                this.panelUpgrade.SetActive(true);    
            }
        }
        else
        {
            this.imgProgress.color = ColorCommon.ColorProgressLightBlue;
            if (this.panelUpgrade != null)
            {
                this.panelUpgrade.SetActive(false);    
            }
        }

        if (this.callback == null)
            return;
        
        var c2 = callback;
        this.callback = null;
        c2.Invoke();
    }
    
    #if UNITY_EDITOR
    [ContextMenu("Full to empty -d 0.5s")]
    private void TestFillFullToEmpty()
    {
        if (!Application.isPlaying)
            return;
        
        this.ParseData(Random.Range(0, 10), 69, 69, Color.yellow, false);
        Invoker.Invoke(() =>
        {
            this.DoFillTo(0, 1.5f, Ease.Linear);
        }, 0.5f);
    }
    #endif
}
