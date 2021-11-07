using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class CustomToogle : MonoBehaviour
{
    [SerializeField]
    private bool isOn;

    public bool IsOn
    {
        get
        {
            return this.isOn;
        }
        set
        {
            this.isOn = value;
            float x = this.isOn
                ? Mathf.Abs(this.checkMask.localPosition.x)
                : -Mathf.Abs(this.checkMask.localPosition.x); //this.checkMask.localPosition.x * -1;
            this.checkMask.DOLocalMoveX(x, 0.1f).SetEase(Ease.Linear);
            Color c = this.isOn ? colorCheck : colorUnCheck;
            this.imgBackground.color = c;
        }
    }
    [SerializeField]
    private Color colorCheck;
    [SerializeField]
    private Color colorUnCheck;
    [SerializeField]
    private Image imgBackground;
    [SerializeField]
    private Transform checkMask;
    [SerializeField]
    private Button btClick;

    public Toggle.ToggleEvent onClick;
    #if UNITY_EDITOR
    private void OnValidate()
    {
        this.btClick = this.gameObject.GetComponent<Button>();
        float x = this.isOn
            ? Mathf.Abs(this.checkMask.localPosition.x)
            : -Mathf.Abs(this.checkMask.localPosition.x);
        this.checkMask.localPosition = new Vector3(x, this.checkMask.localPosition.y);
        Color c = this.isOn ? colorCheck : colorUnCheck;
        this.imgBackground.color = c;
    }
#endif
    
    public void OnClick()
    {
        this.IsOn = !this.isOn;
        this.onClick.Invoke(this.IsOn);
    }
    
}