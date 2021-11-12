using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
public class BulletManager : MonoSingleton<BulletManager>
{
    [SerializeField] private List<BaseBullet> flyingBullets;

    [SerializeField] private Transform bulletStart;

    private Vector3 maxSafeRight;
    private Vector3 maxSafeLeft;
    protected Vector3 MaxSafeRight
    {
        get
        {
            if (maxSafeRight == null || maxSafeRight == Vector3.zero)
                maxSafeRight = MageDiceGameManager.Instance.TfMaxSafeRight;
            return maxSafeRight;
        }
    }
    protected Vector3 MaxSafeLeft
    {
        get
        {
            if (maxSafeLeft == null || maxSafeLeft == Vector3.zero)
                maxSafeLeft = MageDiceGameManager.Instance.TfMaxSafeLeft;
            return maxSafeLeft;
        }
    }
    public void UnregisterBullet(BaseBullet b, bool isTakenBack = true)
    {
        this.flyingBullets.Remove(b);
        if (isTakenBack)
        {
            BulletPoolManager.Instance.ReturnBullet(b);
        }
    }
    public void RegisterBullet(BaseBullet b, bool isScheduleShot = true)
    {
        this.flyingBullets.Add(b);

        if (isScheduleShot)
        {
            b.gameObject.SetActive(true);
            b.transform.SetParent(bulletStart);
            b.transform.SetAsLastSibling();
            b.transform.localPosition = Vector3.zero;

            Sequence seq = DOTween.Sequence();
            seq.Join(b.transform.DOLocalMove(new Vector3(Random.Range(MaxSafeLeft.x, MaxSafeRight.x), Random.Range(-20, 20), 0), 0.15f));
            seq.AppendInterval(0.1f);
            seq.OnComplete(b.Shooted);
        }
    }
    public void RegisterBullets(List<BaseBullet> bs, bool isScheduleShot = true)
    {
        if (this.flyingBullets == null)
            this.flyingBullets = new List<BaseBullet>();

        for (int i = 0; i < bs.Count; i++)
        {
            RegisterBullet(bs[i], isScheduleShot);
        }
    }
    private void Update()
    {
        if (this.flyingBullets == null)
            this.flyingBullets = new List<BaseBullet>();

        for (int i = 0; i < this.flyingBullets.Count; i++)
        {
            this.flyingBullets[i].CustomUpdate();
        }
    }
}
