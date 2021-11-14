using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using TMPro;
public class GameDamageTextManager : MonoSingleton<GameDamageTextManager>
{    
    public const int AMOUNT_PREPARE = 150;
    public const int AMOUNT_PREPARE_EACH_CALL = 5;

    [SerializeField] private TextMeshProUGUI prefab;
    [SerializeField] private Queue<TextMeshProUGUI> pools;
    [SerializeField] private Transform tfPool;
    public int AvailableItem => pools.Count;

    // Start is called before the first frame update
    void Start()
    {
        PreparPool();
    }

    private TextMeshProUGUI CreateAItem()
    {
        return Instantiate<TextMeshProUGUI>(this.prefab, tfPool);
    }
    private void PreparPool()
    {
        this.pools = new Queue<TextMeshProUGUI>();
        StartCoroutine(iePreparePool());
    }
    private IEnumerator iePreparePool()
    {
        YieldInstruction wait = new WaitForEndOfFrame();
        while (AvailableItem < AMOUNT_PREPARE)
        {
            for (int i = 0; i < AMOUNT_PREPARE_EACH_CALL; i++)
            {
                this.pools.Enqueue(CreateAItem());
            }
            yield return wait;
        }
    }

    public TextMeshProUGUI GetABullet()
    {
        if (AvailableItem == 0)
            CreateAItem();

        return this.pools.Dequeue();
    }
    public List<TextMeshProUGUI> GetBullets(int n)
    {
        List<TextMeshProUGUI> res = new List<TextMeshProUGUI>();
        for (int i = 0; i < n; i++)
        {
            res.Add(GetABullet());
        }

        return res;
    }
    public void ReturnBullet(TextMeshProUGUI b)
    {
        b.transform.SetParent(tfPool);
        b.gameObject.SetActive(false);
        b.transform.localPosition = Vector3.zero;
        this.pools.Enqueue(b);
    }
    public void ReturnBullets(List<TextMeshProUGUI> bs)
    {
        for (int i = 0; i < bs.Count; i++)
        {
            ReturnBullet(bs[i]);
        }
    }



    public virtual void ShowDamage(Vector3 des, float time, Vector3 target, float damage, string prefix = "")
    {
        TextMeshProUGUI tmpDamage = GetABullet();

        tmpDamage.transform.position = target;

        tmpDamage.SetText($"{prefix}{damage}");
        tmpDamage.gameObject.SetActive(true);

        tmpDamage.transform.DOMove(des, time)
            .OnComplete(() => OnCompleteShowDamage(tmpDamage));
    }
    protected virtual void OnCompleteShowDamage(TextMeshProUGUI txt)
    {
        ReturnBullet(txt);
    }
}
