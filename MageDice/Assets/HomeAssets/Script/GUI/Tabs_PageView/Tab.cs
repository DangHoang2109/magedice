using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Tab : Toggle
{
    public int tabIndex;
    private TabBase tabBase;

    public void OnClickChangeTab(bool isOn)
    { 
        if(this.isOn)
        {
            if (this.tabBase != null)
            {
                this.tabBase.ChangeTab(this.tabIndex);
            }
            SoundManager.Instance.PlayButtonClick();
        }

    }
    public virtual void Init(TabBase tab)
    {
        this.tabBase = tab;
        this.onValueChanged.AddListener(this.OnClickChangeTab);
    }
}
