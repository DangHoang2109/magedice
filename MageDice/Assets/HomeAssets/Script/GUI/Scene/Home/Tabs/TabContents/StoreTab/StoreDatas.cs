using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using static StatManager;

[System.Serializable]
public class StoreDatas 
{
    public static StoreDatas Instance
    {
        get
        {
            return GameDataManager.Instance.GameDatas.storeDatas;
        }
    }

    public StoreSpecialData storeSpecial;
    public StorePackagesData storePackages;
    public StoreDealsData storeDeals;
    public StoreFreeCoinData freeCoinData;

    public StoreDatas()
    {
        this.storeSpecial = new StoreSpecialData();
        this.storePackages = new StorePackagesData();
        this.storeDeals = new StoreDealsData();
        this.freeCoinData = new StoreFreeCoinData();
    }

    public void OpenGame()
    {
        this.storeSpecial.OpenGame();
    }

    public void CreateUser()
    {
        this.storePackages.CreatePackage();
        this.storeDeals.CreateDeals();
        this.freeCoinData.CreateFree();
    }

    private void SaveData()
    {
        GameDataManager.Instance.SaveUserData();
    }
}


[System.Serializable]
public class StoreSpecialData
{
    public double TOTAL_TIME_WAIT = 300;  //Total time for watting
    public long timeStart;

    public StoreSpecialPackageConfig special;
    public bool isAvailable; //đang có hiệu lực không
    //public bool isBought; //đã mua chưa
    public static StoreSpecialData Instance
    {
        get
        {
            return StoreDatas.Instance.storeSpecial;
        }
    }

    public void OpenGame()
    {
        ///Nếu đã từng tạo gói
        /// => Check gói hết hạn ? => Tạo mới
        /// Nếu khớp ngày thì gói mới khác null và isAvailable = true
        /// Không khớp ngày thì gói mới = null và isAvailable = false
         if(this.special != null)
        {
            double c = 0;
            if(!IsStillTimeOfSpeacial(ref c))
            {
                CreateSpecial();
            }
        }
        ///User mới
        ///Lần trước mở lên ko khớp ngày
        else
        {
            CreateSpecial();
        }

        isTimeLoadHomeScene = 0;
    }

    public void CreateSpecial()
    {
        Debug.Log("edit");
        //this.special = StoreConfigs.Instance.GetRandomAvailableSpecialPackage();
        //this.isAvailable = this.special != null;
        ////this.isBought = false;
        //this.timeStart = DateTime.Now.ToFileTime();
        //this.TOTAL_TIME_WAIT = isAvailable ? this.special.time : 0;
        //SaveData();
    }

    public StoreSpecialPackageConfig GetSpecial()
    {
        return this.special;
    }

    public bool IsStillTimeOfSpeacial(ref double totalRemain)
    {
        if (this.special == null)
            return false;

        double timePassed = DateTime.Now.Subtract(DateTime.FromFileTime(this.timeStart)).TotalSeconds;

        totalRemain = TOTAL_TIME_WAIT - timePassed;

        if (totalRemain <= 0)
        {
            EndOffer();
        }
        return totalRemain > 0; //this.isAvailable;
    }

    public bool IsAvailable()
    {
        return this.isAvailable && this.special!=null;
    }

    /// <summary>
    /// Kết thúc offer
    /// </summary>
    public void EndOffer()
    {
        this.isAvailable = false;
        SaveData();
    }

    /// <summary>
    /// Mua thành công
    /// </summary>
    public void BuySuccess()
    {
        this.isAvailable = false;
        SaveData();
    }

    /// <summary>
    /// Có show offer ở ngoài home không
    /// </summary>
    /// <returns></returns>
    public int isTimeLoadHomeScene = 0;
    public const int DEFINE_TIME_LOADHOME_SHOW_SPECIAL = 3;
    public bool IsShowSpOfferAtHome()
    {
        return false;
        if (this.special != null)
        {
            //check isTimeLoadHomeScene cứ 3 lần load homescene thì show 1 lần nếu có data
            if(isTimeLoadHomeScene++ % DEFINE_TIME_LOADHOME_SHOW_SPECIAL == 0)
            {
                return this.isAvailable;
            }
        }
        return false;
    }

    private void SaveData()
    {
        GameDataManager.Instance.SaveUserData();
    }
}

[System.Serializable]
public class StorePackagesData  //package cứ hết 1 khoảng thời gian sẽ reset 1 lần (3 gói)
{
    public static StorePackagesData Instance
    {
        get
        {
            return StoreDatas.Instance.storePackages;
        }
    }

    public List<StorePackageConfig> packages;

    public double TOTAL_TIME_WAIT = 300;  //Total time for watting
    public long timeStart;
    public void CreatePackage()
    {
        Debug.Log("edit");
        //this.packages = StoreConfigs.Instance.GetPackages();
        //this.timeStart = DateTime.Now.ToFileTime();
        //SaveData();
    }
    public void ResetPackage()
    {
        //TODO random package
        this.timeStart = DateTime.Now.ToFileTime();
        //TEMP
        CreatePackage();
        SaveData();
    }

    public List<StorePackageConfig> GetPackages()
    {
        return this.packages;
    }

    //check package vẫn có hiệu lực
    public bool IsStillTimeOfPackage(ref double totalRemain)
    {
        totalRemain = 0;
        DateTime oldDate = DateTime.FromFileTime(this.timeStart);
        DateTime newDate = DateTime.Now;
        TimeSpan difference = newDate.Subtract(oldDate);
        totalRemain = difference.TotalSeconds;
        if (totalRemain >= TOTAL_TIME_WAIT)
        {
            ResetPackage();
            return false;
        }
        return true;
    }

    public void ForceResetDeal()
    {
        this.ResetPackage();
        //(HomeTabs.Instance.GetTabContent<StoreTabContent>(HomeTabName.STORE).dicChildTabs[StoreTabName.SPECIAL_OFFER] as StorePackageTab).ReloadData();
    }

    private void SaveData()
    {
        GameDataManager.Instance.SaveUserData();
    }
}


/// <summary>
/// Daily deals sẽ được reset sau 12pm
/// </summary>
[System.Serializable]
public class StoreDealsData  //giảm giá, miễn phí
{
    public static StoreDealsData Instance
    {
        get
        {
            return StoreDatas.Instance.storeDeals;
        }
    }

    public StoreDealFreeData dealFreeData;
    public StoreDealCardsData dealCardsData;

    //time, reset per daily
    public long timeStart;

    public StoreDealsData()
    {
        this.dealFreeData = new StoreDealFreeData();
        this.dealCardsData = new StoreDealCardsData();
    }

    public void CreateDeals()
    {
        this.dealFreeData.CreateDeals();
        this.dealCardsData.CreateDeals();
        this.timeStart = DateTime.Now.ToFileTime();
    }

    public void ResetDeals()
    {
        this.dealFreeData.ResetDeals();
        this.dealCardsData.ResetDeals();
        this.timeStart = DateTime.Now.ToFileTime();
    }

    /// <summary>
    /// kiểm tra qua ngày mới chưa để reset deal
    /// </summary>
    /// <param name="totalRemain"></param>
    /// <returns></returns>
    public bool IsResetDeals(ref double totalRemain)
    {
        totalRemain = DateTime.Now.TimeOfDay.TotalSeconds; //thời gian của ngày đó đã trôi qua

        //kiểm tra đã qua ngày mới không
        DateTime dateStart = DateTime.FromFileTime(this.timeStart);
        DateTime dateNow = DateTime.Now.Date;

        if (dateStart.Date != dateNow.Date) 
        {
            this.ResetDeals();
            return true;
        }
        return false;
    }

    /// <summary>
    /// TEST
    /// Bắt buộc rest deal mới
    /// </summary>
    /// <param name="totalRemain"></param>
    /// <returns></returns>
    public void ForceResetDeal()
    {
        this.ResetDeals();
        (HomeTabs.Instance.GetTabContent<StoreTabContent>(HomeTabName.STORE).dicChildTabs[StoreTabName.DAILY_DEALS] as StoreDealTab).ReloadData();

        SaveData();
    }

    private void SaveData()
    {
        GameDataManager.Instance.SaveUserData();
    }
}

[System.Serializable]
public class StoreDealFreeData
{
    public static StoreDealFreeData Instance
    {
        get
        {
            return StoreDealsData.Instance.dealFreeData;
        }
    }


    public List<StoreFreeDealConfig> freeDeals;
    public int indexFree;
    public FreeDealStatus freeStatus;

    public double TOTAL_TIME_WAIT;
    public long timeStart;

    public void CreateDeals()
    {
        Debug.Log("edit");

        //this.freeDeals = StoreConfigs.Instance.GetFreeDeals();
        //this.freeStatus = FreeDealStatus.FREE;
        //this.indexFree = 0;
        //LoadTotalTimeWait();
        //SaveData();
    }

    public void ResetDeals()
    {
        //TODO Randome deals
        CreateDeals();
        SaveData();
    }

    private void LoadTotalTimeWait()
    {
        StoreFreeDealConfig curConfig = GetCurrentFreeDealConfig();
        if (curConfig != null)
        {
            this.TOTAL_TIME_WAIT = curConfig.timeWait;
        }
    }

    /// <summary>
    /// trả về trạng thái của free watch
    /// </summary>
    /// <param name="timeRemain"></param>
    /// <returns></returns>
    public FreeDealStatus IsFreeDeal(ref double timeRemain)
    {
        if (this.freeStatus == FreeDealStatus.WAITTING)
        {
            DateTime oldDate = DateTime.FromFileTime(this.timeStart);
            DateTime newDate = DateTime.Now;
            TimeSpan difference = newDate.Subtract(oldDate);
            timeRemain = difference.TotalSeconds;
            if (timeRemain >= TOTAL_TIME_WAIT)
            {
                this.freeStatus = FreeDealStatus.WATCH;
                SaveData();
            }
        }
        
        return this.freeStatus;
    }

    public void CollectFreeDeal()
    {
        this.indexFree += 1;
        this.timeStart = DateTime.Now.ToFileTime();
        if (this.freeDeals != null)
        {
            this.freeStatus = (this.indexFree >= this.freeDeals.Count)? FreeDealStatus.COMPLETED : FreeDealStatus.WAITTING;
            LoadTotalTimeWait();
        }
        SaveData();
    }

    private void SaveData()
    {
        GameDataManager.Instance.SaveUserData();
    }

    public StoreFreeDealConfig GetCurrentFreeDealConfig()
    {
        if (this.freeDeals != null)
        {
            if (this.indexFree < this.freeDeals.Count)
                return this.freeDeals[this.indexFree];
        }
        return null;
    }

    public enum FreeDealStatus
    {
        NONE = -1,
        FREE = 0,
        WAITTING = 1,
        WATCH = 2,
        COMPLETED = 3 //mở hết free deal => reset lại đợt sau
    }
}


[System.Serializable]
public class StoreDealCardsData
{
    public static StoreDealCardsData Instance
    {
        get
        {
            return StoreDealsData.Instance.dealCardsData;
        }
    }

    public List<StoreDealCardData> statsCardDeals;
    public List<StoreDealStatData> statsCueDeals;

    public List<StoreDealCardData> GetDealCards()
    {
        return this.statsCardDeals;
    }

    public List<StoreDealStatData> GetDealCues()
    {
        return this.statsCueDeals;
    }

    public StoreDealCardsData()
    {
        this.statsCardDeals = new List<StoreDealCardData>();
        this.statsCueDeals = new List<StoreDealStatData>();
    }

    public void CreateDeals()
    {
        //TEMP random stats card type
        RandomDealCards();
        SaveData();
    }

    private void RandomDealCards()
    {
        this.statsCardDeals?.Clear();
        this.statsCardDeals = new List<StoreDealCardData>();

        this.statsCueDeals?.Clear();
        this.statsCueDeals = new List<StoreDealStatData>();

        int iSlot = 0;
        int countSlot = 5;

        //parse cue cards
        //List<StatData> cueCardDatas = StatManager.Instance.QueryCuesForShopDeal_CardPart();
        //if (cueCardDatas != null)
        //{
        //    foreach (StatData StatData in cueCardDatas)
        //    {
        //        if (iSlot < countSlot && StatData != null)
        //        {
        //            StoreDealSlotConfig dealSlot = StoreConfigs.Instance.GetDealSlot_ByIndex(iSlot);

        //            Tier cueTier = StatData.config.tier;
        //            StoreDealCardData dealCardData = new StoreDealCardData();
        //            dealCardData.cueTier = cueTier;
        //            dealCardData.cueID = StatData.id;
        //            dealCardData.idSlot = dealSlot.id;

        //            dealCardData.indexBuy = 0;
        //            dealCardData.countCard = StoreConfigs.GetRairityMaxStock(cueTier); //số lượng bán 1 lần tính theo rarity

        //            dealCardData.basePrice = StoreConfigs.GetRarity_BasePrice(cueTier);
        //            dealCardData.startPrice = StoreConfigs.GetRarity_NotOwnPrice(cueTier);
        //            dealCardData.step = StoreConfigs.GetRarity_StepPrice(cueTier);
        //            dealCardData.priceType = StoreConfigs.GetRarirty_PriceType(cueTier);

        //            this.statsCardDeals.Add(dealCardData);

        //            iSlot += 1;
        //        }
        //    }
        //}

        ////parse cues
        //List<StatData> StatDatas = StatManager.Instance.QueryCuesForShopDeal_CuePartV2();
        //if (StatDatas != null)
        //{
        //    foreach (StatData StatData in StatDatas)
        //    {
        //        if (iSlot < countSlot && StatData != null)
        //        {
        //            StoreDealSlotConfig dealSlot = StoreConfigs.Instance.GetDealSlot_ByIndex(iSlot);
        //            Tier cueTier = StatData.config.tier;

        //            StoreDealStatData dealStatData = new StoreDealStatData();
        //            dealStatData.cueTier = cueTier;
        //            dealStatData.cueID = StatData.id;
        //            dealStatData.idSlot = dealSlot.id;

        //            dealStatData.priceType = StoreConfigs.GetRarirty_PriceType(cueTier);
        //            dealStatData.price = StatData.config.GetPriceNext(0); //StoreConfigs.GetRarity_BasePrice(cueTier);

        //            this.statsCueDeals.Add(dealStatData);

        //            iSlot += 1;
        //        }
        //    }
        //}
    }

    public void ResetDeals()
    {
        CreateDeals();
        SaveData();
    }

    public void OnBuyDealCard(string cueID)
    {
        //TODO update price
        if (this.statsCardDeals != null)
        {
            StoreDealCardData dealCardData = this.statsCardDeals.Find(x => x.cueID == cueID);
            if (dealCardData != null)
            {
                dealCardData.indexBuy += 1;
            }
        }

        SaveData();
    }

    public void OnBuyDealCue(string cueID)
    {
        //TODO buy deal cue
    }

    private void SaveData()
    {
        GameDataManager.Instance.SaveUserData();
    }

#if UNITY_EDITOR
    [UnityEditor.MenuItem("Test/Stores/Create deals")]
    private static void TestCreateDeals()
    {
        StoreDealsData.Instance.CreateDeals();
    }
#endif
}

[System.Serializable]
public class StoreDealData
{
    public int idSlot;

    public Tier cueTier;
    public string cueID;
}

/// <summary>
/// Mua cue
/// </summary>
[System.Serializable]
public class StoreDealStatData: StoreDealData 
{
    public BoosterType priceType;
    public long price;
}


/// <summary>
/// Mua card cue
/// </summary>
[System.Serializable]
public class StoreDealCardData: StoreDealData
{
    public int indexBuy; //lần buy thứ mấy => lấy giá tiền trong config
    public int countCard; //số lượng có thể mua

    public BoosterType priceType;
    public long startPrice; //giá mua lần 0 nếu chưa sở hữu
    public long basePrice; //giá mua lần 0 nếu đã sở hữu
    public long step; //tăng mỗi lần mua, price = step * indexBuy + basePrice

    public bool IsMaxCanBuy()
    {
        return this.indexBuy >= countCard;
    }

    public BoosterCommodity GetCurrentPrice()
    {
        
        if (StatDatas.Instance.GetCue(cueID).kind == StatManager.Kind.NotUnlocked)
            return new BoosterCommodity(priceType, startPrice);
        else
            return new BoosterCommodity(priceType, indexBuy * step + basePrice);

        //Code cũ
        //if (!StatsDatas.Instance.IsUnLock(cueID))
        //    return new BoosterCommodity(priceType, startPrice);
        //else
        //    return new BoosterCommodity(priceType, indexBuy * step + basePrice);
    }
}


[System.Serializable]
public class StoreFreeCoinData
{
    public static StoreFreeCoinData Instance
    {
        get
        {
            return StoreDatas.Instance.freeCoinData;
        }
    }

    public const double TOTAL_TIME_WAIT = GameDefine.TIME_FREE_COIN; 
    public long timeStart;
    public long coinReward; //phần coin quà nhận được

    public bool isFree;

    public void CreateFree()
    {
        this.coinReward = GameDefine.COIN_WATCH_FREE; //TODO calculate coin reward
        this.isFree = false;
    }

    public void ResetFree()
    {
        //TODO Randome deals
        CreateFree();
        SaveData();
    }

    public bool IsFreeCoin(ref double timeRemain)
    {
        if (!this.isFree)
        {
            DateTime oldDate = DateTime.FromFileTime(this.timeStart);
            DateTime newDate = DateTime.Now;
            TimeSpan difference = newDate.Subtract(oldDate);
            timeRemain = difference.TotalSeconds;
            if (timeRemain >= TOTAL_TIME_WAIT)
            {
                this.isFree = true;
                SaveData();
            }
        }

        return this.isFree;
    }

    public void CollectFreeCoin()
    {
        this.timeStart = DateTime.Now.ToFileTime();
        ResetFree();
    }

    private void SaveData()
    {
        GameDataManager.Instance.SaveUserData();
    }
}