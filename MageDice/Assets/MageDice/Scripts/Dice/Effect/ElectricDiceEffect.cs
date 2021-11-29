using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ElectricDiceEffect : BaseDiceEffect
{
    public override DiceID ID =>  DiceID.ELECTRIC;
    public override void ActiveEffect()
    {
        base.ActiveEffect();
        this.ShootBullet(1);
    }

    public override void BulletEffect(BaseMonsterBehavior enemy, float damage)
    {
        base.BulletEffect(enemy, damage);

        enemy.Hitted(damage);

        List<BaseMonsterBehavior> elecEfft = MonsterManager.Instance.GetNearestMonsters(2, enemy.transform, enemy.Id);
        if(elecEfft != null && elecEfft.Count > 0)
        {
            foreach(BaseMonsterBehavior m in elecEfft)
            {
                m.Hitted(Mathf.Floor(this.Damage / 3));
            }
        }
    }
}
