using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.Linq;

[System.Serializable]
public class UserBehaviorDatas
{


    private LogGameAnalytics logAnalytics;
    public LogGameAnalytics LogAnalytics
    {
        get
        {
            if (this.logAnalytics == null)
                logAnalytics = LogGameAnalytics.Instance;

            return this.logAnalytics;
        }
    }

    private UserCareers career;
    public UserCareers Careers
    {
        get
        {
            if (this.career == null)
                career = UserDatas.Instance.careers;

            return this.career;
        }
    }



    //Ngày cái app
    public long dayInstall;

    //Số lượng mua iap
    public List<int> purchases;
    public bool IsPurchased => this.purchases.Count > 0 && this.totalIAPMoney > 0;

    public int totalSession;

    /// <summary>
    /// Value Generation
    /// </summary>
    
    //Số tiền iap đã mua
    public float totalIAPMoney;
    public float maxIAPMoneyInOneTransac;

    public long lastTimeIAP;

    //Số gem đã spend
    public float totalGemMoney;
    public float totalGemTransaction;

    public float maxGemMoneyInOneTransac;

    public long lastTimeGem;

    //Số lượng watch ads
    public int countWatchAds;
    ///Số ngày watch ads
    public int countDayWatchAds;

    /// <summary>
    /// Engage Progress
    /// </summary>
    public int totalItem;
    public int totalUpgradeEvent;
    public long maxTrophyAchieve;

    public bool isLogedConverted;
    public bool isLogedEngaging;

    public List<ResourceData> income;
    public List<ResourceData> outcome;
    public static UserBehaviorDatas Instance
    {
        get
        {
            return GameDataManager.Instance.GameDatas.behaviorDatas;
        }
    }

    private int CountDayInstall
    {
        get
        {
            if (this.dayInstall != -1)
            {
                DateTime dt = DateTime.FromFileTimeUtc(this.dayInstall);
                DateTime now = DateTime.UtcNow;

                return (int)now.Subtract(dt).TotalDays;
            }
            return 0;
        }
    }
    private int CountDayBuyIAP
    {
        get
        {
            if (this.lastTimeIAP != -1)
            {
                DateTime dt = DateTime.FromFileTimeUtc(this.lastTimeIAP);
                DateTime now = DateTime.UtcNow;

                return (int)now.Subtract(dt).TotalDays;
            }
            return 0;
        }
    }
    private int CountDayUseGem
    {
        get
        {
            if (this.lastTimeGem != -1)
            {
                DateTime dt = DateTime.FromFileTimeUtc(this.lastTimeGem);
                DateTime now = DateTime.UtcNow;

                return (int)now.Subtract(dt).TotalDays;
            }
            return 0;
        }
    }
    public UserBehaviorDatas()
    {
        this.dayInstall = -1;
        this.lastTimeGem = -1;
        this.lastTimeIAP = -1;

        this.purchases = new List<int>();
        this.income = new List<ResourceData>();
        this.outcome = new List<ResourceData>();
        this.countWatchAds = 0;
        this.isLogedConverted = false;
        this.isLogedEngaging = false;
    }

    public void CreateUser()
    {
        Debug.Log("Firebase create new user");

        this.dayInstall = DateTime.UtcNow.ToFileTime();
        this.lastTimeGem = DateTime.UtcNow.ToFileTime();
        this.lastTimeIAP = DateTime.UtcNow.ToFileTime();

        totalSession = 0;

        totalItem = 0;
        totalUpgradeEvent = 0;
        maxTrophyAchieve = 0;

        this.isLogedConverted = false;
        this.isLogedEngaging = false;
    }

    public const float DEFINE_ADS_ENGAGING = 5;
    public const float DEFINE_ADS_CONVERTED = 8;
    /// <summary>
    /// Watch Ads
    /// </summary>
    public void WatchAds()
    {
        if (countWatchAds == 0)
        {
            LogAnalytics.SetUserPropertyAndValue(LogPropertyEvent.SessionFirstWatchAds, totalSession.ToString());
        }

        if ((float)countWatchAds / this.totalSession >= DEFINE_ADS_CONVERTED && !this.isLogedConverted)
        {
            LogAnalytics.SetUserPropertyAndValue(LogPropertyEvent.SessionFirstConvertedSucess, totalSession.ToString());
        }
        if ((float)countWatchAds / this.totalSession >= DEFINE_ADS_ENGAGING && !this.isLogedEngaging)
        {
            LogAnalytics.SetUserPropertyAndValue(LogPropertyEvent.SessionFirstEngagingSucess, totalSession.ToString());
        }

        this.countWatchAds++;

        logAnalytics.LogEvent(LogAnalyticsEvent.VIDEO_REWARD);

        LogAnalytics.SetUserPropertyAndValue(LogPropertyEvent.TotalRewardAdsWatch, countWatchAds.ToString());
        LogAnalytics.SetUserPropertyAndValue(LogPropertyEvent.LastRewardAdsWatch, DateTime.Now.ToShortDateString());
        LogAnalytics.SetUserPropertyAndValue(LogPropertyEvent.AvgAdsPerDay, ((float)countWatchAds / this.totalSession).ToString());


        this.Save();
    }
    /// <summary>
    /// Mua IAP
    /// </summary>
    /// <param name="id">id của IAP</param>
    public void Purchase(int id)
    {
        //if (!this.IsPurchased)
        //    LogAnalytics.SetUserPropertyAndValue(LogPropertyEvent.SessionFirstBuyIAP, totalSession.ToString());

        //if(!this.isLogedConverted)
        //    LogAnalytics.SetUserPropertyAndValue(LogPropertyEvent.SessionFirstConvertedSucess, totalSession.ToString());

        //this.purchases.Add(id);

        //StoreIAPBaseConfig config = StoreConfigs.Instance.GetIAPConfig(id);

        //this.totalIAPMoney += config.money;
        //LogAnalytics.SetUserPropertyAndValue(LogPropertyEvent.TotalIAPSpend, totalIAPMoney.ToString());

        //LogAnalytics.SetUserPropertyAndValue(LogPropertyEvent.AvgPurchasePrice, (totalIAPMoney / purchases.Count).ToString());

        //if(this.maxIAPMoneyInOneTransac < config.money)
        //{
        //    maxIAPMoneyInOneTransac = config.money;
        //    LogAnalytics.SetUserPropertyAndValue(LogPropertyEvent.MaxPurchasePrice, maxIAPMoneyInOneTransac.ToString());
        //}

        //LogAnalytics.SetUserPropertyAndValue(LogPropertyEvent.TotalTransaction, purchases.Count.ToString());

        //lastTimeIAP = DateTime.Now.ToFileTime();
        //LogAnalytics.SetUserPropertyAndValue(LogPropertyEvent.LastTransaction, "0");

        //this.Save();
    }

    public void UseGem(long amount)
    {
        this.totalGemTransaction++;
        LogAnalytics.SetUserPropertyAndValue(LogPropertyEvent.TotalGemTransaction, totalGemTransaction.ToString());

        this.totalGemMoney += amount;
        LogAnalytics.SetUserPropertyAndValue(LogPropertyEvent.TotalGemSpend, totalGemMoney.ToString());

        LogAnalytics.SetUserPropertyAndValue(LogPropertyEvent.AvgGemPrice, (totalGemMoney / totalGemTransaction).ToString());

        if(amount > maxGemMoneyInOneTransac)
        {
            maxGemMoneyInOneTransac = amount;
            LogAnalytics.SetUserPropertyAndValue(LogPropertyEvent.MaxGemPrice, maxGemMoneyInOneTransac.ToString());

        }

        LogAnalytics.SetUserPropertyAndValue(LogPropertyEvent.LastGemTransaction, "0");

        lastTimeGem = DateTime.Now.ToFileTime();
        this.Save();

    }

    /// <summary>
    /// Cài game được 3 ngày rồi
    /// </summary>
    /// <returns></returns>
    public bool IsDayThree()
    {
        
        return this.CountDayInstall >= 3 && this.CountDayInstall < 7;
    }
    /// <summary>
    /// Cài game được 7 ngày rồi
    /// </summary>
    /// <returns></returns>
    public bool IsDaySeven()
    {
        return this.CountDayInstall >= 7 && this.CountDayInstall < 14;;
    }

    /// <summary>
    /// Install được ngày 7-14-21...
    /// </summary>
    /// <returns></returns>
    public bool IsDaySevenOrSame()
    {
        return this.CountDayInstall > 0 && this.CountDayInstall % 7 == 0 ;
    }
    /// <summary>
    /// Nó cài lâu lăm rồi
    /// </summary>
    /// <returns></returns>
    public bool IsLowPlayer()
    {
        return this.CountDayInstall >= 14;
    }
    /// <summary>
    /// User này đã từng mua IAP
    /// </summary>
    /// <returns></returns>
    public bool IsPurchaseUser()
    {
        return this.purchases.Count > 0;
    }

    public void SourceInCome(BoosterType type, long value, string source = "")
    {
        this.income.Add(new ResourceData(type, value, source));
        this.Save();
    }

    public void SourceOutCome(BoosterType type, long value, string source = "")
    {
        this.outcome.Add(new ResourceData(type, value, source));
        this.Save();
    }

    public bool IsCheater()
    {
        //Tiêu lớn hơn thu 1000 GEM
        if (this.TotalInCome(BoosterType.CASH) < this.TotalOutCome(BoosterType.CASH) - (1000 + GameDefine.CASH_DEFAULT))
        {
            return true;
        }
        //Tiêu lơn hơn thu 10.000 COIN
        if (this.TotalInCome(BoosterType.COIN) < this.TotalOutCome(BoosterType.COIN) - (10000 + GameDefine.COIN_DEFAULT))
        {
            return true;
        }
        return false;
    }

    private long TotalInCome(BoosterType type)
    {
        return this.income.FindAll(x => x.type == type).Sum(x => x.value);
    }

    private long TotalOutCome(BoosterType type)
    {
        return this.outcome.FindAll(x => x.type == type).Sum(x => x.value);
    }
    private void Save()
    {
        GameDataManager.Instance.SaveUserData();
    }


    public void OpenApp()
    {
        ///Log
        LogAnalytics.SetUserPropertyAndValue(LogPropertyEvent.TotalDayaSinceInstal, CountDayInstall.ToString());

        this.totalSession++;
        LogAnalytics.SetUserPropertyAndValue(LogPropertyEvent.TotalSessionsStarted, totalSession.ToString());

        LogAnalytics.SetUserPropertyAndValue(LogPropertyEvent.LastTransaction, CountDayBuyIAP.ToString());
        LogAnalytics.SetUserPropertyAndValue(LogPropertyEvent.LastGemTransaction, CountDayUseGem.ToString());

        SetUpCallback();
    }

    public void LogFacebook(bool isLogin)
    {
        LogAnalytics.SetUserPropertyAndValue(LogPropertyEvent.CurrentFBConnected, isLogin.ToString());
    }
    public void StartGame()
    {
        LogAnalytics.SetUserPropertyAndValue(LogPropertyEvent.TotalMatchesStarted, Careers.totalMatch.ToString());
    }

    public void EndGame()
    {
        LogAnalytics.SetUserPropertyAndValue(LogPropertyEvent.TotalMatchesCompleted, Careers.CompletedGame.ToString());
        LogAnalytics.SetUserPropertyAndValue(LogPropertyEvent.CurrentPlayerWinRate, Careers.GetWinRate_LastNGame(20).ToString());
    }

    //public void BuyCue(CueSystem.CueData cue)
    //{
    //    if(cue.config.statsPerLevels[0].cardsRequired == 0)
    //    {
    //        LogAnalytics.LogEvent(LogAnalyticsEvent.CUE_BOUGHT, LogParams.STAT_ITEM_ID, cue.id);
    //    }
    //    else
    //    {
    //        LogAnalytics.LogEvent(LogAnalyticsEvent.CUE_SKIP_UNLOCKED, LogParams.STAT_ITEM_ID, cue.id);
    //    }
    //}
    public void CollectNewItem()
    {
        this.totalItem++;

        LogAnalytics.SetUserPropertyAndValue(LogPropertyEvent.TotalItemCollected, this.totalItem.ToString());

        Save();

    }
    //public void UpgradeItem(CueSystem.CueData c)
    //{
    //    totalUpgradeEvent++;

    //    LogAnalytics.SetUserPropertyAndValue(LogPropertyEvent.TotalItemUpgradeEvent, totalUpgradeEvent.ToString());
    //    LogAnalytics.LogEvent(LogAnalyticsEvent.CUE_UPGRADE, LogParams.STAT_ITEM_ID, c.id);

    //    Save();
    //}

    public void OnChangeTrophy(BoosterCommodity b)
    {
        if (b == null)
            return;

        long bValue = b.GetValue();
        if (bValue > this.maxTrophyAchieve)
        {
            this.maxTrophyAchieve = bValue;

            LogAnalytics.SetUserPropertyAndValue(LogPropertyEvent.MaxTrophyAchieve, maxTrophyAchieve.ToString());
            this.Save();
        }

        LogAnalytics.SetUserPropertyAndValue(LogPropertyEvent.CurrentTrophies, bValue.ToString());
    }
    public void OnChangeCoin(BoosterCommodity b)
    {
        if (b != null)
            LogAnalytics.SetUserPropertyAndValue(LogPropertyEvent.CurrentGold, b.value);
    }
    public void OnChangeGem(BoosterCommodity b)
    {
        if (b != null)
            LogAnalytics.SetUserPropertyAndValue(LogPropertyEvent.CurrentGem, b.value);
    }

    public void OnUnlockNewRoom(int id)
    {
        if(id != GameDefine.ROOM_FRIST_AI &&  id != GameDefine.ROOM_PRACTICLE)
        LogAnalytics.SetUserPropertyAndValue(LogPropertyEvent.CurrentMaxRoom, id.ToString());
    }

    /// <summary>
    /// Khi player đánh complete, tự check chỉ log game đầu
    /// </summary>
    /// <param name="shotIndex">Thứ tự cú đánh, chỉ tính của player</param>
    /// <param name="isSuccess">Có thành công hay không, true nếu đánh bi vào hoặc được đánh lượt tiếp theo (cú ba đăng), false nếu hụt </param>
    public void OnPlayerHitted(int shotIndex, bool isSuccess)
    {
        LogAnalytics.LogEvent(string.Format(LogAnalyticsEvent.FIRSTGAME_SHOT_TIMES, shotIndex));
        LogAnalytics.LogEvent(string.Format(LogAnalyticsEvent.FIRSTGAME_SHOT_TIMES_RESULT, shotIndex, isSuccess));

        Debug.LogError($"SHOTT_TIMES: {shotIndex} - {isSuccess}");

    }

    public void ClaimGlove(string where, int time)
    {
        LogAnalytics.LogEvent(
            string.Format(LogAnalyticsEvent.CLAIM_GLOVE, where, time));
    }
    private void SetUpCallback()
    {

        UserProfile.Instance.AddCallbackBooster(BoosterType.CUP, OnChangeTrophy);
        UserProfile.Instance.AddCallbackBooster(BoosterType.COIN, OnChangeCoin);
        UserProfile.Instance.AddCallbackBooster(BoosterType.CASH, OnChangeGem);

        RoomDatas.callbackUnlockRoom += OnUnlockNewRoom;
    }

    [System.Serializable]
    public class ResourceData
    {
        public BoosterType type;
        public long value;
        public ResourceData()
        {}
        public ResourceData(BoosterType type, long value, string source)
        {
            this.type = type;
            this.value = value;
        }
    }
}
