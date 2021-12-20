using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class PersonGameData
{
    protected float currentSpeed;
    protected float currentDamage;
    protected float currentHP;

    public float CurrentSpeed { get => currentSpeed; }
    public float CurrentDamage { get => currentDamage; }
    public float CurrentHP { get => currentHP; }

    public PersonGameData(PersonConfig c)
    {
        this.currentSpeed = c.speed.init_stat;
        this.currentDamage = c.damage.init_stat;
        this.currentHP = c.hp.init_stat;
    }
}
public class MonsterGameData : PersonGameData
{
    public MonsterType ID;


    public MonsterConfig config;

    public bool IsBoss
    {
        get
        {
            return (int)this.ID / 10 == 2;
        }
    }

    public MonsterGameData(MonsterConfig c) : base(c)
    {
        this.config = c;
        this.ID = c.ID;
    }

    public void Upgrade()
    {
        this.currentSpeed  *= (1f + config.speed.percentUpEachWave);
        this.currentDamage *= (1f + config.damage.percentUpEachWave);
        this.currentHP *= (1f + config.hp.percentUpEachWave);

        Debug.Log($"Upgrade hp to {currentHP}");
    }

}
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

    private List<MonsterGameData> monsterData;
    private MapConfig _map;

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

    private MageDiceGameManager _gameManager;
    public MageDiceGameManager MageGameManager
    {
        get
        {
            if (_gameManager == null)
                _gameManager = MageDiceGameManager.Instance;

            return _gameManager;
        }
    }
    private bool _isPause;

    //user Perk
    private float perkKillMonsBonus;

    private void OnValidate()
    {
    }

    [ContextMenu("Jump 10th wave")]
    private void Test_GothWave()
    {
        JumpToWave(9);
    }
    [ContextMenu("Jump final wave")]
    private void Test_GoFinalWave()
    {
        JumpToWave(29);
    }
    private void JumpToWave(int wave)
    {
        GamWaveController.Instance.JumpToWave(wave);
        this.StopAllCoroutines();
        this.StartWave();
    }
    public void StartGame(MapConfig map)
    {
        this._map = map;
        this.activeMonsters = new List<BaseMonsterBehavior>();
        //this.activeWave = new Queue<GameWaveUnit>();
        this.monsterData = new List<MonsterGameData>();
        foreach(MonsterConfig c in map.monsterConfig)
        {
            this.monsterData.Add(new MonsterGameData(c));
        }

        StartWave();
    }
    public void SetPerk(float perkKillMonsBonus)
    {
        this.perkKillMonsBonus = perkKillMonsBonus;
    }
    public void OnPauseGame(bool isPause)
    {
        _isPause = isPause;
    }
    private void Update()
    {
        if (!_isPause)
        {
            for (int i = 0; i < this.activeMonsters.Count; i++)
            {
                activeMonsters[i].CustomUpdate();
            }
        }
    }


    public void StartWave()
    {
        WaveConfig wave = GamWaveController.Instance.GoNextWave();
        if(wave != null)
        {
            GameWaveUnit GameWaveUnit = new GameWaveUnit()
            {
                wave = wave,
                startTime = Time.timeSinceLevelLoad
            };
            if (GamWaveController.Instance.IsOutOfWave)
            {
                this.SpawnABoss(GameWaveUnit);
            }
            else
            {
                this.StartCoroutine(ieWaveProcess(GameWaveUnit));
            }
        }
    }
    public void EndWave(GameWaveUnit unit)
    {
        UpgradeMonster(unit.wave.monsterRate);
        this.StartWave();
    }
    public void UpgradeMonster(List<WaveMonsterRateConfig> waveConfig)
    {
        foreach(WaveMonsterRateConfig c in waveConfig)
        {
            MonsterGameData m = this.monsterData.Find(x => x.ID == c.monsterID);
            m.Upgrade();
        }
    }
    private IEnumerator ieWaveProcess(GameWaveUnit unit)
    {

        YieldInstruction interval = new WaitForSeconds(unit.wave.intervalSpawn);
        float lengthSpawn = unit.wave.lengthSpawn;
        float length = unit.wave.length;

        while (length > 0)
        {
            try
            {
                if (!_isPause)
                {
                    length -= unit.wave.intervalSpawn;
                    if (lengthSpawn >= 0)
                    {
                        lengthSpawn -= unit.wave.intervalSpawn;
                        SpawnAMonster(unit.wave.monsterRate);
                    }
                }

            }
            catch (System.Exception e)
            {
                Debug.LogError(e.StackTrace);
            }
            yield return interval;
        }

        //end wave
        EndWave(unit);

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
        MonsterGameData config;
        if (pick == null)
            config = this.monsterData[0];
        else
            config = this.monsterData.Find(x => x.ID == pick.monsterID);

        //take from pool
        BaseMonsterBehavior monsterObj = MonsterPoolManager.Instance.GetAMonster();

        SetUpMonsterObject(pick, config, monsterObj);

        monsterObj.Run();
    }
    public void BossCallSpawnMonster(int amount)
    {
        for (int i = 0; i < amount; i++)
        {
            SpawnAMonster(GamWaveController.Instance.GetWave(GamWaveController.Instance.CurrentWave - 1).monsterRate);
        }
    }
    private void SetUpMonsterObject(WaveMonsterRateConfig pick, MonsterGameData config, BaseMonsterBehavior monsterObj)
    {
        (monsterObj.transform as RectTransform).pivot = new Vector2(0.5f, 0);
        monsterObj.transform.SetParent(this.tfSpawnPosition);

        monsterObj.transform.localPosition = new Vector3(pick.IsBoss ? 0 : Random.Range(MaxSafeLeft.x, MaxSafeRight.x), 0);

        this.activeMonsters.Add(monsterObj);
        monsterObj.Spawned(config);
    }

    public void SpawnABoss(GameWaveUnit wave)
    {
        WaveMonsterRateConfig boss = wave.wave.monsterRate.Find(x => x.monsterID == MonsterType.SKILL_BOSS);
        if(boss != null)
        {
            MonsterGameData bossConfig = this.monsterData.Find(x => x.ID == MonsterType.SKILL_BOSS);

            //take from pool
            SkillBossBehavior monsterObj = MonsterPoolManager.Instance.GetASkillBooss();

            SetUpMonsterObject(boss, bossConfig, monsterObj);

            BossMonsterSkillHandler skillhandler = System.Activator.CreateInstance(EnumUtility.GetStringType(this._map.FinalBossID)) as BossMonsterSkillHandler;
            skillhandler.Object = monsterObj;
            monsterObj.SetSkill(skillhandler);

            monsterObj.Run();
        }
    }
    public void KillAMonster(BaseMonsterBehavior m)
    {
        this.activeMonsters.Remove(m);
        //return to pool
        MonsterPoolManager.Instance.ReturnMonster(m);
        //add coin
        MageGameManager.OnKillMonster(m.GiftedCoin + (long)this.perkKillMonsBonus);
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
    public List<BaseMonsterBehavior> GetNearestMonsters(int n, float registerFutureDamage = -1)
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
