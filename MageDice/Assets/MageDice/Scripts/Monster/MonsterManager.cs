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

    public void SpawnAMonster()
    {
        //monster.Run();
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

        return activeMonsters.OrderBy(x => Mathf.Abs(x.transform.position.y - this.TowerTop.position.y)).FirstOrDefault();
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

        return activeMonsters.OrderBy(x => Mathf.Abs(x.transform.position.y - this.TowerTop.position.y)).Take(n).ToList();
    }
    public void RegisterHitMonster(float _damage, BaseMonsterBehavior monster)
    {
        //sub monster HP
    }
}
