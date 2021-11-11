using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
public class BaseMonsterBehavior : BasePersonBehavior
{
    public float Distance;

    [SerializeField] private Transform Tower;
    private Vector3 TowerPoint;
    private Vector3 velocity = Vector3.zero;

    private float TimeMove => Distance / _speed;

    public float FutureHP { get => _futureHP;}

    private float _futureHP; //HP sau khi đòn tấn công sắp tới sẽ được hoàn thành
    [SerializeField] private float _damage;
    [SerializeField] private float _speed;

    Tween tweenMoving;
    [SerializeField] private bool _isPause;
    public override void Spawned()
    {
        base.Spawned();

        this.Tower = MageDiceGameManager.Instance.TfTower;
        TowerPoint = new Vector3(this.transform.position.x, Tower.transform.position.y, 0);
        _futureHP = this._currentHP;
        _damage = 1;
        _speed = 2;
    }

    public void CustomUpdate()
    {
        if (!_isPause && this.Tower != null)
        {
            transform.position = Vector3.SmoothDamp(transform.position, TowerPoint, ref velocity, TimeMove);
            if (GameUtils.IsNear(transform.position.y, TowerPoint.y, 5)) //pixel
            {
                AttackTower();
            }
        }
    }
    public void Run()
    {
        tweenMoving = this.transform.DOMoveY(Tower.position.y, TimeMove)
            .SetEase(Ease.Linear)
            .OnComplete(AttackTower);
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

        DOTween.Kill(tweenMoving);
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
