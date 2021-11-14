using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SkillBossBehavior : BaseMonsterBehavior
{
    protected Coroutine coroutineCastSkill;
    protected BossMonsterSkillHandler _skillHandler;

    public void SetSkill(BossMonsterSkillHandler skill)
    {
        this._skillHandler = skill;
    }
    public override void Spawned(PersonGameData config)
    {
        base.Spawned(config);

        _isPause = false;
        this.coroutineCastSkill = StartCoroutine(ieCastSkill());
    }
    private IEnumerator ieCastSkill()
    {
        Debug.Log("Run Ie Cast SKill");
        YieldInstruction wait = new WaitForSeconds(5f);
        while (!this.IsDead() && !this._isPause)
        {
            yield return wait;
            CastSkill();
        }
    }

    public override void CustomUpdate()
    {
        base.CustomUpdate();
    }

    public override void AttackTower()
    {
        base.AttackTower();
    }

    public virtual void CastSkill()
    {
        if(this._skillHandler != null)
        {
            this._skillHandler.CastSkill();
            Debug.Log("Cast Skill");
        }
        else
        {
            Debug.LogError("Boss Skill Handler Null");
        }
    }

    protected override void Dead()
    {
        base.Dead();

        if (GamWaveController.Instance.IsOutOfWave)
            MageDiceGameManager.Instance.OnFinalBossDead();
    }
}
