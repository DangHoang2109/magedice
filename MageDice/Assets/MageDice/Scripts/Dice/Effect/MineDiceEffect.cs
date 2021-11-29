using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MineDiceEffect : BaseDiceEffect
{
    public override DiceID ID =>  DiceID.MINE;
    public override void ActiveEffect()
    {
        base.ActiveEffect();
        ProduceCoin();
    }

    private void ProduceCoin()
    {
        MageDiceGameManager.Instance.OnAddCoin((long)(this.diceStat.rangeStrength * this.dot));
    }
}
