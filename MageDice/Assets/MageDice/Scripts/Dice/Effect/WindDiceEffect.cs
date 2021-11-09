using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WindDiceEffect : BaseDiceEffect
{
    public override DiceID ID =>  DiceID.WIND;
    public override void ActiveEffect()
    {
        base.ActiveEffect();

        Debug.Log("wind fire 3 bullet");
        this.ShootBullet(3);
    }

    public override void BulletEffect(BaseMonsterBehavior enemy)
    {
        base.BulletEffect(enemy);

        enemy.Hitted();
    }
}
