using System.Collections;
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
    [ContextMenu("Load dice config")]
    private void LoadDiceConfig()
    {
        string fontPath = string.Format("Assets/MageDice/Images/Dice/DiceFrontground/");
        List<Sprite> fontSpr = GameUtils.LoadAllAssetsInFolder<Sprite>(fontPath, new List<string> { "*.png" });

        DiceConfig baseC = this.config[0];
        for (int i = 6; i <= 22; i++)
        {
            DiceID id = (DiceID)i;
            DiceConfig config = new DiceConfig()
            {
                id = id,
                Game = new DiceGameConfig()
                {
                    id = id,
                    levels = new List<DiceGameLevelConfig>(baseC.Game.levels).ToArray(),
                    bullet = new DiceBulletConfig()
                    {
                        normalBullet = new DiceBulletStateConfig(),
                        rageBullet = new DiceBulletStateConfig(),
                    }
                },
                Info = new DiceInfoConfig()
                {
                    id = id,
                    name = id.ToString(),
                    front = fontSpr.Find(x => x.name.Equals($"Dice_{id}")),
                    effectDescription = ShopStatConfigs.Instance.GetConfig(id).skillDescription,
                    dotColor = baseC.Info.dotColor
                }
            };

            this.config.Add(config);
        }


    }
    [ContextMenu("Load bullet sprite")]
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

    [Type(typeof(IronDiceEffect))]
    IRON, 
    [Type(typeof(GaleDiceEffect))]
    GALE,
    [Type(typeof(ArrowDiceEffect))]
    ARROW, 
    [Type(typeof(MineDiceEffect))]
    MINE, 
    [Type(typeof(CriticalDiceEffect))]
    CRITICAL,
    [Type(typeof(EnergyDiceEffect))]
    ENERGY, 
    [Type(typeof(GiftDiceEffect))]
    GIFT,
    [Type(typeof(SlingshotDiceEffect))]
    SLINGSHOT,
    [Type(typeof(Light))]
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
    [Type(typeof(WaveDiceEffect))]
    WAVE,
    [Type(typeof(InfectDiceEffect))]
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

    public DiceGameConfig()
    {

    }
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