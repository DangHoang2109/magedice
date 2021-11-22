using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InventoryLineUp : MonoBehaviour
{
    [SerializeField]
    InventoryStatDeck decks;

    int _currentIndex;

    public int CurrentIndex { get => _currentIndex; set => _currentIndex = value; }


    public void ParseData()
    {
        decks.ParseData(StatDatas.Instance.CurrentStatId);
    }
    public void OnLineUpChange(List<StatData> s)
    {
        decks.ParseData(s);
    }
    public void Refresh()
    {
        ParseData();
    }
}
