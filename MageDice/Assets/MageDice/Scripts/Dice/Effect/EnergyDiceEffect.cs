using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnergyDiceEffect : BaseDiceEffect
{
    public override DiceID ID =>  DiceID.ENERGY;
    public override void ActiveEffect()
    {
        base.ActiveEffect();
        this.ShootBullet();
    }

    public override void BulletEffect(BaseMonsterBehavior enemy, float damage)
    {
        base.BulletEffect(enemy, damage);

        float percentCoin = (float)MageDiceGameManager.Instance.CoinController.CurrentCoin / 1000f;
        percentCoin = percentCoin > 1f ? 1f : percentCoin;

        enemy.Hitted(damage * (1f + percentCoin));
    }
}
