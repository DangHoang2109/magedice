using System.Collections;
using System.Collections.Generic;
using Cosina.Components;
using DG.Tweening;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class UpgradeCardDialog : BaseSortingDialog
{
    private static readonly Color COL_WHITE_CLEAR = new Color(1f, 1f, 1f, 0f);
    
    [Header("Linekrs")]
    public TextMeshProUGUI txtName;
    public Image imgFlashy;
    public ParticleMover fxFlyUp;
    public ParticleSystem fxBangLarge;

    public StatCardItemDisplay displayer;
    public UpgradeCardStats statsDisplayer;

    public Transform transTop;
    public Transform transMid;

    [Space(10f)]
    public TextMeshProUGUI txtStatus;

    [Header("Stats")]
    public float durationFillEmpty;
    
    [Space(5f)]
    public Vector3 scaleFlashyInit;
    public PairTweenVector[] scalesSmall;
    public Color[] colFlashies;

    public PairTweenVector[] seqCardBreakFlash;
    
    private StatData d;

    private Transform transFlashy;

    private int step = 0;
    private Tween tween;
    private bool isInitialized = false;
    private void FirstRun()
    {
        this.transFlashy = this.imgFlashy.GetComponent<Transform>();
        this.isInitialized = true;
    }
    
    public override void OnShow(object data = null, UnityAction callback = null)
    {
        if(!this.isInitialized)
            this.FirstRun();
        
        base.OnShow(data, callback);
        this.ParseData();
    }

    private void ParseData()
    {
        this.step = 0;
        this.d = this.data as StatData;
        if (this.d == null)
        {
            Debug.LogError( $"{"UpgradeCardDialog".WrapColor("red")} ParseData exception: data is not StatData, it is: {data?.GetType().Name??"NULL"}" );
            
            MessageBox.Instance.ShowMessageBox("ParseData exception!",
                $"Data is: {data?.GetType().Name??"NULL"}");
            this.ClickCloseDialog();
            return;
        }
        
        this.ClearTween(false);

        this.imgFlashy.color = COL_WHITE_CLEAR;

        this.txtName.text = this.d.config.statName;
        this.displayer.ParseData(this.d, needCapCount: true);
        this.displayer.CachedTransform.localScale = Vector3.one;
        this.displayer.CachedTransform.localPosition = this.transMid.localPosition;
        this.displayer.lvl.DoFillTo(0, this.durationFillEmpty);
        this.fxFlyUp.Play();
        SoundManager.Instance.Play("sfx_glare");

        // this.tween = DOTween.Sequence()
        //     .AppendInterval(this.durationFillEmpty - scalesSmall[0].duration)
        //     .AppendCallback(this.SlightlyShowFlashy).SetId(this);
        
        this.call = this.SlightlyShowFlashy;
        Invoker.Invoke(this.call, this.durationFillEmpty - scalesSmall[0].duration);
        
        this.txtStatus.text = $"Start step 0";
    }

    private System.Action call;

    private void SlightlyShowFlashy()
    {
        SoundManager.Instance.Play("sfx_glare_mid");
        
        this.txtStatus.text = $"Slightly show flashy";
        this.transFlashy.localScale = this.scaleFlashyInit;
        this.imgFlashy.color = COL_WHITE_CLEAR;
        this.imgFlashy.gameObject.SetActive(true);

        this.tween = DOTween.Sequence()
            .Append(this.transFlashy.DOScale(scalesSmall[0].value, scalesSmall[0].duration)
                .SetEase(scalesSmall[0].ease))
            .Join(this.imgFlashy.DOColor(this.colFlashies[0], scalesSmall[0].duration)
                .SetEase(scalesSmall[0].ease))
            .OnComplete(this.HardlyShowFlashy).SetId(this);
    }

    private void HardlyShowFlashy()
    {
        this.txtStatus.text = $"HardlyShowFlashy";
        this.tween = DOTween.Sequence()
            .Append(this.transFlashy.DOScale(scalesSmall[1].value, scalesSmall[1].duration)
                .SetEase(scalesSmall[1].ease))
            .Join(this.imgFlashy.DOColor(this.colFlashies[1], scalesSmall[1].duration)
                .SetEase(scalesSmall[1].ease))
            .Join(this.displayer.CachedTransform.DOScale(scalesSmall[1].value, scalesSmall[1].duration)
                .SetEase(scalesSmall[1].ease))
            .OnComplete(this.OnFinishedAStep).SetId(this);
    }

    private void BreakFlashy()
    {
        this.txtStatus.text = $"BreakFlashy";
        this.fxFlyUp.Stop();
        
        this.imgFlashy.gameObject.SetActive(false);
        this.fxBangLarge.Play();

        this.tween = DOTween.Sequence()
            .Append(this.displayer.CachedTransform.DOScale(seqCardBreakFlash[0].value, seqCardBreakFlash[0].duration)
                .SetEase(seqCardBreakFlash[0].ease))
            .Append(this.displayer.CachedTransform.DOLocalMove(this.transTop.localPosition, seqCardBreakFlash[1].duration)
                .SetEase(Ease.InOutCubic))
            .OnComplete(this.OnFinishedAStep).SetId(this);
    }

    private void ShowStatPanel()
    {
        this.statsDisplayer.StartAnimate();
    }

    private void TrulyUpgradeCard()
    {
        SoundManager.Instance.Play("sfx_glare_end");
        this.statsDisplayer.ParseData(d.PreviousStats, d.CurrentStats, d.FullStats);
        this.displayer.lvl.ParseCueBought(d);
        //var before = d.CurrentStats;
        //var nex = d.NextStats;
        //if (nex != null)
        //{
        //    if (UserProfile.Instance.UseBooster(nex.price, string.Format("Card_{0}", d.id), LogSinkWhere.UPGRADE_CARD))
        //    {
        //        StatData StatData = StatDatas.Instance.UpgradeItem(d.id);
        //        if (StatData != null)
        //        {
        //            this.displayer.lvl.ParseData(StatData.level, 0, StatData.card,
        //                TennisColor.GetLevelColorByCardType(d.type));

        //            this.statsDisplayer.ParseData(before, d.GetStatsCurrentLevel().stats);

        //            return;
        //        }
        //        else
        //        {
        //            Debug.LogError($"UpgradeCardDialog: upgrade failed! -cardId {d.id} -lvl {d.level}");
        //            MessageBox.Instance.ShowMessageBox("Error!",
        //                $"UpgradeCardDialog: upgrade failed! -card {d.config?.statName ?? d.id.ToString()} -lvl {d.level}");
        //            this.ClickCloseDialog();
        //        }
        //    }
        //    else
        //    {
        //        MessageBox.Instance.ShowMessageBox("Error!",
        //            $"UpgradeCardDialog: upgrade failed! Not enough money: {nex.booster} -card {d.config?.statName ?? d.id.ToString()} -lvl {d.level}");
        //        this.ClickCloseDialog();
        //    }
        //}
        //else
        //{
        //    MessageBox.Instance.ShowMessageBox("Error!",
        //        $"UpgradeCardDialog: upgrade failed! NextStat not found -card {d.config?.statName ?? d.id.ToString()} -lvl {d.level}");
        //    this.ClickCloseDialog();
        //}
    }

    public void OnClickStep()
    {
        if (this.call != null)
        {
            Invoker.CancelInvoke(this.call);
            this.call = null;
        }
        this.ClearTween(true);
        this.OnFinishedAStep();
    }

    private void OnFinishedAStep()
    { 
        ++this.step;
        switch (this.step)
        {
            case 1:
                this.txtStatus.text = $"Complete step 0\nNow Break card";
                this.TrulyUpgradeCard();
                this.BreakFlashy();
                break;
            case 2:
                this.txtStatus.text = $"Complete step 1\nNow show stat";
                this.ShowStatPanel();
                break;
            case 3:
                this.txtStatus.text = $"Complete step 2\nNow wait click";
                // waiting
                break;
            default:
                this.ClickCloseDialog();
                break;
        }
    }
    
    protected override void AnimationShow()
    {
        this.OnCompleteShow();
    }

    protected override void AnimationHide()
    {
        DOTween.Kill(this);
        this.OnCompleteHide();
    }

    private void ClearTween(bool isComplete = false)
    {
        if (this.tween != null)
        {
            this.tween.Kill(isComplete);
            this.tween = null;
        }
    }


    public override void ClickCloseDialog()
    {
        this.statsDisplayer.Clear();
        base.ClickCloseDialog();
    }
}
