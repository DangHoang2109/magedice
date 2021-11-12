using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
public class BasePersonBehavior : MonoBehaviour
{
    [SerializeField] protected int id;

    protected float _currentHP;
    protected float _maxHP;

    [Header("UI")]
    public GameStatBar HPBar;

    public int Id { get => id; set => id = value; }

    public virtual void Spawned(PersonConfig config)
    {
        this.gameObject.SetActive(true);
        _maxHP = config.hp.init_stat;
        _currentHP = config.hp.init_stat;

        HPBar.ParseData(max: _maxHP, current: _currentHP);
    }

    public virtual void Hitted(float damage)
    {
        this._currentHP -= damage;
        this.HPBar.CurrentValue = _currentHP;

        if (IsDead())
            Dead();
    }

    public virtual bool IsDead()
    {
        return _currentHP <= 0;
    } 
    protected virtual void Dead()
    {

    }

    public virtual void ShowDamage(float damage)
    {

    }
}
