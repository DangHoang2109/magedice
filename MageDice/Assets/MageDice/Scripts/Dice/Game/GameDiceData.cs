using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameDiceData : BaseDiceData
{
    int dot;
    public int Dot => this.dot;

    public BaseDiceEffect diceEffect;
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
    public virtual T SetEffect<T>(StatItemStats userStat) where T : BaseDiceData
    {
        this.diceEffect = Activator.CreateInstance(EnumUtility.GetStringType(this.id)) as BaseDiceEffect;
        this.diceEffect.GameConfig = this.Config.Game.levels[this.Dot-1];
        this.diceEffect.diceStat = userStat;
        this.diceEffect.UIConfig = this.Config.Game.bullet;

        return this as T;
    }
    public virtual void ActiveEffect()
    {
        this.diceEffect.ActiveEffect();
    }
}
