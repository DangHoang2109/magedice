using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class RewardConfig 
{
    public BoosterCommodity booster;
    public BagAmount bag;

    public float boostRate;

    public bool IsRewardBag => this.booster.type == BoosterType.BAG && this.bag != null;

    public RewardConfig()
    {

    }
    public RewardConfig(RewardConfig c)
    {
        this.bag = new BagAmount(c.bag);
        this.booster = new BoosterCommodity(c.booster.type, c.booster.GetValue());

        this.boostRate = c.boostRate;
    }

    public BoosterCommodity GetBoosterPrize(int tour = -1)
    {
        //coin
        if (booster != null)
        {
            tour = tour > 0 ? tour : RoomDatas.Instance.GetRoomUnlockedMax();

            RoomConfig room = RoomConfigs.Instance.GetRoom(tour);
            if (room == null)
                room = RoomConfigs.Instance.GetRoom(RoomDatas.Instance.GetRoomUnlockedMax());

            GiftBagPerTourConfig bagConfig = GiftBagConfigs.Instance.GetGiftBag(BagType.GOLD_BAG, tour);

            switch (booster.type)
            {
                case BoosterType.COIN:
                    return new BoosterCommodity(key: booster.type,
                        value: (long)(boostRate * 5 * room.prizePerWave.GetValue())); //(long)(boostRate * (room.rateBotWin / 100) * room.fee.GetValue()));
                case BoosterType.CASH:
                    return new BoosterCommodity(BoosterType.CASH,
                        value: this.booster.GetValue() * (1 + (tour - 1) / 2)); //cứ 2 tour tăng lên 1 lần nhân: t1-2: x1, t3-4: x2;
                default:
                    Debug.LogError($"YOU HAVE NOT CONFIG THIS TYPE {booster.type}");
                    return new BoosterCommodity(key: BoosterType.COIN,
                        value: (long)(boostRate * 0.5 * room.prizePerWave.GetValue()));
                    //single card if you like it
            }
        }
        return null;
    }

    public BagAmount GetBagPrize(int tour = -1)
    {
        tour = tour > 0 ? tour : RoomDatas.Instance.GetRoomUnlockedMax();

        if (bag.bagType != BagType.FREE_BAG)
            return new BagAmount()
            {
                bagType = bag.bagType,
                amount = (int)(bag.amount * boostRate),
                tour = tour
            };
        else
            return null;
    }
}
