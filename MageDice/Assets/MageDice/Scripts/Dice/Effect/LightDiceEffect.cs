using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LightDiceEffect : BaseDiceEffect
{
    public override DiceID ID =>  DiceID.LIGHT;
    public override void ActiveEffect()
    {
        Debug.Log("not config");
        base.ActiveEffect();
        this.ShootBullet();
    }

    public override void BulletEffect(BaseMonsterBehavior enemy, float damage)
    {
        base.BulletEffect(enemy, damage);

        enemy.Hitted(damage);
    }
}
