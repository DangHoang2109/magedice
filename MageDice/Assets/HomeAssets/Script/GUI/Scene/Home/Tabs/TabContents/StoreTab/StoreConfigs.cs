using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using UnityEngine.Events;

[CreateAssetMenu(menuName = "Configs/StoreConfigs")]
public class StoreConfigs : ScriptableObject
{
    public static StoreConfigs Instance
    {
        get
        {
            return LoaderUtility.Instance.GetAsset<StoreConfigs>("Home/Configs/StoreConfigs");
        }
    }

    public void BuyIAP(int id, string key_iap, string where, UnityAction<bool> callback)
    {
        IAPManager.Instance.BuyProduct(key_iap, where, (success) =>
        {
            if (success)
            {
                UserBehaviorDatas.Instance.Purchase(id);
            }
            callback?.Invoke(success);
        });
    }

    public static BoosterCommodity GetPriceCueCardNeed(StatData stat)
    {
        long need = stat.RequirementCard - stat.cards;

        if (need > 0)
        {
            return new BoosterCommodity(
                key: GetRarirty_PriceUpgradeType(stat.config.tier),
                value: GetRarity_PriceCardUpgrade(stat.config.tier) * need);
        }

        return null;
    }

    public static long GetRarity_PriceCardUpgrade(StatManager.Tier r)
    {
        switch (r)
        {
            case StatManager.Tier.Standard:
                return 1;
            case StatManager.Tier.Rare:
                return 5;
            case StatManager.Tier.Legendary:
                return 50;
            //case StatManager.Tier.CHARACTER:
            //    return 25;
            default:
                return 25;
        }
    }
    public static BoosterType GetRarirty_PriceUpgradeType(StatManager.Tier t)
    {
        switch (t)
        {
            case StatManager.Tier.Standard:
                return BoosterType.CASH;
            case StatManager.Tier.Rare:
                return BoosterType.CASH;
            case StatManager.Tier.Legendary:
                return BoosterType.CASH;
            //case StatManager.Tier.CHARACTER:
            //    return 25;
            default:
                return BoosterType.CASH;
        }
    }
    public static int GetRairityMaxStock(StatManager.Tier r)
    {
        switch (r)
        {
            case StatManager.Tier.Standard:
                return 100;
            case StatManager.Tier.Rare:
                return 50;
            case StatManager.Tier.Legendary:
                return 20;
            default:
                return 1;
        }
    }

    public static long GetRarity_NotOwnPrice(StatManager.Tier r)
    {
        switch (r)
        {
            case StatManager.Tier.Standard:
                return 200;
            case StatManager.Tier.Rare:
                return 500;
            case StatManager.Tier.Legendary:
                return 1000;
            default:
                return 4000;
        }
    }
    public static long GetRarity_BasePrice(StatManager.Tier r)
    {
        switch (r)
        {
            case StatManager.Tier.Standard:
                return 2;
            case StatManager.Tier.Rare:
                return 20;
            case StatManager.Tier.Legendary:
                return 200;
            default:
                return 500;
        }
    }
    public static long GetRarity_StepPrice(StatManager.Tier r)
    {
        switch (r)
        {
            case StatManager.Tier.Standard:
                return 2;
            case StatManager.Tier.Rare:
                return 20;
            case StatManager.Tier.Legendary:
                return 200;
            default:
                return 500;
        }
    }

#if UNITY_EDITOR
    [ContextMenu("Add More Bag")]
    private void Editor_AddCoinBag()
    {
        //foreach (StoreBoosterConfig c in this.bags)
        //    c.isShowInStore = true;

        StoreBoosterConfig kingBag = new StoreBoosterConfig()
        {
            id = 413,
            bagAmounts = new List<BagAmount>() { new BagAmount(BagType.KING_BAG, 1, 1) },
            price = new BoosterCommodity(BoosterType.CASH, 1500),
            isShowInStore = false
        };
        this.bags.Insert(3, kingBag);

        StoreBoosterConfig s1Bag = new StoreBoosterConfig()
        {
            id = 400,
            bagAmounts = new List<BagAmount>() { new BagAmount(BagType.SUPER_BAG_1, 1, 1) },
            price = new BoosterCommodity(BoosterType.CASH, 100),
            isShowInStore = false
        };
        this.bags.Insert(4, s1Bag);
    }
#endif


    [Header("Packages")]
    public List<StorePackageConfig> packgages;
    public List<StorePackageConfig> GetPackages()
    {
        return this.packgages;
    }

    [Header("Deals")]
    public List<StoreFreeDealConfig> freeDeals;
    public List<StoreFreeDealConfig> GetFreeDeals()
    {
        return this.freeDeals;
    }

    public List<StoreDealSlotConfig> dealSlots;
    public List<StoreDealSlotConfig> GetDealSlots()
    {
        return this.dealSlots;
    }

    public StoreDealSlotConfig GetDealSlot_ByIndex(int index)
    {
        return this.GetDealSlots()[index];
    }
    public StoreDealSlotConfig GetDealSlot_ByID(int id)
    {
        return this.GetDealSlots().Find(x => x.id == id);
    }

    [Header("Bags")]
    public List<StoreBoosterConfig> bags;
    public List<StoreBoosterConfig> GetBags()
    {
        return this.bags;
    }
    public List<StoreBoosterConfig> GetBagsShowInStore()
    {
        return this.bags.Where(x => x.isShowInStore).ToList();
    }
    public StoreBoosterConfig GetBag(BagType type)
    {
        return this.bags.Find(x => x.bagAmounts.Find(x => x.bagType == type) != null);
    }
    [Header("Cash")]
    public List<StoreCashConfig> cashs;
    public List<StoreCashConfig> GetCashs()
    {
        return this.cashs;
    }

    public List<StoreCashConfig> GetBetterPlaceCashs()
    {
        //3 id better place được đưa vào config cho từng user
        return new List<StoreCashConfig>(this.GetCashs());
    }
    public List<StoreBoosterConfig> GetBetterPlaceCoins()
    {
        //3 id better place được đưa vào config cho từng user
        return new List<StoreBoosterConfig>(this.GetCoins());
    }
    /// <summary>
    /// Trả về [options] item có value khớp với [need] nhất
    /// Có thể Sort theo item.GetBonus để sắp xếp thứ tự muốn
    /// </summary>
    /// <param name="need"></param>
    /// <param name="OnlyGetUpon">False if You need one below,one match and one upper</param>
    /// By default is true, I will return options item with value higher need
    /// <param name="options"></param>
    /// <returns></returns>
    public List<StoreCashConfig> GetCashOption_ByNeed(long need, int options = 3, bool OnlyGetUpon = true)
    {
        return this.cashs
            .Where(x => x.boosters[0].GetValue() >= need)
            .OrderBy(x => x.boosters[0].GetValue() - need)
            .ToList();
    }

    [Header("Coins")]
    public List<StoreBoosterConfig> coins;
    public List<StoreBoosterConfig> GetCoins()
    {
        return this.coins;
    }

    /// <summary>
    /// Trả về [options] item có value khớp với [need] nhất
    /// Có thể Sort theo item.GetBonus để sắp xếp thứ tự muốn
    /// </summary>
    /// <param name="need"></param>
    /// <param name="OnlyGetUpon">False if You need one below,one match and one upper</param>
    /// By default is true, I will return options item with value higher need
    /// <param name="options"></param>
    /// <returns></returns>
    public List<StoreBoosterConfig> GetCoinOption_ByNeed(long need, int options = 3, bool OnlyGetUpon = true)
    {
        return this.coins
            .Where(x => x.boosters[0].GetValue() >= need)
            .OrderBy(x => x.boosters[0].GetValue() - need)
            .ToList();
    }

    public List<StoreIAPBaseConfig> GetAllIAPConfig()
    {
        List<StoreIAPBaseConfig> res = new List<StoreIAPBaseConfig>();
        res.AddRange(this.packgages);
        res.AddRange(this.cashs);
        res.Add(bulleyeIAPConfig);
        res.Add(wheelIAPConfig);

        return res;
    }

    public StoreIAPBaseConfig GetIAPConfig(string key_iap)
    {
        return GetAllIAPConfig().Find(x => x.key_iap.Equals(key_iap));
    }
    public StoreIAPBaseConfig GetIAPConfig(int id)
    {
        return GetAllIAPConfig().Find(x => x.id == id);
    }

    [Header("Bulleye")]
    public StoreBoosterBaseConfig bulleyeCashConfig;
    public StoreBoosterBaseConfig GetBulleyeCashConfig()
    {
        return bulleyeCashConfig;
    }

    public StoreIAPBaseConfig bulleyeIAPConfig;
    public StoreIAPBaseConfig GetBulleyeIAPConfig()
    {
        return bulleyeIAPConfig;
    }

    [Header("Wheel")]
    public StoreIAPBaseConfig wheelIAPConfig;
    public StoreIAPBaseConfig GetWheelIAPConfig()
    {
        return wheelIAPConfig;
    }

}


[System.Serializable]
public class StorePackageConfig : StoreCashConfig
{
    public string name;
    public int xBonus; //x2, x3, x4
    public StorePackageLayoutType layoutType;

    public List<BagAmount> bagAmounts;
    public DiceID diceID;

    public StorePackageConfig() : base()
    {

    }

    public StorePackageConfig(StorePackageConfig c) : base(c)
    {
        this.name = c.name;
        this.xBonus = c.xBonus;
        this.layoutType = c.layoutType;
        this.bagAmounts = new List<BagAmount>(c.bagAmounts);
        this.diceID = c.diceID;
    }
}

[System.Serializable]
public class StoreSpecialPackageConfig : StorePackageConfig
{
    public const double DEFAULT_TIME = 259200; //3d //86400;
    public enum SpecialPackageType
    {
        FIRSTBUY,
        DAILY,
        VETARAN,
    }
    public double time;
    public SpecialPackageType type;
    public string Title = "First Buy Super Deal!";
    public StoreSpecialPackageConfig(StorePackageConfig c) : base(c)
    {
        this.type = SpecialPackageType.DAILY;
        this.time = DEFAULT_TIME;
        this.boosters = new List<BoosterCommodity>();
        foreach (BoosterCommodity b in c.boosters)
        {
            this.boosters.Add(new BoosterCommodity(b.type, b.GetValue()));
        }
    }
}
/// <summary>
/// Store cash (mua cashs bằng IAP)
/// </summary>
[System.Serializable]
public class StoreCashConfig : StoreIAPBaseConfig
{
    public List<BoosterCommodity> boosters;


    public StoreCashConfig()
    {
        this.boosters = new List<BoosterCommodity>();
    }
    public StoreCashConfig(StoreCashConfig c) : base()
    {
        this.id = c.id;
        this.key_iap = c.key_iap;
        this.money = c.money;

        this.Bonus = c.Bonus;
        this.boosters = new List<BoosterCommodity>(c.boosters);
    }
}

/// <summary>
/// Store deal free (lần đầu tiên free, mấy lần sau watch)
/// </summary>
[System.Serializable]
public class StoreFreeDealConfig : StoreItemConfig
{
    public List<BoosterCommodity> boosters;
    public List<BagAmount> bagAmounts;
    public double timeWait;
}

/// <summary>
/// Store booster (mua boosters bằng boosters)
/// </summary>
[System.Serializable]
public class StoreBoosterConfig : StoreBoosterBaseConfig
{
    public List<BoosterCommodity> boosters; //những booster nhận được
    public List<BagAmount> bagAmounts; //bags

    public bool isShowInStore;
}

[System.Serializable]
public class StoreDealSlotConfig
{
    public int id;
    public int tourUnlock;

    public float rate_Owned;
    public float rate_Equiped; //nếu ra owned thì bao nhiêu % ra item user đang dùng

    public List<StoreDealCardConfig> dealCards;

    public StoreDealCardConfig RandomDealCard()
    {
        return this.dealCards[Random.Range(0, this.dealCards.Count)];
    }

    public StoreDealCardConfig GetDealCard(int id)
    {
        return this.dealCards.Find(x => x.id == id);
    }
}


[System.Serializable]
public class StoreDealCardConfig : StoreItemConfig
{
    [Header("Card")]
    public StatManager.Tier rarity;

    [Header("Tỷ lệ lấy card đã sở hữu")]
    public float rateOwn = 0.8f;

    [Header("Randome số lượng card")]
    public ValueRandom countCard; //randome count card

    [Header("Tiền tăng lên sau mỗi lần buy")]
    public List<BoosterCommodity> prices;
}

#region Base config
[System.Serializable]
public class StoreItemConfig
{
    public int id;


}

[System.Serializable] //shop mua bằng IAP
public class StoreIAPBaseConfig : StoreItemConfig
{
    public string key_iap;
    public float money;
    public float Bonus;

    public float GetBonus()
    {
        return Bonus + 1f;
    }
}

[System.Serializable] //shop mua booster
public class StoreBoosterBaseConfig : StoreItemConfig
{
    public BoosterCommodity price;

    public float Bonus;

    public float GetBonus()
    {
        return Bonus + 1f;
    }
}

#endregion


public enum StoreTabName
{
    PIGGY_BANK = 0,
    SPECIAL_OFFER = 1,
    PRO_PASS = 2,
    DAILY_DEALS = 3,
    BAGS = 4,
    STRINGS = 5,
    OUTFITS = 6,
    CASHS = 7,
    COINS = 8
}
