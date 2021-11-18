using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Configs/BagConfigs", fileName = "BagConfigs")]
public class BagConfigs : ScriptableObject
{
    public static BagConfigs Instance
    {
        get
        {
            return LoaderUtility.Instance.GetAsset<BagConfigs>("Home/Configs/BagConfigs");
        }
    }

    public List<BagConfig> configs;

    public BagConfig GetBag(BagType type)
    {
        return this.configs.Find(x => x.type == type);
    }
}

#region Bags - Cards

[System.Serializable]
public class BagConfig
{
    public BagType type; //loại bag

    [Header("Number of cards")]
    [System.Obsolete("Move the amount of card to giftbag tour config")]
    public List<CardAmount> cardAmounts;

    [Header("If waitting for open bag")]
    public BoosterCommodity bstOpenNow;         // giá trị để bỏ qua đợi
    public double totalTimeWait;               // thời gian đợi để mở
    public double timeReduce = 3000;           // thời gian bỏ qua cho mỗi lần quảng cáo (15 phút)

    public CardAmount GetCardAmount(CardType cardType)
    {
        return this.cardAmounts.Find(x => x.cardType == cardType);
    }

    public int GetCountCard(CardType cardType)
    {
        CardAmount cardAmount = this.GetCardAmount(cardType);
        if (cardAmount != null)
            return cardAmount.amount;
        return 0;
    }
}

[System.Serializable]
public enum BagType
{
    //BAGs
    FREE_BAG = 0, 
    SILVER_BAG = 1, //blue bag
    GOLD_BAG = 2, //grand bag - red
    PLATINUM_BAG = 3, //elite bag - white
    PRISM_BAG = 4,

    SUPER_BAG_1 = 10,
    SUPER_BAG_2 = 11,
    SUPER_BAG_3 = 12,
    SUPER_BAG_4 = 13,

    BULLSEYE_BAG_1 = 20,
    BULLSEYE_BAG_2 = 21,
    BULLSEYE_BAG_3 = 22,
    BULLSEYE_BAG_4 = 23,
    BULLSEYE_BAG_5 = 24,

    KING_BAG = 50 //king bag black


}

[System.Serializable]
public class BagAmount
{
    public BagType bagType;
    public int tour;
    public int amount;  

    public BagAmount()
    {
        this.bagType = BagType.FREE_BAG;
        this.tour = 0;
        this.amount = 0;

    }
    public BagAmount(BagAmount c)
    {
        this.bagType = c.bagType;
        this.tour = c.tour;
        this.amount = c.amount;
    }

    public BagAmount(BagType bagType, int amount, int tour)
    {
        this.bagType = bagType;
        this.amount = amount;
        this.tour = tour;
    }
}


[System.Serializable]
public class CardAmount
{
    public StatManager.Tier tier;
    public CardType cardType; 
    public int amount;
}

public enum CardType
{
    //TODO cue, các loại booster

    NONE = 0,
    STANDARD,
    COUNTRY,
    RARE,
    EPIC,

    CHARACTER = 10,
    STRING = 11,

    [System.Obsolete("SMASHES is times to change to result")]
    SMASHES = 20 
}

#endregion