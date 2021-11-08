using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameDiceSlot : MonoBehaviour
{
    private int row;
    public int Row => row;
    private int col;
    public int Column => col;


    public GameDiceItem item;
    public GameDiceData data
    {
        get
        {
            if (this.item != null)
                return this.item.Data;

            return null;
        }        
    }

    public void LineSwipe()
    {
        if (this.item != null)
            this.item.Active();
    }
}
