using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CriticalDiceEffect : BaseDiceEffect
{
    public override DiceID ID =>  DiceID.CRITICAL;
    public override bool RandomCritical => Random.value < this.perkBulletCritical * 3;
    public override void ActiveEffect()
    {
        base.ActiveEffect();
        this.ShootBullet(1);
    }

    public override void BulletEffect(BaseMonsterBehavior enemy, float damage)
    {
        base.BulletEffect(enemy, damage);

        enemy.Hitted(damage);
    }
}
