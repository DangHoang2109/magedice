using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BossMonsterSkillHandler 
{
    public virtual void CastSkill()
    {

    }
}

public class DragonBossSkillHandler : BossMonsterSkillHandler
{
    public override void CastSkill()
    {
        base.CastSkill();

        //block a dice row in main user
        GameBoardManager.Instance.BlockRow(true, Random.Range(0, 2), 1f);
    }
}
