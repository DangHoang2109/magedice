using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameBoardSlot : MonoBehaviour
{
    public GameDiceItem item;
    public Transform tfPlaceDice;
    public bool IsPlacing => this.item != null;

    public void PlaceDice(GameDiceItem item)
    {
        if (!IsPlacing)
        {
            item.transform.SetParent(this.tfPlaceDice);
            item.transform.localScale = Vector3.one;
            item.transform.localPosition = Vector3.zero;
            this.item = item;
        }
    }

    public void RemoveDice()
    {
        this.item = null;
    }
    public void Clear()
    {
        this.RemoveDice();
    }
    public void Active()
    {
        if (this.IsPlacing)
            this.item.Active();
    }
}
