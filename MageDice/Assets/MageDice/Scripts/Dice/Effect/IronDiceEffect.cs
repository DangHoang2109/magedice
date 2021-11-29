using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IronDiceEffect : BaseDiceEffect
{
    public override DiceID ID =>  DiceID.IRON;
    public override void ActiveEffect()
    {
        base.ActiveEffect();
        this.ShootBullet();
    }

    public override void BulletEffect(BaseMonsterBehavior enemy, float damage)
    {
        base.BulletEffect(enemy, damage);

        enemy.Hitted(enemy.IsBoss ? damage * 2 : damage);
    }
}
