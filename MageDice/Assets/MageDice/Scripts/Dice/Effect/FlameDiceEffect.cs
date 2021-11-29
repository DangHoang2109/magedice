using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FlameDiceEffect : BaseDiceEffect
{
    public override DiceID ID =>  DiceID.FLAME;
    public override void ActiveEffect()
    {
        base.ActiveEffect();
        this.ShootBullet(1);
    }

    public override void BulletEffect(BaseMonsterBehavior enemy, float damage)
    {
        base.BulletEffect(enemy, damage);

        enemy.Hitted(damage);
        enemy.Poision(this.diceStat.timeEffectStrength, this.diceStat.timeEffectStrength / 5, Mathf.Floor(this.Damage / 2) <= 0 ? 1f : Mathf.Floor(this.Damage / 2));
    }
}
