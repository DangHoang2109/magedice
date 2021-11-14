﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using UnityEngine.UI;
public class BaseBullet : MonoBehaviour
{
    [SerializeField] private BaseMonsterBehavior Enemy;

    [Header("UI")]
    [SerializeField] private Image imgFront;
    [SerializeField] private Image imgLight;

    [Header("Explosion")]
    public TMPro.TextMeshProUGUI tmpDamage;

    [Header("Config")]
    [SerializeField] private float _speed;
    [SerializeField] private float _damage;

    bool isFlying;
    public bool IsFlying => this.isFlying;

    private System.Action<BaseMonsterBehavior> effectHitted;

    public BaseBullet SetUI(DiceBulletStateConfig ui)
    {
        if(ui != null)
        {
            if (ui.sprBullet != null)
                this.imgFront.sprite = ui.sprBullet;

            if (ui.sprLight != null)
                this.imgLight.sprite = ui.sprLight;
        }
        return this;
    }
    public BaseBullet SetEnemy(BaseMonsterBehavior enemy)
    {
        this.Enemy = enemy;

        enemy.RegisterHitting(this._damage);
        return this;
    }
    public BaseBullet SetData(DiceGameLevelConfig config)
    {
        this._speed = config.speed;
        this._damage = config.damage;
        return this;
    }
    public BaseBullet SetHitEffect(System.Action<BaseMonsterBehavior> enemy)
    {
        this.effectHitted = enemy;
        return this;
    }
    public void Shooted()
    {
        isFlying = true;
    }

    /// <summary>
    /// Will be called by bullet manager
    /// </summary>
    public void CustomUpdate()
    {
        if (isFlying && this.Enemy != null)
        {
            if(Enemy.CurrentHP < 0)
            {
                BaseMonsterBehavior newEnemy = MonsterManager.Instance.GetNearestMonster(Enemy.transform, Enemy.Id);
                Debug.LogError($"HP < 0? {this.Enemy.Id} change to {newEnemy.Id}");
                SetEnemy(newEnemy);
//#if UNITY_EDITOR
//                UnityEditor.EditorApplication.isPaused = true;
//#endif      
            }
                transform.position = Vector3.MoveTowards(transform.position, Enemy.transform.position, _speed * 20 * Time.deltaTime);
            if (GameUtils.IsNear(transform.position, Enemy.transform.position, 20)) //pixel
            {
                Hitted();
            }
        }
    }

    public void Hitted()
    {
        isFlying = false;
        //show annimation

        //do effect
        this.effectHitted?.Invoke(this.Enemy);

        BulletManager.Instance.UnregisterBullet(this);
    }

}
