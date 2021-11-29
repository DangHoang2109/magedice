using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ArrowDiceEffect : BaseDiceEffect
{
    public override DiceID ID =>  DiceID.ARROW;

    private int shootedBullet;

    public override void ActiveEffect()
    {
        base.ActiveEffect();
        this.ShootBullet();
    }
    public override void ShootBullet(int amount = 1)
    {
        shootedBullet += amount;

        base.ShootBullet(amount);
    }
    public override void BulletEffect(BaseMonsterBehavior enemy, float damage)
    {
        base.BulletEffect(enemy, damage);

        if(shootedBullet >= 5)
        {
            shootedBullet = 0;
            enemy.Hitted(damage * this.diceStat.rangeStrength);
        }
        else
            enemy.Hitted(damage);

    }
}
