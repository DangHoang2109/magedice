using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using UnityEngine.UI;
public class BaseMonsterBehavior : BasePersonBehavior
{
    [Header("UI")]
    public Image imgFront;
    public CanvasGroup canvas;

    protected Transform Tower;
    protected Vector3 TowerPoint;

    public float FutureHP { get => _futureHP;}
    public float CurrentHP => this._currentHP;
    public long GiftedCoin { get => _giftedCoin; set => _giftedCoin = value; }

    protected float _futureHP; //HP sau khi đòn tấn công sắp tới sẽ được hoàn thành
    protected float _damage;
    protected float _speed;
    protected long _giftedCoin;
    protected MonsterType _monsterID;
    public MonsterType MonsterID => _monsterID;

    protected bool _isPause;
    protected bool _isAttacking;
    protected Coroutine ieAttacking;
    protected MageBehavior _mage;
    protected MageBehavior Mage
    {
        get
        {
            if (_mage == null)
                _mage = MageDiceGameManager.Instance.Mage;
            return _mage;
        }
    }
    public override void Spawned(PersonGameData config)
    {
        _isPause = true;
        _isAttacking = false;
        this.canvas.alpha = 1;

        base.Spawned(config);

        this.Tower = MageDiceGameManager.Instance.TfTower;
        TowerPoint = new Vector3(this.transform.position.x, Tower.transform.position.y, 0);
        _futureHP = this._currentHP;

        _damage = config.CurrentDamage;
        _speed = config.CurrentSpeed;

        MonsterGameData monsterConfig = config as MonsterGameData;
        imgFront.sprite = monsterConfig.config.UI.spr;
        this.transform.localScale = new Vector3(monsterConfig.config.UI.scale, monsterConfig.config.UI.scale);

        _giftedCoin = monsterConfig.config.coinGifted;

    }

    public virtual void CustomUpdate()
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
    }

    public virtual void AttackTower()
    {
        if (!IsDead() && !_isAttacking)
        {
            _isAttacking = true;
            this.ieAttacking = StartCoroutine(ieAttack());
        }
    }
    private IEnumerator ieAttack()
    {
        YieldInstruction wait = new WaitForSeconds(1f);
        while(!this.IsDead() && !this._isPause && _isAttacking)
        {
            yield return wait;
            Mage.Hitted(_damage);
        }
    }

    public virtual void RegisterHitting(float damage)
    {
        _futureHP -= damage;
    }
    public override void Hitted(float damage)
    {
        if (IsDead())
            return;

        base.Hitted(damage);
        this._futureHP = this._currentHP;

        ShowDamage(damage);
    }
    protected override void Dead()
    {
        base.Dead();
        if(ieAttacking != null)
            StopCoroutine(ieAttacking);

        this.PauseMe(true);
        this.imgFront.color = Color.black;
        this.canvas.DOFade(0f, 0.25f).OnComplete(() =>
        {
            MonsterManager.Instance.KillAMonster(this);
        });
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
        seq.AppendInterval(time);
        seq.AppendCallback(()=>{
            _isPause = false;
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
