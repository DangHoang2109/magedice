using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
public class BasePersonBehavior : MonoBehaviour
{
    [SerializeField] protected int id;

    protected float _currentHP;
    protected float _maxHP;

    public GameStatBar HPBar;

    public int Id { get => id; set => id = value; }

    public virtual void Spawned(PersonGameData config)
    {
        this.gameObject.SetActive(true);
        _maxHP = config.CurrentHP;
        _currentHP = config.CurrentHP;

        HPBar.ParseData(max: _maxHP, current: _currentHP);
    }

    public virtual bool Hitted(float damage)
    {
        this._currentHP -= damage;
        
        this.HPBar.CurrentValue = _currentHP;

        if (IsDead())
        {
            Dead();
            return true;
        }


        return false;
    }
    public virtual void AddHP(float hp)
    {
        this._currentHP += hp;
        this.HPBar.CurrentValue = _currentHP;
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
