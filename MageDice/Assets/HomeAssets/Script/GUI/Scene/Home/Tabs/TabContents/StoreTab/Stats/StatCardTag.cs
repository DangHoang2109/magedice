using System;
using System.Collections;
using System.Collections.Generic;
using Coffee.UIEffects;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class StatCardTag : MonoBehaviour
{
    private const float X = 35f;
    private const float Y = 8.5f;
    
    
    public enum TagType
    {
        None,
        New,
        Equipped,
    }
    
    [Header("Linkers")]
    public Image imgTag;
    public TextMeshProUGUI txtTag;

    private RectTransform rectTag;

    private TagType tag;
    public TagType Tag => tag;
    
    
    public void ParseData(TagType tagType)
    {
        if (this.rectTag == null)
            this.rectTag = this.imgTag.GetComponent<RectTransform>();
        
        switch (this.tag = tagType)
        {
            case TagType.None:
                this.Show(false);
                return;// this is a RETURN ! ! !
            
            case TagType.New:
                this.Show(true);
                this.txtTag.text = LanguageManager.GetString("TITLE_NEW");
                this.imgTag.color = ColorCommon.ColorTagNew;
                break;
            case TagType.Equipped:
                this.Show(true);
                this.txtTag.text = LanguageManager.GetString("TITLE_EQUIPPED");
                this.imgTag.color = ColorCommon.ColorTagEquipped;
                break;
            default:
                throw new ArgumentOutOfRangeException(nameof(tagType), tagType, null);
        }

        var s = this.rectTag.sizeDelta;
        s.x = X + Y * this.txtTag.text.Length;
        this.rectTag.sizeDelta = s;
    }

    public void Show(bool isShow)
    {
        this.gameObject.SetActive(isShow);
    }
}
