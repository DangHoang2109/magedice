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

    public override void BulletEffect(BaseMonsterBehavior enemy)
    {
        base.BulletEffect(enemy);

        enemy.Hitted(this.GameConfig.damage);

        List<BaseMonsterBehavior> elecEfft = MonsterManager.Instance.GetNearestMonsters(2, enemy.transform, enemy.Id);
        if(elecEfft.Count > 0)
        {
            foreach(BaseMonsterBehavior m in elecEfft)
            {
                m.Hitted(Mathf.Floor(this.GameConfig.damage / 3));
            }
        }
    }
}
