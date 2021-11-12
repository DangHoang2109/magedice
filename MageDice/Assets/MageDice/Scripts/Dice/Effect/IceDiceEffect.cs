using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IceDiceEffect : BaseDiceEffect
{
    public override DiceID ID =>  DiceID.ICE;
    public override void ActiveEffect()
    {
        base.ActiveEffect();
        this.ShootBullet(1);
    }

    public override void BulletEffect(BaseMonsterBehavior enemy)
    {
        base.BulletEffect(enemy);

        enemy.Hitted(this.GameConfig.damage);
        enemy.Freeze(this.GameConfig.timeEffect);
    }
}
