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

    FIRE,
    ICE,
    WIND,
    ELECTRIC,
    POISION
}
[System.Serializable]
public class DiceConfig
{
    public DiceID id;
    public Sprite front;
    public string name;
    public string effectDescription;

    public Color dotColor;

}
[System.Serializable]
public class DiceDotConfig
{
    public List<Vector3> positions;
}