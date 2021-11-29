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
    private Dictionary<DiceID, ShopStatConfig> dicConfigs;

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
                    price = (long)((levelUpgrades.Count == 0 ? 0 : levelUpgrades[i - 1].price) + this.BPUnit * (1 + (i + 1) * this.offsetLevel))
                });
            }
        }
    }

    [ContextMenu("Config Cue Level")]
    private void Editor_ConfigCue()
    {
        Editor_ConfigCueLevel(null, 0, this.configs.Length);
    }


    public void Editor_ConfigCueLevel(Dictionary<StatManager.Tier, Editor_TierConfigData> excelConfig, int indexStart, int amount)
    {
        if (excelConfig == null)
        {
            excelConfig = new Dictionary<StatManager.Tier, Editor_TierConfigData>();
            excelConfig.Add(StatManager.Tier.Standard, new Editor_TierConfigData(
            tier: StatManager.Tier.Standard,
            step: 2,
            maxLevel: 18,
            BPUnit: 1000,
            offsetLevel: 0.1f));

            excelConfig.Add(StatManager.Tier.Rare, new Editor_TierConfigData(
                tier: StatManager.Tier.Rare,
                step: 3,
                maxLevel: 10,
                BPUnit: 7500,
                offsetLevel: 0.2f));

            excelConfig.Add(StatManager.Tier.Legendary, new Editor_TierConfigData(
                tier: StatManager.Tier.Legendary,
                step: 4,
                maxLevel: 5,
                BPUnit: 50000,
                offsetLevel: 0.25f));
        }

        int endIndex = amount + indexStart > this.configs.Length ? this.configs.Length : amount + indexStart;
        for (int i = indexStart; i < endIndex; i++)
        {
            if (!excelConfig.ContainsKey(this.configs[i].tier) || this.configs[i].isHide)
                continue;

            ieSetUpACue(this.configs[i], excelConfig[this.configs[i].tier]);

            Debug.Log($"Finish a cue {this.configs[i].id}");
        }
    }
    private void ieSetUpACue(ShopStatConfig cue, Editor_TierConfigData config)
    {
        List<StatItemStats> listStat = cue.statsPerLevels.ToList();

        listStat.RemoveRange(1, listStat.Count - 1);

        for (int i = 1; i <= config.maxLevel; i++)
        {
            StatItemStats stat = SetUpCueLevel(listStat[i - 1], config.levelUpgrades[i - 1]);
            listStat.Add(stat);
        }

        cue.statsPerLevels = listStat.ToArray();
    }
    private StatItemStats SetUpCueLevel(StatItemStats pre, Editor_TierConfigData.Editor_LevelConfigData config)
    {
        StatItemStats stat = new StatItemStats()
        {
            cardsRequired = config.cards,
            price = config.price,

            damageStrength = pre.damageStrength,
            rangeStrength = pre.rangeStrength,
            speedStrength = pre.speedStrength,
            timeEffectStrength = pre.timeEffectStrength,
        };

        List<float> statToLisr = new List<float>() { stat.damageStrength, stat.rangeStrength, stat.speedStrength, stat.timeEffectStrength };

        stat.damageStrength = Mathf.CeilToInt(statToLisr[0] * 1.1f);
        stat.rangeStrength = statToLisr[1];
        stat.speedStrength = statToLisr[2];
        stat.timeEffectStrength = statToLisr[3];

        return stat;
    }
#endif

    private void Init()
    {
        this.dicConfigs = new Dictionary<DiceID, ShopStatConfig>(this.configs.Length);
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

    public DiceID[] GetAllIds()
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


    public List<ShopStatConfig> GetConfigs(List<DiceID> ids)
    {
        return ids.Select(GetConfigRisk).ToList();
    }
    public List<ShopStatConfig> GetConfigs(DiceID[] ids)
    {
        return ids.Select(GetConfigRisk).ToList();
    }
    public List<ShopStatConfig> GetConfigsSafe(List<DiceID> ids)
    {
        return ids.Where(this.dicConfigs.ContainsKey).Select(GetConfigRisk).ToList();
    }
    public List<ShopStatConfig> GetConfigsSafe(DiceID[] ids)
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

    public ShopStatConfig GetConfig(DiceID id)
    {
        if (this.dicConfigs != null)
        {
           if (this.dicConfigs.ContainsKey(id))
                return this.dicConfigs[id];
        }
            

        Debug.LogError("ShopCueConfigs GetConfig NOT FOUND: " + id);
        return null;
    }

    private ShopStatConfig GetConfigRisk(DiceID id)
    {
        return this.dicConfigs[id];
    }
}

[System.Serializable]
public class StatItemStats
{
    public const float MIN_DAMAGE = 5f;
    public const float MIN_SPEED = 5f;
    public const float MIN_RANGE = 0f;
    public const float MIN_TIMEEFFECT = 0.0f;

    public const float MAX_DAMAGE = 100f;
    public const float MAX_SPEED = 20f;// old 110
    public const float MAX_RANGE = 6f;
    public const float MAX_TIMEEFFECT = 10.0f;

    [System.NonSerialized]
    public Sprite sprCard;

    [System.NonSerialized]
    public Sprite sprStatItem;

    public long price = 10000;
    public int cardsRequired = 10;

    [Space(5f)]
    public float damageStrength;
    public float speedStrength;
    public float rangeStrength;
    public float timeEffectStrength;

    [Space(5f)]
    [Tooltip("You dont need to assign this")]
    public DiceID id;

    [Obsolete("Should call the static function instead!")]
    public StatItemStats()
    {
        this.damageStrength = 0;
        this.speedStrength = 0;
        this.rangeStrength = 0;
        this.timeEffectStrength = 0;
    }

    public static StatItemStats CreateZero()
    {
        StatItemStats result = new StatItemStats()
        {
            damageStrength = 0f,
            speedStrength = 0f,
            rangeStrength = 0f,
            timeEffectStrength = 0f,
        };
        return result;
    }
    public static StatItemStats CreateBasic()
    {
        StatItemStats result = new StatItemStats()
        {
            damageStrength = MIN_DAMAGE,
            speedStrength = MIN_SPEED,
            rangeStrength = MIN_RANGE,
            timeEffectStrength = MIN_TIMEEFFECT,
        };
        return result;
    }
    public static StatItemStats CreateSafe(float pwr, float aim, float spn, float t)
    {
        StatItemStats result = new StatItemStats()
        {
            damageStrength = Mathf.Clamp(pwr, MIN_DAMAGE, MAX_DAMAGE),
            speedStrength = Mathf.Clamp(aim, MIN_SPEED, MAX_SPEED),
            rangeStrength = Mathf.Clamp(spn, MIN_RANGE, MAX_RANGE),
            timeEffectStrength = Mathf.Clamp(t, MIN_TIMEEFFECT, MAX_TIMEEFFECT),
        };
        return result;
    }

    public static StatItemStats Clone(StatItemStats target)
    {
        StatItemStats result = new StatItemStats()
        {
            damageStrength = target.damageStrength,
            speedStrength = target.speedStrength,
            rangeStrength = target.rangeStrength,
            timeEffectStrength = target.timeEffectStrength,

            id = target.id
        };
        return result;
    }

    public static StatItemStats CreateRandom()
    {
        StatItemStats result = new StatItemStats()
        {
            damageStrength = UnityEngine.Random.Range(MIN_DAMAGE, MAX_DAMAGE),
            speedStrength = UnityEngine.Random.Range(MIN_SPEED, MAX_SPEED),
            rangeStrength = UnityEngine.Random.Range(MIN_RANGE, MAX_RANGE),
            timeEffectStrength = UnityEngine.Random.Range(MIN_TIMEEFFECT, MAX_TIMEEFFECT),
        };
        return result;
    }

    public static StatItemStats CreateForRealUsing(StatItemStats stat, ShopStatConfig config)
    {
        return new StatItemStats()
        {
            damageStrength = stat.damageStrength,
            speedStrength = stat.speedStrength,
            rangeStrength = stat.rangeStrength,
            timeEffectStrength = stat.timeEffectStrength,
            sprCard = config.sprCard,
            sprStatItem = config.sprStatItem,

            id = config.id
        };
    }
}

[System.Serializable]
public class ShopStatConfig
{
    public DiceID id;
    public string statName;
    public string skillDescription;

    public Sprite sprCard;
    public Sprite sprStatItem;

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

    public StatItemStats[] statsPerLevels;

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
            id = original.id,
            statName = original.statName,
            skillDescription = original.skillDescription,

            sprStatItem = original.sprStatItem,
            tier = original.tier,
            unlockType = original.unlockType,
            unlockText = original.unlockText,
            upgradeType = original.upgradeType,
            unlockLinkedId = original.unlockLinkedId,
            appearTier = original.appearTier,
            isHide = original.isHide,
            statsPerLevels = new StatItemStats[original.statsPerLevels.Length]
        };
        for(int iz = 0; iz < newConfig.statsPerLevels.Length; ++iz)
        {
            newConfig.statsPerLevels[iz] = StatItemStats.Clone(original.statsPerLevels[iz]);
            newConfig.statsPerLevels[iz].price = original.statsPerLevels[iz].price;
            newConfig.statsPerLevels[iz].cardsRequired = original.statsPerLevels[iz].cardsRequired;
        }
        return newConfig;
    }
#endif

    public ShopStatConfig(DiceID id, string name, StatItemStats baseStats, int levels, StatItemStats additionsEachLevel)
    {
        this.id = id;
        this.statName = name;
        this.statsPerLevels = new StatItemStats[levels];
        if (levels > 0)
        {
            this.statsPerLevels[0] = baseStats;
            this.AutoParseStats(additionsEachLevel);
        }
    }

    public void AutoParseStats(StatItemStats additionsEachLevel)
    {
        if (this.statsPerLevels == null)
            return;

        int n = this.statsPerLevels.Length;
        if (n == 0 || n == 1)
            return;

        StatItemStats baseStats = this.statsPerLevels[0],
            newStats;
        for (int i = 1; i < n; ++i)
        {
            newStats = StatItemStats.CreateSafe(
                pwr: additionsEachLevel.damageStrength * i + baseStats.damageStrength,
                aim: additionsEachLevel.speedStrength * i + baseStats.speedStrength,
                spn: additionsEachLevel.rangeStrength * i + baseStats.rangeStrength,
                t: additionsEachLevel.timeEffectStrength * i + baseStats.timeEffectStrength
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

    public StatItemStats GetLevelStats(int level)
    {
        if (level <= 0 || this.statsPerLevels == null || level > this.statsPerLevels.Length)
            return StatItemStats.CreateZero();

        return this.statsPerLevels[level - 1];
    }

    public StatItemStats GetFullStat()
    {
        if (this.statsPerLevels == null || this.statsPerLevels.Length == 0)
            return StatItemStats.CreateZero();

        return this.statsPerLevels[this.statsPerLevels.Length - 1];
    }

#if UNITY_EDITOR
    public override string ToString()
    {
        return string.Format("{0}-{1}", this.id, this.statName);
    }
#endif
}
