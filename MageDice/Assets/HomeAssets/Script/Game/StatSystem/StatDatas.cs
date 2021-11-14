//using PoolOffline;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[System.Serializable]
public class StatDatas
{

    [SerializeField]
    public string currentStatId;

    public string CurrentCueId
    {
        get { return this.currentStatId; }
    }

    [SerializeField]
    public List<ListStatData> listsCue;

    [System.NonSerialized]
    private Dictionary<StatManager.Kind, ListStatData> dictCueDatas;

    public static StatDatas Instance
    {
        get
        {
            return GameDataManager.Instance.GameDatas.cueDatas;
        }

    }

    public void ParseDataFirstTime()
    {
        this.currentStatId = "default";//new List<DiceID>() { DiceID.FIRE, DiceID.ICE, DiceID.WIND, DiceID.ELECTRIC, DiceID.POISION };
        this.listsCue = new List<ListStatData>(3);

        dictCueDatas = new Dictionary<StatManager.Kind, ListStatData>(3);

        // 3 = hard code
        for (int i = 0; i < 3; ++i)
        {
            this.listsCue.Add(new ListStatData((StatManager.Kind)(1 << i)));
            this.listsCue[i].EnsureDict();

            this.dictCueDatas.Add(this.listsCue[i].kind, this.listsCue[i]);
        }

        this.dictCueDatas[StatManager.Kind.NotUnlocked].ImportConfigDataFirstTime();

        //ShopCueRef.Instance.Prepare();
    }


    public void ParseDatasNotFirstTime()
    {
        // TODO! mr K: need impletement something to check new cue (after updating)

        dictCueDatas = new Dictionary<StatManager.Kind, ListStatData>(3);
        for (int i = 0; i < 3; ++i)
        {
            this.listsCue[i].ParseDataNotFirstTime();
            this.dictCueDatas.Add(this.listsCue[i].kind, this.listsCue[i]);
        }

        //ShopCueRef.Instance.Prepare();
    }


    /// <summary>
    /// Select List Config Count
    /// </summary>
    private static int SLCCount(List<ShopStatConfig> x)
    { return x.Count; }


    internal List<StatData> GetListSimple(StatManager.Kind kind)
    {
        if (this.dictCueDatas.ContainsKey(kind))
        {
            return this.dictCueDatas[kind].datas;
        }
        return null;
    }

    internal List<StatData> GetListComplex(StatManager.Kind kind)
    {
        List<StatManager.Kind> kinds = StatManager.SplitKind(kind);
        if (kinds.Count == 0)
            return null;

        List<StatData> results = new List<StatData>();
        for (int i = kinds.Count - 1; i >= 0; --i)
        {
            results.AddRange(this.GetListSimple(kinds[i]));
        }
        if (results.Count == 0)
            return null;

        return results;
    }
    internal void UnlockCue(string id)
    {
        StatData cue = this.dictCueDatas[StatManager.Kind.NotUnlocked].GetCueData(id);
        if (cue == null)
        {
            Debug.LogError("CueDatas:BuyCue failed - cue is null");
            return;
        }

        this.UnlockCue(cue);
    }

    /// <summary>
    /// WARNING! This cue must be in "Not Unlocked" list
    /// </summary>
    internal void UnlockCue(StatData cue)
    {
        cue.level = 1;
        if (cue.IsMaxLevel)
        {
            this.dictCueDatas[StatManager.Kind.Maxed]
                .TakeCueFrom(this.dictCueDatas[StatManager.Kind.NotUnlocked], cue.id);
            cue.kind = StatManager.Kind.Maxed;
        }
        else
        {
            this.dictCueDatas[StatManager.Kind.UnlockedNonMaxed]
                .TakeCueFrom(this.dictCueDatas[StatManager.Kind.NotUnlocked], cue.id);
            cue.kind = StatManager.Kind.UnlockedNonMaxed;
        }
    }

    public int CountCueTier(StatManager.Kind kind, StatManager.Tier tier)
    {
        return this.GetListSimple(kind).Where(x => x.config.tier == tier).Count();
    }

    public int CountCueKind(StatManager.Kind kind)
    {
        List<StatData> l = this.GetListSimple(kind);
        return l == null ? 0 : l.Count();
    }

    internal void UpgradeCue(string id)
    {
        StatData cue = this.dictCueDatas[StatManager.Kind.UnlockedNonMaxed].GetCueData(id);
        if (cue == null)
        {
            Debug.LogError("CueDatas:UpgradeCue failed - cue is null");
            return;
        }
        cue.cards -= cue.RequirementCard;
        ++cue.level;
        if (cue.IsMaxLevel)
        {
            this.dictCueDatas[StatManager.Kind.Maxed]
                .TakeCueFrom(this.dictCueDatas[StatManager.Kind.UnlockedNonMaxed], id);
            cue.kind = StatManager.Kind.Maxed;
        }
    }



#if CHEAT || UNITY_EDITOR// TODO! this is CHEAT
    //internal void BuyAllCueTier(CueSystem.CueManager.Tier tier)
    //{
    //    ListCueData notUnlocked = this.dictCueDatas[CueManager.Kind.NotUnlocked];
    //    ListCueData notMaxed = this.dictCueDatas[CueManager.Kind.UnlockedNonMaxed];
    //    ListCueData maxed = this.dictCueDatas[CueManager.Kind.Maxed];

    //    int count = notUnlocked.datas.Count;
    //    CueData cue;
    //    for (int i = count - 1; i >= 0; --i)
    //    {
    //        cue = notUnlocked.datas[i];

    //        if (cue.config.tier != tier)
    //            continue;

    //        cue.level = 1;
    //        if (cue.IsMaxLevel)
    //        {
    //            maxed.TakeCueFrom(notUnlocked, cue.id);
    //            cue.kind = CueManager.Kind.Maxed;
    //        }
    //        else
    //        {
    //            notMaxed.TakeCueFrom(notUnlocked, cue.id);
    //            cue.kind = CueManager.Kind.UnlockedNonMaxed;
    //        }
    //    }
    //}
    //internal void BuyAllCue()
    //{
    //    ListCueData notUnlocked = this.dictCueDatas[CueManager.Kind.NotUnlocked];
    //    ListCueData notMaxed = this.dictCueDatas[CueManager.Kind.UnlockedNonMaxed];
    //    ListCueData maxed = this.dictCueDatas[CueManager.Kind.Maxed];

    //    int count = notUnlocked.datas.Count;
    //    CueData cue;
    //    for (int i = count - 1; i >= 0; --i)
    //    {
    //        cue = notUnlocked.datas[i];
    //        cue.level = 1;
    //        if (cue.IsMaxLevel)
    //        {
    //            maxed.TakeCueFrom(notUnlocked, cue.id);
    //            cue.kind = CueManager.Kind.Maxed;
    //        }
    //        else
    //        {
    //            notMaxed.TakeCueFrom(notUnlocked, cue.id);
    //            cue.kind = CueManager.Kind.UnlockedNonMaxed;
    //        }
    //    }
    //}

    //internal void ResetCue(CueData c)
    //{
    //    c.cards = 0;
    //    if (this.dictCueDatas[CueManager.Kind.NotUnlocked].CheckContainId(c.id))
    //    {
    //        return;
    //    }
    //    c.level = 0;
    //    c.kind = CueManager.Kind.NotUnlocked;

    //    ListCueData source = this.dictCueDatas[CueManager.Kind.UnlockedNonMaxed].CheckContainId(c.id)
    //        ? this.dictCueDatas[CueManager.Kind.UnlockedNonMaxed]
    //        : this.dictCueDatas[CueManager.Kind.Maxed];
    //    this.dictCueDatas[CueManager.Kind.NotUnlocked].TakeCueFrom(source, c.id);
    //}

    //internal void ResetAllCue()
    //{
    //    ListCueData notUnlocked = this.dictCueDatas[CueManager.Kind.NotUnlocked];
    //    ListCueData notMaxed = this.dictCueDatas[CueManager.Kind.UnlockedNonMaxed];
    //    ListCueData maxed = this.dictCueDatas[CueManager.Kind.Maxed];

    //    int count = notUnlocked.datas.Count;
    //    CueData cue;
    //    for (int i1 = count - 1; i1 >= 0; --i1)
    //    {
    //        cue = notUnlocked.datas[i1];
    //        cue.cards = 0;
    //    }
    //    count = notMaxed.datas.Count;
    //    for (int i2 = count - 1; i2 >= 0; --i2)
    //    {
    //        cue = notMaxed.datas[i2];
    //        cue.cards = 0;
    //        cue.level = 0;
    //        cue.kind = CueManager.Kind.NotUnlocked;

    //        notUnlocked.TakeCueFrom(notMaxed, cue.id);
    //    }
    //    count = maxed.datas.Count;
    //    for (int i3 = count - 1; i3 >= 0; --i3)
    //    {
    //        cue = maxed.datas[i3];
    //        cue.cards = 0;
    //        cue.level = 0;
    //        cue.kind = CueManager.Kind.NotUnlocked;

    //        notUnlocked.TakeCueFrom(maxed, cue.id);
    //    }
    //}

    //internal void UpgradeAllCueToMax()
    //{
    //    ListCueData notMaxed = this.dictCueDatas[CueManager.Kind.UnlockedNonMaxed];
    //    ListCueData maxed = this.dictCueDatas[CueManager.Kind.Maxed];

    //    int count = notMaxed.datas.Count;
    //    CueData cue;
    //    for (int i = count - 1; i >= 0; --i)
    //    {
    //        cue = notMaxed.datas[i];
    //        if (!(cue.config.statsPerLevels == null || cue.config.statsPerLevels.Length == 0))
    //        {
    //            cue.level = cue.config.statsPerLevels.Length;
    //        }
    //        maxed.TakeCueFrom(notMaxed, cue.id);
    //        cue.kind = CueManager.Kind.Maxed;
    //    }
    //}
#endif // CHEAT

    internal void ChangeCurrentCue(string id)
    {
        this.currentStatId = id;
    }

    internal StatData GetCurrentCue()
    {
        return this.GetCue(this.currentStatId);
    }

    internal StatData GetCue(string id)
    {
        StatData cueData = this.dictCueDatas[StatManager.Kind.NotUnlocked].GetCueData(id);
        if (cueData != null)
            return cueData;
        cueData = this.dictCueDatas[StatManager.Kind.UnlockedNonMaxed].GetCueData(id);
        if (cueData != null)
            return cueData;
        cueData = this.dictCueDatas[StatManager.Kind.Maxed].GetCueData(id);
        return cueData;
    }

    /// <summary>
    /// Return the cueData since <br></br>
    /// - unlocking the cue <br></br>
    /// - have enough card for upgrading
    /// </summary>
    internal StatData AddCard(string id, long count)
    {
        StatData cueData = this.GetCue(id);
        if (cueData == null)
        {
            Debug.LogError("Cue not found: " + id);
            return null;
        }
        return this.AddCard(cueData, count);
    }

    /// <summary>
    /// Return the cueData since <br></br>
    /// - unlocking the cue <br></br>
    /// - have enough card for upgrading
    /// </summary>
    internal StatData AddCard(StatData cueData, long count)
    {
        cueData.cards += count;

        long req = cueData.RequirementCard;

        if (cueData.level == 0)
        {
            if (req <= 0 || cueData.cards < req)
                return null;

            cueData.cards -= req;
            this.UnlockCue(cueData);
            return cueData;
        }

        return cueData.cards >= req ? cueData : null;
    }

    private void SaveData()
    {
        GameDataManager.Instance.SaveUserData();
    }
}


[System.Serializable]
public class ListStatData
{
    public StatManager.Kind kind;
    public List<StatData> datas;

    private Dictionary<string, StatData> dictDatas;

    public ListStatData(StatManager.Kind kind)
    {
        this.kind = kind;
        this.datas = new List<StatData>();
    }

    public bool CheckContainId(string id)
    {
        if (this.datas == null || this.datas.Count == 0)
            return false;

        var c = this.datas.FirstOrDefault(x => x.id == id);
        return c != null;
    }


    public void TakeCueFrom(ListStatData otherList, StatData data)
    {
        if (otherList.datas.Contains(data))
        {
            otherList.datas.Remove(data);
            otherList.dictDatas.Remove(data.id);

            if (!this.datas.Contains(data))
            {
                this.datas.Add(data);
                this.dictDatas.Add(data.id, data);
            }
        }
    }


    public StatData TakeCueFrom(ListStatData otherList, string id)
    {
        StatData data = otherList.GetCueData(id);
        if (data == null)
            return null;

        otherList.datas.Remove(data);
        otherList.dictDatas.Remove(data.id);

        if (!this.datas.Contains(data))
        {
            this.datas.Add(data);
            this.dictDatas.Add(data.id, data);
        }

        data.kind = this.kind;
        return data;
    }

    public void ImportConfigDataFirstTime()
    {
        List<ShopStatConfig> configs = ShopStatConfigs.Instance.GetConfigs();

        this.datas.Clear();
        this.dictDatas.Clear();
        this.datas.Capacity = configs.Count;
        for (int i = 0; i < configs.Count; ++i)
        {
            this.datas.Add(new StatData(configs[i].id));
            this.dictDatas.Add(configs[i].id, this.datas[i]);
            this.datas[i].config = configs[i];
            configs[i].cueData = this.datas[i];
        }
    }

    /// <summary>
    /// Without ensure dics
    /// </summary>
    public void ImportNewReleasedCuesRawly(List<string> ids)
    {
        if (ids == null || ids.Count == 0)
            return;
        List<ShopStatConfig> configs = ShopStatConfigs.Instance.GetConfigs(ids);
        for (int i = 0; i < configs.Count; ++i)
        {
            this.datas.Add(new StatData(configs[i].id));
        }
    }

    public void ParseDataNotFirstTime()
    {
        this.dictDatas = new Dictionary<string, StatData>();
        for (int i = 0; i < this.datas.Count; ++i)
        {
            this.dictDatas.Add(this.datas[i].id, this.datas[i]);
            this.datas[i].config = ShopStatConfigs.Instance.GetConfig(this.datas[i].id);
            this.datas[i].config.cueData = this.datas[i];
        }
    }

    public void EnsureDict()
    {
        this.dictDatas = new Dictionary<string, StatData>();
        for (int i = 0; i < this.datas.Count; ++i)
        {
            this.dictDatas.Add(this.datas[i].id, this.datas[i]);
        }
    }

    public StatData GetCueData(string id)
    {
        if (this.dictDatas.ContainsKey(id))
            return this.dictDatas[id];
        return null;
    }


    public override string ToString()
    {
        if (this.datas == null)
        {
            return string.Format("List CueData kind: {0}, count: {1}", this.kind, "null");
        }

        return string.Format("List CueData kind: {0}, count: {1}", this.kind, this.datas);
    }
}

[System.Serializable]
public class StatData
{
    public string id;
    [Tooltip("Level start from 1")]
    public int level;
    public long cards;

    public StatManager.Kind kind = StatManager.Kind.NotUnlocked;

    [System.NonSerialized]
    public ShopStatConfig config;

    public StatData(string id)
    {
        this.id = id;
        this.level = 0;
        this.kind = StatManager.Kind.NotUnlocked;
    }

    internal static StatData CreateForBot(StatData source, int level)
    {
        StatData result = new StatData(source.id)
        {
            id = source.id,
            config = source.config,
            kind = StatManager.Kind.Unlocked,
            level = level
        };

        return result;
    }


    public bool IsMaxLevel
    {
        get { return this.config.IsMaxLevel(this.level); }
    }

    public CueStats PreviousStats
    {
        get { return this.config.GetLevelStats(this.level - 1); }
    }
    public CueStats CurrentStats
    {
        get { return this.config.GetLevelStats(this.level); }
    }
    public CueStats NextStats
    {
        get { return this.config.GetLevelStats(this.level + 1); }
    }
    public CueStats FullStats
    {
        get { return this.config.GetFullStat(); }
    }

    public string UnlockText
    {
        get { return this.config.unlockText; }
    }

    public long UpgradePrice
    {
        get { return this.config.GetPriceNext(this.level); }
    }

    public long RequirementCard
    {
        get { return this.config.GetRequirementCardsNext(this.level); }
    }

    public long RequirementCardNext
    {
        get { return this.config.GetRequirementCardsNext(this.level + 1); }
    }

    public override string ToString()
    {
        return string.Format("CueData: {0}", this.id);
    }
}