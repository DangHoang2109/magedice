using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Games/MageGameConfigs")]
public class MageGameConfigs : ScriptableObject
{
    private static MageGameConfigs _instance;
    public static MageGameConfigs Instance
    {
        get
        {
            if (_instance == null)
            {
                _instance = LoaderUtility.Instance.GetAsset<MageGameConfigs>("Games/Configs/MageGameConfigs");
            }
            return _instance;
        }
    }

    public MageConfig Mage;

    public DicePriceIncrementConfig CoinIncrement;
}

[System.Serializable]
public class PersonConfig
{
    public PersonStatConfig speed;
    public PersonStatConfig damage;
    public PersonStatConfig hp;

    public PersonConfig()
    {

    }
    public PersonConfig(PersonConfig c)
    {
        this.speed = new PersonStatConfig(c.speed);
        this.damage = new PersonStatConfig(c.damage);
        this.hp = new PersonStatConfig(c.hp);
    }
}
[System.Serializable]
public class MageConfig : PersonConfig
{
    public MageUIConfig UI;

    public long stamina;
    [Header("Booster Inital stat")]
    public long initCoin;

    public long addCoinKillMons;
    public long addCoinSkipWave;
    public float recoverHPEachWave;
    public float chance2SpotDice;
    public float chanceCriticalAtk;
    public float timeChargeMana;
    public float barSpeed;
}
[System.Serializable]
public class MageUIConfig
{
    public string name;
    public string description;

    public MageUIConfig()
    {

    }
    public MageUIConfig(MageUIConfig c)
    {
        this.name = c.name;
        this.description = c.description;
    }
}

[System.Serializable]
public class DicePriceIncrementConfig
{
    public long basePrice;
    public long increStep;

    public DicePriceIncrementConfig()
    {

    }
    public DicePriceIncrementConfig(DicePriceIncrementConfig c)
    {

    }
}