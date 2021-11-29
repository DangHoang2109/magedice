using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cosina.Components;
using System.Linq;
[System.Serializable]
public class UserDatas
{
    public UserInfo info;
    public UserCareers careers;
    public UserLanguageData languages;
    //public UserWheelData wheel;
    public PerkDatas perkDatas;

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
            
        //this.wheel = new UserWheelData();
    }
   public void CreateUser()
   {
        languages.NewUser();
        perkDatas.CreateUser();
        //wheel.NewDay();
   }

    public void OpenGame()
    {
        if(perkDatas == null || perkDatas.data == null || perkDatas.data.Count == 0)
            perkDatas.CreateUser();
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