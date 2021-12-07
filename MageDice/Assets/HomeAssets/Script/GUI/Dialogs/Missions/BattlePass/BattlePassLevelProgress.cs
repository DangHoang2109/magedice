using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class BattlePassLevelProgress : MonoBehaviour
{
    public TextMeshProUGUI txtProgress;
    public Image imgProgress;
    public Transform tranIcon;

    public TextMeshProUGUI txtLevel;

    [Header("Layout")]
    public Image imgBg;

    private int level;

    private void OnEnable()
    {
        BattlepassDatas.callbackProgress += this.DoProgress;
    }

    private void OnDisable()
    {
        BattlepassDatas.callbackProgress -= this.DoProgress;
    }

    public void ParseData(BattlepassStepData stepData, int level)
    {
        this.level = level;
        this.txtLevel.text = $"{level}";
        this.txtProgress.text = $"{stepData.GetProgress()}";
        this.imgProgress.fillAmount = stepData.GetProgressFill();
    }
    private void DoProgress(BattlepassStepData stepData, int level)
    {    
        this.txtLevel.text = $"{level}";
        this.txtProgress.text = $"{stepData.GetProgress()}";
        if (level <= this.level)
        {
            this.imgProgress.DOFillAmount(stepData.GetProgressFill(), 0.5f).SetEase(Ease.Linear);
        }
        else
        {
            Sequence seq = DOTween.Sequence();
            seq.Append(this.imgProgress.DOFillAmount(1, 0.3f).SetEase(Ease.Linear));
            seq.AppendCallback(() =>
            {
                this.imgProgress.fillAmount = 0;
            });
            seq.Append(this.imgProgress.DOFillAmount(stepData.GetProgressFill(), 0.5f).SetEase(Ease.Linear));
        }

        this.level = level;
    }

    public void AnimCollectPoint()
    {
        DOTween.Kill(this.GetInstanceID() + "collect");

        Sequence seq = DOTween.Sequence();
        seq.AppendInterval(0.5f);
        seq.Append(this.tranIcon.DOPunchScale(new Vector3(0.1f, 0.1f), 1f, 5));
        seq.SetId(this.GetInstanceID() + "collect");
    }

    public void ParseLayout(BattlePassAssetConfig battlePassAsset)
    {
        this.imgBg.color = battlePassAsset.color2;
    }
}
