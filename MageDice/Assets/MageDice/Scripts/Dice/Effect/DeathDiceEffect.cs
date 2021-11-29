using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DeathDiceEffect : BaseDiceEffect
{
    public override DiceID ID =>  DiceID.DEATH;
    public override void ActiveEffect()
    {
        base.ActiveEffect();
        this.ShootBullet(1);
    }

    public override void BulletEffect(BaseMonsterBehavior enemy, float damage)
    {
        base.BulletEffect(enemy, damage);

        if(!enemy.IsBoss && IsInstantKill)
            enemy.Hitted(damage);
    }

    public override void ShootBullet(int amount = 1)
    {
        try
        {
            List<BaseMonsterBehavior> monsters = MonsterManager.Instance.GetNearestMonsters(amount);

            if (monsters != null && monsters.Count > 0)
            {
                List<BaseBullet> bullet = BulletPoolManager.Instance.GetBullets(monsters.Count);

                if (bullet != null && bullet.Count > 0)
                {
                    for (int i = 0; i < bullet.Count; i++)
                    {
                        BaseMonsterBehavior m = i >= monsters.Count ? monsters[0] : monsters[i];

                        bool isCritical = RandomCritical;
                        BaseMonsterBehavior enemy = i >= monsters.Count ? monsters[0] : monsters[i];

                        float damage = (!enemy.IsBoss && IsInstantKill) ? enemy.CurrentHP : this.ActualDamage(isCritical);
                        bullet[i].SetData(Speed, damage, isCritical)
                            .SetUI(this.UIConfig.normalBullet)
                            .SetEnemy(enemy)
                            .SetHitEffect(this.BulletEffect);
                    }
                }

                BulletManager.Instance.RegisterBullets(bullet, true);
            }
        }
        catch (System.Exception e)
        {
            Debug.LogError(e.StackTrace);
        }
    }
    private bool IsInstantKill => Random.value < this.diceStat.rangeStrength;
}
