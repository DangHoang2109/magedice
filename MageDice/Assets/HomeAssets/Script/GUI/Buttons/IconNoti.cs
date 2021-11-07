using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

[System.Serializable]
public class NotiTextUI
{
    public RectTransform rectNoti;
    protected TextMeshProUGUI tmpNoti;
    public void ShowNoti(string text)
    {
        if (this.tmpNoti == null) 
            this.tmpNoti = this.rectNoti.GetComponentInChildren<TextMeshProUGUI>();
        this.tmpNoti?.SetText(text);
        OnNoti(true);
    }

    public void HideNoti()
    {
        this.rectNoti.gameObject.SetActive(false);
    }

    public void OnNoti(bool isOn)
    {
        this.rectNoti.gameObject.SetActive(isOn);
    }
}


public class IconNoti : BaseIcon
{
   public NotiTextUI noti;

    private void Update()
    {
        this.CustomUpdate();
    }

    protected virtual void CustomUpdate()
    {

    }

    public virtual void OnNoti(bool onNoti)
    {
        this.noti.OnNoti(onNoti);
    }

    public virtual void ShowNoti(string text)
    {
        this.noti.ShowNoti(text);
    }
}
