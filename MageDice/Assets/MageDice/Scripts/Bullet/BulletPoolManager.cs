using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BulletPoolManager : MonoSingleton<BulletPoolManager>
{
    public const int AMOUNT_BULLET_PREPARE = 150;
    public const int AMOUNT_BULLET_PREPARE_EACH_CALL = 5;

    [SerializeField] private BaseBullet bullet;
    [SerializeField] private Queue<BaseBullet> bullets;
    [SerializeField] private Transform tfPool;
    public int AvailableBullet => bullets.Count;
    // Start is called before the first frame update
    void Start()
    {
        PreparPool();
    }

    private BaseBullet CreateABullet()
    {
        return Instantiate<BaseBullet>(this.bullet, tfPool);
    }
    private void PreparPool()
    {
        this.bullets = new Queue<BaseBullet>();
        StartCoroutine(iePreparePool(AMOUNT_BULLET_PREPARE));
    }
    private IEnumerator iePreparePool(int amount)
    {
        YieldInstruction wait = new WaitForEndOfFrame();
        int spawn = 0;
        while (spawn < amount)
        {
            for (int i = 0; i < AMOUNT_BULLET_PREPARE_EACH_CALL; i++)
            {
                this.bullets.Enqueue(CreateABullet());
                spawn++;
            }
            yield return wait;
        }
    }

    public BaseBullet GetABullet()
    {
        if (AvailableBullet == 0)
        {
            Debug.Log("out of bullet");
            return CreateABullet();
        }

        if (AvailableBullet <= AMOUNT_BULLET_PREPARE_EACH_CALL)
        {
            Debug.Log("short of bullet");
            StartCoroutine(iePreparePool(10));
        }

        return this.bullets.Dequeue();
    }
    public List<BaseBullet> GetBullets(int n)
    {
        List<BaseBullet> res = new List<BaseBullet>();
        for (int i = 0; i < n; i++)
        {
            res.Add(GetABullet());
        }

        return res;
    }
    public void ReturnBullet(BaseBullet b)
    {
        b.transform.SetParent(tfPool);
        b.gameObject.SetActive(false);
        b.transform.localPosition = Vector3.zero;
        this.bullets.Enqueue(b);
    }
    public void ReturnBullets(List<BaseBullet> bs)
    {
        for (int i = 0; i < bs.Count; i++)
        {
            ReturnBullet(bs[i]);
        }
    }
}
