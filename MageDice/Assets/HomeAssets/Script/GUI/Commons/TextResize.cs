using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class TextResize : MonoBehaviour
{
    public TextMeshProUGUI tmpValue;
#if UNITY_EDITOR
    protected void OnValidate()
    {
        this.tmpValue = this.GetComponentInChildren<TextMeshProUGUI>();
        this.AutoResize();
        
    }
#endif

    private RectTransform rectTransform => (RectTransform) this.transform;
    public void UpdateText(long value = 0)
    {
        this.AutoResize();
    }

    private void AutoResize()
    {
        this.rectTransform.sizeDelta = new Vector2(this.tmpValue.preferredWidth * 1.1f, this.rectTransform.sizeDelta.y);

    }
}
