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
        if(_tweenBar != null)
            DOTween.Kill(_tweenBar);

        imgBar.fillAmount = this._currentValue;
        _tweenBar = imgBar.DOFillAmount(newValue/MaxValue, 0.1f);
        this._currentValue = newValue;
    }
}
