using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SlingshotDiceEffect : BaseDiceEffect
{
    public override DiceID ID =>  DiceID.SLINGSHOT;
    public override void ActiveEffect()
    {
        base.ActiveEffect();
        this.ShootBullet(1);
    }

    public override void BulletEffect(BaseMonsterBehavior enemy, float damage)
    {
        base.BulletEffect(enemy, damage);

        enemy.Hitted((enemy.HPRation < 0.5f) ? damage * 2 : damage);
    }
}
