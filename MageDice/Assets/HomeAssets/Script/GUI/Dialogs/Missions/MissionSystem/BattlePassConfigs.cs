using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
[CreateAssetMenu(menuName = "Games/BattlePassConfigs")]
public class BattlePassConfigs : ScriptableObject
{
    [Header("Battlepass Mission")]
    public List<BattlePassConfig> config;

    //public List<MissionConfig> configs; //daily mission
    private static BattlePassConfigs _instance;
    public static BattlePassConfigs Instance
    {
        get
        {
            if (_instance == null)
            {
                _instance = LoaderUtility.Instance.GetAsset<BattlePassConfigs>("Home/Configs/BattlePassConfigs");
            }
            return _instance;
        }
    }
    public BattlePassConfig GetPass(int id)
    {
        return this.config.Find(x => x.id == id);
    }
    public BattlePassConfig GetPass(string name)
    {
        return this.config.Find(x => x.name.Equals(name));
    }

    public BattlePassMissionStep GetPassStep(int passID, int stepID)
    {
        return this.GetPass(passID).step.Find(x => x.id == stepID);
    }

#if UNITY_EDITOR
    public void OnValidate()
    {
    }
#endif

    [ContextMenu("Config Step")]
    private void ConfigStep()
    {
        foreach(BattlePassConfig c in this.config)
        {
            foreach(BattlePassMissionStep s in c.step)
            {
                s.step = (long)(s.step * 0.75f);
            }
        }
         
    }
}

#region Battle Pass
public enum BattlePassType { FREE_PASS, PRO_PASS }


[System.Serializable]
public class BattlePassConfig
{
    public int id;
    public string name;
    public double length;

    public List<BattlePassMissionStep> step;

    public BattlePassConfig()
    {
        this.id = 0;
        this.name = "";
        this.length = 0;

        this.step = new List<BattlePassMissionStep>();
    }
    public BattlePassConfig(BattlePassConfig c)
    {
        this.id = c.id;
        this.name = c.name;
        this.length = c.length;

        this.step = new List<BattlePassMissionStep>(c.step);
    }
}

[System.Serializable]
public class BattlePassMissionStep
{
    [System.Serializable]
    public class StepReward
    {
        public BoosterCommodity reward;
        public BagAmount bag;
    }

    public int id;
    public long step;

    public StepReward freeReward;
    public StepReward proReward;

    public BattlePassMissionStep()
    {
        this.step = 0;
    }
    public BattlePassMissionStep(BattlePassMissionStep c)
    {
        this.step = c.step;
    }

    public long GetStep(int tour)
    {
        return RedefindingStep(tour);
    }

    /// <summary>
    /// Khi nhận output, nhớ check nếu là bag thì gọi lấy GetBag() để nhận reward
    /// </summary>
    /// <param name="passType"></param>
    /// <param name="tour"></param>
    /// <returns></returns>
    public BoosterCommodity GetReward(BattlePassType passType, int tour)
    {
        return RedefindingReward_Booster(passType,tour);
    }

    /// <summary>
    /// Null nếu reward của step này không phải bag
    /// </summary>
    /// <param name="passType"></param>
    /// <param name="tour"></param>
    /// <returns></returns>
    public BagAmount GetBag(BattlePassType passType, int tour)
    {
        return RedefindingReward_Bag(passType,tour);
    }

    private long RedefindingStep(int tour)
    {
        return this.step;
    }


    /// <summary>
    /// Khi nhận output, nhớ check nếu là bag thì gọi lấy bag reward.
    /// </summary>
    /// <param name="passType"></param>
    /// <param name="tour"></param>
    /// <returns></returns>
    private BoosterCommodity RedefindingReward_Booster(BattlePassType passType,int tour)
    {
        ///Convert value to the tour
        ///Value booster coin = rateBotWin_normalize of that tour * tour fee join => Treat as user free win a match
        
        BoosterCommodity booster = passType == BattlePassType.FREE_PASS ? this.freeReward.reward : this.proReward.reward;

        if (booster == null)
        {
            Debug.LogError("DAMN BRO THIS STEP REWARD IS BAG");
            return null;
        }

        if (booster.type == BoosterType.CASH)
            return booster;

        if(booster.type == BoosterType.COIN)
        {
            RoomConfig room = RoomConfigs.Instance.GetRoom(tour);
            if(room == null)
                room = RoomConfigs.Instance.GetRoom(RoomDatas.Instance.GetRoomUnlockedMax());

            return new BoosterCommodity(key: booster.type,
                value: (long)((50f / 100) * room.prizePerWave.GetValue()));
        }
            

        return passType == BattlePassType.FREE_PASS ? this.freeReward.reward : this.proReward.reward;
    }

    /// <summary>
    /// Return the bag of Pass [type], with the input tour
    /// Null nếu reward của step này không phải bag
    /// </summary>
    /// <param name="passType"></param>
    /// <param name="tour"></param>
    /// <returns></returns>
    private BagAmount RedefindingReward_Bag(BattlePassType passType, int tour)
    {
        //TODO
        //RoomDatas RoomDatas = RoomDatas.Instance;
        //int MaxTourUnlocked = RoomDatas.GetRoomUnlockedMax();

        StepReward r = passType == BattlePassType.FREE_PASS ? this.freeReward : this.proReward;

        if (r.bag.bagType != BagType.FREE_BAG)
            return new BagAmount()
            {
                bagType = r.bag.bagType,
                amount = r.bag.amount,
                tour = tour
            };
        else
            return null;
    }
}



#endregion Battle Pass