using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MimicDiceEffect : BaseDiceEffect
{
    public override DiceID ID =>  DiceID.MIMIC;
    public override bool IsCanMergeWithAny => true;
    public override void ActiveEffect()
    {
        base.ActiveEffect();
        this.ShootBullet();
    }

    public override void BulletEffect(BaseMonsterBehavior enemy, float damage)
    {
        base.BulletEffect(enemy, damage);

        enemy.Hitted(damage);
    }
}
