using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GiftDiceEffect : BaseDiceEffect
{
    public override DiceID ID =>  DiceID.GIFT;
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
        MageDiceGameManager.Instance.OnAddCoin((long)(this.diceStat.rangeStrength * this.dot));
    }
}
