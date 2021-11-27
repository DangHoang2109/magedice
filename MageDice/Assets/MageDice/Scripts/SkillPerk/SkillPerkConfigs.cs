using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Configs/SkillPerkConfigs", fileName = "SkillPerkConfigs")]
public class SkillPerkConfigs : ScriptableObject
{
    public static SkillPerkConfigs Instance
    {
        get
        {
            return LoaderUtility.Instance.GetAsset<SkillPerkConfigs>("Games/Configs/SkillPerkConfigs");
        }
    }

#if UNITY_EDITOR
    [ContextMenu("Config")]
    private void Editor_Config()
    {
        for (int i = 0; i < Cost.upgradeSteps.Length; i++)
        {
            Cost.upgradeSteps[i] = i == 0 ? 1000 : (long)(Cost.upgradeSteps[i-1] * (2 - i * 0.08f));
        }
        SkillConfigs = new List<SkillPerkConfig>();
        for (int i = 0; i < 12; i++)
        {
            SkillPerkConfig s = new SkillPerkConfig()
            {
                id = i,
                upgradeSteps = new float[10]
            };

            for (int j = 0; j < s.upgradeSteps.Length; j++)
            {
                s.upgradeSteps[j] = j == 0 ? 0.4f : s.upgradeSteps[j-1] + 0.2f;
            }

            SkillConfigs.Add(s);

        }
    }

    [SerializeField] private float Editor_IDConfig;
    [SerializeField] private float Editor_StartConfig;
    [SerializeField] private float Editor_StepConfig;

    [ContextMenu("Config ID 1")]
    private void Editor_ConfigID1()
    {
        SkillPerkConfig s = this.SkillConfigs.Find(x => x.id == Editor_IDConfig);
        for (int i = 0; i < s.upgradeSteps.Length; i++)
        {
            s.upgradeSteps[i] = i == 0 ? Editor_StartConfig : s.upgradeSteps[i - 1] + Editor_StepConfig;
        }
    }
#endif

    public CostUpgradePerkConfig Cost;
    public List<SkillPerkConfig> SkillConfigs;

    public BoosterCommodity GetCostUpgrade(int upgradedTimes)
    {
        if(upgradedTimes >= 0 && upgradedTimes < Cost.upgradeSteps.Length)
            return new BoosterCommodity(Cost.priceType, Cost.upgradeSteps[upgradedTimes]);

        return new BoosterCommodity(Cost.priceType, (long)(Cost.upgradeSteps[Cost.upgradeSteps.Length - 1] * Mathf.Pow((1 + 0.2f), upgradedTimes - Cost.upgradeSteps.Length))); ;
    }

    public float GetUpgradeValue(int id, int currentUpgradeStep)
    {
        SkillPerkConfig c = this.SkillConfigs.Find(x => x.id == id);
        if(c != null)
        {
            if (currentUpgradeStep >= 0 && currentUpgradeStep < Cost.upgradeSteps.Length)
                return c.upgradeSteps[currentUpgradeStep];
        }

        return -1;
    }

    public SkillPerkConfig GetConfig(int id)
    {
        return this.SkillConfigs.Find(x => x.id == id);
    }
}

public class PerkID
{
    public const int TWO_SPOT_DICE = 0;
    public const int CRITICAL_CHANCE = 1;
    public const int RECOVER_HP = 2;
    public const int BULLET_SPEED = 3;
    public const int TOTAL_HP = 4;
    public const int BASE_ATTACK = 5;
    public const int STARTING_COIN = 6;
    public const int KILL_MONS_BONUS = 7;
    public const int OFFLINE_EARNING = 8;
    public const int BAR_SPEED = 9;
    public const int WAVE_REWARD = 10;
    public const int ONLINE_EARNING = 11;
}

[System.Serializable]
public class SkillPerkConfig
{
    public int id;
    public float[] upgradeSteps;
}

[System.Serializable]
public class CostUpgradePerkConfig
{
    public BoosterType priceType;
    public long[] upgradeSteps;
}