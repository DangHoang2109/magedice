using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
[CreateAssetMenu(menuName = "Configs/RoomConfigs", fileName = "RoomConfigs")]
public class RoomConfigs : ScriptableObject
{
    public static RoomConfigs Instance
    {
        get
        {
            return LoaderUtility.Instance.GetAsset<RoomConfigs>("Home/Configs/RoomConfigs");
        }
    }

     

    public List<RoomConfig> rooms;
    //public string inputSplitBag;

#if UNITY_EDITOR
    [ContextMenu("Convert Data From String")]
    private void ConvertData()
    {
        //inputSplitBag = inputSplitBag.Replace(" ", "");
        //string[] split = inputSplitBag.Split('-');

        foreach (RoomConfig r in this.rooms)
        {
            r.bagReward.sequenceBagReward = new List<BagType>() { BagType.SILVER_BAG, BagType.SILVER_BAG, BagType.GOLD_BAG };

            //for (int i = 0; i < split.Length; i++)
            //{
            //    switch (split[i])
            //    {
            //        case "B":
            //            r.bagReward.sequenceBagReward.Add(BagType.SILVER_BAG);
            //            break;
            //        case "R":
            //            r.bagReward.sequenceBagReward.Add(BagType.GOLD_BAG);
            //            break;
            //        case "E":
            //            r.bagReward.sequenceBagReward.Add(BagType.PLATINUM_BAG);
            //            break;
            //        case "K":
            //            r.bagReward.sequenceBagReward.Add(BagType.KING_BAG);
            //            break;
            //    }


            //}
        }
    }

    [ContextMenu("ReConfigRoom")]
    private void ReConfigRoom()
    {
        for (int i = 2; i < this.rooms.Count; i++)
        {
            RoomConfig r = rooms[i];
            r.prizePerWave = new BoosterCommodity(key: BoosterType.COIN, value: 50 + (i-2) * 20);
            r.multiplierBossWave = 5;
        }
    }

#endif
    public List<RoomConfig> GetRooms()
    {
        return this.rooms;
    }

    public RoomConfig GetRoom(int id)
    {
        return this.rooms.Find(x => x.id == id);
    }
}

[System.Serializable]
public class RoomConfig
{
    public string name;
    public int id;
    //public BoosterCommodity unlock;
    //public BoosterCommodity fee;
    public BoosterCommodity prizePerWave;
    public float multiplierBossWave; //ex 5: 0 - 5- 10 - 15

    [Header("Bag rewards")]
    public RoomBagRewardConfig bagReward;

    //[Header("Point increate/recreate when win/lose")]
    //public int pointWin;
    //public int pointLose;
    //public int pointMax;


    //[Header("Match Making Support")]

    //[Tooltip("Tổng chênh lệch stat")]
    //public int statDifferent;
    //[Tooltip("Tổng chênh lệch trophy")]
    //public int trophyDifferent;
    //[Tooltip("Tổng stat thấp nhất của bot room này")]
    //public int minTotalStat;
    //[Tooltip("Bot win difficulty")]
    //public float rateBotWin;
    //[Tooltip("Mỗi stat Main thấp hơn sẽ làm giảm winrate xuống x%")]
    //public float downRateAffectByStat;

    //[Tooltip("Số lượt điểm lưu lại trong set")]
    //public int PointHistoryCapacity;

    //public BagAmount GetBagReward()
    //{
    //    BagAmount bag = new BagAmount();
    //    bag.bagType = bagReward.GetBagReward();
    //    bag.tour = this.id;
    //    bag.amount = 1;
    //    return bag;
    //}

    public BagAmount GetBagReward(int index)
    {
        BagAmount bag = new BagAmount();
        bag.bagType = bagReward.GetBagReward(index);
        bag.tour = this.id;
        bag.amount = 1;
        return bag;
    }

    //public List<BagType> GetBagsCanReward()
    //{
    //    return bagReward.GetAllBagRewardTypes();
    //}

    //public int GetTrophyDifferent()
    //{
    //    return this.trophyDifferent;
    //}


    //public int GetPointHistoryCapacity()
    //{
    //    return this.PointHistoryCapacity;
    //}
}
[System.Serializable]
public class RoomDifficulty
{

}


/// <summary>
/// List bag có thể nhận và tỷ lệ random ra bag
/// </summary>
[System.Serializable]
public class RoomBagRewardConfig
{
    //[Header("Tỷ lệ ratio 0 ~ 1")]
    //public List<RoomBagRewardRatio> roomRewards;

    public List<BagType> sequenceBagReward;

    public RoomBagRewardConfig(RoomBagRewardConfig bagReward)
    {
        if (bagReward != null)
        {
            //this.roomRewards = new List<RoomBagRewardRatio>(bagReward.roomRewards);
            this.sequenceBagReward = new List<BagType>(bagReward.sequenceBagReward);
        }     
    }

    //public BagType GetBagReward()
    //{
        //float rand = Random.value;
        //float ratio = 0f;
        //foreach (RoomBagRewardRatio bagReward in roomRewards)
        //{
        //    ratio += bagReward.ratio;
        //    if (rand < ratio)
        //    {
        //        return bagReward.bag;
        //    }
        //}

        //return BagType.FREE_BAG;
    //}

    public BagType GetBagReward(int index)
    {
        if (sequenceBagReward == null || sequenceBagReward.Count == 0)
            return BagType.SILVER_BAG;
        else
        {
            return this.sequenceBagReward[index];
        }
    }

    //public List<BagType> GetAllBagRewardTypes()
    //{
    //    return sequenceBagReward.Distinct().ToList();



    //    List<BagType> bagTypes = new List<BagType>();
    //    foreach (RoomBagRewardRatio bagRewardRatio in roomRewards)
    //    {
    //        bagTypes.Add(bagRewardRatio.bag);
    //    }
    //    return bagTypes;
    //}
}

[System.Serializable]
public class RoomBagRewardRatio
{
    public BagType bag;
    public float ratio;
}
