using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Games/GameMapConfigs")]
public class GameMapConfigs : ScriptableObject
{
    //public List<MissionConfig> configs; //daily mission
    private static GameMapConfigs _instance;
    public static GameMapConfigs Instance
    {
        get
        {
            if (_instance == null)
            {
                _instance = LoaderUtility.Instance.GetAsset<GameMapConfigs>("Games/Configs/GameMapConfigs");
            }
            return _instance;
        }
    }

#if UNITY_EDITOR
    [ContextMenu("Config/SetUpCreep")]
    private void SetUpCreep()
    {
        float upSpeed = 0.025f;
        float upDamage = 0.025f;
        float upHP = 0.1f;

        MapConfig green = this.maps.Find(x => x.ID == MapName.GREENLAND);
        //green.monsterConfig.Add(new MonsterConfig()
        //{
        //    ID = MonsterType.NORMAL_CREEP,

        //    speed = new MonsterStatConfig()
        //    {
        //        init_stat = 5,
        //        percentUpEachWave = upSpeed
        //    },

        //    damage = new MonsterStatConfig()
        //    {
        //        init_stat = 5,
        //        percentUpEachWave = upDamage
        //    },

        //    hp = new MonsterStatConfig()
        //    {
        //        init_stat = 25,
        //        percentUpEachWave = upHP
        //    },

        //    UI = new MonsterUIConfig()
        //    {
        //        name = "Normal Creep",
        //    }
        //}); ;

        green.monsterConfig.Add(new MonsterConfig()
        {
            ID = MonsterType.SPEEDER_CREEP,

            speed = new PersonStatConfig()
            {
                init_stat = 10,
                percentUpEachWave = upSpeed
            },

            damage = new PersonStatConfig()
            {
                init_stat = 3,
                percentUpEachWave = upDamage
            },

            hp = new PersonStatConfig()
            {
                init_stat = 15,
                percentUpEachWave = upHP
            },

            UI = new MonsterUIConfig()
            {
                name = "Speeder",
            }
        });;

        green.monsterConfig.Add(new MonsterConfig()
        {
            ID = MonsterType.ELITE_CREEP,

            speed = new PersonStatConfig()
            {
                init_stat = 5,
                percentUpEachWave = upSpeed
            },

            damage = new PersonStatConfig()
            {
                init_stat = 10,
                percentUpEachWave = upDamage
            },

            hp = new PersonStatConfig()
            {
                init_stat = 30,
                percentUpEachWave = upHP
            },

            UI = new MonsterUIConfig()
            {
                name = "Elite Creep",
            }
        });

        green.monsterConfig.Add(new MonsterConfig()
        {
            ID = MonsterType.SUPER_CREEP,

            speed = new PersonStatConfig()
            {
                init_stat = 3,
                percentUpEachWave = upSpeed
            },

            damage = new PersonStatConfig()
            {
                init_stat = 15,
                percentUpEachWave = upDamage
            },

            hp = new PersonStatConfig()
            {
                init_stat = 40,
                percentUpEachWave = upHP
            },

            UI = new MonsterUIConfig()
            {
                name = "Super Creep",
            }
        });

        green.monsterConfig.Add(new MonsterConfig()
        {
            ID = MonsterType.MINI_BOSS,

            speed = new PersonStatConfig()
            {
                init_stat = 3,
                percentUpEachWave = upSpeed
            },

            damage = new PersonStatConfig()
            {
                init_stat = 25,
                percentUpEachWave = upDamage
            },

            hp = new PersonStatConfig()
            {
                init_stat = 150,
                percentUpEachWave = upHP
            },

            UI = new MonsterUIConfig()
            {
                name = "Mini Boss",
            }
        });

        green.monsterConfig.Add(new MonsterConfig()
        {
            ID = MonsterType.SKILL_BOSS,

            speed = new PersonStatConfig()
            {
                init_stat = 3,
                percentUpEachWave = upSpeed
            },

            damage = new PersonStatConfig()
            {
                init_stat = 50,
                percentUpEachWave = upDamage
            },

            hp = new PersonStatConfig()
            {
                init_stat = 200,
                percentUpEachWave = upHP
            },

            UI = new MonsterUIConfig()
            {
                name = "Skill Boss",
            }
        });
    }

    [ContextMenu("Config/EditCreep")]
    private void EditCreep()
    {
        //float upSpeed = 0.025f;
        //float upDamage = 0.025f;
        //float upHP = 0.1f;

        MapConfig green = this.maps.Find(x => x.ID == MapName.GREENLAND);

        foreach(MonsterConfig m in green.monsterConfig)
        {
            if (m.ID == MonsterType.MINI_BOSS || m.ID == MonsterType.SKILL_BOSS)
                continue;

            //m.coinGifted = (int)m.ID;
            //m.speed.init_stat *= 0.8f;
            m.hp.init_stat *= 1.25f;
        }
    }

    [ContextMenu("Config/EditWave")]
    private void EditWave()
    {
        //float upSpeed = 0.025f;
        //float upDamage = 0.025f;
        //float upHP = 0.1f;

        MapConfig green = this.maps.Find(x => x.ID == MapName.GREENLAND);

        //for (int i = 0; i < green.waves.Count; i++)
        //{
        //    WaveConfig w = green.waves[i];
        //    w.length = 20;
        //    w.lengthSpawn = 15;

        //    w.monsterRate = new List<WaveMonsterRateConfig>();

        //    w.monsterRate.Add(new WaveMonsterRateConfig()
        //    {
        //        monsterID = MonsterType.NORMAL_CREEP
        //    });

        //    if (i == 0) continue;

        //    w.monsterRate.Add(new WaveMonsterRateConfig()
        //    {
        //        monsterID = MonsterType.ELITE_CREEP
        //    });

        //    if (i == 1) continue;

        //    w.monsterRate.Add(new WaveMonsterRateConfig()
        //    {
        //        monsterID = MonsterType.SUPER_CREEP
        //    });
        //}
        for (int i = 0; i < green.waves.Count; i++)
        {
            if (i < 5)
                green.waves[i].intervalSpawn = 1.2f;
            else if(i < 9)
                green.waves[i].intervalSpawn = 1f;
            else if (i < 19)
                green.waves[i].intervalSpawn = 0.8f;
            else
                green.waves[i].intervalSpawn = 0.6f;
            //foreach (WaveMonsterRateConfig item in green.waves[i].monsterRate)
            //{
            //    item.rate = 1f/ green.waves[i].monsterRate.Count;
            //}
        }

        //green.waves[9].monsterRate = new List<WaveMonsterRateConfig>() { new WaveMonsterRateConfig() { monsterID = MonsterType.MINI_BOSS, rate = 1 } };
        //green.waves[19].monsterRate = new List<WaveMonsterRateConfig>() { new WaveMonsterRateConfig() { monsterID = MonsterType.MINI_BOSS, rate = 1 } };
        //green.waves[29].monsterRate = new List<WaveMonsterRateConfig>() { new WaveMonsterRateConfig() { monsterID = MonsterType.SKILL_BOSS, rate = 1 } };

    }

    [ContextMenu("Config/SimulateOtherRoom")]
    private void EditSimulateOtherRoom()
    {
        MapConfig green = maps.Find(x => x.ID == MapName.GREENLAND);

        for (int i = 1; i < this.maps.Count; i++)
        {
            MapConfig map = maps[i];
            MapConfig preMap = maps[i-1];

            for (int j = 0; j < map.monsterConfig.Count; j++)
            {
                MonsterConfig m = map.monsterConfig[j];
                m.damage.init_stat =  preMap.monsterConfig[j].damage.init_stat * 2;
                m.hp.init_stat = preMap.monsterConfig[j].hp.init_stat * 2;
            }

        }
    }
#endif

    public List<MapConfig> maps;

    public MapConfig GetMap(MapName id)
    {
        return maps.Find(x => x.ID == id);
    }
}
public enum MapName
{
    NONE = -1,
    CHEST = 0,

    GREENLAND = 1,
    STORMDESERT = 2,
    FORZENPINNACLE = 3,
    CRYSTALMINE = 4,
    CAVEBONE = 5,
    SILENTEXPANSE = 6,
    EVILDOOM = 7,
    DEATHCAPITAL = 8,
}
[System.Serializable]
public class MapUIConfig
{
    public MapName ID;
    public string name;
    public string description;

    public Sprite bgGame;
}
[System.Serializable]
public class MapConfig
{
    public MapName ID;
    public string mapName;
    public List<WaveConfig> waves;
    [Space(5f)]
    public List<MonsterConfig> monsterConfig;

    public BossSkillHandler FinalBossID;

    public string GetName()
    {
        return this.mapName;
    }
}
[System.Serializable]
public class WaveConfig
{
    public List<WaveMonsterRateConfig> monsterRate;

    public float length;
    public float lengthSpawn;
    public float intervalSpawn = 0.25f;

    public WaveConfig()
    {

    }
    public WaveConfig(WaveConfig c)
    {
        this.monsterRate = new List<WaveMonsterRateConfig>(c.monsterRate);
        this.length = c.length;
        this.lengthSpawn = c.lengthSpawn;
        this.intervalSpawn = c.intervalSpawn;
    }
}
[System.Serializable]
public class WaveMonsterRateConfig
{
    public MonsterType monsterID;
    public float rate;

    public bool IsBoss
    {
        get
        {
            return (int)this.monsterID / 10 == 2;
        }
    }
}

public enum MonsterType
{
    NONE = 0,
    NORMAL_CREEP = 10,
    ELITE_CREEP = 11,
    SUPER_CREEP = 12,
    SPEEDER_CREEP = 13,
    GROUP_CREEP = 14,

    MINI_BOSS = 20,
    ELITE_BOSS = 21,
    SUPER_BOSS = 22,
    SKILL_BOSS = 23,

    CHEST = 50,
}
public enum BossSkillHandler
{
    NONE = 0,

    [Type(typeof(DragonBossSkillHandler))]
    DRAGON = 10,
    [Type(typeof(SandManBossSkillHandler))]
    SANDMAN = 11,
    [Type(typeof(CerberusSBossSkillHandler))]
    CERBERUS = 12,
    [Type(typeof(OrgeBossSkillHandler))]
    ORGE = 13,
    [Type(typeof(DarkKnightBossSkillHandler))]
    DARKKNIGHT = 14,

}
[System.Serializable]
public class MonsterConfig : PersonConfig
{
    public MonsterType ID;

    public MonsterUIConfig UI;
    public long coinGifted;

    public MonsterConfig() : base()
    {

    }
    public MonsterConfig(MonsterConfig c) : base(c)
    {
        this.ID = c.ID;
        this.UI = new MonsterUIConfig(c.UI);
    }
}
[System.Serializable]
public class MonsterUIConfig
{
    public Sprite spr;
    public string name;
    public string description;

    public float scale;

    public MonsterUIConfig()
    {

    }
    public MonsterUIConfig(MonsterUIConfig c)
    {
        this.spr = c.spr;
        this.name = c.name;
        this.description = c.description;
        this.scale = c.scale;
    }
}
[System.Serializable]
public class PersonStatConfig
{
    public float init_stat;
    public float percentUpEachWave;

    public PersonStatConfig()
    {

    }
    public PersonStatConfig(PersonStatConfig c)
    {
        this.init_stat = c.init_stat;
        this.percentUpEachWave = c.percentUpEachWave;
    }
}


