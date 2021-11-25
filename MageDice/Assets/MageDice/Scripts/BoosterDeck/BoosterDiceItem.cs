using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
public class BoosterDiceItem : MonoBehaviour
{
    private DiceID _id;
    private int _currentLevel;

    public Image imgDice;
    public TextMeshProUGUI tmpAmount;
    public TextMeshProUGUI tmpPrice;

    public System.Action<DiceID, BoosterDiceItem> onUpgrade;

    public int CurrentLevel { get => _currentLevel; 
        set 
        { 
            _currentLevel = value;
        } 
    }

    public void Init(DiceID id, System.Action<DiceID, BoosterDiceItem> callbackUpgrade)
    {
        this._id = id;
        this.onUpgrade = callbackUpgrade;
        this.imgDice.sprite = ShopStatConfigs.Instance.GetConfig(id).sprStatItem;
        this.CurrentLevel = 0;
    }
    public void OnDiceAmountChange(int currentAmount)
    {
        this.tmpAmount.SetText(currentAmount.ToString());
    }

    public void OnClickUpgrade()
    {
        this.onUpgrade?.Invoke(this._id, this);
    }
    public void OnUpgradeSuccess(int newLevel, long cost)
    {
        CurrentLevel = newLevel;
        if (cost <= 0)
        {
            this.tmpPrice.SetText("Max");
        }
        else
        {
            this.tmpPrice.SetText(cost.ToString());
        }
    }
}
