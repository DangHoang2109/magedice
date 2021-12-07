using System.Collections;
using System.Collections.Generic;
using UnityEngine;
[CreateAssetMenu(menuName = "Configs/BonusConfigs", fileName = "BonusConfigs")]
public class BonusConfigs : ScriptableObject
{
    private static BonusConfigs _instance;
    public static BonusConfigs Instance
    {
        get
        {
            if (_instance == null)
            {
                _instance = LoaderUtility.Instance.GetAsset<BonusConfigs>("Home/Configs/BonusConfigs");
            }
            return _instance;
        }
    }

    public DailyRewardConfigs dailyReward;

#if UNITY_EDITOR
    [ContextMenu("Config dailyreward")]
    private void Config_Dailyreward()
    {
        dailyReward.config = new DailyRewardConfig[28];
        for (int i = 0; i < 28; i++)
        {
            DailyRewardConfig config = new DailyRewardConfig() { day = i };
            config.reward = new RewardConfig() { boostRate = (i / 7) + 1 };

            int day = i + 1;
            if(day == 2)
            {
                config.reward.boostRate = 1;
                config.reward.bag = new BagAmount() { amount = 1, tour = 1, bagType = BagType.PLATINUM_BAG };
                config.reward.booster = new BoosterCommodity() { type = BoosterType.BAG };
            }
            else if (day % 7 == 0)
            {
                config.reward.boostRate = 1;
                config.reward.bag = new BagAmount() { amount = 1, tour = 1, bagType = BagType.PLATINUM_BAG };
                config.reward.booster = new BoosterCommodity() { type = BoosterType.BAG };
            }
            else if (day % 3 == 0)
            {
                config.reward.booster = new BoosterCommodity() { type = BoosterType.CASH };
            }
            else
            {
                config.reward.boostRate += 1 ;
                config.reward.booster = new BoosterCommodity() { type = BoosterType.COIN };
            }



            dailyReward.config[i] = config;
        }
    }
#endif 
}

#region Dialy Reward
[System.Serializable]
public class DailyRewardConfigs
{
    private static DailyRewardConfigs _instance;
    public static DailyRewardConfigs Instance
    {
        get
        {
            if (_instance == null)
            {
                _instance = BonusConfigs.Instance.dailyReward;
            }
            return _instance;
        }
    }

    public DailyRewardConfig[] config;
    public float boostRatePerMonth;
    public int MonthAmountDay => config.Length;
    public DailyRewardConfig GetDay(int day)
    {
        if (day < 0 || day >= config.Length)
            return null;

        return config[day];
    }

    public List<DailyRewardConfig> GetDayRange(int startDay, int endDay)
    {
        List<DailyRewardConfig> res = new List<DailyRewardConfig>();

        if (startDay >= 0 && endDay < config.Length && startDay <= endDay)
        {
            for (int i = startDay; i <= endDay; i++)
            {
                res.Add(GetDay(i));
            }
        }

        return res;
    }


}
[System.Serializable]
public class DailyRewardConfig
{
    public int day;
    public RewardConfig reward;

    public bool IsRewardBag => reward.IsRewardBag;

    public BoosterCommodity Booster => reward.GetBoosterPrize();

    public BagAmount Bag => reward.GetBagPrize();
}
#endregion Dialy Reward