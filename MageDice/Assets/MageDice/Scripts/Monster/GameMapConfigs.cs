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

            speed = new MonsterStatConfig()
            {
                init_stat = 10,
                percentUpEachWave = upSpeed
            },

            damage = new MonsterStatConfig()
            {
                init_stat = 3,
                percentUpEachWave = upDamage
            },

            hp = new MonsterStatConfig()
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

            speed = new MonsterStatConfig()
            {
                init_stat = 5,
                percentUpEachWave = upSpeed
            },

            damage = new MonsterStatConfig()
            {
                init_stat = 10,
                percentUpEachWave = upDamage
            },

            hp = new MonsterStatConfig()
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

            speed = new MonsterStatConfig()
            {
                init_stat = 3,
                percentUpEachWave = upSpeed
            },

            damage = new MonsterStatConfig()
            {
                init_stat = 15,
                percentUpEachWave = upDamage
            },

            hp = new MonsterStatConfig()
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

            speed = new MonsterStatConfig()
            {
                init_stat = 3,
                percentUpEachWave = upSpeed
            },

            damage = new MonsterStatConfig()
            {
                init_stat = 25,
                percentUpEachWave = upDamage
            },

            hp = new MonsterStatConfig()
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

            speed = new MonsterStatConfig()
            {
                init_stat = 3,
                percentUpEachWave = upSpeed
            },

            damage = new MonsterStatConfig()
            {
                init_stat = 50,
                percentUpEachWave = upDamage
            },

            hp = new MonsterStatConfig()
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
            m.hp.init_stat *= 10;
            //switch (m.ID)
            //{
            //    case MonsterType.NORMAL_CREEP:
            //        m.UI.scale = 1f;
            //        break;
            //    case MonsterType.SPEEDER_CREEP:
            //        m.UI.scale = 1.2f;
            //        break;
            //    case MonsterType.ELITE_CREEP:
            //        m.UI.scale = 1.2f;
            //        break;
            //    case MonsterType.SUPER_CREEP:
            //        m.UI.scale = 1.5f;
            //        break;
            //    case MonsterType.MINI_BOSS:
            //        m.UI.scale = 1.75f;
            //        break;
            //    case MonsterType.SKILL_BOSS:
            //        m.UI.scale = 2f;
            //        break;
            //}
           
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
            green.waves[i].intervalSpawn = 0.5f;
            //foreach (WaveMonsterRateConfig item in green.waves[i].monsterRate)
            //{
            //    item.rate = 1f/ green.waves[i].monsterRate.Count;
            //}
        }

        //green.waves[9].monsterRate = new List<WaveMonsterRateConfig>() { new WaveMonsterRateConfig() { monsterID = MonsterType.MINI_BOSS, rate = 1 } };
        //green.waves[19].monsterRate = new List<WaveMonsterRateConfig>() { new WaveMonsterRateConfig() { monsterID = MonsterType.MINI_BOSS, rate = 1 } };
        //green.waves[29].monsterRate = new List<WaveMonsterRateConfig>() { new WaveMonsterRateConfig() { monsterID = MonsterType.SKILL_BOSS, rate = 1 } };
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
    NONE = 0,
    CHEST = 1,

    GREENLAND,
    ICELAND,
    VOLCANO,
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
    public List<WaveConfig> waves;
    [Space(5f)]
    public List<MonsterConfig> monsterConfig;

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

[System.Serializable]
public class PersonConfig
{
    public MonsterStatConfig speed;
    public MonsterStatConfig damage;
    public MonsterStatConfig hp;

    public PersonConfig()
    {

    }
    public PersonConfig(PersonConfig c)
    {
        this.speed = new MonsterStatConfig(c.speed);
        this.damage = new MonsterStatConfig(c.damage);
        this.hp = new MonsterStatConfig(c.hp);
    }
}
[System.Serializable]
public class MonsterConfig : PersonConfig
{
    public MonsterType ID;

    public MonsterUIConfig UI;

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
public class MonsterStatConfig
{
    public float init_stat;
    public float percentUpEachWave;

    public MonsterStatConfig()
    {

    }
    public MonsterStatConfig(MonsterStatConfig c)
    {
        this.init_stat = c.init_stat;
        this.percentUpEachWave = c.percentUpEachWave;
    }
}


