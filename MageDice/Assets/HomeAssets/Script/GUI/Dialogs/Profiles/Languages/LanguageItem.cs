using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class LanguageItem : MonoBehaviour
{
    public Toggle toggle;
    public TextMeshProUGUI txtLanguage;
    private LanguageDefine id;
    public Color colorCheck;
    public Color colorUnCheck;

    public void ParseData(LanguageDefine id, ToggleGroup group)
    {
        this.id = id;
        this.toggle.group = group;
        if (this.id == LanguageManager.Instance.Language)
        {
            this.toggle.isOn = true;
        }
        this.UpdateText();

        this.txtLanguage.text = string.Format("{0}", id.ToString());
    }

    public void ClickChangeLanguage(bool isOn)
    {
        if (isOn)
        {
            LanguageManager.Instance.ChangeLanguage(this.id);
        }
        this.UpdateText();
    }

    private void UpdateText()
    {
        this.txtLanguage.color = this.toggle.isOn ? this.colorCheck : this.colorUnCheck;
    }
}
