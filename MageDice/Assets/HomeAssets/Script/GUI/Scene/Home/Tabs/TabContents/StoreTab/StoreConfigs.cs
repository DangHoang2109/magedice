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

    [ContextMenu("CreateRemote")]
    private void CreateRemote()
    {
        for (int i = 1; i < this.listRemote.Count; i++)
        {
            StoreRemoteConfig c = listRemote[i];

            for (int j = 0; j < c.idPackgages.Count; j++)
            {
                c.idPackgages[j] = c.idPackgages[j] - c.userType * 10;
                if (this.packgages.Find(x => x.id == c.idPackgages[j]) == null)
                    Debug.LogError($"THERE IS NO ITEM ID {c.idPackgages[j]}");
            }

            for (int j = 0; j < c.idPackgages.Count; j++)
            {
                c.idFreeDeals[j] = c.idFreeDeals[j] - c.userType * 10;
                if (this.freeDeals.Find(x => x.id == c.idFreeDeals[j]) == null)
                    Debug.LogError($"THERE IS NO ITEM ID {c.idFreeDeals[j]}");
            }

            for (int j = 0; j < c.idPackgages.Count; j++)
            {
                c.idBags[j] = c.idBags[j] - c.userType * 10;
                if (this.bags.Find(x => x.id == c.idBags[j]) == null)
                    Debug.LogError($"THERE IS NO ITEM ID {c.idBags[j]}");
            }
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

    public static BoosterCommodity GetPriceCueCardNeed(StatData cue)
    {
        long need = cue.RequirementCard - cue.cards;

        if(need > 0)
        {
            return new BoosterCommodity(
                key: StoreConfigs.GetRarirty_PriceUpgradeType(cue.config.tier),
                value: StoreConfigs.GetRarity_PriceCardUpgrade(cue.config.tier) * need);
        }

        return null;
    }

    public static int GetRairityMaxStock(StatManager.Tier r)
    {
        return 4;
    }

    public static long GetRarity_NotOwnPrice(StatManager.Tier r)
    {
        switch (r)
        {
            case StatManager.Tier.Standard:
                return 4;
            case StatManager.Tier.Rare:
                return 45;
            case StatManager.Tier.Legendary:
                return 400;
            //case StatManager.Tier.CHARACTER:
            //    return 4000;
            default:
                return 100;
        }
    }
    public static long GetRarity_BasePrice(StatManager.Tier r)
    {
        switch (r)
        {
            case StatManager.Tier.Standard:
                return 2; //coin
            case StatManager.Tier.Rare:
                return 35; //coin
            case StatManager.Tier.Legendary:
                return 300;
            //case StatsItemRarity.CHARACTER:
            //    return 25;
            default:
                return 25;
        }
    }
    public static long GetRarity_StepPrice(StatManager.Tier r)
    {
        switch (r)
        {
            case StatManager.Tier.Standard:
                return 2;
            case StatManager.Tier.Rare:
                return 35;
            case StatManager.Tier.Legendary:
                return 300;
            //case StatsItemRarity.CHARACTER:
            //    return 25;
            default:
                return 25;
        }
    }

    public static BoosterType GetRarirty_PriceType(StatManager.Tier t)
    {
        switch (t)
        {
            case StatManager.Tier.Standard:
                return BoosterType.CASH;
            case StatManager.Tier.Rare:
                return BoosterType.CASH;
            case StatManager.Tier.Legendary:
                return BoosterType.CASH;
            //case StatsItemRarity.CHARACTER:
            //    return 25;
            default:
                return BoosterType.CASH;
        }
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
            //case StatsItemRarity.CHARACTER:
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
            //case StatsItemRarity.CHARACTER:
            //    return 25;
            default:
                return BoosterType.CASH;
        }
    }
#if UNITY_EDITOR
    [ContextMenu("UpdateProductCoin")]
    private void Editor_UpdateProductCoin()
    {
        foreach(StoreBoosterConfig c in this.coins)
        {
            foreach(BoosterCommodity b in c.boosters)
            {
                b.Set((long)(c.price.GetValue() * GameDefine.RATE_CASH_TO_COIN * (1 + c.Bonus)));
            }
        }
    }

    [ContextMenu("UpdateSpecialPackage")]
    private void Editor_UpdateSpecialPackage()
    {
        foreach (StoreSpecialPackageConfig c in this.specialPackgagesV2)
        {
            c.bagAmounts.Add(new BagAmount(c.bagAmounts[0]));
            c.bagAmounts.RemoveAt(0);
        }
    }
#endif
    [Header("Special Packages")]
    public List<StoreSpecialPackageConfig> specialPackgagesV2;

    public List<StoreSpecialPackageConfig> GetSpecialPackages()
    {
        return this.specialPackgagesV2;
    }

    /// <summary>
    /// Call this function to get a package.
    /// If receive null => dont show package to UI
    /// </summary>
    /// <returns></returns>
    public StoreSpecialPackageConfig GetRandomAvailableSpecialPackage()
    {
        //get mastery cue that user have bought;
        List<StatData> available = StatManager.Instance.QueryDatasByTierKind_Simple(StatManager.Tier.Legendary, StatManager.Kind.NotUnlocked)
                                                .Where(x => x.config.isHide).ToList();

        if (available.Count > 0)
        {
            StatData cue = available.GetRandom();
            StoreSpecialPackageConfig config = this.specialPackgagesV2.Find(x => x.cueid == cue.id);
            StoreSpecialPackageConfig p = new StoreSpecialPackageConfig(config);
            //check if user have bought any package?
            if (!UserBehaviorDatas.Instance.IsPurchased)
            {
                if(p != null)
                {
                    p.name = p.name.Replace("The Mastery Series", "First Buy Super Deal");
                    p.Title = "First Buy Super Deal";

                    return p;
                }
                else
                {
                    Debug.LogError($"Wait what this id is null {cue.id}");
                }
            }
            //else => check if user have reach D7 or D14
            else
            {
                if (UserBehaviorDatas.Instance.IsDaySevenOrSame())
                {
                    if (p != null)
                    {
                        p.Title = "Veteran Booster!";

                        return p;
                    }
                    else
                    {
                        Debug.LogError($"Wait what this id is null {cue.id}");
                    }
                }
            }
        }

        return null;
    }


    [Header("Packages")]
    public List<StorePackageConfig> packgages;
    private string GetRandomCueMasterySeriesID()
    {
        int cueID = Random.Range(8, 20); //cue l8 đến l19
        return $"l{cueID}";
    }
    public List<StorePackageConfig> GetPackages()
    {
        List<StorePackageConfig> res = this.packgages.FindAll(x => this.RemoteData.idPackgages.Contains(x.id));

        StorePackageConfig best = res.Find(x => x.layoutType == StorePackageLayoutType.Hor_Middle);
        if(best != null)
        {
            best.cueid = GetRandomCueMasterySeriesID();
        }

        return res;
    }

    [Header("Deals")]
    public List<StoreFreeDealConfig> freeDeals;
    public List<StoreFreeDealConfig> GetFreeDeals()
    {
        return this.freeDeals.FindAll(x => this.RemoteData.idFreeDeals.Contains(x.id));
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
        return this.bags.FindAll(x => this.RemoteData.idBags.Contains(x.id));
    }

    [Header("Cash")]
    public List<StoreCashConfig> cashs;
    public List<StoreCashConfig> GetCashs()
    {
        return this.cashs.FindAll(x => this.RemoteData.idCashs.Contains(x.id));
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
        return this.coins.FindAll(x => this.RemoteData.idCoins.Contains(x.id));
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
        res.AddRange(this.specialPackgagesV2);
        res.AddRange(this.packgages);
        res.AddRange(this.cashs);
        res.Add(wheelIAPConfig);
        res.Add(quickFireIAPConfig);

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
    public StoreBoosterBaseConfig quickFireCashConfig;
    public StoreBoosterBaseConfig GetQuickFireCashConfig()
    {
        return quickFireCashConfig;
    }

    public StoreIAPBaseConfig quickFireIAPConfig;
    public StoreIAPBaseConfig GetQuickFireIAPConfig()
    {
        return quickFireIAPConfig;
    }

    [Header("Wheel")]
    public StoreIAPBaseConfig wheelIAPConfig;
    public StoreIAPBaseConfig GetWheelIAPConfig()
    {
        return wheelIAPConfig;
    }

    #region Remote Config
    /// <summary>
    /// Download this config from firebase
    /// All config in shop with finding in this id list
    /// 
    /// Note fore Hoang below
    /// ID of each item will has format [Seg of Type][userType][itemIndex]
    /// 100 will 1 0 0 : Package Item - User never paid - package index 0
    /// </summary>
    public List<StoreRemoteConfig> listRemote;

    public StoreRemoteConfig RemoteData
    {
        get
        {
            if (RemoteConfigsManager.UserType >= listRemote.Count)
                return listRemote[0];

            return listRemote[RemoteConfigsManager.UserType];
        }
    }
    #endregion
}

[System.Serializable]
public class StoreRemoteConfig
{
    /// <summary>
    /// Label user from their audice in firebase
    /// currently we get:
    /// 0: user never paid IAP and rarely paid gem
    /// 1: user paid gem only or rarely use IAP
    /// 2: user paid a lot, both gem and IAP
    /// </summary>
    public int userType;

    public List<int> idPackgages;
    public List<int> idFreeDeals;
    public List<int> idBags;
    public List<int> idCashs;
    public List<int> idCoins;
}

[System.Serializable]
public class StorePackageConfig: StoreCashConfig
{
    public string name;
    public int xBonus; //x2, x3, x4
    public StorePackageLayoutType layoutType;

    public List<BagAmount> bagAmounts;
    public string cueid;

    public StorePackageConfig() : base()
    {

    }

    public StorePackageConfig(StorePackageConfig c) : base(c)
    {
        this.name = c.name;
        this.xBonus = c.xBonus;
        this.layoutType = c.layoutType;
        this.bagAmounts = new List<BagAmount>(c.bagAmounts);
        this.cueid = c.cueid;
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
        foreach(BoosterCommodity b in c.boosters)
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
public class StoreFreeDealConfig: StoreItemConfig
{
    public List<BoosterCommodity> boosters;
    public List<BagAmount> bagAmounts;
    public double timeWait;
}

/// <summary>
/// Store booster (mua boosters bằng boosters)
/// </summary>
[System.Serializable]
public class StoreBoosterConfig: StoreBoosterBaseConfig
{
    public List<BoosterCommodity> boosters; //những booster nhận được
    public List<BagAmount> bagAmounts; //bags
}

[System.Serializable]
public class StoreDealSlotConfig
{
    public int id;
    public int tourUnlock;

    public float rate_Owned; 
}


[System.Serializable]
public class StoreDealCardConfig : StoreItemConfig
{

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
