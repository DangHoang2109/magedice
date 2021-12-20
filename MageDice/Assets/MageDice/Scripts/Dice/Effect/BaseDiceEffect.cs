using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BaseDiceEffect 
{
    public DiceGameLevelConfig GameConfig;
    public StatItemStats diceStat;
    public DiceBulletConfig UIConfig;

    public int dot;

    public float diceBoosterDamage;

    protected float perkBulletDamage, perkBulletSpeed, perkBulletCritical;

    protected float Damage => diceStat.damageStrength * GameConfig.damageMultiplier * diceBoosterDamage * (1 + perkBulletDamage) ;
    protected float Speed => diceStat.speedStrength * (1 + perkBulletSpeed);

    public float CriticalDamage => Damage * 3;

    public virtual bool RandomCritical => Random.value <= perkBulletCritical;
    public virtual bool IsCanMergeWithAny => false;

    public virtual DiceID ID => DiceID.NONE;
    public virtual void ActiveEffect()
    {
    }

    protected virtual float ActualDamage(bool isCritical) 
    {
        return isCritical ? CriticalDamage : Damage;
    }
    public virtual void ShootBullet(int amount = 1)
    {
        try
        {
            List<BaseMonsterBehavior> monsters = MonsterManager.Instance.GetNearestMonsters(amount);

            if(monsters != null && monsters.Count > 0)
            {
                List<BaseBullet> bullet = BulletPoolManager.Instance.GetBullets(monsters.Count);

                if (bullet != null && bullet.Count > 0)
                {
                    for (int i = 0; i < bullet.Count; i++)
                    {
                        BaseMonsterBehavior m = i >= monsters.Count ? monsters[0] : monsters[i];

                        bool isCritical = RandomCritical;
                        bullet[i].SetData(Speed, this.ActualDamage(isCritical), isCritical)
                            .SetUI(this.UIConfig.normalBullet)
                            .SetEnemy(i >= monsters.Count ? monsters[0] : monsters[i])
                            .SetHitEffect(this.BulletEffect);
                    }
                }

                BulletManager.Instance.RegisterBullets(bullet, true);
            }
        }
        catch(System.Exception e)
        {
            Debug.LogError(e.StackTrace);
        }
    }
    public virtual void BulletEffect(BaseMonsterBehavior enemy, float damage)
    {

    }

    public virtual void EffectWhenMerged()
    {

    }
    public virtual void EffectWhenSpawned()
    {

    }
}
