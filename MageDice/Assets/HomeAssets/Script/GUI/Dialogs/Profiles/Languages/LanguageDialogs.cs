using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class LanguageDialogs : BaseSortingDialog
{
    public ToggleGroup group;
    public Transform panelLanguage;
    public LanguageItem prefabLanguage;

    public override void OnShow(object data = null, UnityAction callback = null)
    {
        base.OnShow(data, callback);
        this.ParseData();
    }

    private void ParseData()
    {
        for (int i = 0; i < 16; i++)
        {
            LanguageItem item = Instantiate(this.prefabLanguage, this.panelLanguage);
            item?.ParseData((LanguageDefine)i, this.group);
        }
    }

    protected override void OnCompleteHide()
    {
        for (int i = 0; i < this.panelLanguage.childCount; i++)
        {
            Destroy(this.panelLanguage.GetChild(i).gameObject);
        }
        base.OnCompleteHide();
    }
}
