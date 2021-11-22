using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using UnityEngine.UI;
using TMPro;
public class InventoryStatSlot : MonoBehaviour
{
    private StatData _data;

    public TextMeshProUGUI tmpLevel;
    public Image imgDice;

    public void ParseData(StatData _data)
    {
        this._data = _data;
        Display();
    }
    public void Display()
    {
        if(this._data != null)
        {
            this.imgDice.sprite = _data.config.sprStatItem;
            this.tmpLevel.SetText($"LV {_data.level}");
        }
    }
}
