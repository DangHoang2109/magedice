using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
public class BaseMonsterBehavior : MonoBehaviour
{
    public float Distance;

    [SerializeField] private Transform Tower;
    [SerializeField] private float _speed;

    private float TimeMove => Distance / _speed;

    public void Run(Transform tower)
    {
        this.transform.DOMoveY(tower.position.y, TimeMove)
            .SetEase(Ease.Linear)
            .OnComplete(AttackTower);
    }

    public void AttackTower()
    {
        Debug.Log("Reach Tower");
    }

    public void Hitted()
    {

    }
}
