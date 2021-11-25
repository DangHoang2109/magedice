using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BoosterDiceDeck : MonoBehaviour
{
    public class BoosterDiceData
    {
        public DiceID _id;
        public int _currentLevel;
        public bool _isMax;
        public float _currentBoostPercent;
        public long _priceUpgradeNext;

        public BoosterDiceData(DiceID id, DiceBoosterConfig config)
        {
            _id = id;
            _currentLevel = 0;
            _isMax = false;

            _currentBoostPercent = config.currentBoostPercent;
            _priceUpgradeNext = config.costUpgradeNext;
        }


    }

    public BoosterDiceItem[] items;

    private DiceBoosterConfigs boosterConfig;

    public Dictionary<DiceID, BoosterDiceItem> dicItem;
    public Dictionary<DiceID, BoosterDiceData> dicBoosterDiceStat;

#if UNITY_EDITOR
    private void OnValidate()
    {
        List<BoosterDiceItem> t = new List<BoosterDiceItem>(GetComponentsInChildren<BoosterDiceItem>());
        items = t.ToArray() ;
    }
#endif

    public void ParseData(List<DiceID> ids)
    {
        boosterConfig = DiceConfigs.Instance.boosterConfigs;

        dicItem = new Dictionary<DiceID, BoosterDiceItem>();
        this.dicBoosterDiceStat = new Dictionary<DiceID, BoosterDiceData>();
        DiceBoosterConfig config = this.boosterConfig.GetLevel(0);

        for (int i = 0; i < items.Length; i++)
        {
            items[i].Init(ids[i], OnBoosterUpgrade);
            dicBoosterDiceStat.Add(ids[i], new BoosterDiceData(ids[i], config));
            items[i].OnUpgradeSuccess(0, config.costUpgradeNext);

            dicItem.Add(ids[i], items[i]);
        }

    }
    public void OnBoosterUpgrade(DiceID id, BoosterDiceItem item)
    {
        this.dicBoosterDiceStat[id] = GameBoardManager.Instance.UpgradeBooster(this.dicBoosterDiceStat[id]);
        item.OnUpgradeSuccess(this.dicBoosterDiceStat[id]._currentLevel, this.dicBoosterDiceStat[id]._priceUpgradeNext);
    }

    public void OnDotChange(DiceID id, int newValue)
    {
        Debug.Log($"{id} change to {newValue}");
        this.dicItem[id].OnDiceAmountChange(newValue);
    }
}
