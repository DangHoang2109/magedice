using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BaseDiceEffect 
{
    public DiceGameLevelConfig GameConfig;
    public DiceBulletConfig UIConfig;
    public virtual DiceID ID => DiceID.NONE;
    public virtual void ActiveEffect()
    {
        Debug.Log($"Dice {this.ID} {GameConfig.dot} activated ");
    }

    public virtual void ShootBullet(int amount)
    {
        List<BaseBullet> bs = BulletPoolManager.Instance.GetBullets(amount);
        List< BaseMonsterBehavior> monsters = MonsterManager.Instance.GetNearestMonsters(amount);
        
        if(bs != null && bs.Count > 0)
        {
            for (int i = 0; i < bs.Count; i++)
            {
                bs[i].SetData(GameConfig)
                    .SetUI(this.UIConfig.normalBullet)
                    .SetEnemy(i >= monsters.Count ? monsters[0] : monsters[i])
                    .SetHitEffect(this.BulletEffect);
            }
        }

        BulletManager.Instance.RegisterBullets(bs, true);
    }
    public virtual void BulletEffect(BaseMonsterBehavior enemy)
    {

    }
}
