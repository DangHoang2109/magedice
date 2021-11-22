using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PoisionDiceEffect : BaseDiceEffect
{
    public override DiceID ID =>  DiceID.POISION;
    public override void ActiveEffect()
    {
        base.ActiveEffect();
        this.ShootBullet(1);
    }

    public override void BulletEffect(BaseMonsterBehavior enemy)
    {
        base.BulletEffect(enemy);

        enemy.Hitted(this.Damage);
        enemy.Poision(this.diceStat.timeEffectStrength, this.diceStat.timeEffectStrength / 5, Mathf.Floor(this.Damage / 10) <= 0 ? 1f : Mathf.Floor(this.Damage / 10));
    }
}
