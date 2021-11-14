using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using UnityEngine.UI;
public class GameStatBar : MonoBehaviour
{
    [SerializeField] private float _currentValue;
    [SerializeField] private float _maxValue;

    public Image imgBar;

    private Tween _tweenBar;
#if UNITY_EDITOR
    private void OnValidate()
    {
    }
#endif

    public float CurrentValue { get => _currentValue; set => OnChangeValue(value); }
    public float MaxValue { get => _maxValue; set => _maxValue = value; }

    public void ParseData(float max, float current)
    {
        this.MaxValue = max;
        this.CurrentValue = current;
    }
    public void OnChangeValue(float newValue)
    {
        DOTween.Kill(this);

        imgBar.fillAmount = this._currentValue / MaxValue;
        imgBar.DOFillAmount(newValue/MaxValue, 0.1f).SetId(this);
        this._currentValue = newValue;
    }
}
