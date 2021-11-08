using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameDiceData : BaseDiceData
{
    int dot;
    public int Dot => this.dot;
    public override void ClearData()
    {
        base.ClearData();
    }
    public override T SetData<T>(DiceID id, DiceConfig config = null)
    {
        base.SetData<T>(id, config);

        return this as T;
    }
    public virtual T SetDot<T>(int dot) where T : BaseDiceData
    {
        if (dot >= 0 && dot <= 6)
        {
            this.dot = dot;
        }
        else Debug.LogError($"Dot? {dot}");

        return this as T;
    }

    public virtual void ActiveEffect()
    {
        Debug.Log($"Dice {this.id} {this.Dot} activated ");
    }
}
