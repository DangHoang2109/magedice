using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PoisionDiceEffect : BaseDiceEffect
{
    public override DiceID ID =>  DiceID.POISION;
    public override void ActiveEffect()
    {
        base.ActiveEffect();
        this.ShootBullet(1);
    }

    public override void BulletEffect(BaseMonsterBehavior enemy)
    {
        base.BulletEffect(enemy);

        enemy.Hitted(this.GameConfig.damage);
        enemy.Poision(this.GameConfig.timeEffect, this.GameConfig.timeEffect / 5, Mathf.Floor(this.GameConfig.damage / 10) <= 0 ? 1f : Mathf.Floor(this.GameConfig.damage / 10));
    }
}
