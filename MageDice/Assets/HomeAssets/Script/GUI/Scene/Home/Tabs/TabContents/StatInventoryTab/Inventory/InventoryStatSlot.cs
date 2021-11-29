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

    public InventoryDiceItem displayer;
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
            if(this.imgDice != null)
                this.imgDice.sprite = _data.config.sprStatItem;

            this.tmpLevel.SetText($"LV {_data.level}");

            if(this.displayer != null)
            {
                BaseDiceData diceData = new BaseDiceData();
                diceData.SetData<BaseDiceData>(this._data.id);

                this.displayer.SetData(diceData);
            }
        }
    }
    public void SetClick(System.Action<DiceID> onClick)
    {
        if (this.displayer != null)
            this.displayer.SetCallback(onClick);
    }
}
