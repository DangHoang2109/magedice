using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TextCurrencyResize : TextCurrency
{
    #if UNITY_EDITOR
    protected override void OnValidate()
    {
        base.OnValidate();
        this.AutoResize();
    }
    #endif

    private RectTransform rectTransform => (RectTransform) this.transform;
    protected override void UpdateText(long value = 0)
    {
        base.UpdateText(value);
        this.AutoResize();
    }

    private void AutoResize()
    {
        this.rectTransform.sizeDelta = new Vector2(this.tmpValue.preferredWidth, this.tmpValue.preferredHeight);

    }
}
