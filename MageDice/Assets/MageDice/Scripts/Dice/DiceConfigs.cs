﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Games/DiceConfigs")]
public class DiceConfigs : ScriptableObject
{
    [Header("Dot Position Config")]
    public List<DiceDotConfig> dotConfig; //config posiotn for 6 dot rank

    [Header("Dice Info Config")]
    public List<DiceConfig> config;

    [Header("Dice Booster Deck")]
    public DiceBoosterConfigs boosterConfigs;

    //public List<MissionConfig> configs; //daily mission
    private static DiceConfigs _instance;
    public static DiceConfigs Instance
    {
        get
        {
            if (_instance == null)
            {
                _instance = LoaderUtility.Instance.GetAsset<DiceConfigs>("Games/Configs/DiceConfigs");
            }
            return _instance;
        }
    }

#if UNITY_EDITOR
    private void OnValidate()
    {
        //foreach (DiceConfig c in this.config)
        //{
        //    for (int i = 1; i < 6; i++)
        //    {
        //        c.Game.levels[i].damage = c.Game.levels[0].damage * (i + 1);
        //    }
        //}
    }

    [ContextMenu("Load Sprite")]
    private void LoadBulletSprite()
    {
        string fontPath = string.Format("Assets/MageDice/Images/Bullet/Font/");
        List<Sprite> fontSpr = GameUtils.LoadAllAssetsInFolder<Sprite>(fontPath, new List<string> { "*.png"});

        string lightPath = string.Format("Assets/MageDice/Images/Bullet/Light/");
        List<Sprite> lightSpr = GameUtils.LoadAllAssetsInFolder<Sprite>(lightPath, new List<string> { "*.png" });

        foreach(DiceConfig config in this.config)
        {
            Sprite f = fontSpr.Find(x => x.name.Equals(config.id.ToString()));
            if (f == null)
                Debug.Log($"CAN NOT FIND {config.id}");
            config.Game.bullet.normalBullet.sprBullet = f;

            Sprite l = lightSpr.Find(x => x.name.Equals(config.id.ToString()));
            if (l == null)
                Debug.Log($"CAN NOT FIND {config.id}");
            config.Game.bullet.normalBullet.sprLight = l;

        }
    }
    [ContextMenu("Load Stat")]
    private void LoadBulletStat()
    {
        foreach (DiceConfig config in this.config)
        {
            for (int i = 0; i < config.Game.levels.Length; i++)
            {
                config.Game.levels[i].damageMultiplier = (i + 1);
            }
        }
    }

    [ContextMenu("Load Booster")]
    private void LoadBooster()
    {
        boosterConfigs = new DiceBoosterConfigs();
        boosterConfigs.level = new DiceBoosterConfig[10];
        for (int i = 0; i < 10; i++)
        {
            boosterConfigs.level[i] = new DiceBoosterConfig()
            {
                costUpgradeNext = i == 0 ? 400 : boosterConfigs.level[i - 1].costUpgradeNext + 300,
                currentBoostPercent = i == 0 ? 1f : boosterConfigs.level[i - 1].currentBoostPercent + (0.7f - i * 0.065f)
            };
        }
    }
#endif
    public DiceConfig GetConfig(DiceID id)
    {
        return this.config.Find(x => x.id == id);
    }
    public DiceDotConfig GetDotConfig(int dot)
    {
        return this.dotConfig[dot-1];
    }
}

public enum DiceID
{
    NONE = 0,

    [Type(typeof(FireDiceEffect))]
    FIRE,
    [Type(typeof(IceDiceEffect))]
    ICE,
    [Type(typeof(WindDiceEffect))]
    WIND,
    [Type(typeof(ElectricDiceEffect))]
    ELECTRIC,
    [Type(typeof(PoisionDiceEffect))]
    POISION,

    [Type(typeof(PoisionDiceEffect))]
    IRON, 
    [Type(typeof(PoisionDiceEffect))]
    GALE,
    [Type(typeof(PoisionDiceEffect))]
    ARROW, 
    [Type(typeof(PoisionDiceEffect))]
    MINE, 
    [Type(typeof(PoisionDiceEffect))]
    CRITICAL,
    [Type(typeof(PoisionDiceEffect))]
    ENERGY, 
    [Type(typeof(PoisionDiceEffect))]
    GIFT,
    [Type(typeof(PoisionDiceEffect))]
    SLINGSHOT,
    [Type(typeof(PoisionDiceEffect))]
    LIGHT, 
    [Type(typeof(PoisionDiceEffect))]
    ABSORD, 
    [Type(typeof(PoisionDiceEffect))]
    MIMIC,
    [Type(typeof(PoisionDiceEffect))]
    CLONE,
    [Type(typeof(PoisionDiceEffect))]
    DEATH,
    [Type(typeof(PoisionDiceEffect))]
    FLAME,
    [Type(typeof(PoisionDiceEffect))]
    HEAL,
    [Type(typeof(PoisionDiceEffect))]
    WAVE,
    [Type(typeof(PoisionDiceEffect))]
    INFECT,
}
[System.Serializable]
public class DiceConfig
{
    public DiceID id;
    //public Sprite front;
    //public string name;
    //public string effectDescription;

    //public Color dotColor;

    public DiceInfoConfig Info;
    public DiceGameConfig Game;

}
[System.Serializable]
public class DiceDotConfig
{
    public List<Vector3> positions;
}
[System.Serializable]
public class DiceInfoConfig
{
    public DiceID id;
    public Sprite front;
    public string name;
    public string effectDescription;

    public Color dotColor;

}
[System.Serializable]
public class DiceGameConfig
{
    public DiceID id;
    public DiceGameLevelConfig[] levels;
    public DiceBulletConfig bullet;
}
[System.Serializable]
public class DiceGameLevelConfig
{
    public int dot;

    public float damageMultiplier;
    //public float speed;
    //public float range;

    //public float timeEffect;
}

[System.Serializable]
public class DiceBulletStateConfig
{
    public Sprite sprBullet;
    public Sprite sprLight;
}

[System.Serializable]
public class DiceBulletConfig
{
    public DiceBulletStateConfig normalBullet;

    public DiceBulletStateConfig rageBullet;
}
[System.Serializable]
public class DiceBoosterConfigs
{
    public DiceBoosterConfig[] level;

    public DiceBoosterConfig GetLevel(int level)
    {
        if (level < 0 || level >= this.level.Length)
            return null;

        return this.level[level];
    }
}
[System.Serializable]
public class DiceBoosterConfig
{
    public long costUpgradeNext;
    public float currentBoostPercent;
    public bool isMax;
}