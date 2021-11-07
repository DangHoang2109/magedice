using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class TextCurrency : MonoBehaviour
{
    private long curValue;
    public TextMeshProUGUI tmpValue;
    public Image imgIcon;
#if UNITY_EDITOR
    protected virtual void OnValidate()
    {
        this.tmpValue = this.GetComponent<TextMeshProUGUI>();
        this.imgIcon = this.GetComponentInChildren<Image>();
    }
#endif
    public virtual void ParseData(BoosterCommodity booster)
    {
        if (booster.type == BoosterType.NONE)
        {
            return;
        }
        this.UpdateText(booster.GetValue());
        this.imgIcon.sprite = GameAssetsConfigs.Instance.boosters.GetBooster(booster.type)?.spr;
    }
    public virtual void ParseData(long value)
    {
        Debug.Log("<color=yellow>Parse value </color>" + value);
        this.UpdateText(value);
    }
    protected virtual void UpdateText(long value)
    {
        //Debug.Log("<color=blue>Value </color>" + value);
        this.tmpValue.SetText(GameUtils.FormatMoneyDot(value));
    }

    public Sequence AddValueAnimtion(long current, long max, float time = 1.0f, float timeDelay = 0f, UnityAction callback = null)
    {
        this.curValue = current;
        DOTween.Kill(this.GetInstanceID());
        Sequence seq = DOTween.Sequence();
        seq.AppendInterval(timeDelay);
        seq.Append(DOTween.To(() => this.curValue, x => this.curValue = x, max, time)).SetEase(Ease.Linear).OnUpdate(() => {
            this.UpdateText(this.curValue);
        });
        seq.SetId(this.GetInstanceID());
        seq.OnComplete(() =>
        {
            callback?.Invoke();
            Debug.Log("<color=red>callback Invoke</color>");
        });
        return seq;
    }
}
