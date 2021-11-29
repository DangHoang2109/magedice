using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CloneDiceEffect : BaseDiceEffect
{
    public override DiceID ID =>  DiceID.CLONE;

    //no use this
    public override void ActiveEffect()
    {
        base.ActiveEffect();
        this.ShootBullet();
    }

    public override void BulletEffect(BaseMonsterBehavior enemy, float damage)
    {
        base.BulletEffect(enemy, damage);
    }
}
