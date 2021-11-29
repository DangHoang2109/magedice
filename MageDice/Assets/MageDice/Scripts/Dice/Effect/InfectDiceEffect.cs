using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InfectDiceEffect : BaseDiceEffect
{
    public override DiceID ID =>  DiceID.INFECT;
    public override void ActiveEffect()
    {
        base.ActiveEffect();
        this.ShootBullet(1);
    }

    public override void BulletEffect(BaseMonsterBehavior enemy, float damage)
    {
        base.BulletEffect(enemy, damage);

       bool isKill = enemy.Hitted(damage);
        if (isKill)
        {
            List<BaseMonsterBehavior> infectEfft = MonsterManager.Instance.GetNearestMonsters(3, enemy.transform, enemy.Id);
            if (infectEfft != null && infectEfft.Count > 0)
            {
                foreach (BaseMonsterBehavior m in infectEfft)
                {
                    m.Hitted(Mathf.Floor(this.Damage));
                }
            }
        }
    }
}
