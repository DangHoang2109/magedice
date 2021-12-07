using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using System.Linq;

#if UNITY_EDITOR
public class TestMission
{
    public static MissionDatas Missions = MissionDatas.Instance;

    [UnityEditor.MenuItem("Test/Mission/Show all mission")]
    public static void ShowAllMission()
    {
        foreach (MissionData d in MissionDatas.Instance.missions)
        {
            Debug.Log(string.Format("Has mission name {0}, current step {1}, total {2}",
                d.name,
                d.step, d.totalStep));
        }


        PointbagMissionData p = MissionDatas.Instance.CurrentPointBag;
        if (p != null)
            Debug.Log(string.Format("Current point bag step {0} in total {1}. {2} another bag available",
                p.step,
                p.totalStep,
                MissionDatas.Instance.pointBagMissions.Count - 1));
        else
            Debug.Log("There is no point bag, wait");
    }

    [UnityEditor.MenuItem("Test/Mission/GenNewMission")]
    public static void GenNewMission()
    {
        MissionDatas.Instance.NewDay();

        string r = "";
        foreach (MissionData d in MissionDatas.Instance.missions)
        {
            r = string.Format("{0} Id: {1} Name {2}", r,
                d.id, d.GetName());
        }

        Debug.Log(r);

        PointbagMissionData p = MissionDatas.Instance.CurrentPointBag;
        Debug.Log(string.Format("Current point bag step {0} in total {1}. {2} another bag available",
            p.step,
            p.totalStep,
            MissionDatas.Instance.pointBagMissions.Count - 1));
    }

    [UnityEditor.MenuItem("Test/Mission/Do All Mission 20%")]
    public static void DoAllMission20Percent()
    {
        foreach (MissionData d in MissionDatas.Instance.missions)
        {
            d.DoStep((long)Mathf.Clamp(d.totalStep * 0.2f, 1, d.totalStep * 0.2f));
        }
    }

    [UnityEditor.MenuItem("Test/Mission/Do Point Bag Max Tour")]
    public static void DoPointBagMaxTour()
    {
        MissionDatas.Instance.DoPointbagStep(RoomDatas.Instance.GetRoomUnlockedMax(), 7);

        for (int i = 0; i < MissionDatas.Instance.pointBagMissions.Count; i++)
        {
            Debug.Log(string.Format("Point bag index {0} has step {1}, status {2}",
                i,
                MissionDatas.Instance.pointBagMissions[i].step,
                MissionDatas.Instance.pointBagMissions[i].status));
        }
    }

    [UnityEditor.MenuItem("Test/Mission/Add40Point")]
    public static void Add40Point()
    {
        MissionDatas.Instance.DoPointbagStep(RoomDatas.Instance.GetRoomUnlockedMax(), 40);


        for (int i = 0; i < MissionDatas.Instance.pointBagMissions.Count; i++)
        {
            Debug.Log(string.Format("Point bag index {0} has step {1}, status",
                i,
                MissionDatas.Instance.pointBagMissions[i].step,
                MissionDatas.Instance.pointBagMissions[i].status));
        }
    }
}
#endif

[System.Serializable]
public class MissionDatas
{
    public List<MissionData> missions;
    public List<PointbagMissionData> pointBagMissions;

    public BattlepassDatas battlepass;

    private CallbackEventObject callbacks;
    public System.Action callbackCompleteMission;

    protected bool isNewUser;

    public static MissionDatas Instance
    {
        get
        {
#if UNITY_EDITOR
            if (GameManager.isApplicationQuit)
                return default;
#endif
            return GameDataManager.Instance.GameDatas.missionDatas;
        }
    }

    public PointbagMissionData CurrentPointBag
    {
        get
        {
            if (this.pointBagMissions != null && this.pointBagMissions.Count > 0)
                return this.pointBagMissions.Find(x => x.status == MissionData.MissionStatus.DOING);

            return null;
        }

        set
        {
            if (this.pointBagMissions != null && this.pointBagMissions.Count > 0)
                this.pointBagMissions[0] = value;
        }
    }

    public MissionDatas()
    {
        this.missions = new List<MissionData>();
        this.pointBagMissions = new List<PointbagMissionData>();
        this.battlepass = new BattlepassDatas();
    }

    public void OpenGame()
    {
        double timeRemain = 0f;
        IsNewDay(ref timeRemain);

        battlepass.OpenGame();
    }

    public void CreateUser()
    {
        timeOld = DateTime.Now.ToFileTime();
        battlepass.CreateUser();

        NewDay();
    }

    public void NewDay()
    {
        this.missions = new List<MissionData>();

        int currentTour = RoomDatas.Instance.GetRoomUnlockedMax();

        //for daily mission
        List<MissionConfig> configs = MissionConfigs.Instance.GetDailyMission();
        for (int i = 0; i < configs.Count; i++)
        {
            MissionConfig config = configs[i];
            if (!this.IsHasMission(config.id))
            {
                MissionData dt = new MissionData(config);
                dt.SetData(currentTour, i == 0, config);
                this.AddMission(dt);
            }
        }

        //for point bag
        if (this.pointBagMissions.Count < MissionConfigs.Instance.MAX_POINT_BAGS_AVAILABLE)
            pointBagMissions.Add(new PointbagMissionData(MissionConfigs.Instance.GetMission(MissionID.POINT_BAGS)));

        this.Save();
    }

    protected List<MissionData> GetAllDailyMisson()
    {
        if (this.missions == null) this.NewDay();
        return this.missions;
    }

    public List<MissionData> GetMissionDatas()
    {
        return this.GetAllDailyMisson().OrderBy(x => x.IndexSortSort()).ToList();//.FindAll(x => x.status == MissionData.MissionStatus.DOING || x.status == MissionData.MissionStatus.COMPLETE);

        //x.status != MissionData.MissionStatus.COMPLETE &&
    }

    public bool IsHasMission(int id)
    {
        MissionData dt = this.GetMission(id);
        return dt != null;
    }

    public MissionData NavigateToMission(int id, long step = 1)
    {
        try
        {
            MissionData dt = this.GetMission(id);
            if (dt != null)
            {
                dt.NavigateToMission(step);
                this.ChangeCallback();
                return dt;

            }
            else
            {
                MissionConfig config = MissionConfigs.Instance.GetMission(id);
                if (config != null)
                {
                    dt = new MissionData(config);
                    dt.NavigateToMission(step);
                    this.missions.Add(dt);
                    this.Save();
                    this.ChangeCallback();
                    return dt;
                }
                else
                {
                    Debug.LogError("KHONG TIM THAY MISSION CONFIG:  " + id);
                }
            }
            return null;
        }
        catch (Exception e)
        {
            Debug.LogError(e.StackTrace);
            return null;
        }

    }

    /// <summary>
    /// CAUTION: Call this to Do a point bag mission will cause misBehavior
    /// CALL DoPointbagStep instead
    /// </summary>
    /// <param name="id"></param>
    /// <param name="step"></param>
    /// <returns></returns>
    public MissionData DoStep(int id, long step = 1)
    {
        MissionData dt = this.GetMission(id);
        if (dt != null)
        {
            if (dt.status == MissionData.MissionStatus.WATCHVIDEO) return null;

            if (dt is PointbagMissionData)
                (dt as PointbagMissionData).DoStep((int)step, RoomDatas.Instance.GetRoomUnlockedMax());
            else
                dt.DoStep(step);

            this.ChangeCallback();
            this.Save();
            return dt;
        }
        return null;
    }

    public PointbagMissionData DoPointbagStep(int tourID, int step = 1)
    {
        PointbagMissionData dt = this.CurrentPointBag;
        if (dt != null)
        {

            bool isComplete = dt.DoStep(step, tourID);

            this.ChangeCallback();

            if (isComplete)
            {
                int exceed = (int)(dt.step - dt.totalStep);
                ///Remove this bag from list
                ///Move left points to another bag
                //this.pointBagMissions.Remove(dt);

                if (this.CurrentPointBag != null)// && exceed > 0
                {
                    return DoPointbagStep(tourID, exceed);
                }
            }
            else
            {
                this.Save();
                return dt;
            }
        }
        return null;
    }

    public MissionData GetMission(int id)
    {
        return this.missions.Find(x => x.id.Equals(id));
    }

    public List<MissionData> GetCompletedMission()
    {
        return this.missions.FindAll(x => x.IsComplete());
    }

    public bool IsComplete(int id)
    {
        MissionData dt = this.GetMission(id);
        if (dt != null)
        {
            return dt.IsComplete();
        }
        return false;
    }

    public int GetCountMissionCanReward()
    {
        int count = 0;
        if (this.missions != null)
        {
            foreach (MissionData mission in this.missions)
            {
                if (mission.IsCanReward())
                    count += 1;
            }
        }
        return count;
    }

    public MissionData RewardMission(int id)
    {
        MissionData dt = this.GetMission(id);
        if (dt != null)
        {
            MissionData newMission = dt.CompleteMission();

            this.ChangeCallback();
            return newMission;
        }
        return dt;
    }

    public void AddMission(MissionData dt)
    {
        if (this.missions == null) this.missions = new List<MissionData>();
        this.missions.Add(dt);
    }

    private void Save()
    {
        GameDataManager.Instance.SaveUserData();
    }

    private int CountCompleteMission()
    {
        List<MissionData> dts = this.GetCompletedMission();
        if (dts != null)
        {
            return dts.Count;
        }
        return 0;
    }

    #region Callback
    public void AddCallback(UnityAction<object> callback)
    {
        if (this.callbacks == null)
        {
            this.callbacks = new CallbackEventObject();
        }
        this.callbacks.AddListener(callback);
        callback.Invoke(this.CountCompleteMission());
    }
    public void RemoveCallback(UnityAction<object> callback)
    {
        if (this.callbacks == null)
        {
            this.callbacks = new CallbackEventObject();
            return;
        }
        this.callbacks.RemoveListener(callback);
    }
    private void ChangeCallback()
    {
        if (this.callbacks == null)
        {
            this.callbacks = new CallbackEventObject();
        }
        this.callbacks?.Invoke(this.CountCompleteMission());
    }
    #endregion

    #region Time
    public const double TIME_TOTAL = 86400; //24h
    public long timeOld;

    /// <summary>
    /// kiểm tra qua ngày mới chưa để reset deal
    /// </summary>
    /// <param name="totalRemain"></param>
    /// <returns></returns>
    public bool IsNewDay(ref double totalRemain, bool isSaveData = true)
    {
        DateTime timeOldFT = DateTime.FromFileTime(this.timeOld);

        if (timeOldFT == null)
        {
            this.NewDay();
            this.timeOld = DateTime.Now.ToFileTime();

            return true;
        }

        bool isDiffDate = timeOldFT.Date != DateTime.Now.Date;

        //double timePass = DateTime.Now.Subtract(timeOldFT).TotalSeconds;
        //bool isMore24H = timePass >= TIME_TOTAL || timePass < 0; //< 0 : case lùi giờ hoặc data cũ

        totalRemain = TIME_TOTAL - DateTime.Now.TimeOfDay.TotalSeconds;
        if (isDiffDate) //isMore24H || 
        {
            this.NewDay();

            this.timeOld = DateTime.Now.ToFileTime();

            if (isSaveData)
                this.Save();

            return true;
        }
        return false;
    }
    #endregion
}

[System.Serializable]
public class MissionData
{
    public int id;
    public int index;

    public long preStep;
    public long step;
    public long totalStep;

    public MissionType type;

    public string name;
    public string description;

    public BoosterCommodity reward;

    public long stepAddpass;

    private MissionTarget target;

    public MissionStatus status;

    public bool _hasShowNotify;
    //public bool _takenReward;

    public bool isProMission;

    private MissionConfig _config;

    private MissionConfig Config
    {
        get
        {
            if (this._config == null)
            {
                this._config = MissionConfigs.Instance.GetMission(this.id);
            }

            return this._config;
        }
    }


    public MissionData SetData(int tour, bool isProMission, MissionConfig config = null, bool isResetStep = true)
    {
        if (config == null)
            config = MissionConfigs.Instance.GetMission(this.id);

        if (config == null)
            return this;

        this.isProMission = isProMission;

        this._config = config;
        this.totalStep = config.GetStep(tour);
        this.reward = config.GetReward(tour);
        this.stepAddpass = config.stepAddPass;

        if (isResetStep)
        {
            preStep = 0;
            step = 0;
        }

        return this;
    }

    public enum MissionStatus
    {
        NONE = -1,
        WATCHVIDEO = 0, //cần watch ads để claim
        DOING = 1, //đang làm
        CAN_REWARD = 2, //có thể nhận reward
        DONE = 3, //đã done = nhận reward rồi
        CANCEL = 4,
    }


    public MissionData()
    {
        this.id = -1;
        this.index = 0;
        this.step = -1;
        this.totalStep = 0;
        this.name = "";
        this.description = "";
        this.type = MissionType.NONE;

        this.status = MissionStatus.NONE;

        this.preStep = 0;
        this.step = 0;

        this._hasShowNotify = false;
        //this._takenReward = false;
    }
    public MissionData(MissionConfig config)
    {
        this.id = config.id;
        this.index = 0; //(level)
        this.step = 0; //value target
        this.totalStep = config.GetStep(this.index);
        this.reward = config.GetReward(this.index);
        this.stepAddpass = config.stepAddPass;
        this.type = config.type;
        this.name = config.name;
        this.description = config.description;
        this.target = Activator.CreateInstance(EnumUtility.GetStringType(this.type)) as MissionTarget;

        this.preStep = 0;

        this._hasShowNotify = false;
        //this._takenReward = false;

        this.SetStatus(MissionStatus.DOING);
    }

    public virtual void Load()
    {
        if (this.target == null)
        {

        }
    }

    /// <summary>
    /// Điều hướng đến nơi làm Mission
    /// </summary>
    /// <param name="step"></param>
    /// <returns></returns>
    public virtual MissionData NavigateToMission(long step)
    {
        if (this.target == null)
        {
            this.target = Activator.CreateInstance(EnumUtility.GetStringType(this.type)) as MissionTarget;
        }
        this.target.DoMisison(this, step);
        return this;
    }

    public virtual MissionData SetStep(long step)
    {
        this.step = step;

        return this;
    }

    public virtual MissionData DoStep(long step)
    {
        if (this.status == MissionData.MissionStatus.DONE || this.status == MissionData.MissionStatus.CAN_REWARD) return this;

        this.preStep = this.step;
        this.step += step;

        if (this.IsComplete())
        {
            //_hasShowNotify = true;

            this.status = MissionData.MissionStatus.CAN_REWARD;

            //callback complete
            MissionDatas.Instance.callbackCompleteMission?.Invoke();

            this.Save();
        }

#if UNITY_EDITOR
        Debug.LogError(string.Format("Mission ID {0} preStep {1}, currentStep {2} in Total {3}",
            this.id, this.preStep, this.step,
            this.totalStep));
#endif
        return this;
    }

    public virtual MissionData SetStatus(MissionStatus status)
    {
        this.status = status;
        this.Save();

        return this;
    }

    public virtual bool IsComplete()
    {
        return this.step >= this.totalStep;
    }

    public virtual bool IsCanReward()
    {
        bool isComplete = IsComplete() && this.status == MissionStatus.CAN_REWARD; //!this._takenReward;
        if (isProMission)
            return isComplete && BattlepassDatas.Instance.vipType == BattlepassDatas.VipType.VipFull;
        else
            return isComplete;
    }

    public int IndexSortSort()
    {
        //pro mission luôn trên top
        if (this.isProMission)
            return -1;

        switch (this.status)
        {
            case MissionStatus.CAN_REWARD:
                return 0;
            case MissionStatus.DOING:
                return 1;
            case MissionStatus.DONE:
                return 2;
            default:
                return 3;
        }

        ////Complete và có thể nhận reward
        //if (this.IsCanReward())
        //{
        //    return 0;
        //}

        ////chưa conplete
        //if (!this.IsComplete())
        //{
        //    return 1;
        //}

        //return 2;
    }

    /// <summary>
    /// Including Reward
    /// </summary>
    /// <returns></returns>
    public virtual MissionData CompleteMission()
    {
        //add reward
        UserProfile.Instance.AddBooster(
            booster: this.reward,
            from: string.Format("Reward from Mission {0}", this.id),
            where: "Mission");

        //this._takenReward = true;

        this.SetStatus(MissionStatus.DONE);

        //add pass step (callback)
        BattlepassDatas.Instance.DoStep(this.stepAddpass);

        Debug.Log(string.Format("Reward from Mission {0} : {1} {2}", this.id, this.reward.GetValue(), this.reward.type));

        this.Save();
        return this;
    }

    public virtual string GetName()
    {
        if (this.target == null)
        {
            this.target = Activator.CreateInstance(EnumUtility.GetStringType(this.type)) as MissionTarget;
        }
        return this.target.GetName(this);
    }

    public virtual Sprite GetIcon()
    {
        if (this.Config != null)
        {
            return this.Config.sprIcon;
        }
        return null;
    }

    public virtual string GetDescription()
    {
        if (this.target == null)
        {
            this.target = Activator.CreateInstance(EnumUtility.GetStringType(this.type)) as MissionTarget;
        }
        return this.target.GetDescription(this);
    }

    public virtual float GetProgressFill()
    {
        return ((float)this.step / (float)this.totalStep);
    }

    public virtual string GetProgress()
    {
        return string.Format("{0}/{1}", GameUtils.FormatMoneyDot(this.step), GameUtils.FormatMoneyDot(this.totalStep));
    }

    public virtual string GetPlay()
    {
        if (this.target == null)
        {
            this.target = Activator.CreateInstance(EnumUtility.GetStringType(this.type)) as MissionTarget;
        }
        return this.target.GetPlay(this);
    }

    public virtual void Save()
    {
        GameDataManager.Instance.SaveUserData();
    }
}

[System.Serializable]
public class PointbagMissionData : MissionData
{
    [System.Serializable]
    public class PointbagComponent
    {
        public int TourID;
        public int PointGained;
    }

    public List<PointbagComponent> DetailBag;

    public PointbagMissionData()
    {
        this.DetailBag = new List<PointbagComponent>();
    }
    public PointbagMissionData(MissionConfig c) : base(c)
    {
        this.DetailBag = new List<PointbagComponent>();
    }

    /// <summary>
    /// return true if complete. Move the left point to another bag
    /// </summary>
    /// <param name="step"></param>
    /// <param name="tourID"></param>
    /// <returns></returns>
    public bool DoStep(int step, int tourID)
    {
        PointbagComponent existedTour = this.DetailBag.Find(x => x.TourID == tourID);

        if (existedTour != null)
            existedTour.PointGained += step;
        else
            this.DetailBag.Add(new PointbagComponent()
            {
                TourID = tourID,
                PointGained = step
            });

        this.DoStep(step);

        return this.status == MissionStatus.CAN_REWARD || this.status == MissionStatus.DONE;
    }

    public override MissionData CompleteMission()
    {
        int tourMost = DetailBag.OrderByDescending(x => x.PointGained).First().TourID;

        Debug.Log(string.Format("Most of point create by tour {0}", tourMost));

        ///Gain player a bag
        MainBagSlots.Instance.OpenPointBag(tourMost);
        this.SetStatus(MissionStatus.CAN_REWARD);

        this.Save();
        return this;
    }
}

