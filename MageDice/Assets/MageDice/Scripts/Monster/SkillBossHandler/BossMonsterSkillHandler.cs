using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BossMonsterSkillHandler 
{
    public SkillBossBehavior Object;

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

public class SandManBossSkillHandler : BossMonsterSkillHandler
{
    public override void CastSkill()
    {
        base.CastSkill();

        //slow bullet speed
        BulletManager.Instance.OnSlowDownBulletSpeed(0.5f, 3f);
    }
}

public class CerberusSBossSkillHandler : BossMonsterSkillHandler
{
    public override void CastSkill()
    {
        base.CastSkill();

        //make user line active in 1 row only
        GameBoardManager.Instance.OnRestrictOneLineOnly(1,3);
    }
}

public class OrgeBossSkillHandler : BossMonsterSkillHandler
{
    public override void CastSkill()
    {
        base.CastSkill();
        //Can heal him self and spawn 3 creep
        this.Object?.AddHPByPercent(0.2f);
        MonsterManager.Instance.BossCallSpawnMonster(3);
    }
}

public class DarkKnightBossSkillHandler : BossMonsterSkillHandler
{
    public override void CastSkill()
    {
        base.CastSkill();

        //destroy random 3 dice of user
        GameBoardManager.Instance.DestroyDice(3);
    }
}
