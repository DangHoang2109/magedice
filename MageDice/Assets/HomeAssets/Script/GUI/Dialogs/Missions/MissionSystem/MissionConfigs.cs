using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
[CreateAssetMenu(menuName = "Games/MissionConfigs")]
public class MissionConfigs : ScriptableObject
{
    [Header("Daily mission")]
    public List<MissionConfig> configs;

    [Header("Point bag mission")]
    public MissionConfig pointBagConfig;

    [Space(5f)]
    public int NUM_MISSION_IN_DAY = 5;
    public int MAX_POINT_BAGS_AVAILABLE = 2;

    //public List<MissionConfig> configs; //daily mission
    private static MissionConfigs _instance;
    public static MissionConfigs Instance
    {
        get
        {
            if (_instance == null)
            {
                _instance = LoaderUtility.Instance.GetAsset<MissionConfigs>("Home/Configs/MissionConfigs");
            }
            return _instance;
        }
    }

    public MissionConfig GetMission(int id)
    {
        if (id == MissionID.POINT_BAGS)
            return pointBagConfig;

        if (configs != null && configs.Count > 0)
            return configs.Find(x => x.id == id);

        return null;
    }

    public List<MissionConfig> GetDailyMission()
    {
        //return this.configs;

        int currentTour = RoomDatas.Instance.GetRoomUnlockedMax();

        //TODO: Get it more detail
        IEnumerable<MissionConfig> availableMission = this.configs
            .Where(x => x.startLevel <= currentTour && x.endLevel >= currentTour);

        if (availableMission.Count() > this.NUM_MISSION_IN_DAY)
            return availableMission
                .ToList().Shuffle()
                .Take(NUM_MISSION_IN_DAY)
                .ToList();
        else
            return availableMission.ToList();

    }

#if UNITY_EDITOR
    public void OnValidate()
    {
        #region Free Pass
        //BattlePassMissionConfig free = new BattlePassMissionConfig();
        //free.steps = new List<BattlePassMissionStep>();
        //for (int i = 0; i <= 20; i++)
        //{
        //    BattlePassMissionStep s = new BattlePassMissionStep();

        //    s.id = 100 + i;
        //    s.PassType = BattlePassType.FREE_PASS;
        //    s.step = GameUtils.RoundUpValue((long)(120 * Mathf.Pow(1.2f, i - 1)), 1);

        //    if(i == 0)
        //        s.reward = new BoosterCommodity(BoosterType.CASH, 10);
        //    else if(i == 7)
        //    {
        //        s.reward = new BoosterCommodity(BoosterType.BAG, 1);
        //        s.bag = new BagAmount() { bagType = BagType.PRISM_BAG, amount = 1, tour = 0 };
        //    }
        //    else if (i == 20)
        //    {
        //        s.reward = new BoosterCommodity(BoosterType.BAG, 1);
        //        s.bag = new BagAmount() { bagType = BagType.KING_BAG, amount = 1, tour = 0 };
        //    }
        //    else
        //    {
        //        if(i % 2 == 0)
        //        {
        //            s.reward = new BoosterCommodity(BoosterType.BAG, 1);
        //            s.bag = new BagAmount() { bagType = BagType.GOLD_BAG, amount = 1, tour = 0 };
        //        }
        //        else
        //        {
        //            s.reward = new BoosterCommodity(BoosterType.BAG, 1);
        //            s.bag = new BagAmount() { bagType = BagType.SILVER_BAG, amount = 1, tour = 0 };
        //        }
        //    }

        //    free.steps.Add(s);
        //}

        //this.battlepasss.Add(free);

        #endregion Free Pass

        #region Pro pass
        //BattlePassMissionConfig pro = new BattlePassMissionConfig();
        //pro.steps = new List<BattlePassMissionStep>();
        //for (int i = 0; i <= 20; i++)
        //{
        //    BattlePassMissionStep s = new BattlePassMissionStep();

        //    s.id = 200 + i;
        //    s.PassType = BattlePassType.PRO_PASS;

        //    s.step = this.GetPass(BattlePassType.FREE_PASS).steps[i].step;

        //    if (i == 0 || i == 20)
        //    {
        //        s.reward = new BoosterCommodity(BoosterType.BAG, 1);
        //        s.bag = new BagAmount() { bagType = BagType.KING_BAG, amount = 1, tour = 0 };
        //    }
        //    else if (i == 1 || i == 3 || i == 5 || i == 9 || i == 13 || i == 16)
        //    {
        //        s.reward = new BoosterCommodity(BoosterType.COIN, 75 * GameDefine.RATE_CASH_TO_COIN);
        //    }
        //    else if (i == 2 || i == 12)
        //    {
        //        s.reward = new BoosterCommodity(BoosterType.BAG, 1);
        //        s.bag = new BagAmount() { bagType = BagType.SUPER_BAG_1, amount = 1, tour = 0 };
        //    }
        //    else if (i == 4 || i == 7 || i == 11 || i == 15 || i == 17 || i == 19)
        //    {
        //        s.reward = new BoosterCommodity(BoosterType.BAG, 1);
        //        s.bag = new BagAmount() { bagType = BagType.SUPER_BAG_3, amount = 1, tour = 0 };
        //    }
        //    else if (i == 6 || i == 10)
        //    {
        //        s.reward = new BoosterCommodity(BoosterType.BAG, 1);
        //        s.bag = new BagAmount() { bagType = BagType.SUPER_BAG_2, amount = 1, tour = 0 };
        //    }
        //    else if (i == 8 || i == 14 || i == 18)
        //    {
        //        s.reward = new BoosterCommodity(BoosterType.CASH, 100);
        //    }

        //    pro.steps.Add(s);
        //}

        //this.battlepasss.Add(pro);
        #endregion

        //for (int i = 0; i < this.configs.Count; i++)
        //{
        //    this.configs[i].stepAddPass = 40;
        //}
    }
#endif
}

#region Mission 
public enum MissionType
{
    NONE = 0,

    [Type(typeof(MissionPurchase))]
    BUY_IAP,

    [Type(typeof(MissionPlayGame))]
    PLAY_GAME,
    [Type(typeof(MissionWinGame))]
    WIN_GAME,

    [Type(typeof(MissionEarnCoin))]
    EARN_COIN,

    [Type(typeof(MissionPlayGame))]
    LOGIN,

    [Type(typeof(MissionTakeFreeCard))]
    BUY_CARD_SHOP,

    [Type(typeof(MissionEarnCoin))]
    SPEND_COIN,

    [Type(typeof(MissionOpenBox))]
    OPEN_NORMAL_BOX,

    [Type(typeof(MissionCollectCard))]
    COLLECT_CARD,

    [Type(typeof(MissionUpgradeCard))]
    UPGRADE_CARD,

    [Type(typeof(MissionPlayGame))]
    POINT_BAG,

    [Type(typeof(MissionMiniGame))]
    PLAY_BULLEYE,

    [Type(typeof(MissionMiniGame))]
    PLAY_WHEEL,
    //[Type(typeof(MissionClaimDaily))]
    //CLAIM_DAILY,
}

[System.Serializable]
public class MissionConfig
{
    public int id;
    /// <summary>
    /// For assigning Goal script
    /// </summary>
    public MissionType type;

    public Sprite sprIcon;
    public string name;
    public string description;

    /// <summary>
    /// Start Tour 
    /// Default is 0
    /// </summary>
    public int startLevel;

    /// <summary>
    /// Default is 10000
    /// </summary>
    public int endLevel;

    /// <summary>
    /// Mission Require
    /// </summary>
    public long step;

    /// <summary>
    /// Prize
    /// </summary>
    public BoosterCommodity baseReward;

    /// <summary>
    /// Step add to battle pass if complete this mission
    /// </summary>
    public long stepAddPass;

    /// <summary>
    /// Step add by tour up
    /// </summary>
    public float userLevelProgress = -1;

    /// <summary>
    /// Min step
    /// </summary>
    public float minClamp_Step;

    /// <summary>
    /// Max Step
    /// </summary>
    public float maxClamp_Step;

    public MissionConfig()
    {
        this.id = -1;
        this.type = MissionType.NONE;
        this.sprIcon = null;
        this.name = "";
        this.description = "";

        this.startLevel = 0;
        this.endLevel = 1000;

        this.step = 0;
        this.baseReward = new BoosterCommodity();

        this.userLevelProgress = 0;
        this.minClamp_Step = 0;
        this.maxClamp_Step = 0;
    }
    public MissionConfig(MissionConfig c)
    {
        this.id = c.id;
        this.type = c.type;
        this.sprIcon = c.sprIcon;
        this.name = c.name;
        this.description = c.description;

        this.startLevel = c.startLevel;
        this.endLevel = c.endLevel;

        this.step = c.step;
        this.baseReward = new BoosterCommodity(c.baseReward.type, c.baseReward.GetValue());

        this.userLevelProgress = c.userLevelProgress;
        this.minClamp_Step = c.minClamp_Step;
        this.maxClamp_Step = c.maxClamp_Step;
    }

    public long GetStep(int tour)
    {
        return RedefindingStep(tour);
    }
    public BoosterCommodity GetReward(int tour)
    {
        return RedefindingReward(tour);
    }

    private long RedefindingStep(int tour)
    {
        RoomDatas RoomDatas = RoomDatas.Instance;
        int MaxTourUnlocked = RoomDatas.GetRoomUnlockedMax();

        //TODO: validate
        switch (this.id)
        {
            case MissionID.EARN_COIN:
                //Bằng chơi 4 ván max tour
                return RoomConfigs.Instance.GetRoom(MaxTourUnlocked).prizePerWave.GetValue() * 20;
            case MissionID.USE_COIN:
                //Bằng chơi 4 ván max tour
                return RoomConfigs.Instance.GetRoom(MaxTourUnlocked).prizePerWave.GetValue() * 20;
            case MissionID.COLLECT_CARD:
                //bằng 4 bag silver
                GiftBagPerTourConfig bag = GiftBagConfigs.Instance.GetCurrentTourGiftBag(BagType.SILVER_BAG);
                if(bag != null && bag.cardAmounts.Count > 0 )
                    return bag.cardAmounts[0].amount * 5;
                return 30;
            default:
                return this.step;
        }
    }

    private BoosterCommodity RedefindingReward(int tour)
    {
        //TODO
        RoomDatas RoomDatas = RoomDatas.Instance;
        int MaxTourUnlocked = RoomDatas.GetRoomUnlockedMax();

        //TODO: validate
        switch (this.id)
        {
            case MissionID.POINT_BAGS:
                return this.baseReward;
            default:
                //bằng reward coin của freebag
                GiftBagPerTourConfig bag = GiftBagConfigs.Instance.GetCurrentTourGiftBag(BagType.FREE_BAG);
                if (bag != null && bag.valueCoin != null)
                    return new BoosterCommodity(this.baseReward.type, bag.valueCoin.GetRandomValue());

                return this.baseReward;
        }
    }

    public virtual bool IsComplete()
    {
        return MissionDatas.Instance.IsComplete(this.id);
    }
}

public class MissionID
{
    #region Core Mission Pack
    public const int PLAY_GAME = 1001;
    public const int WIN_GAME = 1002;
    public const int UPGRADE_CARD = 1003;
    public const int PURCHASE = 1004;
    #endregion

    #region Beginer Mission Pack
    public const int COLLECT_CARD = 2001;
    #endregion

    #region FarmCoinPack
    public const int EARN_COIN = 3001;
    public const int USE_COIN = 3002; //use by playing game, buying avatar,...
    #endregion

    #region Game pack
    public const int CLAIM_DAILY_REWARD = 4001;
    public const int BUY_CARD_SHOP = 4002;

    #endregion

    #region OpenBox 
    public const int OPEN_BAGS = 5001;

    #endregion

    public const int POINT_BAGS = 6001;

    public const int PLAY_BULLEYE = 7001;
    public const int PLAY_WHEEL = 7002;

}
#endregion
