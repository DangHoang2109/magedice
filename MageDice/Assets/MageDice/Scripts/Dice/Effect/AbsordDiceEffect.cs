using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AbsordDiceEffect : BaseDiceEffect
{
    public override DiceID ID =>  DiceID.ABSORD;
    public override void ActiveEffect()
    {
        base.ActiveEffect();
        this.ShootBullet();
    }

    public override void BulletEffect(BaseMonsterBehavior enemy, float damage)
    {
        base.BulletEffect(enemy, damage);

        enemy.Hitted(damage);
        MageDiceGameManager.Instance.OnAddHP(this.diceStat.rangeStrength);
    }
}
