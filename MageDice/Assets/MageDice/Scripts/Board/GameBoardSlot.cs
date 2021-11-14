using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
public class GameBoardSlot : MonoBehaviour
{
    public GameDiceItem item;
    public Transform tfPlaceDice;
    public bool IsPlacing => this.item != null;

    public void PlaceDice(GameDiceItem item)
    {
        if (!IsPlacing)
        {
            this.item = item;

            item.transform.SetParent(this.tfPlaceDice);
            item.transform.localScale = Vector3.zero;
            item.transform.localPosition = Vector3.zero;

            item.transform.DOScale(Vector3.one,0.15f);

            item.Place(this);
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
    public void Block(bool isBlock)
    {
        if(this.IsPlacing)
            this.item.Block(isBlock);
    }
    public void SetTransformDice()
    {
        item.transform.SetParent(this.tfPlaceDice);
        item.transform.localScale = Vector3.one;
        item.transform.localPosition = Vector3.zero;
    }
}
