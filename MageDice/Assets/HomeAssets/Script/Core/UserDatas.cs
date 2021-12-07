using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cosina.Components;
using System.Linq;
using UnityEngine.Events;

[System.Serializable]
public class UserDatas
{
    public UserInfo info;
    public UserCareers careers;
    public UserLanguageData languages;
    //public UserWheelData wheel;
    public PerkDatas perkDatas;
    public UserBonusData bonusDatas;
    public static UserDatas Instance
   {
      get
      {
         return GameDataManager.Instance.GameDatas.userDatas;
      }
   }
   public UserDatas()
   {
        this.info = new UserInfo();
        this.careers = new UserCareers();
        this.languages = new UserLanguageData();
        this.perkDatas = new PerkDatas();
        bonusDatas = new UserBonusData();
        //this.wheel = new UserWheelData();
    }
   public void CreateUser()
   {
        languages.NewUser();
        perkDatas.CreateUser();
        bonusDatas.CreateUser();
        //wheel.NewDay();
   }

    public void OpenGame()
    {
        if(perkDatas == null || perkDatas.data == null || perkDatas.data.Count == 0)
            perkDatas.CreateUser();

        bonusDatas.OnOpenApp();
    }
}

[System.Serializable]
public class UserFacebookInfo
{
    public string facebookID;
    public string facebookName;
    public string base64Avatar;
    public bool isLogin;
    private Sprite sprAvatar;

    public UserFacebookInfo()
    {
        this.facebookID = string.Empty;
        this.facebookName = string.Empty;
        this.isLogin = false;
    }

    public void LoginFacebook()
    {
        this.isLogin = true;
    }
    public void LoginFacebook(string facebookId, string facebookName)
    {
        this.facebookID = facebookId;
        this.facebookName = facebookName;
        this.isLogin = true;

        UserBehaviorDatas.Instance.LogFacebook(true);

        this.Save();
    }

    public void SetAvatar(string avatar)
    {
        this.base64Avatar = avatar;
        this.Save();
    }

    public Sprite GetAvatarFromBase64
    {
        get
        {
            if (this.sprAvatar == null)
            {
                if (!string.IsNullOrEmpty(base64Avatar) && this.isLogin)
                {
                    byte[]  imageBytes = Convert.FromBase64String(base64Avatar);
                    Texture2D tex = new Texture2D(200, 200);
                    tex.LoadImage( imageBytes );
                    sprAvatar = Sprite.Create(tex, new Rect(0.0f, 0.0f, tex.width, tex.height), new Vector2(0.5f, 0.5f), 100.0f);
                    return sprAvatar;
                }
            }
            
            return null;
        }
       
    }

    public void LogoutFacebook()
    {
        this.facebookID = string.Empty;
        this.facebookName = string.Empty;
        this.isLogin = false;

        UserBehaviorDatas.Instance.LogFacebook(false);

        this.Save();
    }

    public void Save()
    {
        GameDataManager.Instance.SaveUserData();
    }
}
[System.Serializable]
public class UserInfo
{
   
   public string id;
   public string nickname;
   public string avatar;
   public UserFacebookInfo facebook;
   public bool isLoginFirebase;
   private Sprite sprAvatar;

   public string Avatar
   {
       get => this.avatar;
       set { 
           this.avatar = value;
           this.SprAvatar = CommonAvatar.Instance.GetAvatarById(this.avatar);
       }
   }

   public Sprite SprAvatar
   {
       get
       {
           if (this.facebook.isLogin)
           {
               this.sprAvatar = this.facebook.GetAvatarFromBase64;
               if (this.sprAvatar == null)
               {
                   this.sprAvatar = CommonAvatar.Instance.GetAvatarById(this.avatar);
               }
           }
           else
           {
               this.sprAvatar = CommonAvatar.Instance.GetAvatarById(this.avatar);
           }
           return this.sprAvatar;
       }
       set
       {
           this.sprAvatar = value;
       }
   }
   
   public UserInfo()
   {
       
       this.facebook = new UserFacebookInfo();
        this.id = string.Empty;
        this.nickname = "Cosina";
        this.avatar = "0";
        this.isLoginFirebase = false;
   }

    public UserInfo(UserInfo i)
    {
        
        this.facebook = new UserFacebookInfo();
        this.id = i.id;
        this.nickname = i.nickname;
        this.Avatar = i.avatar;
    }

    public UserInfo(string id, string name, string avatar, string skinID)
    {
        
        this.facebook = new UserFacebookInfo();
        this.id = id;
        this.nickname = name;
        this.Avatar = avatar;
    }

    public void LoginFirebase(string userId)
    {
        this.id = userId;
        this.isLoginFirebase = true;
        this.SaveData();
    }
    public void ChangeName(string name)
    {
        this.nickname = name;
        SaveData();
    }

    public void ChangeAvatar(string avatar)
    {
        this.Avatar = avatar;
        SaveData();
    }

    private void SaveData()
    {
        GameDataManager.Instance.SaveUserData();
    }
}

[System.Serializable]
public class UserCareers
{
   public long coinEarned;
   public int totalMatch;
   public int matchWin;
   public long trophyMax;
   public int winStreak;
   public int loseStreak;
   public int maxWinStreak;

    public int maxTrainPoint;

   public List<CareerMatchData> careerMatches;
    public int CompletedGame
    {
        get
        {
#if TEST_BOT_FIRST_GAME
            Debug.LogError("CLEAR SYMBOL TEST_BOT_FIRST_GAME IF BUILD APP");
            return 1;
#endif
            return careerMatches == null ? 0 : careerMatches.Count;
        }
    }

   public UserCareers()
   {
        careerMatches = careerMatches ?? new List<CareerMatchData>();
    }
    public static UserCareers MainUserInstance => UserDatas.Instance.careers;

   public void StartGame(int roomID)
   {
        //MissionDatas.Instance.DoStep(MissionID.PLAY_GAME, 1);
        this.totalMatch++;
        UserBehaviorDatas.Instance.StartGame();
        LogGameAnalytics.Instance.LogEvent(LogAnalyticsEvent.PLAY_GAME, LogParams.ROOM_ID, roomID);

        if(totalMatch <= 100)
            LogGameAnalytics.Instance.LogEvent(string.Format(LogAnalyticsEvent.PLAY_GAME_TIMES, this.totalMatch), LogParams.ROOM_ID, roomID);

        careerMatches.Add(new CareerMatchData()
        {
            id = string.Format("Room{0}_{1}", roomID, careerMatches.Count + 1),
            isWin = false,
        }) ;

        this.Save();
   }
   public void FinishGame(bool isWin, int roomID)
   {
          if (isWin)
          {
             this.loseStreak = 0;
             this.matchWin++;
             this.winStreak++;
             if (this.winStreak > this.maxWinStreak)
             {
                this.maxWinStreak = this.winStreak;
             }
            //MissionDatas.Instance.DoStep(MissionID.WIN_GAME, 1);
        }
          else
          {
             this.loseStreak++;
             this.winStreak = 0;
          }

        careerMatches[careerMatches.Count - 1].isWin = isWin;

        UserBehaviorDatas.Instance.EndGame();
        LogGameAnalytics.Instance.LogEvent(LogAnalyticsEvent.COMPLETE_GAME, LogParams.ROOM_ID, roomID);

        if (CompletedGame <= 100)
        {
            LogGameAnalytics.Instance.LogEvent(string.Format(LogAnalyticsEvent.COMPLETE_GAME_TIMES, CompletedGame), LogParams.GAME_RESULT, (isWin ? 0 : 1));
        }

        this.Save();
   }

    public void FinishTrain(int point)
    {
        if(point > maxTrainPoint)
        {
            maxTrainPoint = point;
            this.Save();
        }
    }

   public void EarnedCoin(long coin)
   {
      this.coinEarned += coin;
      this.Save();
   }

   public void UpdateTrophy(long current)
   {
      if (current > this.trophyMax)
      {
         this.trophyMax = current;
         this.Save();
      }
   }
   public float GetWinRate()
   {
      if (this.totalMatch <= 0)
      {
         return 0.0f;
      }

      return (float) this.matchWin / (float) this.totalMatch;
   }

    ///Winrate của N game gần nhất
    public float GetWinRate_LastNGame(int n = 10)
    {
        if (this.totalMatch <= 0)
        {
            return 0.0f;
        }

        if (this.careerMatches.Count < n)
            n = this.careerMatches.Count-1;

        if (n <= 0)
        {
            return 0f;
        }

        //Lấy thêm 1 phần tử vì phần tử cuối cùng là game hiện đang chơi => chưa có kết quả
        float parseWinAmount = (float)this.careerMatches.TakeLast(n+1).Where(x => x.isWin).Count();
        return parseWinAmount / (n);
    }
    private void Save()
   {
      GameDataManager.Instance.SaveUserData();
   }

    [System.Serializable]
    public class CareerMatchData
    {
        public string id; //RoomID_MatchIndex
        public List<int> historyHit;

        public bool isWin;
    }
}

[System.Serializable]
public class UserLanguageData
{
    public static UserLanguageData Instance => UserDatas.Instance.languages;

    public LanguageDefine Language;

    public void NewUser()
    {
        //detect system language
        ///In 10-09-2021, we dicide to shut down this fucking auto set language feature
        ///As the translation script is not good, in true, is fucking suck
        
        this.Language = LanguageDefine.English;
        return;

#if !UNITY_EDITOR

        switch (Application.systemLanguage)
        {
            case SystemLanguage.English:
                this.Language = LanguageDefine.English;
                break;
            case SystemLanguage.Vietnamese:
                this.Language = LanguageDefine.Vietnamese;
                break;
            case SystemLanguage.Indonesian:
                this.Language = LanguageDefine.Indonesian;
                break;
            case SystemLanguage.Arabic:
                this.Language = LanguageDefine.Arabic;
                break;
            case SystemLanguage.Portuguese:
                this.Language = LanguageDefine.Portuguese;
                break;
            case SystemLanguage.Japanese:
                this.Language = LanguageDefine.Japanese;
                break;
            case SystemLanguage.Korean:
                this.Language = LanguageDefine.Korean;
                break;
            case SystemLanguage.Chinese:
                this.Language = LanguageDefine.Chinese;
                break;
            case SystemLanguage.ChineseSimplified:
                this.Language = LanguageDefine.Chinese;
                break;
            case SystemLanguage.ChineseTraditional:
                this.Language = LanguageDefine.Taiwan;
                break;
            case SystemLanguage.Spanish:
                this.Language = LanguageDefine.Spanish;
                break;
            case SystemLanguage.Thai:
                this.Language = LanguageDefine.Thai;
                break;
            case SystemLanguage.German:
                this.Language = LanguageDefine.German;
                break;
            default:
                this.Language = LanguageDefine.English;
                break;
        }
#else
        this.Language = LanguageDefine.English;
#endif
    }

    public void ChangeLanguage(LanguageDefine Language)
    {
        if(Language != this.Language)
        {
            this.Language = Language;
            Save();
        }
    }

    private void Save()
    {
        GameDataManager.Instance.SaveUserData();
    }
}

#region PerkData
[System.Serializable]
public class PerkDatas
{
    public List<PerkData> data;
    public int totalUpgradeStep;

    public static PerkDatas Instance
    {
        get
        {
            return UserDatas.Instance.perkDatas;
        }
    }

    public int TotalUpgradeStep { get => totalUpgradeStep; set {
            totalUpgradeStep = value;
            _costUpgradeNext = SkillPerkConfigs.Instance.GetCostUpgrade(totalUpgradeStep);
        } 
    }

    private BoosterCommodity _costUpgradeNext;
    public BoosterCommodity CostUpgradeNext
    {
        get
        {
            if (_costUpgradeNext == null)
                _costUpgradeNext = SkillPerkConfigs.Instance.GetCostUpgrade(totalUpgradeStep);

            return _costUpgradeNext;
        }
    }

    public PerkDatas()
    {
        data = new List<PerkData>();
    }

    public float GetCurrentStat(int id)
    {
        Debug.Log("get stat " + id);
        return this.data.Find(x => x.id == id).CurrentStat;
    }

    public void CreateUser()
    {
        data = new List<PerkData>();
        List<SkillPerkConfig> SkillConfigs = SkillPerkConfigs.Instance.SkillConfigs;

        for (int i = 0; i < SkillConfigs.Count; i++)
        {
            data.Add(new PerkData(SkillConfigs[i])
            {
                currentUpgradeStep = -1,
            });
        }

        TotalUpgradeStep = 0;
    }

    public void OnCompleteUpgrade(int id)
    {
        this.data.Find(x => x.id == id).currentUpgradeStep++;
    }
    public bool OnClickUpgrade()
    {
        if(UserProfile.Instance.UseBooster(this.CostUpgradeNext, "UpgradePerk", "UpgradePerk"))
        {
            TotalUpgradeStep++;
            SaveData();
            return true;
        }

        return false;
    }
    public int GetUpgradeResult()
    {
        return UnityEngine.Random.Range(0, data.Count);
    }

    private void SaveData()
    {
        GameDataManager.Instance.SaveUserData();
    }
}
[System.Serializable]
public class PerkData
{
    public int id;
    public int currentUpgradeStep;

    private SkillPerkAsset _asset;
    public SkillPerkAsset Asset
    {
        get
        {
            if (_asset == null)
                _asset = SkillPerkAssets.Instance.GetPerkAsset(this.id);

            return _asset;
        }
    }

    private SkillPerkConfig _config;
    public SkillPerkConfig Config
    {
        get
        {
            if (_config == null)
                _config = SkillPerkConfigs.Instance.GetConfig(this.id);

            return _config;
        }
    }

    public float CurrentStat => currentUpgradeStep >= 0 ? this.Config.upgradeSteps[currentUpgradeStep] : 0f;
    public float PreviousStat => currentUpgradeStep - 1 >= 0 ? this.Config.upgradeSteps[currentUpgradeStep-1] : 0f;
    public float MaxStat => this.Config.upgradeSteps[this.Config.upgradeSteps.Length - 1];

    public bool IsMax => this.currentUpgradeStep >= this.Config.upgradeSteps.Length - 1;

    public PerkData()
    {

    }
    public PerkData(SkillPerkConfig c)
    {
        this._config = c;
        this.id = c.id;
    }

}
#endregion

#region Bonus Data
[System.Serializable]
public class UserBonusData
{
    public static UserBonusData Instance
    {
        get
        {
            return UserDatas.Instance.bonusDatas;
        }
    }

    public enum BonusType
    {
        NONE = -1,
        DAILY_REWARD = 0,
        ONLINE_REWARD = 1,
        OFFLINE_REWARD = 2,
    }
    public UserDailyRewardData dailyReward;
    public UserOnlineBonusData onlineBonus;
    public UserOfflineBonusData offlineBonus;

    public UnityAction<BonusType> eventCanReward;

    public UserBonusData()
    {
        this.dailyReward = new UserDailyRewardData();
        this.onlineBonus = new UserOnlineBonusData();
        this.offlineBonus = new UserOfflineBonusData();
    }

    public void CreateUser()
    {
        this.timeOld = DateTime.Now.ToFileTime();

        dailyReward = new UserDailyRewardData();
        dailyReward.CreateUser();

        onlineBonus = new UserOnlineBonusData();
        onlineBonus.CreateUser();

        offlineBonus = new  UserOfflineBonusData();
        offlineBonus.CreateUser();
    }
    public void StartNewDay(int dayPassedSinceLastLogin)
    {
        if(dayPassedSinceLastLogin >= 0)
        {
            dailyReward.NewDay(dayPassedSinceLastLogin);
        }
        onlineBonus.StartNewDay();
        offlineBonus.NewDay(dayPassedSinceLastLogin);
    }
    public void OnOpenApp()
    {
        dailyReward.OnOpenApp();
        onlineBonus.OnOpenApp();
        offlineBonus.OnOpenApp();

        IsNewDay();
    }
    public BonusType IsAnyBonusActive()
    {
        if (dailyReward.IsCanReward)
        {
            eventCanReward?.Invoke(BonusType.DAILY_REWARD);
            return BonusType.DAILY_REWARD;
        }
        if (onlineBonus.IsCanReward)
        {
            eventCanReward?.Invoke(BonusType.ONLINE_REWARD);
            return BonusType.ONLINE_REWARD;
        }
        if (offlineBonus.IsCanReward)
        {
            eventCanReward?.Invoke(BonusType.OFFLINE_REWARD);
            return BonusType.OFFLINE_REWARD;
        }
        //Không có loại bonus nào active
        return BonusType.NONE; 
    }

    public void RegisterEventCanReward(UnityAction<BonusType> callback)
    {
        eventCanReward += callback;
    }
    public void UnRegisterEventCanReward(UnityAction<BonusType> callback)
    {
        eventCanReward -= callback;
    }

    private void SaveData()
    {
        GameDataManager.Instance.SaveUserData();
    }


    #region Check day
    public long timeOld;

    /// <summary>
    /// kiểm tra qua ngày mới chưa để reset deal
    /// </summary>
    /// <param name="totalRemain"></param>
    /// <returns></returns>
    public bool IsNewDay()
    {
        if (timeOld <= 0)
            return false;

        DateTime timeOldFT = DateTime.FromFileTime(this.timeOld);

        if (timeOldFT == null)
        {
            this.StartNewDay(1);
            this.timeOld = DateTime.Now.ToFileTime();

            return true;
        }

        bool isDiffDate = timeOldFT.Date != DateTime.Now.Date;

        if (isDiffDate)
        {
            this.StartNewDay(DateTime.Now.Date.Subtract(timeOldFT.Date).Days);

            this.timeOld = DateTime.Now.ToFileTime();

            this.SaveData();

            return true;
        }
        return false;
    }
    #endregion
}

#region Daily
public enum DailyDayClaimState
{
    NONE = -1,
    CLAIMED = 0,
    RECLAIM = 1,
    READYCLAIM = 2,
    UNREACHED = 3
}
[System.Serializable]
public class UserDailyRewardData
{
    public static UserDailyRewardData Instance
    {
        get
        {
            return UserBonusData.Instance.dailyReward;
        }
    }

    public List<DailyRewardDayData> dataPass;
    public int currentDay;

    private DailyRewardConfigs dailyRewardConfigs;
    public DailyRewardConfigs DailyRewardConfigs
    {
        get
        {
            if(dailyRewardConfigs == null)
                dailyRewardConfigs = DailyRewardConfigs.Instance;
            return dailyRewardConfigs;
        }
    }

    public DailyRewardDayData StickData => dataPass[dataPass.Count - 1];
    public void CreateUser()
    {
        currentDay = 0;

        dataPass = new List<DailyRewardDayData>();
        AddNewWeek();

        dataPass[currentDay].state = DailyDayClaimState.READYCLAIM;
    }
    public void NewDay(int dayPassedSinceLastLogin)
    {
        if (currentDay + dayPassedSinceLastLogin >= DailyRewardConfigs.MonthAmountDay)
        {
            dayPassedSinceLastLogin = dataPass.Count - 1 - currentDay;
        }
        else
        {
            if (currentDay + dayPassedSinceLastLogin >= dataPass.Count)
                AddNewWeek();
        }

        for (int i = currentDay; i < currentDay + dayPassedSinceLastLogin; i++)
        {
            switch (dataPass[i].state)
            {
                case DailyDayClaimState.READYCLAIM:
                    dataPass[i].state = DailyDayClaimState.RECLAIM;
                    break;
                case DailyDayClaimState.UNREACHED:
                    dataPass[i].state = DailyDayClaimState.RECLAIM;
                    break;
            }
        }

        currentDay += dayPassedSinceLastLogin;
        dataPass[currentDay].state = DailyDayClaimState.READYCLAIM;
    }
    public void OnOpenApp()
    {
    }

    public void AddNewWeek()
    {
        if (dataPass.Count >= DailyRewardConfigs.MonthAmountDay || dataPass.Count + 7 >= DailyRewardConfigs.MonthAmountDay)
        {
            ResetMonth();
            return;
        }

        List<DailyRewardDayData> week = new List<DailyRewardDayData>();
        DailyRewardDayData d;
        for (int i = dataPass.Count; i < dataPass.Count + 7; i++)
        {
            d = new DailyRewardDayData(DailyRewardConfigs.GetDay(i));
            d.state = DailyDayClaimState.UNREACHED;
            week.Add(d);
        }

        this.dataPass.AddRange(week);
        SaveData();
    }
    public void ResetMonth()
    {
        currentDay = 0;
        dataPass.Clear();
        AddNewWeek();

        SaveData();
    }

    public bool IsCanReward
    {
        get
        {
            return dataPass.Find(x => x.state == DailyDayClaimState.READYCLAIM || x.state == DailyDayClaimState.RECLAIM) != null;
        }
    }

    public void SetDayState(int day, DailyDayClaimState state)
    {
        this.dataPass[day].state = state;
        SaveData();
    }
    private void SaveData()
    {
        GameDataManager.Instance.SaveUserData();
    }
}

[System.Serializable]
public class DailyRewardDayData
{
    public BoosterCommodity booster;
    public BagAmount bag;
    public DailyDayClaimState state;

    public bool IsRewardBag;
    public DailyRewardDayData()
    {

    }
    public DailyRewardDayData(DailyRewardConfig config)
    {
        IsRewardBag = config.IsRewardBag;
        if (config.IsRewardBag)
        {
            this.bag = new BagAmount(config.Bag);
            this.booster = null;
        }
        else
        {
            this.booster = new BoosterCommodity(config.Booster);
            this.bag = null;
        }
    }
}
#endregion Daily

#region  Online Bonus
[System.Serializable]
public class UserOnlineBonusData
{
    public OnlineRewardConfigs.OnlineRewardUserMode urrentModeID;

    public const double TIME_WAIT_ONLINE = 60; //1200; //20m online
    public long timeStartOnlineLatest;
    public double timeWaitRemain;

    public int currnentDay;
    public int _nextRewardIndex;
    public bool isOutOfReward;
    public bool IsClaimAllReward => this._nextRewardIndex >= this.CurrentModeConfig.RewardAmount;
    public bool IsCanReward
    {
        get
        {
            double v = 0;
            return IsWaitEnough(ref v);
        }
    }

    private OnlineRewardDayConfig currentModeConfig;
    public OnlineRewardDayConfig CurrentModeConfig
    {
        get
        {
            if (this.currentModeConfig == null)
                currentModeConfig = OnlineRewardConfigs.Instance.GetDayConfig(this.urrentModeID, currnentDay);

            return this.currentModeConfig;
        }
    }

    private OnlineRewardConfigItem _currentRewardConfig;
    public OnlineRewardConfigItem CurrentRewardConfig
    {
        get
        {
            if (IsClaimAllReward)
            {
                isOutOfReward = true;
                SaveData();
            }

            if (_currentRewardConfig == null)
                _currentRewardConfig = CurrentModeConfig.config[_nextRewardIndex];

            return _currentRewardConfig;
        }
    }
    public int TotalReward
    {
        get
        {
            return CurrentModeConfig.RewardAmount;
        }
    }
    public static UserOnlineBonusData Instance
    {
        get
        {
            return UserBonusData.Instance.onlineBonus;
        }
    }

    public UserOnlineBonusData()
    {
        _nextRewardIndex = 0;
        this.urrentModeID = OnlineRewardConfigs.OnlineRewardUserMode.FREE;
    }

    [System.NonSerialized]
    public System.Action callbackChangeUserMode;

    public void OnUserPurchase()
    {
        this.urrentModeID = OnlineRewardConfigs.OnlineRewardUserMode.PAID;
        currentModeConfig = OnlineRewardConfigs.Instance.GetDayConfig(this.urrentModeID, currnentDay);
        _currentRewardConfig = CurrentModeConfig.config[_nextRewardIndex];

        callbackChangeUserMode?.Invoke();
        SaveData();
    }


    /// <summary>
    /// kiểm tra user đã online đủ chưa
    /// </summary>
    /// <param name="totalRemain">Thời gian còn lại của pass để show lên UI, in TotalSecond</param>
    /// <param name="saveData">Chỉ cho bằng true 1 lần khi mở dialog để save lại data, trong vòng update bình thường không cần</param>
    /// <returns></returns>
    public bool IsWaitEnough(ref double totalRemain)
    {
        double timePassed = DateTime.Now.Subtract(DateTime.FromFileTime(timeStartOnlineLatest)).TotalSeconds;

        totalRemain = this.timeWaitRemain - timePassed;


        if (totalRemain <= 0)
        {
            timeWaitRemain = 0;
            SaveData();

            return true;
        }

        return false;
    }

    public void OnClaimReward()
    {
        //domission
        //MissionDatas.Instance.DoStep(MissionID.PLAY_WHEEL);
        //if (urrentModeID == OnlineRewardConfigs.OnlineRewardUserMode.PAID)
        //    LogGameAnalytics.Instance.LogEvent(string.Format(LogAnalyticsEvent.CLAIM_ONLINE_FREEREWARD, this._nextRewardIndex), LogParams.USER_SESSION_DAY, currnentDay);
        //else
        //    LogGameAnalytics.Instance.LogEvent(string.Format(LogAnalyticsEvent.CLAIM_ONLINE_PAIDREWARD, this._nextRewardIndex), LogParams.USER_SESSION_DAY, currnentDay);

        this._nextRewardIndex++;
        this.timeStartOnlineLatest = System.DateTime.Now.ToFileTime();
        this.timeWaitRemain = TIME_WAIT_ONLINE;

        //check next reward or out reward
        if (IsClaimAllReward)
        {
            isOutOfReward = true;
        }
        else
        {
            _currentRewardConfig = CurrentModeConfig.config[_nextRewardIndex];
        }


        this.SaveData();
    }

    public void StartNewDay()
    {
        this.currnentDay++;
        this._nextRewardIndex = 0;
        this.timeStartOnlineLatest = DateTime.Now.ToFileTime();
        timeWaitRemain = TIME_WAIT_ONLINE;
        isOutOfReward = false;

        Debug.Log($"StartNewDay {currnentDay}");
        SaveData();
    }

    //Call this on AppQuit or OnAppFocus
    public void OnQuitApp()
    {
        this.timeWaitRemain = timeWaitRemain - DateTime.Now.Subtract(DateTime.FromFileTime(timeStartOnlineLatest)).TotalSeconds;
        this.timeStartOnlineLatest = DateTime.Now.ToFileTime();


        Debug.Log($"Time online remain {timeWaitRemain}");
        SaveData();
    }

    public void OnOpenApp()
    {
        IsNewDay();

        this.timeStartOnlineLatest = DateTime.Now.ToFileTime();

        SaveData();
    }
    public void CreateUser()
    {
        currnentDay = -1;
    }
    private void SaveData()
    {
        GameDataManager.Instance.SaveUserData();
    }


    #region Check day
    public long timeOld;

    /// <summary>
    /// kiểm tra qua ngày mới chưa để reset deal
    /// </summary>
    /// <param name="totalRemain"></param>
    /// <returns></returns>
    public bool IsNewDay()
    {
        DateTime timeOldFT = DateTime.FromFileTime(this.timeOld);

        if (timeOldFT == null)
        {
            this.StartNewDay();
            this.timeOld = DateTime.Now.ToFileTime();

            return true;
        }

        bool isDiffDate = timeOldFT.Date != DateTime.Now.Date;

        if (isDiffDate)
        {
            this.StartNewDay();

            this.timeOld = DateTime.Now.ToFileTime();

            this.SaveData();

            return true;
        }
        return false;
    }
    #endregion
}

#endregion Online Bonus

#region Offline Bonus
[System.Serializable]
public class UserOfflineBonusData
{
    public const double TIME_COLLECT_MAX = 259200; //72h
    public const double TIME_MIN_ALLOW_COLLECT = 3600; //1h

    public long timeCollectLatest;

    public float coinPerMinus;
    public float cardPerMinus;

    public bool isClaimSkip;

    private DateTime dtTimeCollectLatest;

    public static UserOfflineBonusData Instance
    {
        get
        {
            return UserBonusData.Instance.offlineBonus;
        }
    }

    public DateTime DTTimeCollectLatest
    {
        get
        {
            if (dtTimeCollectLatest == null)
                dtTimeCollectLatest = DateTime.FromFileTime(timeCollectLatest);
            return dtTimeCollectLatest;
        }
    }
    public double TimeOfflinePassed
    {
        get
        {
            double timePassed = DateTime.Now.Subtract(DTTimeCollectLatest).TotalSeconds;

            return timePassed >= TIME_COLLECT_MAX ? TIME_COLLECT_MAX : timePassed;
        }
    }
    public int TimeOfflineToMinus => (int)(TimeOfflinePassed / 60);
    
    public void CreateUser()
    {
        timeCollectLatest = DateTime.Now.ToFileTime();
        dtTimeCollectLatest = DateTime.FromFileTime(timeCollectLatest);

        coinPerMinus = 10f;
        cardPerMinus = 0.02f;
        SaveData();
    }
    public void NewDay(int dayPassedSinceLastLogin)
    {
        isClaimSkip = false;
    }
    public void OnOpenApp()
    {
        dtTimeCollectLatest = DateTime.FromFileTime(timeCollectLatest);

        float v = PerkDatas.Instance.GetCurrentStat(PerkID.COIN_OFFLINE_EARNING);
        if (v > 0)
            coinPerMinus = v;

        v = PerkDatas.Instance.GetCurrentStat(PerkID.CARD_OFFLINE_EARNING);
        if (v > 0)
            cardPerMinus = v;
    }

    public bool IsCanReward
    {
        get
        {
            double v = 0;
            return IsCanCollect(ref v);
        }
    }

    /// <summary>
    /// kiểm tra user đã offline đủ để claim chưa
    /// </summary>
    /// <param name="timePassed">Thời gian còn lại của pass để show lên UI, in TotalSecond</param>
    /// <returns></returns>
    public bool IsCanCollect(ref double timePassed)
    {
        timePassed = this.TimeOfflinePassed;
        return timePassed >= TIME_MIN_ALLOW_COLLECT;
    }

    public void ClaimReward()
    {
        timeCollectLatest = DateTime.Now.ToFileTime();
        dtTimeCollectLatest = DateTime.FromFileTime(timeCollectLatest);

        SaveData();
    }
    public void ClaimRewardSkip()
    {
        isClaimSkip = true;
        SaveData();
    }
    public void ClearOfflineEarning()
    {
        isClaimSkip = false;

        SaveData();
    }
    private void SaveData()
    {
        GameDataManager.Instance.SaveUserData();
    }

    public List<object> GetPrizesByTime(double time, ref List<object> prizes)
    {
        if (prizes == null)
            prizes = new List<object>();

        long value = (long)(time * coinPerMinus);
        Debug.Log($"value coin {value} {time} {coinPerMinus}");
        object b = prizes.Find(x => x is BoosterCommodity && (x as BoosterCommodity).type == BoosterType.COIN);
        if (b != null && b != default)
            (b as BoosterCommodity).Set(value);
        else
            prizes.Add(new BoosterCommodity(BoosterType.COIN, value));


        value = (long)(time * cardPerMinus); //totalCard

        if (value <= 0)
        {
            prizes.RemoveRange(1, prizes.Count - 1);
            return prizes;
        }

        int totalLootCard = Mathf.FloorToInt(value / 1.22f); //1 for loot, 0.2 for rare, 0.02 for epic
        int totalRareCard = Mathf.FloorToInt(totalLootCard * 0.2f);
        int totalEpicCard = Mathf.FloorToInt(totalLootCard * 0.02f);

        List<KeyValuePair<StatManager.Tier, int>> lst = new List<KeyValuePair<StatManager.Tier, int>>();
        lst.Add(new KeyValuePair<StatManager.Tier, int>(StatManager.Tier.Standard, totalLootCard));
        lst.Add(new KeyValuePair<StatManager.Tier, int>(StatManager.Tier.Rare, totalRareCard));
        lst.Add(new KeyValuePair<StatManager.Tier, int>(StatManager.Tier.Legendary, totalEpicCard));

        for (int i = 0; i < lst.Count; i++)
        {
            b = prizes.Find(x => x is CardAmount && (x as CardAmount).tier == lst[i].Key);

            if (lst[i].Value > 0)
            {
                if (b != null)
                    (b as CardAmount).amount = lst[i].Value;
                else
                    prizes.Add(new CardAmount(lst[i].Key, lst[i].Value));
            }
            else
            {
                if (b != null)
                    prizes.Remove(b);
            }
        }

        return prizes;
    }
    public void UpdateListPrize(ref List<object> prizes)
    {
        prizes = GetPrizesByTime(TimeOfflineToMinus, ref prizes);
    }
}
#endregion Offline Bonus

#endregion Bonus Data
