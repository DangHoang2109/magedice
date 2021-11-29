using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HealDiceEffect : BaseDiceEffect
{
    public override DiceID ID =>  DiceID.HEAL;
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
    public override void EffectWhenMerged()
    {
        base.EffectWhenMerged();
        MageDiceGameManager.Instance.OnAddHP(this.diceStat.rangeStrength);
    }
    public override void EffectWhenSpawned()
    {
        base.EffectWhenSpawned();
        MageDiceGameManager.Instance.OnAddHP(this.diceStat.rangeStrength);
    }
}
