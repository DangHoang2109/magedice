using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static OnlineRewardConfigs;

[CreateAssetMenu(menuName = "Configs/OnlineRewardConfigs")]
public class OnlineRewardConfigs : ScriptableObject
{
    private static OnlineRewardConfigs _instance;
    public static OnlineRewardConfigs Instance
    {
        get
        {
            if (_instance == null)
            {
                _instance = LoaderUtility.Instance.GetAsset<OnlineRewardConfigs>("Home/Configs/OnlineRewardConfigs");
            }
            return _instance;
        }
    }

    public enum OnlineRewardUserMode
    {
        NONE = 0,
        FREE,
        PAID,
    }

    public List<OnlineRewardConfig> configs;

    public OnlineRewardConfig GetConfig(OnlineRewardUserMode id)
    {
        return configs.Find(x => x.id == id);
    }
    public OnlineRewardDayConfig GetDayConfig(OnlineRewardUserMode id, int currentDay)
    {
        OnlineRewardConfig c = configs.Find(x => x.id == id);
        if(c != null)
        {
            return c.GetConfig(currentDay);
        }
        return null;
    }
}
[System.Serializable]
public class OnlineRewardConfig
{
    public OnlineRewardUserMode id;
    public List<OnlineRewardDayConfig> config;

    public OnlineRewardDayConfig GetConfig(int currentDay)
    {
        for (int i = 1; i < this.config.Count; i++)
        {
            if (currentDay < this.config[i].dayStart)
                return config[i-1];

        }
        return this.config[this.config.Count - 1];
    }
    public List<OnlineRewardConfigItem> GetRewardList(int currentDay)
    {
        return new List<OnlineRewardConfigItem>(GetConfig(currentDay).config);
    }
}

[System.Serializable]
public class OnlineRewardDayConfig
{
    public int dayStart;
    public int dayEnd;
    public List<OnlineRewardConfigItem> config;

    public int RewardAmount => config == null ? 0 : config.Count;
    public List<OnlineRewardConfigItem> GetReward()
    {
        return new List<OnlineRewardConfigItem>(config);
    }
}

[System.Serializable]
public class OnlineRewardConfigItem
{
    public RewardConfig rewardConfig;

    public BagAmount Bag => rewardConfig.GetBagPrize();
    public BoosterCommodity Booster => rewardConfig.GetBoosterPrize();

    public bool IsRewardBag => rewardConfig.IsRewardBag;
}

