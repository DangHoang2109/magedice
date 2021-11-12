using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MonsterPoolManager : MonoSingleton<MonsterPoolManager>
{
    public const int AMOUNT_BULLET_PREPARE = 100;
    public const int AMOUNT_BULLET_PREPARE_EACH_CALL = 5;

    [SerializeField] private BaseMonsterBehavior prefabMonster;
    [SerializeField] private Queue<BaseMonsterBehavior> monsters;
    [SerializeField] private Transform tfPool;
    public int AvailableBullet => monsters.Count;
    // Start is called before the first frame update
    void Start()
    {
        PreparPool();
    }

    private BaseMonsterBehavior CreateAMonster()
    {
        return Instantiate<BaseMonsterBehavior>(this.prefabMonster, tfPool);
    }
    private void PreparPool()
    {
        this.monsters = new Queue<BaseMonsterBehavior>();
        StartCoroutine(iePreparePool(AMOUNT_BULLET_PREPARE));
    }
    private IEnumerator iePreparePool(int amount)
    {
        YieldInstruction wait = new WaitForEndOfFrame();
        int spawn = 0;
        while(spawn < amount )
        {
            for (int i = 0; i < AMOUNT_BULLET_PREPARE_EACH_CALL; i++)
            {
                this.monsters.Enqueue(CreateAMonster());
                spawn++;
            }
            yield return wait;
        }
    }
    
    public BaseMonsterBehavior GetAMonster()
    {
        if (AvailableBullet == 0)
            return CreateAMonster();

        if(AvailableBullet <= AMOUNT_BULLET_PREPARE_EACH_CALL)
            StartCoroutine(iePreparePool(10));

        return this.monsters.Dequeue();
    }
    public List<BaseMonsterBehavior> GetMonsters(int n)
    {
        List<BaseMonsterBehavior> res = new List<BaseMonsterBehavior>();
        for (int i = 0; i < n; i++)
        {
            res.Add(GetAMonster());
        }

        return res;
    }
    public void ReturnMonster(BaseMonsterBehavior b)
    {
        b.transform.SetParent(tfPool);
        b.gameObject.SetActive(false);
        b.transform.localPosition = Vector3.zero;
        this.monsters.Enqueue(b);
    }
    public void ReturnMonster(List<BaseMonsterBehavior> bs)
    {
        for (int i = 0; i < bs.Count; i++)
        {
            ReturnMonster(bs[i]);
        }
    }
}
