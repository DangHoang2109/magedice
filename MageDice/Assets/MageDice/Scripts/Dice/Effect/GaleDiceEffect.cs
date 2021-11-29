using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GaleDiceEffect : BaseDiceEffect
{
    public override DiceID ID =>  DiceID.GALE;
    public override void ActiveEffect()
    {
        base.ActiveEffect();
        this.ShootBullet(5);
    }

    public override void BulletEffect(BaseMonsterBehavior enemy, float damage)
    {
        base.BulletEffect(enemy, damage);

        enemy.Hitted(damage);
    }
}
