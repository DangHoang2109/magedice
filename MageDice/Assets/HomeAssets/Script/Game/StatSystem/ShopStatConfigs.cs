#pragma warning disable 0618
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using UnityEngine;
using UnityEngine.UI;

[CreateAssetMenu(menuName = "Games/ShopStatConfigs")]
public class ShopStatConfigs : ScriptableObject
{
    [SerializeField]
    private ShopStatConfig[] configs;

    [System.NonSerialized]
    private Dictionary<string, ShopStatConfig> dicConfigs;

    [System.NonSerialized]
    private Dictionary<StatManager.Tier, List<ShopStatConfig>> dictTierLists;
    public Dictionary<StatManager.Tier, List<ShopStatConfig>> DictTierLists
    { get { return this.dictTierLists; } }


    private static ShopStatConfigs _instance;
    public static ShopStatConfigs Instance
    {
        get
        {
            if (_instance == null)
            {
                _instance = LoaderUtility.Instance.GetAsset<ShopStatConfigs>("Games/Configs/ShopStatConfigs");
                _instance.Init();
            }
            return _instance;
        }
    }

#if UNITY_EDITOR
    public class Editor_TierConfigData
    {
        public StatManager.Tier tier;
        public int step;
        public int maxLevel;
        public long BPUnit;
        public float offsetLevel;

        public class Editor_LevelConfigData
        {
            public int level;
            public int cards;
            public long price;
        }

        public List<Editor_LevelConfigData> levelUpgrades;

        public Editor_TierConfigData(StatManager.Tier tier, int step, int maxLevel, long BPUnit, float offsetLevel)
        {
            this.tier = tier;
            this.step = step;
            this.maxLevel = maxLevel;
            this.BPUnit = BPUnit;
            this.offsetLevel = offsetLevel;

            List<int> ExcelCardsConfig = new List<int>() { 2, 4, 10, 20, 40, 60, 100, 150, 230, 350, 500, 700, 900, 1100, 1300, 1500, 1750, 2000 };


            this.levelUpgrades = new List<Editor_LevelConfigData>();
            for (int i = 0; i < this.maxLevel; i++)
            {
                this.levelUpgrades.Add(new Editor_LevelConfigData()
                {
                    level = i + 1,
                    cards = ExcelCardsConfig[i],
                    price = (long)((levelUpgrades.Count == 0 ? 0 : levelUpgrades[i - 1].price) + this.step * this.BPUnit * (1 + (i + 1) * this.offsetLevel))
                });
            }
        }
    }
#endif

    private void Init()
    {
        this.dicConfigs = new Dictionary<string, ShopStatConfig>(this.configs.Length);
        this.dictTierLists = new Dictionary<StatManager.Tier, List<ShopStatConfig>>();

        for (int i = 0; i < this.configs.Length; ++i)
        {
            this.dicConfigs.Add(this.configs[i].id, this.configs[i]);
            if (!this.dictTierLists.ContainsKey(this.configs[i].tier))
            {
                this.dictTierLists.Add(this.configs[i].tier, new List<ShopStatConfig>());
            }
            this.dictTierLists[this.configs[i].tier].Add(this.configs[i]);
        }
    }

    public string[] GetAllIds()
    {
        if (this.dicConfigs == null)
        {
            Debug.LogError("ShopCueConfigs GetAllIds dictionary NULL");
            return null;
        }

        return this.dicConfigs.Keys.ToArray();
    }

    public List<StatData> JoinCuesByTier(StatManager.Tier tier, List<StatData> cues)
    {
        if (!this.dictTierLists.ContainsKey(tier))
        {
            Debug.LogError("ShopCueConfigs JoinCuesByTier tier NOT FOUND: " + tier);
            return null;
        }

        List<StatData> results = (from a in cues
                                 join b in this.dictTierLists[tier]
                                 on a.id equals b.id
                                 select a).ToList();
        return results;
    }

    public List<ShopStatConfig> GetConfigs()
    {
        return this.configs.ToList();
    }


    public List<ShopStatConfig> GetConfigs(List<string> ids)
    {
        return ids.Select(GetConfigRisk).ToList();
    }
    public List<ShopStatConfig> GetConfigs(string[] ids)
    {
        return ids.Select(GetConfigRisk).ToList();
    }
    public List<ShopStatConfig> GetConfigsSafe(List<string> ids)
    {
        return ids.Where(this.dicConfigs.ContainsKey).Select(GetConfigRisk).ToList();
    }
    public List<ShopStatConfig> GetConfigsSafe(string[] ids)
    {
        return ids.Where(this.dicConfigs.ContainsKey).Select(GetConfigRisk).ToList();
    }

    public List<ShopStatConfig> GetConfigsList()
    {
        return this.configs.ToList();
    }
    public List<ShopStatConfig> GetConfigsByTier(StatManager.Tier tier)
    {
        if (this.dictTierLists.ContainsKey(tier))
        {
            return this.dictTierLists[tier];
        }
        Debug.LogError("ShopCueConfigs GetConfigsByTier NOT FOUND: " + tier);
        return null;
    }

    public ShopStatConfig GetConfig(string id)
    {
        if (this.dicConfigs != null)
        {
           if (this.dicConfigs.ContainsKey(id))
                return this.dicConfigs[id];
        }
            

        Debug.LogError("ShopCueConfigs GetConfig NOT FOUND: " + id);
        return null;
    }

    private ShopStatConfig GetConfigRisk(string id)
    {
        return this.dicConfigs[id];
    }
}

[System.Serializable]
public class CueStats
{
    public const float MIN_POWER = 1.2f;
    public const float MIN_AIM = 80.0f;//old 70
    public const float MIN_SPIN = 4.5f;
    public const float MIN_TIME = 0.0f;

    public const float MAX_POWER = 1.6f;
    public const float MAX_AIM = 145.0f;// old 110
    public const float MAX_SPIN = 6f;
    public const float MAX_TIME = 10.0f;

    [System.NonSerialized]
    public Sprite sprCard;

    [System.NonSerialized]
    public Sprite sprCue;

    public long price = 10000;
    public int cardsRequired = 10;

    [Space(5f)]
    public float powerStrength;
    public float aimLength;
    public float spin;
    public float time;

    [Obsolete("Should call the static function instead!")]
    public CueStats()
    {
        this.powerStrength = 0;
        this.aimLength = 0;
        this.spin = 0;
        this.time = 0;
    }

    public static CueStats CreateZero()
    {
        CueStats result = new CueStats()
        {
            powerStrength = 0f,
            aimLength = 0f,
            spin = 0f,
            time = 0f,
        };
        return result;
    }
    public static CueStats CreateBasic()
    {
        CueStats result = new CueStats()
        {
            powerStrength = MIN_POWER,
            aimLength = MIN_AIM,
            spin = MIN_SPIN,
            time = MIN_TIME,
        };
        return result;
    }
    public static CueStats CreateSafe(float pwr, float aim, float spn, float t)
    {
        CueStats result = new CueStats()
        {
            powerStrength = Mathf.Clamp(pwr, MIN_POWER, MAX_POWER),
            aimLength = Mathf.Clamp(aim, MIN_AIM, MAX_AIM),
            spin = Mathf.Clamp(spn, MIN_SPIN, MAX_SPIN),
            time = Mathf.Clamp(t, MIN_TIME, MAX_TIME),
        };
        return result;
    }

    public static CueStats Clone(CueStats target)
    {
        CueStats result = new CueStats()
        {
            powerStrength = target.powerStrength,
            aimLength = target.aimLength,
            spin = target.spin,
            time = target.time,
        };
        return result;
    }

    public static CueStats CreateRandom()
    {
        CueStats result = new CueStats()
        {
            powerStrength = UnityEngine.Random.Range(0f, 10f),
            aimLength = UnityEngine.Random.Range(0f, 10f),
            spin = UnityEngine.Random.Range(0f, 10f),
            time = UnityEngine.Random.Range(0f, 10f),
        };
        return result;
    }

    public static CueStats CreateRandomRealUsing(ShopStatConfig config)
    {
        CueStats result = new CueStats()
        {
            powerStrength = UnityEngine.Random.Range(MIN_POWER, MAX_POWER),
            aimLength = UnityEngine.Random.Range(MIN_AIM, MAX_AIM),
            spin = UnityEngine.Random.Range(MIN_SPIN, MAX_SPIN),
            time = UnityEngine.Random.Range(MIN_TIME, MAX_TIME),

            sprCard = config.sprCard,
            sprCue = config.sprCue
        };
        return result;
    }

    public static CueStats CreateForRealUsing(CueStats stats, ShopStatConfig config)
    {
        CueStats result = new CueStats()
        {
            powerStrength = (MAX_POWER - MIN_POWER) * stats.powerStrength / 10f + MIN_POWER,
            aimLength = (MAX_AIM - MIN_AIM) * stats.aimLength / 10f + MIN_AIM,
            spin = (MAX_SPIN - MIN_SPIN) * stats.spin / 10f + MIN_SPIN,
            time = (MAX_TIME - MIN_TIME) * stats.time / 10f + MIN_TIME,
            sprCard = config.sprCard,
            sprCue = config.sprCue
        };

        return result;
    }
}

[System.Serializable]
public class ShopStatConfig
{
    public string id;
    public string cueName;

    public Sprite sprCard;
    public Sprite sprCue;

    [Tooltip("the higher the more value of this cue over other cues")]
    public int appearTier;
    public bool isHide;

    public StatManager.Tier tier;
    public StatManager.UnlockType unlockType;
    public StatManager.UnlockType upgradeType;
    /// <summary>
    /// unlock type: play level => linked id: the level
    /// </summary>
    public int unlockLinkedId;
    public string unlockText;

    public CueStats[] statsPerLevels;

    public float rateRandomUnlock; //rate random ra cue này trong bag bonus config;


    [System.NonSerialized]
    public StatData cueData;

    public ShopStatConfig()
    {

    }

#if UNITY_EDITOR
    public static ShopStatConfig CloneConfig(ShopStatConfig original)
    {
        ShopStatConfig newConfig = new ShopStatConfig()
        {
            id = original.id + "_clone",
            cueName = original.cueName,
            sprCue = original.sprCue,
            tier = original.tier,
            unlockType = original.unlockType,
            unlockText = original.unlockText,
            upgradeType = original.upgradeType,
            unlockLinkedId = original.unlockLinkedId,
            appearTier = original.appearTier,
            isHide = original.isHide,
            statsPerLevels = new CueStats[original.statsPerLevels.Length]
        };
        for(int iz = 0; iz < newConfig.statsPerLevels.Length; ++iz)
        {
            newConfig.statsPerLevels[iz] = CueStats.Clone(original.statsPerLevels[iz]);
            newConfig.statsPerLevels[iz].price = original.statsPerLevels[iz].price;
            newConfig.statsPerLevels[iz].cardsRequired = original.statsPerLevels[iz].cardsRequired;
        }
        return newConfig;
    }
#endif

    public ShopStatConfig(string id, string name, CueStats baseStats, int levels, CueStats additionsEachLevel)
    {
        this.id = id;
        this.cueName = name;
        this.statsPerLevels = new CueStats[levels];
        if (levels > 0)
        {
            this.statsPerLevels[0] = baseStats;
            this.AutoParseStats(additionsEachLevel);
        }
    }

    public void AutoParseStats(CueStats additionsEachLevel)
    {
        if (this.statsPerLevels == null)
            return;

        int n = this.statsPerLevels.Length;
        if (n == 0 || n == 1)
            return;

        CueStats baseStats = this.statsPerLevels[0],
            newStats;
        for (int i = 1; i < n; ++i)
        {
            newStats = CueStats.CreateSafe(
                pwr: additionsEachLevel.powerStrength * i + baseStats.powerStrength,
                aim: additionsEachLevel.aimLength * i + baseStats.aimLength,
                spn: additionsEachLevel.spin * i + baseStats.spin,
                t: additionsEachLevel.time * i + baseStats.time
                );
            this.statsPerLevels[i] = newStats;
        }
    }


    public bool IsMaxLevel(int currentLevel)
    {
        return this.statsPerLevels == null || (currentLevel >= this.statsPerLevels.Length);
    }

    /// <summary>
    /// level < 0 => 0
    /// <para> level >= max => -1 </para>
    /// </summary>
    public long GetPriceNext(int currentLevel)
    {
        if (currentLevel < 0 || this.statsPerLevels == null)
            return 0;

        if (currentLevel > this.statsPerLevels.Length - 1)
            return -1;

        return this.statsPerLevels[currentLevel].price;
    }


    /// <summary>
    /// level < 0 => -1
    /// <para> level >= max => 0 </para>
    /// </summary>
    public int GetRequirementCardsNext(int currentLevel)
    {
        if (currentLevel < 0 || this.statsPerLevels == null)
            return -1;

        if (currentLevel >= this.statsPerLevels.Length)
            return 0;

        return this.statsPerLevels[currentLevel].cardsRequired;
    }

    public CueStats GetLevelStats(int level)
    {
        if (level <= 0 || this.statsPerLevels == null || level > this.statsPerLevels.Length)
            return CueStats.CreateZero();

        return this.statsPerLevels[level - 1];
    }

    public CueStats GetFullStat()
    {
        if (this.statsPerLevels == null || this.statsPerLevels.Length == 0)
            return CueStats.CreateZero();

        return this.statsPerLevels[this.statsPerLevels.Length - 1];
    }

#if UNITY_EDITOR
    public override string ToString()
    {
        return string.Format("{0}-{1}", this.id, this.cueName);
    }
#endif
}
