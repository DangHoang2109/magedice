using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RectResizeBooster : IBooster
{
    [Header("Resize rect")]
    public RectTransform rect;
    public float sizePerChar; //size của 1 chữ
    public float sizeMin = 20;
    public float sizeMax = 500;

#if UNITY_EDITOR
    protected override void OnValidate()
    {
        base.OnValidate();
        if (this.rect == null)
        {
            this.rect = this.GetComponent<RectTransform>();  
        }
        this.sizePerChar = this.rect.sizeDelta.x / this.tmpValue.text.Length;
    }
#endif

    protected override void SetValueText(string text)
    {
        base.SetValueText(text);
        Vector2 size = new Vector2(text.Length * sizePerChar, this.rect.sizeDelta.y);
        if (size.x < sizeMin) size.x = sizeMin;
        if (size.x > sizeMax) size.x = sizeMax;
        this.rect.sizeDelta = size;
    }
}
