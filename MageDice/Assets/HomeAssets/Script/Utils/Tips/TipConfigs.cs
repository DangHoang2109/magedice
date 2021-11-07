using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Configs/TipConfigs", fileName = "TipConfigs")]
public class TipConfigs : ScriptableObject
{
    public List<TipConfig> tips;
    private static TipConfigs _instance;

    public static TipConfigs Instance
    {
        get
        {
            if (_instance == null)
            {
                _instance = LoaderUtility.Instance.GetAsset<TipConfigs>("Home/Configs/TipConfigs");
            }

            return _instance;
        }
    }
    public int GetRandomTipIndex()
    {
        if (this.tips.Count > 0)
        {
            
            int index = Random.Range(0, this.tips.Count);
            return index;
        }
        return -1;
    }
}

[System.Serializable]
public class TipConfig
{
    public int id;
    public TipType type;
    public string key;
    public string message;
}

public enum TipType
{
    COMMON = 0,
    GAME = 1,
    TIP = 2
}
