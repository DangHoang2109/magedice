using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;


public class StatManager : MonoSingleton<StatManager>
{
    public static class Constant
    {
        public const int COUNT_SHOP_CARD_PICK_CUE_UNLOCKED_NON_MAX = 5;
        public const int COUNT_SHOP_CARD_PICK_CUE_NOT_UNLOCKED = 4;

        public const string CUE_NOT_ENOUGH_MONEY = "CUE_NOT_ENOUGH_MONEY";
        public const string CUE_NOT_ENOUGH_CASH = "CUE_NOT_ENOUGH_CASH";
        public const string CUE_CANT_BE_BOUGHT = "CUE_CANT_BE_BOUGHT";
        public const string CUE_MAX_LEVEL = "CUE_MAX_LEVEL";
        public const string CUE_NOT_ENOUGH_CARDS = "CUE_NOT_ENOUGH_CARDS";
        public const string CUE_UPGRADE_BY_OTHER_METHOD = "CUE_UPGRADE_BY_OTHER_METHOD";
        public const string CUE_CANT_UPGRADE = "CUE_CANT_UPGRADE";
    }

    #region enums
    public enum Tier
    {
        None = 0,
        Standard,
        Rare,
        Legendary,
        Event = 999,
    }

    /// <summary>
    /// Simple: Not unlocked, unlocked non maxed, maxed
    /// <para>
    /// The rest are complex, except None
    /// </para>
    /// </summary>
    public enum Kind
    {
        None = 0,
        NotUnlocked = 1,//              0001
        UnlockedNonMaxed = 1 << 1,//    0010
        Maxed = 1 << 2,//               0100

        NonMaxed = NotUnlocked | UnlockedNonMaxed,//    0011
        Unlocked = UnlockedNonMaxed | Maxed,//          0110
        All = NotUnlocked | UnlockedNonMaxed | Maxed//  0111
    }

    public enum UnlockType
    {
        None = 0,
        Coin = 1,
        Cash = 2,
        PlayLevel = 3,
        PlayChampion = 4
    }


    public enum SortType
    {
        None = 0,
        PriceUp = 1,
        CardsRequiredUp = 2,
        CardsHaveUp = 3,
        PriceDown = 4,
        CardsRequiredDown = 5,
        CardsHaveDown = 6,
        CardsRatioUp = 7,
        CardsRatioDown = 8,
    }
    #endregion enum

    #region events
    public event System.Action<StatData> OnCueChanged;
    public event System.Action<StatData> OnCueGained;
    public event System.Action<StatData> OnCueUpgraded;
    public event System.Action<StatData> OnCardBought;

    #endregion events

    #region variables

    private bool isHome;
    public bool IsHome => this.isHome;

    #endregion variables

    #region static methods
    public static bool CheckKind(Kind source, Kind check)
    {
        return (source & check) == source;// 0010 & 0110 == 0010 // 0100 & 0010 == 0000
    }

    public static List<Kind> SplitKind(Kind source, int level = 3)
    {
        List<Kind> results = new List<Kind>();

        int checker = 1;
        Kind tmp;

        for (int i = 0; i < level; ++i)
        {
            tmp = source & (Kind)(checker << i);
            if (tmp != Kind.None)
                results.Add(tmp);
        }
        return results;
    }

    public static string TierToString(Tier tier)
    {
        switch (tier)
        {
            case Tier.Standard:
                return "Standard";
            case Tier.Rare:
                return "Rare";
            case Tier.Legendary:
                return "Legendary";
            default:
                return "";
        }
    }
    #endregion static methods


    #region internal
    public override void Init()
    {
        base.Init();

        this.StartCoroutine(this.DelayInit());

    }

    private IEnumerator DelayInit()
    {
        yield return new WaitForEndOfFrame();
        yield return new WaitForEndOfFrame();

        GameManager.Instance.OnSceneBound += this.OnSceneBound;
    }

    private void OnSceneBound(BaseScene s)
    {
        this.isHome = s is HomeScene;
    }

    /// <summary>
    /// get him first cue | set that cue default
    /// </summary>
    public void NewUser()
    {
        StatDatas.Instance.UnlockCue(GameDefine.ID_CUE_DEFAULT);
        StatDatas.Instance.ChangeCurrentCue(GameDefine.ID_CUE_DEFAULT);
    }

    private StatData GetCueDataFromConfig(ShopStatConfig config)
    {
        return config.cueData;
    }

    private StatData _cueOfOBot;


    #endregion internal

    #region public method - get data

    public string GetRandomCardId(Tier tier, Kind kind)
    {
        StatData c = this.GetRandomCue(tier, kind);
        return c?.id;
    }

    public string GetRandomCardId(Tier tier, Kind kind, bool isIncludeHideCue = false)
    {
        List<StatData> cues = this.GetDatasByTierKind_Complex(tier, kind);
        if (!isIncludeHideCue)
            cues = cues.Where(x => !x.config.isHide).ToList();

        if ((cues == null) || (cues.Count == 0))
            return null;

        return cues.GetRandom()?.id;
    }

    private StatData GetRandomCue(Tier tier, Kind kind)
    {
        List<StatData> cues = this.GetDatasByTierKind_Complex(tier, kind);

        if ((cues == null) || (cues.Count == 0))
            return null;
        return cues.GetRandom();
    }

    public string GetRandomStatForPackage()
    {
        List<StatData> l = new List<StatData>();
        l = StatDatas.Instance.GetListSimple(Kind.NotUnlocked)
            .Where(x => x.config.isHide && x.config.tier == Tier.Legendary).ToList();

        if (l != null && l.Any())
        {
            StatData c = l.GetRandomSafe();
            if (c != null) return c.id;
        }

        StatData t = GetRandomCue(Tier.Legendary, Kind.NotUnlocked);
        if (t != null)
            return t.id;

        return null;
    }

    public StatData GetDataById(string id)
    {
        return ShopStatConfigs.Instance.GetConfig(id).cueData;
    }

    public List<StatData> GetDatasByTier(Tier tier)
    {
        return ShopStatConfigs.Instance.GetConfigsByTier(tier)
            .Select(GetCueDataFromConfig).ToList();
    }

    public List<StatData> GetDatasByTierKind_Simple(Tier tier, Kind kind)
    {
        List<StatData> l = StatDatas.Instance.GetListSimple(kind);
        if (l == null)
            return null;
        return l.Where(x => x.config.tier == tier).ToList();
    }

    /// <summary>
    /// Use like this:
    /// <para>
    /// GetDatasByTierKind(Tier.Standard, Kind.NotUnlocked, CueManager.FilterHaveRequireCards);
    /// </para>
    /// </summary>
    public List<StatData> GetDatasByTierKind_Simple(Tier tier, Kind kind, System.Func<StatData, bool> filter)
    {
        List<StatData> l = StatDatas.Instance.GetListSimple(kind);
        if (l == null)
            return null;
        return l.Where(x => (x.config.tier == tier && filter(x))).ToList();
    }

    public List<StatData> GetDatasByTierKind_Simple<T>(Tier tier, Kind kind, System.Func<StatData, T> orderer)
    {
        List<StatData> l = StatDatas.Instance.GetListSimple(kind);
        if (l == null)
            return null;
        return l.Where(x => x.config.tier == tier)?.OrderBy(orderer).ToList();
    }

    public IEnumerable<StatData> QueryDatasByTierKind_Simple(Tier tier, Kind kind)
    {
        List<StatData> l = StatDatas.Instance.GetListSimple(kind);
        if (l == null)
            return null;
        return l.Where(x => x.config.tier == tier);
    }

    public IEnumerable<StatData> QueryDatasNeedUpgradeWithCardsLeft_Complex(int maxCardNeed)
    {
        List<StatData> l = StatDatas.Instance.GetListComplex(Kind.NonMaxed);
        if (l == null)
            return null;
        return l.Where((x) =>
        {
            long countLeft = x.RequirementCard - x.cards;
            return countLeft > 0 && countLeft <= maxCardNeed;
        });
    }

    public StatData QueryCueForUnlockBonusCard(Tier tier, string showedIDs = "")
    {
        List<StatData> cues = GetDatasByTierKind_Simple(tier, Kind.NotUnlocked).Where(x => x.config.statsPerLevels[0].cardsRequired > 0).ToList();

        if (cues.Count > 0)
        {
            List<StatData> l = cues.OrderByDescending(x => x.config.rateRandomUnlock).ToList();
            float rand = Random.Range(0f, 1f);

            int index = 0;

            float totalValue = l[index].config.rateRandomUnlock;
            float rateValue = rand - totalValue;
            while (rateValue > 0 && index < l.Count - 1)
            {
                index += 1;

                totalValue += l[index].config.rateRandomUnlock;
                rateValue = rand - totalValue;
            }

            StatData c = l[index];
            if (!string.IsNullOrEmpty(showedIDs) && c.id.Equals(showedIDs))
            {
                if (index == 0)
                    index += 1;
                else
                    index -= 1;

                c = l[index];
            }

            return c;
        }
        else
        {
            return null;
        }

    }

    /// <summary>
    /// Pick cue follow along by shop config
    /// confgi contain rateOwn and Tier
    /// </summary>
    /// <returns></returns>
    public StatData QueryCuesForShopDeal_CardPart()
    {
        //check tier slot này
        Tier tier = Tier.Standard;

        //float get rate ra card mới hay card cũ
        StatData c = StatDatas.Instance.GetListSimple(Kind.NotUnlocked).Where(x => x.config.tier == tier && !x.config.isHide)
            .ToList().Shuffle().FirstOrDefault();

        if (c == null || c == default)
        {
            Debug.LogError("Cue null");
            c = StatDatas.Instance.GetListComplex(Kind.All).Where(x => x.config.tier == tier && !x.config.isHide)
                .ToList().Shuffle().FirstOrDefault();
        }

        return c;
    }

    public List<StatData> GetDatasByKind_Simple(Kind kind)
    {
        return StatDatas.Instance.GetListSimple(kind);
    }

    public List<StatData> GetDatasByTierKind_Complex(Tier tier, Kind kind)
    {
        List<StatData> l = StatDatas.Instance.GetListComplex(kind);
        if (l == null)
            return null;
        return l.Where(x => x.config.tier == tier).ToList();
    }

    public List<StatData> GetDatasByKind_Complex(Kind kind)
    {
        return StatDatas.Instance.GetListComplex(kind);
    }
    public List<StatData> GetDatasByKind_Complex(Kind kind, System.Func<StatData, bool> filter)
    {
        return StatDatas.Instance.GetListComplex(kind).Where(x => filter(x)).ToList();
    }

    /// <summary>
    /// WARNING! This function query data, so it will make cpu load heavily if called too many times!
    /// </summary>
    public int GetCountTierKind_Simple(Tier tier, Kind kind)
    {
        return this.GetDatasByTierKind_Simple(tier, kind)?.Count ?? 0;
    }


    /// <summary>
    /// WARNING! This function query data, so it will make cpu load heavily if called too many times!
    /// H add
    /// </summary>
    public int GetCountKind_Simple(Kind kind)
    {
        return this.GetDatasByKind_Simple(kind)?.Count ?? 0;
    }

    /// <summary>
    /// WARNING! This function query data, so it will make cpu load heavily if called too many times!
    /// </summary>
    public int GetCountTierKind_Complex(Tier tier, Kind kind)
    {
        return this.GetDatasByTierKind_Complex(tier, kind)?.Count ?? 0;
    }

    public string CurrentCueId
    {
        get { return StatDatas.Instance.CurrentCueId; }
    }

    /// <summary>
    /// take current cue's stats
    /// </summary>
    public StatItemStats CurrentCueStats
    {
        get
        {
            StatData currentCue = StatDatas.Instance.GetCurrentCue();
            if (currentCue == null)
            {
                currentCue = StatDatas.Instance.GetCue(GameDefine.ID_CUE_DEFAULT);

                if (currentCue == null)
                {
                    UnityEngine.Debug.LogError("CueManager.GetCurrentCueStats - Cue default ERROR");
                    return StatItemStats.CreateBasic();
                }
            }

            StatItemStats results = StatItemStats.CreateForRealUsing(
                currentCue.CurrentStats, currentCue.config);
            return results;
        }
    }

    #endregion public method - get data

    #region public method - check data
    public bool CheckCanBuy(StatData cue, out string reason)
    {

        switch (cue.config.unlockType)
        {
            case UnlockType.Coin:
                if (!UserProfile.Instance.IsCanUseBooster(BoosterType.COIN, cue.UpgradePrice))
                {
                    reason = Constant.CUE_NOT_ENOUGH_MONEY;
                    return false;
                }
                reason = string.Empty;
                return true;
            case UnlockType.Cash:
                if (!UserProfile.Instance.IsCanUseBooster(BoosterType.CASH, cue.UpgradePrice))
                {
                    reason = Constant.CUE_NOT_ENOUGH_CASH;
                    return false;
                }
                reason = string.Empty;
                return true;
            default:
                reason = Constant.CUE_CANT_BE_BOUGHT;
                return false;
        }
    }

    public bool CheckCanUpgrade(StatData cue, out string reason)
    {

        if (cue.IsMaxLevel)
        {
            reason = Constant.CUE_MAX_LEVEL;
            return false;
        }

        if (cue.cards < cue.RequirementCard)
        {
            reason = Constant.CUE_NOT_ENOUGH_CARDS;
            return false;
        }

        switch (cue.config.upgradeType)
        {
            case UnlockType.Coin:
                if (!UserProfile.Instance.IsCanUseBooster(BoosterType.COIN, cue.UpgradePrice))
                {
                    reason = Constant.CUE_NOT_ENOUGH_MONEY;
                    return false;
                }
                break;
            case UnlockType.Cash:
                if (!UserProfile.Instance.IsCanUseBooster(BoosterType.CASH, cue.UpgradePrice))
                {
                    reason = Constant.CUE_NOT_ENOUGH_CASH;
                    return false;
                }
                break;
            case UnlockType.PlayLevel:
            case UnlockType.PlayChampion:
                reason = Constant.CUE_UPGRADE_BY_OTHER_METHOD;
                return false;
            default:
                reason = Constant.CUE_CANT_UPGRADE;
                return false;
        }

        reason = string.Empty;
        return true;
    }

    public static bool FilterHaveRequireCards(StatData c)
    {
        return c.RequirementCard != 0;
    }
    public static bool FilterHaveCards(StatData c)
    {
        return c.cards != 0;
    }
    public static bool FilterNotHaveCards(StatData c)
    {
        return c.cards == 0;
    }

    public static bool FilterNotMaxed(StatData c)
    {
        return !c.IsMaxLevel;
    }

    private static Tier s_tempTier;
    /// <summary>
    /// set value for CueManager.s_tempTier
    /// </summary>
    public static bool FilterStaticTier(StatData c)
    {
        return c.config.tier == s_tempTier;
    }
    /// <summary>
    /// set value for CueManager.s_tempTier
    /// </summary>
    public static bool FilterNotStaticTier(StatData c)
    {
        return c.config.tier != s_tempTier;
    }

    /// <summary>
    /// select price video
    /// </summary>
    /// <param name="c"></param>
    /// <returns></returns>
    public static long PriceSelector(StatData c)
    {
        return c.UpgradePrice;
    }

    public static long CardsRequiredSelector(StatData c)
    {
        return c.RequirementCard;
    }

    public static long CardsHaveSelector(StatData c)
    {
        return c.cards;
    }

    public static float CardsRatioSelector(StatData c)
    {
        if (c.RequirementCard == 0)
        {
            return float.MaxValue;
        }

        return (float)c.cards / (float)c.RequirementCard;
    }
    public static float CardsExceedRatioSelector(StatData c)
    {
        if (c.RequirementCard == 0)
        {
            return 50;
        }

        return Mathf.Abs(c.cards - c.RequirementCard);
    }
    public static float CoinExceedRatioSelector(StatData c, long userMoney)
    {
        return Mathf.Abs(userMoney - c.UpgradePrice);
    }

    public static string IDSelector(StatData c)
    {
        return c.id;
    }

    public static string Name3Selector(StatData c)
    {
        return c.config.statName.Substring(0, 3);
    }

    public static int CueTierSelector(StatData c)
    {
        switch (c.config.tier)
        {
            case Tier.Legendary:
                return 100;
            case Tier.Rare:
                return 50;
            case Tier.Standard:
                return 10;
            default:
                return 0;
        }
    }

    public static long CueValueSelector(StatData c)
    {
        long v = c.config.GetPriceNext(1);

        switch (c.config.tier)
        {
            case Tier.Legendary:
                v *= 100;
                break;
            case Tier.Rare:
                v *= 20;
                break;
        }
        return v;
    }

    public static IEnumerable<StatData> SortDataComplex(IEnumerable<StatData> source, params SortType[] sortTypes)
    {
        IEnumerable<StatData> q = source;

        if (sortTypes != null && sortTypes.Length != 0)
        {
            for (int i = sortTypes.Length - 1; i >= 0; --i)
            {
                switch (sortTypes[i])
                {
                    case StatManager.SortType.PriceUp:
                        q = q.OrderBy(StatManager.PriceSelector);
                        break;
                    case StatManager.SortType.CardsRequiredUp:
                        q = q.OrderBy(StatManager.CardsRequiredSelector);
                        break;
                    case StatManager.SortType.CardsHaveUp:
                        q = q.OrderBy(StatManager.CardsHaveSelector);
                        break;
                    case StatManager.SortType.PriceDown:
                        q = q.OrderByDescending(StatManager.PriceSelector);
                        break;
                    case StatManager.SortType.CardsRequiredDown:
                        q = q.OrderByDescending(StatManager.CardsRequiredSelector);
                        break;
                    case StatManager.SortType.CardsHaveDown:
                        q = q.OrderByDescending(StatManager.CardsHaveSelector);
                        break;
                    case StatManager.SortType.CardsRatioUp:
                        q = q.OrderBy(StatManager.CardsRatioSelector);
                        break;
                    case StatManager.SortType.CardsRatioDown:
                        q = q.OrderByDescending(StatManager.CardsRatioSelector);
                        break;
                    default:
                        break;
                }
            }
        }
        return q;
    }

    #endregion public method - check data

    #region public method - change data

    /// <summary>
    /// buy cue with availability and money <br></br>
    /// the string passed in the callback is reason for the result
    /// </summary>
    public void BuyCue(StatData cue, System.Action<string> onSuccess = null, System.Action<string> onFail = null)
    {
        bool needProcFail = true;

        if (!this.CheckCanBuy(cue, out string reason))
        {
            switch (reason)
            {
                case Constant.CUE_NOT_ENOUGH_MONEY:
                    needProcFail = false;
                    NeedMoreCoinDialogs dM =
                        GameManager.Instance.OnShowDialogWithSorting<NeedMoreCoinDialogs>(
                            "Home/GUI/Dialogs/NeedMoreCoin/NeedMoreCoinDialog",
                            PopupSortingType.CenterBottomAndTopBar);
                    dM?.ParseData(cue.UpgradePrice
                             - UserBoosters.Instance.GetBoosterCommodity(BoosterType.COIN).GetValue(),
                        string.Format("BuyCue_{0}", cue.id),
                        () =>
                        {
                            this.BuyCue(cue, onSuccess, onFail);
                        });
                    break;
                case Constant.CUE_NOT_ENOUGH_CASH:
                    NeedMoreGemDialog dG =
                        GameManager.Instance.OnShowDialogWithSorting<NeedMoreGemDialog>(
                            "Home/GUI/Dialogs/NeedMoreGem/NeedMoreGemDialog",
                            PopupSortingType.CenterBottomAndTopBar);
                    dG?.ParseData(new BoosterCommodity(BoosterType.CASH, cue.UpgradePrice
                            - UserBoosters.Instance.GetBoosterCommodity(BoosterType.CASH).GetValue()));
                    break;
            }

            if (needProcFail)
                onFail?.Invoke(reason);
            return;
        }

        switch (cue.config.unlockType)
        {
            case UnlockType.Coin:
                UserProfile.Instance.UseBooster(BoosterType.COIN, cue.UpgradePrice,
                    $"Shop_Cue_{cue.id}", LogSinkWhere.SHOP_BUY_CUE);
                break;
            case UnlockType.Cash:
                UserProfile.Instance.UseBooster(BoosterType.CASH, cue.UpgradePrice,
                    $"Shop_Cue_{cue.id}", LogSinkWhere.SHOP_BUY_CUE);
                break;
        }

        StatDatas.Instance.UnlockCue(cue);
        this.OnCueGained?.Invoke(cue);
        onSuccess?.Invoke(string.Empty);
        GameDataManager.Instance.SaveUserData();
    }


    /// <summary>
    /// upgrade cue with cards and money
    /// </summary>
    public void UpgradeCue(StatData cue, System.Action<string> onSuccess = null, System.Action<string> onFail = null)
    {
        bool needProcFail = true;
        if (!this.CheckCanUpgrade(cue, out string reason))
        {
            switch (reason)
            {
                case Constant.CUE_NOT_ENOUGH_CARDS:
                    long cardLack = cue.RequirementCard - cue.cards;
                    Notification.Instance.ShowNotification(string.Format(LanguageManager.GetString("CUE_NEEDMORECARD", LanguageCategory.Feature), cardLack));

                    if (this.isHome)
                    {
                        HomeTabs.Instance.MoveToTab(HomeTabName.STORE);
                    }
                    break;
                case Constant.CUE_NOT_ENOUGH_MONEY:
                    needProcFail = false;
                    NeedMoreCoinDialogs dM =
                        GameManager.Instance.OnShowDialogWithSorting<NeedMoreCoinDialogs>(
                            "Home/GUI/Dialogs/NeedMoreCoin/NeedMoreCoinDialog",
                            PopupSortingType.CenterBottomAndTopBar);
                    dM?.ParseData(cue.UpgradePrice
                        - UserBoosters.Instance.GetBoosterCommodity(BoosterType.COIN).GetValue(),
                        string.Format("UpgradeCue_{0}", cue.id),
                        () =>
                        {
                            this.UpgradeCue(cue, onSuccess, onFail);
                        });
                    break;
                case Constant.CUE_NOT_ENOUGH_CASH:
                    NeedMoreGemDialog dG =
                        GameManager.Instance.OnShowDialogWithSorting<NeedMoreGemDialog>(
                            "Home/GUI/Dialogs/NeedMoreGem/NeedMoreGemDialog",
                            PopupSortingType.CenterBottomAndTopBar);
                    dG?.ParseData(new BoosterCommodity(BoosterType.CASH, cue.UpgradePrice
                         - UserBoosters.Instance.GetBoosterCommodity(BoosterType.CASH).GetValue()));
                    break;
            }

            if (needProcFail)
                onFail?.Invoke(reason);
            return;
        }

        switch (cue.config.upgradeType)
        {
            case UnlockType.Coin:
                UserProfile.Instance.UseBooster(BoosterType.COIN, cue.UpgradePrice,
                    $"Upgrade_Cue_{cue.id}", LogSinkWhere.SHOP_BUY_CUE);
                break;
            case UnlockType.Cash:
                UserProfile.Instance.UseBooster(BoosterType.CASH, cue.UpgradePrice,
                    $"Upgrade_Cue_{cue.id}", LogSinkWhere.SHOP_BUY_CUE);
                break;
        }
        StatDatas.Instance.UpgradeCue(cue.id);
        onSuccess?.Invoke(string.Empty);
        this.OnCueUpgraded?.Invoke(cue);
        GameDataManager.Instance.SaveUserData();

        //AchievementDatas.Instance.DoStep(MissionID.USE_CARD, cue.RequirementCard);
        LogGameAnalytics.Instance.LogEvent(LogAnalyticsEvent.COMPLETE_UPGRADE_STAT_ITEM, LogParams.STAT_ITEM_ID, cue.id);
    }

    /// <summary>
    /// switch to use this cue
    /// </summary>
    public void ChangeCue(StatData cue, System.Action<string> onSuccess = null, System.Action<string> onFail = null)
    {
        StatDatas.Instance.ChangeCurrentCue(cue.id);
        onSuccess?.Invoke(string.Empty);
        GameDataManager.Instance.SaveUserData();

        this.OnCueChanged?.Invoke(cue);
    }

    /// <summary>
    /// suddenly gain a card
    /// </summary>
    public void AddCard(StatData cueData, long count, System.Action<StatData> callbackUnlockedCue = null)
    {
        int oldLvl = cueData.level;

        StatData c = StatDatas.Instance.AddCard(cueData, count);
        this.OnCardBought?.Invoke(cueData);

        // bought
        if (oldLvl == 0 && c != null && c.level > oldLvl)
        {
            this.OnCueGained?.Invoke(c);

            LogGameAnalytics.Instance.LogEvent(LogAnalyticsEvent.CUE_UNLOCKED, LogParams.STAT_ITEM_ID, c.id); //unlock khi đủ card
        }

        GameDataManager.Instance.SaveUserData();
    }

    /// <summary>
    /// suddenly gain a cue
    /// </summary>
    public void WinCue(StatData c)
    {
        StatDatas.Instance.UnlockCue(c);
        this.OnCueGained?.Invoke(c);
        GameDataManager.Instance.SaveUserData();
    }
    #endregion public method - change data
}