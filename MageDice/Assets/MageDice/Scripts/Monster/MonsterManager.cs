using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;


public class MonsterManager : MonoSingleton<MonsterManager>
{
    [SerializeField] private List<BaseMonsterBehavior> activeMonsters;
    [SerializeField] private Transform tfSpawnPosition;
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

    private List<MonsterConfig> monsterConfig;

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

    public Queue<GameWaveUnit> activeWave;

    private void OnValidate()
    {
    }

    public void StartGame(MapConfig map)
    {
        this.activeMonsters = new List<BaseMonsterBehavior>();
        this.activeWave = new Queue<GameWaveUnit>();
        this.monsterConfig = new List<MonsterConfig>(map.monsterConfig);

        StartWave();
    }
    private void Update()
    {
        for (int i = 0; i < this.activeMonsters.Count; i++)
        {
            activeMonsters[i].CustomUpdate();
        }
    }

    
    public void StartWave()
    {
        WaveConfig wave = GamWaveController.Instance.GoNextWave();
        int test_Wave = GamWaveController.Instance.CurrentWave;

        GameWaveUnit waveUnit = new GameWaveUnit()
        {
            wave = wave,
            startTime = Time.timeSinceLevelLoad
        };
        activeWave.Enqueue(waveUnit);

        this.StartCoroutine(ieWaveProcess(waveUnit, test_Wave));
    }
    private IEnumerator ieWaveProcess(GameWaveUnit unit, int test_currentWave)
    {
        YieldInstruction interval = new WaitForSeconds(unit.wave.intervalSpawn);
        float lengthSpawn = unit.wave.lengthSpawn;
        float length = unit.wave.length;

        while(length > 0)
        {
            length -= unit.wave.intervalSpawn;
            if (lengthSpawn > 0)
            {
                lengthSpawn -= unit.wave.intervalSpawn;
                SpawnAMonster(unit.wave.monsterRate);
            }
            yield return interval;
        }

        //end wave
        StartWave();
    }

    private WaveMonsterRateConfig RandomMonster(List<WaveMonsterRateConfig> listRand)
    {
        if (listRand.Count > 0)
        {
            List<WaveMonsterRateConfig> l = listRand.OrderByDescending(x => x.rate).ToList();
            float rand = Random.Range(0f, 1f);

            int index = 0;

            float totalValue = l[index].rate;
            float rateValue = rand - totalValue;
            while (rateValue > 0 && index < l.Count - 1)
            {
                index += 1;

                totalValue += l[index].rate;
                rateValue = rand - totalValue;
            }

            return l[index];
        }
        else
        {
            return null;
        }
        //Debug.Log("random a monster");
    }
    public void SpawnAMonster(List<WaveMonsterRateConfig> listRand)
    {
        //decise which to spawn 
        WaveMonsterRateConfig pick = RandomMonster(listRand);
        MonsterConfig config;
        if (pick == null)
            config = this.monsterConfig[0];
        else
            config = this.monsterConfig.Find(x => x.ID == pick.monsterID);

        //take from pool
        BaseMonsterBehavior monsterObj = MonsterPoolManager.Instance.GetAMonster();
        monsterObj.transform.SetParent(this.tfSpawnPosition);
        monsterObj.transform.localPosition = new Vector3(Random.Range(MaxSafeLeft.x, MaxSafeRight.x), 0);
        this.activeMonsters.Add(monsterObj);
        monsterObj.Spawned(config);

        monsterObj.Run();
    }
    public void KillAMonster(BaseMonsterBehavior m)
    {
        this.activeMonsters.Remove(m);
        //return to pool
        MonsterPoolManager.Instance.ReturnMonster(m);
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
