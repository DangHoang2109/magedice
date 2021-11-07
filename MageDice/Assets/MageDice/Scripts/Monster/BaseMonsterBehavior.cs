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

    [ContextMenu("Run")]
    public void Run()
    {
        this.transform.DOMoveY(Tower.position.y, TimeMove)
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
