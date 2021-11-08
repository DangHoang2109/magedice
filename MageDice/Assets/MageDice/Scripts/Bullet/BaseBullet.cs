using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
public class BaseBullet : MonoBehaviour
{
    [SerializeField] private Transform Enemy;

    [SerializeField] private float _ConfigDistance = 5;
    [SerializeField] private float _speed;
    private float FlyTime => _ConfigDistance / _speed;
    private Vector3 velocity = Vector3.zero;

    bool isFlying;

    [ContextMenu("Shoot")]
    public void Shooted()
    {
        isFlying = true;
        //float distance = GameUtils.DistanceBetween(this.transform, Enemy);

        //DOTween.To(() => this.transform.transform.position, x => this.transform.transform.position = x, desPosition, distance / this._speed)
        //.SetEase(Ease.Linear)
        //.OnUpdate(() =>
        //{
        //    Debug.Log($"Aim to {Enemy.transform.position}");
        //    desPosition = Enemy.transform.position;
        //    Debug.Log($"Bullet {this.transform.transform.position} fly to {desPosition}");

        //})
        //.OnComplete(Hitted);

        //this.transform.DOMove(Enemy.transform.position, distance / this._speed)
        //    .SetEase(Ease.Linear)
        //    .OnComplete(this.Hitted);
    }


    private void Update()
    {
        if(isFlying && this.Enemy != null)
        {
            transform.position = Vector3.SmoothDamp(transform.position, Enemy.transform.position, ref velocity, FlyTime);
            if (GameUtils.IsNear(transform.position, Enemy.transform.position, 20)) //pixel
            {
                Hitted();
            }
        }
    }

    public void Hitted()
    {
        isFlying = false;
    }
}
