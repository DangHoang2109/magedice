using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IceDiceEffect : BaseDiceEffect
{
    public override DiceID ID =>  DiceID.ICE;
    public override void ActiveEffect()
    {
        base.ActiveEffect();
        this.ShootBullet(1);
    }

    public override void BulletEffect(BaseMonsterBehavior enemy, float damage)
    {
        base.BulletEffect(enemy, damage);

        enemy.Hitted(damage);
        enemy.Freeze(enemy.MonsterID == MonsterType.SKILL_BOSS ? 0.2f : this.diceStat.timeEffectStrength);
    }
}
