using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PoisionDiceEffect : BaseDiceEffect
{
    public override DiceID ID =>  DiceID.POISION;
    public override void ActiveEffect()
    {
        base.ActiveEffect();

        Debug.Log("poision fire 1 bullet");
        this.ShootBullet(1);
    }

    public override void BulletEffect(BaseMonsterBehavior enemy)
    {
        base.BulletEffect(enemy);

        enemy.Hitted();
    }
}
