using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MonsterPoolManager : MonoSingleton<MonsterPoolManager>
{
    public const int AMOUNT_PREPARE = 100;
    public const int AMOUNT_PREPARE_EACH_CALL = 5;

    [SerializeField] private BaseMonsterBehavior prefabMonster;
    [SerializeField] private SkillBossBehavior finalBoss;

    [SerializeField] private Queue<BaseMonsterBehavior> monsters;
    [SerializeField] private Transform tfPool;
    public int AvailableItem => monsters.Count;
    private int _currentId;
    // Start is called before the first frame update
    void Start()
    {
        _currentId = 0;
        PreparPool();
    }

    private BaseMonsterBehavior CreateAMonster()
    {
        BaseMonsterBehavior m = Instantiate<BaseMonsterBehavior>(this.prefabMonster, tfPool);
        m.Id = _currentId++;

        return m;
    }
    private void PreparPool()
    {
        this.monsters = new Queue<BaseMonsterBehavior>();
        StartCoroutine(iePreparePool(AMOUNT_PREPARE));
    }
    private IEnumerator iePreparePool(int amount)
    {
        YieldInstruction wait = new WaitForEndOfFrame();
        int spawn = 0;
        while(spawn < amount )
        {
            for (int i = 0; i < AMOUNT_PREPARE_EACH_CALL; i++)
            {
                this.monsters.Enqueue(CreateAMonster());
                spawn++;
            }
            yield return wait;
        }
    }
    
    public BaseMonsterBehavior GetAMonster()
    {
        if (AvailableItem == 0)
            return CreateAMonster();

        if(AvailableItem <= AMOUNT_PREPARE_EACH_CALL)
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
        b.imgFront.color = Color.white;
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

    public SkillBossBehavior GetASkillBooss()
    {
        return this.finalBoss;
    }
    public void ReturnSkillBooss(SkillBossBehavior b)
    {
        b.transform.SetParent(tfPool);
        b.gameObject.SetActive(false);
        b.transform.localPosition = Vector3.zero;
    }
}
