using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BaseDiceEffect 
{
    public DiceGameLevelConfig GameConfig;
    public StatItemStats diceStat;
    public DiceBulletConfig UIConfig;

    public float diceBoosterDamage;

    protected float perkBulletDamage, perkBulletSpeed, perkBulletCritical;

    protected float Damage => diceStat.damageStrength * GameConfig.damageMultiplier * diceBoosterDamage * (1 + perkBulletDamage) ;
    protected float Speed => diceStat.speedStrength * (1 + perkBulletSpeed);

    public float CriticalDamage => Damage * 3;

    public bool RandomCritical => Random.value <= perkBulletCritical;

    public virtual DiceID ID => DiceID.NONE;
    public virtual void ActiveEffect()
    {
    }

    public virtual void ShootBullet(int amount)
    {
        try
        {
            List<BaseMonsterBehavior> monsters = MonsterManager.Instance.GetNearestMonsters(amount);

            Debug.Log($"{ID} Damage factor diceStat {diceStat.damageStrength} gamedot {GameConfig.damageMultiplier} booster {diceBoosterDamage}");

            if(monsters != null && monsters.Count > 0)
            {
                List<BaseBullet> bs = BulletPoolManager.Instance.GetBullets(monsters.Count);

                if (bs != null && bs.Count > 0)
                {
                    for (int i = 0; i < bs.Count; i++)
                    {
                        BaseMonsterBehavior m = i >= monsters.Count ? monsters[0] : monsters[i];

                        bool isCritical = RandomCritical;
                        bs[i].SetData(Speed, isCritical ? CriticalDamage : Damage, isCritical)
                            .SetUI(this.UIConfig.normalBullet)
                            .SetEnemy(i >= monsters.Count ? monsters[0] : monsters[i])
                            .SetHitEffect(this.BulletEffect);
                    }
                }

                BulletManager.Instance.RegisterBullets(bs, true);
            }
        }
        catch(System.Exception e)
        {
            Debug.LogError(e.StackTrace);
        }
    }
    public virtual void BulletEffect(BaseMonsterBehavior enemy)
    {

    }
}
