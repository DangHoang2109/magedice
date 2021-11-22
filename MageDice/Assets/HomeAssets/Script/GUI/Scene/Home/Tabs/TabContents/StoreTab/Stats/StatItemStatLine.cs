using System.Collections;
using System.Collections.Generic;
using Cosina.Components;
using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using Coffee.UIEffects;
public class StatItemStatLine : MonoBehaviour
{
    public Image bg;
    public TextMeshProUGUI tmpCurrentValue;
    public TextMeshProUGUI tmpNextStatReduce;
    public UIShiny shiny;

    public static Color colorBlack = Color.black;
    public static Color colorUpgrade = new Color(0, 157, 157, 255);


    /// <summary>
    /// value must be 0 ~ 10
    /// </summary>
    public void ParseData(float currentV, float nextV)
    {
        tmpCurrentValue.SetText(currentV.ToString());

        float nextStatReduce = nextV - currentV;
        tmpNextStatReduce.SetText((nextStatReduce > 0) ? $"+{nextStatReduce}" : nextStatReduce.ToString());
        tmpNextStatReduce.gameObject.SetActive(nextStatReduce != 0);

        bg.color = nextStatReduce != 0 ? colorUpgrade : colorBlack;
        shiny.enabled = nextStatReduce != 0;
    }
}
