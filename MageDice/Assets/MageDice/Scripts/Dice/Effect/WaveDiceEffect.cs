using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WaveDiceEffect : BaseDiceEffect
{
    public override DiceID ID =>  DiceID.WAVE;
    public override void ActiveEffect()
    {
        base.ActiveEffect();
        this.ShootBullet();
    }

    public override void BulletEffect(BaseMonsterBehavior enemy, float damage)
    {
        base.BulletEffect(enemy, damage);

        Debug.Log("ration " + enemy.DistanceRation);

        enemy.Hitted((int)(damage * enemy.DistanceRation / 2));
    }
}
