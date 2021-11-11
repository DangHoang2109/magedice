using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
public class MonsterManager : MonoSingleton<MonsterManager>
{
    [SerializeField] private List<BaseMonsterBehavior> activeMonsters;
    [SerializeField] private List<BaseMonsterBehavior> prefabMonster;

    private Transform towerTop;
    public Transform TowerTop
    {
        get
        {
            if (towerTop == null)
                towerTop = MageDiceGameManager.Instance.TfTower;

            return towerTop;
        }
    }

    private void OnValidate()
    {
        activeMonsters = new List<BaseMonsterBehavior>(GetComponentsInChildren<BaseMonsterBehavior>());
    }
    private void Start()
    {
        for (int i = 0; i < this.activeMonsters.Count; i++)
        {
            activeMonsters[i].Id = i;
            activeMonsters[i].Spawned();
            //activeMonsters[i].Run();
        }
    }
    private void Update()
    {
        for (int i = 0; i < this.activeMonsters.Count; i++)
        {
            activeMonsters[i].CustomUpdate();
        }
    }
    public void SpawnAMonster()
    {
        //take from pool
        //monster.Run();
    }
    public void KillAMonster(BaseMonsterBehavior m)
    {
        this.activeMonsters.Remove(m);
        m.gameObject.SetActive(false);
        //return to pool
    }
    /// <summary>
    /// get nearest monster
    /// exclude the monster with register damage over his HP => monster may not dead but will dead before this bullet reach will be pass
    /// </summary>
    /// <returns></returns>
    public BaseMonsterBehavior GetNearestMonster()
    {
        if (activeMonsters.Count == 0)
            return null;

        return activeMonsters
            .Where(x => x.FutureHP > 0)
            .OrderBy(x => Mathf.Abs(x.transform.position.y - this.TowerTop.position.y)).FirstOrDefault();
    }
    /// <summary>
    /// get nearest monster
    /// exclude the monster with register damage over his HP => monster may not dead but will dead before this bullet reach will be pass
    /// </summary>
    /// <returns></returns>
    public BaseMonsterBehavior GetNearestMonster(Transform pivot, int id = -1)
    {
        if (activeMonsters.Count == 0)
            return null;

        return activeMonsters
            .Where(x => x.FutureHP > 0 && (id == -1 || (id >= 0 && x.Id != id)))
            .OrderBy(x => GameUtils.DistanceBetween(pivot, x.transform)).FirstOrDefault();
    }

    /// <summary>
    /// get nearest monster
    /// exclude the monster with register damage over his HP => monster may not dead but will dead before this bullet reach will be pass
    /// </summary>
    /// <returns></returns>
    public List<BaseMonsterBehavior> GetNearestMonsters(int n)
    {
        if (activeMonsters.Count == 0)
            return null;

        return activeMonsters
            .Where(x => x.FutureHP > 0)
            .OrderBy(x => Mathf.Abs(x.transform.position.y - this.TowerTop.position.y))
            .Take(n).ToList();
    }

    /// <summary>
    /// get nearest monster
    /// exclude the monster with register damage over his HP => monster may not dead but will dead before this bullet reach will be pass
    /// </summary>
    /// <returns></returns>
    public List<BaseMonsterBehavior> GetNearestMonsters(int n, Transform pivot, int id = -1)
    {
        if (activeMonsters.Count == 0)
            return null;

        return activeMonsters
            .Where(x => x.FutureHP > 0 && (id == -1 || (id >= 0 && x.Id != id)))
            .OrderBy(x => GameUtils.DistanceBetween(pivot, x.transform))
            .Take(n).ToList();
    }
}
