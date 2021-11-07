using System.Collections;
using System.Collections.Generic;
using Cosinas.Firebase;
using UnityEngine;

public class LogGameAnalytics : MonoSingleton<LogGameAnalytics>
{
    public void SetUserProperty(string propertyName, string userId)
    {
        CFirebaseManager.Instance.SetUserProperty(propertyName, userId);
    }

    public void SetUserPropertyAndValue(string propertyName, string propertyValue)
    {
#if UNITY_EDITOR
        Debug.Log($"Set firebase property {propertyName} with value {propertyValue}");
#endif
        CFirebaseManager.Instance.SetUserProperty(propertyName, propertyValue);
    }

    public void LogEvent(string name, string parameterName, long parameterValue)
    {
        CFirebaseManager.Instance.LogEvent(name, parameterName, parameterValue);
    }
    public void LogEvent(string name, string parameterName, int parameterValue)
    {
        CFirebaseManager.Instance.LogEvent(name, parameterName, parameterValue);
#if UNITY_EDITOR
        Debug.Log($"Log firebase event {name}, param {parameterName} with value {parameterValue}");
#endif
    }
    public void LogEvent(string name)
    {
        CFirebaseManager.Instance.LogEvent(name);
#if UNITY_EDITOR
        Debug.Log($"Log firebase event {name}");
#endif
    }
    public void LogEvent(string name, string parameterName, string parameterValue)
    {
        CFirebaseManager.Instance.LogEvent(name, parameterName, parameterValue);
#if UNITY_EDITOR
        Debug.Log($"Log firebase event {name}, param {parameterName} with value {parameterValue}");
#endif
    }
    public void LogEvent(string name, string parameterName, double parameterValue)
    {
        CFirebaseManager.Instance.LogEvent(name, parameterName, parameterValue);
       // Debug.Log($"Log firebase event {name}, param {parameterName} with value {parameterValue}");

    }

    public void LogEvent(string nameEvent, Dictionary<string, string> values)
    { 
        CFirebaseManager.Instance.LogEvent(name, values);
    }

    public void LogEvent(string nameEvent, Dictionary<string, object> values)
    {
        CFirebaseManager.Instance.LogEvent(name, values);
    }
        
    public void AnalyticsLevelUp(int level, string parameterCharacter)
    {
        CFirebaseManager.Instance.AnalyticsLevelUp(level, parameterCharacter);
    }
}

public static class LogAnalyticsEvent
{
    /// <summary>
    /// Tutorial
    /// </summary>
    /// 
    public const string FIRSTGAME_SHOT_TIMES = "FIRSTGAME_SHOT_TIMES_{0}"; //x
    public const string FIRSTGAME_SHOT_TIMES_RESULT = "FIRSTGAME_SHOT_TIMES_RESULT_{0}_{1}"; //x

    //Tạo 1 phễu Từng step tutorial >> Complete Tutorial xem bao nhiêu % user mất đi khi trải nghiệm tutorial
    public const string TUTORIAL_STEP_ACCEPTTERM = "TUTORIAL_STEP_ACCEPTTERM"; //no use
    public const string TUTORIAL_STEP_TUTDRAGCUE = "TUTORIAL_STEP_TUTDRAGCUE"; //x
    public const string TUTORIAL_STEP_TUTPOTTEDBALL = "TUTORIAL_STEP_TUTPOTTEDBALL"; //x
    public const string TUTORIAL_STEP_DONESCENETUT = "TUTORIAL_STEP_DONESCENETUT"; //x

    public const string TUTORIAL_STEP_GAME_TEACHBOOSTAIM = "TUTORIAL_STEP_GAME_TEACHBOOSTAIM"; //x
    public const string TUTORIAL_STEP_BREAKING = "TUTORIAL_STEP_BREAKING"; //x
    public const string TUTORIAL_STEP_GAME_TEACHTARGETBALL = "TUTORIAL_STEP_GAME_TEACHTARGETBALL"; //x

    public const string TUTORIAL_STEP_DONEFIRSTGAME = "TUTORIAL_STEP_WINFIRSTGAME"; //x

    public const string TUTORIAL_STEP_TUTBOX = "TUTORIAL_STEP_TUTBOX"; //x
    public const string TUTORIAL_STEP_DONE = "TUTORIAL_STEP_DONE"; //x

    public const string FIRSTGAMERESULT_WIN = "FIRSTGAMERESULT_WIN"; //x
    public const string FIRSTGAMERESULT_LOSE = "FIRSTGAMERESULT_LOSE";//x

    /// <summary>
    /// engage progression
    /// </summary>

    //Tạo 1 phễu Click PlayGame >> Complete Game xem bao nhiêu % user tắt app khi đang chơi
    public const string PLAY_GAME = "PLAY_GAME"; //x
    public const string COMPLETE_GAME = "COMPLETE_GAME"; //x

    //Tạo 1 phễu Play game thứ bao nhiêu. Dùng cho 3 funnel sau
    /// TÍnh lượng game trung bình 1 ngày : Dựa theo property TotalMatchesCompleted/TotalSessionsStarted
    /// Tính lượng session trung bình trong N ngày: TotalSessionsStarted / TotalDayaSinceInstal
    /// TỪ median trên, chia event này vào 3 funnel:
    /// D1: log play game từ game 1 => game median
    /// D2: log play game từ game median => game median x 2 + 1
    /// D1 to D7: log play game từ game 1 => game median x 7 + 1
    public const string PLAY_GAME_TIMES = "PLAY_GAME_TIMES_{0}"; //x
    public const string COMPLETE_GAME_TIMES = "COMPLETE_GAME_TIMES_{0}"; //x

    //Tạo 1 phễu Click OPEN_SHOP >> CLICK_SHOP_ITEM >> COMPLETE_BUY_SHOP_ITEM: xem bao nhiêu churn user khi lựa chọn mua hàng
    public const string CLICK_SHOP_ITEM = "CLICK_SHOP_ITEM"; //x
    public const string COMPLETE_BUY_SHOP_ITEM = "COMPLETE_BUY_SHOP_ITEM"; //x

    //Tạo 1 phễu Click CLICK_STAT_ITEM >> CLICK_UPGRADE_STAT_ITEM >> COMPLETE_UPGRADE_STAT_ITEM: xem bao nhiêu churn user khi lựa chọn upgrade
    public const string CLICK_UPGRADE_STAT_ITEM = "CLICK_UPGRADE_STAT_ITEM"; //x
    public const string COMPLETE_UPGRADE_STAT_ITEM = "COMPLETE_UPGRADE_STAT_ITEM"; //x

    //Tạo 1 phễu Click CLICK_BAG_INFO >> CLICK_SPEEDUP_BAGINFO >> COMPLETE_SPEEDUP_BAGINFO: xem churn user khi lựa chọn soeed up => đưa các gói perk cho pay user
    public const string CLICK_BAG_INFO = "CLICK_BAG_INFO";
    public const string CLICK_SPEEDUP_BAGINFO = "CLICK_SPEEDUP_BAGINFO";
    public const string COMPLETE_SPEEDUP_BAGINFO = "COMPLETE_SPEEDUP_BAGINFO";

    //Tạo 1 phễu Click SHOW_FULL_BAGINFO >> CLICK_OPEN_FULL_BAGINFO >> COMPLETE_OPEN_FULL_BAGINFO: xem churn user khi thấy bag lúc full slot và mở nó ngay trong endgame
    public const string SHOW_FULL_BAGINFO = "SHOW_FULL_BAGINFO";
    public const string CLICK_OPEN_FULL_BAGINFO = "CLICK_OPEN_FULL_BAGINFO";
    public const string COMPLETE_OPEN_FULL_BAGINFO = "COMPLETE_OPEN_FULL_BAGINFO";

    //Tạo 1 phễu Unlock Room Id 1 => 8, thống kê churn user ở từng giai đoạn mở room. Dùng string format với ID room được unlock
    public const string UNLOCK_ROOM_WITH_ID = "UNLOCK_ROOM_{0}";//x

    //Tạo 1 phễu Complete Battle Pass FREE, thống kê lượng user churn qua từng step pass
    public const string COMPLETE_PASS_STEP_ID = "COMPLETE_PASS_STEP_{0}"; //x

    public const string CLAIM_SHOP_FREE = "CLAIM_SHOP_FREE_{0}"; //x

    public const string BUYPROPASS = "BUYPROPASS"; //x
    public const string BUYFULLPROPASS = "BUYFULLPROPASS";
    public const string UPGRADEFULLPROPASS = "UPGRADEFULLPROPASS"; //UPGRADE LÊN TỪ BẢN PRO THƯỜNG //x

    public const string VIDEO_REWARD = "WATCH_ADS";

    public const string QUICKFIRE_FREE = "QUICKFIRE_FREE"; //x
    public const string QUICKFIRE_ADS = "QUICKFIRE_ADS";
    public const string QUICKFIRE_GEM = "QUICKFIRE_GEM";
    public const string QUICKFIRE_IAP = "QUICKFIRE_IAP";

    public const string CLAIM_QUICKFIRE_REWARD = "CLAIM_QUICKFIRE_REWARD_{0}";


    public const string WHELL_SPIN_FREE = "WHELL_SPIN_FREE"; //x
    public const string WHELL_SPIN_ADS = "WHELL_SPIN_ADS";
    public const string WHELL_SPIN_GEM = "WHELL_SPIN_GEM";
    public const string WHELL_SPIN_IAP = "WHELL_SPIN_IAP";

    public const string TOURNAMENT_TRYAGAIN = "TOURNAMENT_TRYAGAIN_{0}"; 
    public const string TOURNAMENT_COMPLETE_FRACTION = "TOURNAMENT_COMPLETE_FRACTION_{0}"; //include win and lose
    public const string TOURNAMENT_WIN_FRACTION = "TOURNAMENT_WIN_FRACTION_{0}"; //only count winz
    public const string TOURNAMENT_PICK_LEAGUE = "TOURNAMENT_PICK_LEAGUE_{0}";

    public const string CUE_BOUGHT = "CUE_BOUGHT"; //Chủ động mua cue //x
    public const string CUE_UNLOCKED = "CUE_UNLOCKED"; //Cue được unlock khi đủ card hoặc bag //x
    public const string CUE_SKIP_UNLOCKED = "CUE_SKIP_UNLOCKED"; //Cue được unlock khi chưa đủ card //x
    public const string CUE_UPGRADE = "CUE_UPGRADE"; //Cue được upgrade

    public const string CLAIM_GLOVE = "CLAIM_GLOVE_{0}_{1}"; //Nhận glove tour {0} lần thứ {1}

    public const string CLAIM_WINSTEAK = "CLAIM_WINSTEAK_{0}";
}

public static class LogParams
{

    /// <summary>
    /// engage progression
    /// </summary>

    public const string GAME_RESULT = "GAME_RESULT";

    public const string ROOM_ID = "ROOM_ID";

    public const string SHOP_ITEM_ID = "SHOP_ITEM_ID";

    public const string STAT_ITEM_ID = "STAT_ITEM_ID";

    public const string BAG_TYPE = "BAG_TYPE";

    public const string BATTLEPASS = "BATTLEPASS_ID";

    public const string TUTORIAL_SHOT_FAIL_TIME = "TUTORIAL_SHOT_FAIL_TIME";

}

public static class LogPropertyEvent
{
    /// <summary>
    /// User Basic Info
    /// </summary>
    public const string TotalDayaSinceInstal = "total_days_since_install"; //x
    public const string CurrentFBConnected = "current_facebook_connected";

    /// <summary>
    /// Aggregate Engagement
    /// </summary>
    public const string TotalSessionsStarted = "total_sessions_started"; //x

    public const string TotalMatchesStarted = "total_matches_started"; //x
    public const string TotalMatchesCompleted = "total_matches_completed"; //x

    public const string TotalItemCollected = "total_item_collected"; //x
    public const string TotalItemUpgrade = "total_item_upgrade"; //no use
    public const string TotalItemUpgradeEvent = "total_item_upgrade_events";//x
    public const string TotalItemUnlocked = "total_itemtype_unlocked"; //no use


    /// <summary>
    /// Game Progression
    /// </summary>
    public const string CurrentPlayerLevel = "current_player_level"; //No Use in Tennis
    public const string CurrentTrophies = "current_trophy_count"; //x

    public const string CurrentMaxRoom = "current_max_room"; //x
    public const string CurrentPlayRoom = "current_play_room"; //no use in tennis

    public const string MaxTrophyAchieve = "highest_trophy_count_achieved"; //x
    public const string CurrentPlayerWinRate = "current_player_winrate"; //x


    /// <summary>
    /// Value Generation
    /// </summary>
    public const string TotalIAPSpend = "total_IAP_spend";
    public const string AvgPurchasePrice = "avg_purchase_price";
    public const string MaxPurchasePrice = "highest_purchase_price";
    public const string LastTransaction = "last_transactions";
    public const string TotalTransaction = "total_transactions";


    public const string TotalGemSpend = "total_gem_spend";
    public const string AvgGemPrice = "avg_gem_price";
    public const string MaxGemPrice = "highest_gem_price";
    public const string LastGemTransaction = "last_gem_transactions";
    public const string TotalGemTransaction = "total_gem_transactions";


    public const string TotalRewardAdsWatch = "total_reward_ads_watch"; //x
    public const string LastRewardAdsWatch = "last_ads_watch"; //x

    public const string AvgAdsPerDay = "avg_Ads_perday"; //x

    /// <summary>
    /// Resources Snapshot
    /// </summary>
    public const string CurrentGold = "current_wallet_gold"; //x
    public const string CurrentGem = "current_wallet_gems"; //x

    public const string SessionFirstWatchAds = "session_first_watchads"; //x
    public const string SessionFirstBuyIAP = "Session_First_Buy_IAP"; //x
    public const string SessionFirstConvertedSucess = "Session_First_Converted_Sucess"; //x
    public const string SessionFirstEngagingSucess = "Session_First_Engaging_Sucess"; //x

}

public static class LogAnalyticsParams
{
    ///Params RoomID with format
    ///ROOM_{ID}
}
