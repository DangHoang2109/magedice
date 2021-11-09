using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameBoardCollumn : MonoBehaviour
{
    public GameBoardSlot[] slots;
    public GameObject gEntryLine;

    private bool _isActivated;
    public bool IsActivated => this._isActivated;
    public bool IsFull
    {
        get
        {
            for (int i = 0; i < this.slots.Length; i++)
            {
                if (!this.slots[i].IsPlacing)
                    return false;
            }
            return true;
        }
    }

    public void Active()
    {
        Debug.Log("Active Collumn");
        this._isActivated = true;
        for (int i = 0; i < this.slots.Length; i++)
        {
            this.slots[i].Active();
        }
    }
    public void ResetData()
    {
        this._isActivated = false;
    }

    public void PlaceDice(GameDiceItem dice)
    {
        for (int i = 0; i < this.slots.Length; i++)
        {
            if (!this.slots[i].IsPlacing)
            {
                this.slots[i].PlaceDice(dice);
                return;
            }
        }

        Debug.LogError("There is no free slot");
    }

    private void OnValidate()
    {
        this.slots = this.GetComponentsInChildren<GameBoardSlot>();
    }
}
