using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using UnityEngine.UI;
public class BaseMonsterBehavior : BasePersonBehavior
{
    [Header("UI")]
    public Image imgFront;

    private Transform Tower;
    private Vector3 TowerPoint;

    public float FutureHP { get => _futureHP;}

    private float _futureHP; //HP sau khi đòn tấn công sắp tới sẽ được hoàn thành
    private float _damage;
    private float _speed;

    Tween tweenMoving;
    private bool _isPause;

    public override void Spawned(PersonGameData config)
    {
        _isPause = true;

        base.Spawned(config);

        this.Tower = MageDiceGameManager.Instance.TfTower;
        TowerPoint = new Vector3(this.transform.position.x, Tower.transform.position.y, 0);
        _futureHP = this._currentHP;

        _damage = config.CurrentDamage;
        _speed = config.CurrentSpeed;

        MonsterGameData monsterConfig = config as MonsterGameData;
        imgFront.sprite = monsterConfig.config.UI.spr;
        this.transform.localScale = new Vector3(monsterConfig.config.UI.scale, monsterConfig.config.UI.scale);
    }

    public void CustomUpdate()
    {
        if (!_isPause && this.Tower != null)
        {
            transform.position = Vector3.MoveTowards(transform.position, TowerPoint, _speed * Time.deltaTime);

            //transform.position = Vector3.SmoothDamp(transform.position, TowerPoint, ref velocity, TimeMove);
            if (GameUtils.IsNear(transform.position.y, TowerPoint.y, 5)) //pixel
            {
                AttackTower();
            }
        }
    }
    public void Run()
    {
        _isPause = false;
    }
    public void PauseMe(bool isPause)
    {
        _isPause = isPause;
        if (isPause)
            tweenMoving.Pause();
        else
            tweenMoving.Play();
    }

    [ContextMenu("test pause")]
    public void TestPause()
    {
        PauseMe(true);
    }

    public void AttackTower()
    {
        if(!IsDead())
            Debug.Log("Reach Tower");
    }

    public virtual void RegisterHitting(float damage)
    {
        _futureHP -= damage;
    }
    public override void Hitted(float damage)
    {
        base.Hitted(damage);
        ShowDamage(damage);
    }
    protected override void Dead()
    {
        base.Dead();

        MonsterManager.Instance.KillAMonster(this);
    }

    public override void ShowDamage(float damage)
    {
        base.ShowDamage(damage);

        Vector3 start = new Vector3(this.transform.position.x + Random.Range(-10, 10), this.transform.position.y + Random.Range(-10, 10));
        GameDamageTextManager.Instance.ShowDamage(
            des: new Vector3(start.x, start.y + 20),
            time: 0.5f,
            target: start,
            damage: damage);
    }

    #region Effect Dice

    public void Freeze(float time)
    {
        DOTween.Kill(this.GetInstanceID() + "Freeze");

        _isPause = true;
        Sequence seq = DOTween.Sequence();
        seq.SetId(this.GetInstanceID() + "Freeze");
        seq.Join(tweenMoving.Pause());
        seq.AppendInterval(time);
        seq.AppendCallback(()=>{
            _isPause = false;
            tweenMoving.Play();
        });
    }

    public void Poision(float time,float interval, float damageEachInterval)
    {
        DOTween.Kill(this.GetInstanceID() + "Poision");

        Sequence seq = DOTween.Sequence();
        seq.SetId(this.GetInstanceID() + "Poision");

        while(time > 0)
        {
            time -= interval;

            seq.AppendInterval(interval);
            seq.AppendCallback(()=> this.Hitted(damageEachInterval));
        }
    }
    #endregion  Effect Dice
}
