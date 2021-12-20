using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;


#if UNITY_EDITOR
public class TestPass
{
    public static BattlepassDatas Pass = BattlepassDatas.Instance;

    [UnityEditor.MenuItem("Test/pass/Show all pass")]
    public static void ShowAllpass()
    {
        foreach (BattlepassStepData d in Pass.activePass.steps)
        {
            Debug.Log(string.Format("Free pass has step: id {0}, cur step {1} total {2}. Can do pro ? {3}.",
                d.id,
                d.step, d.totalStep,
                Pass.IsProPass()));
        }
    }


    [UnityEditor.MenuItem("Test/pass/GenNewpass")]
    public static void GenNewpass()
    {
        Debug.Log("Gen new pass");

        Pass.ResetData(1);
        ShowAllpass();
    }

    [UnityEditor.MenuItem("Test/pass/Do 50 step")]
    public static void DoPass20Step()
    {
        Pass.DoStep(50);
        ShowAllpass();
    }
    [UnityEditor.MenuItem("Test/pass/Do 200 step")]
    public static void DoPass200Step()
    {
        Pass.DoStep(200);
        ShowAllpass();
    }

    [UnityEditor.MenuItem("Test/pass/Remove vip")]
    public static void TestRemoveVip()
    {
        Pass.BuyVip(BattlepassDatas.VipType.None);
    }


#if DATE_PRO_PASS
    [UnityEditor.MenuItem("Test/pass/Purchase Pass 7 days")]
    public static void Purchase()
    {
        BattlepassDatas.Instance.PurchaseProPass(7);
        ShowAllpass();
    }

    [UnityEditor.MenuItem("Test/pass/Remove Pro pass")]
    public static void RemveProPass()
    {
        //TODO remove pass
        BattlepassDatas.Instance.OutOfDayProPass();
    }
#endif
    public static void CheckProPassMaxGem(int tour)
    {
        long totalGem = 0;

        BattlepassData data = new BattlepassData(BattlePassConfigs.Instance.GetPass(1));
        data.SetData(tour);

        long totalStep = 0;
        int avgGameWinEachDay = 3;
        int boostPointPro = 3;

        for (int i = 0; i < data.steps.Count; i++)
        {
            BattlepassStepData step = data.steps[i];
            long gem = 0;

            totalStep += step.totalStep;

            if (step.proReward.GetBoosterData().type == BoosterType.CASH)
            {
                gem = step.proReward.GetBoosterData().GetValue();
                totalGem += gem;
                Debug.Log($"Step {i} Gem: gift step {gem} total {totalGem}. Need {step.totalStep} pts = {totalStep / 40} mission = {totalStep / 160} free day = {totalStep / 200} pro day");
                Debug.Log($"Step {i} Need {step.totalStep} = {totalStep / (160 + 7* avgGameWinEachDay)} free day = {totalStep / (200 + 7 * avgGameWinEachDay)} pro day = {totalStep / (200 + 7 * avgGameWinEachDay * boostPointPro)} pro full day");
            }

            if (step.proReward.IsRewardBag())
            {
                GiftBagPerTourConfig config = GiftBagConfigs.Instance.GetGiftBag(step.proReward.GetBagData().bagType, tour);
                gem = config.valueGem.max;
                totalGem += gem;
                Debug.Log($"Max Step {i} Bag {step.proReward.GetBagData().bagType}: gift gem {gem} total {totalGem}.Need {step.totalStep} pts = {totalStep / 40} mission = {totalStep / 160} free day = {totalStep / 200} pro day");
                Debug.Log($"Step {i} Need {step.totalStep} = {totalStep / (160 + 7 * avgGameWinEachDay)} free day = {totalStep / (200 + 7 * avgGameWinEachDay)} pro day = {totalStep / (200 + 7 * avgGameWinEachDay * boostPointPro)} pro full day");
            }
        }

        Debug.Log($"Max Complete pass user will get {totalGem}");
    }
    public static void CheckProPassAvgGem(int tour)
    {
        long totalGem = 0;
        int testCheck = 100;

        BattlepassData data = new BattlepassData(BattlePassConfigs.Instance.GetPass(1));
        data.SetData(tour);
        long totalStep = 0;
        int avgGameWinEachDay = 3;
        int boostPointPro = 3;
        for (int i = 0; i < data.steps.Count; i++)
        {
            bool isDebug = false;

            BattlepassStepData step = data.steps[i];
            long gem = 0;

            totalStep += step.totalStep;

            if (step.proReward.GetBoosterData().type == BoosterType.CASH)
            {
                gem = step.proReward.GetBoosterData().GetValue();
                totalGem += gem;

                isDebug = true;
            }
            if (step.freeReward.GetBoosterData().type == BoosterType.CASH)
            {
                gem = step.freeReward.GetBoosterData().GetValue();
                totalGem += gem;

                isDebug = true;
            }

            if (step.proReward.IsRewardBag())
            {
                GiftBagPerTourConfig config = GiftBagConfigs.Instance.GetGiftBag(step.proReward.GetBagData().bagType, tour);

                long testRandTotal = 0;
                for (int j = 0; j < testCheck; j++)
                {
                    testRandTotal += config.valueGem.GetRandomValue();
                }

                gem = testRandTotal / testCheck;
                totalGem += gem;

                isDebug = true;
            }
            if (step.freeReward.IsRewardBag())
            {
                GiftBagPerTourConfig config = GiftBagConfigs.Instance.GetGiftBag(step.freeReward.GetBagData().bagType, tour);

                long testRandTotal = 0;
                for (int j = 0; j < testCheck; j++)
                {
                    testRandTotal += config.valueGem.GetRandomValue();
                }

                gem = testRandTotal / testCheck;
                totalGem += gem;

                isDebug = true;
            }

            if (isDebug)
            {
                Debug.Log($"Step {i} Gem: gift {gem} total {totalGem}. Need {step.totalStep} pts = {totalStep / 40} mission = {totalStep / 160} free day = {totalStep / 200} pro day");
                Debug.Log($"Step {i} Need {step.totalStep} = {totalStep / (160 + 7 * avgGameWinEachDay)} free day = {totalStep / (200 + 7 * avgGameWinEachDay)} pro day = {totalStep / (200 + 7 * avgGameWinEachDay * boostPointPro)} pro full day");
            }

        }

        Debug.Log($"AVG Complete pass user will get {totalGem}");
    }

    [UnityEditor.MenuItem("Test/pass/Check max Gem from pro pass/Current")]
    public static void CheckProPassMaxGem_Current()
    {
        CheckProPassMaxGem(RoomDatas.Instance.GetRoomUnlockedMax());
    }
    [UnityEditor.MenuItem("Test/pass/Check max Gem from pro pass/tour 3")]
    public static void CheckProPassMaxGem_3()
    {
        CheckProPassMaxGem(3);
    }
    [UnityEditor.MenuItem("Test/pass/Check max Gem from pro pass/tour 4")]
    public static void CheckProPassMaxGem_4()
    {
        CheckProPassMaxGem(4);
    }
    [UnityEditor.MenuItem("Test/pass/Check Avg Gem from pro pass/Current")]
    public static void CheckProPassAvgGem_Current()
    {
        CheckProPassAvgGem(RoomDatas.Instance.GetRoomUnlockedMax());
    }
    [UnityEditor.MenuItem("Test/pass/Check Avg Gem from pro pass/Tour 3")]
    public static void CheckProPassAvgGem_3()
    {
        CheckProPassAvgGem(3);
    }
    [UnityEditor.MenuItem("Test/pass/Check Avg Gem from pro pass/Tour 4")]
    public static void CheckProPassAvgGem_4()
    {
        CheckProPassAvgGem(4);
    }

}
#endif

    [System.Serializable]
public class BattlepassDatas
{
    public static Action<BattlepassStepData, int> callbackProgress;
    public static Action<BattlepassStepData> callbackReward;
    public static Action<bool> callbackBuyBattlePass;
    public static BattlepassDatas Instance
    {
        get
        {
#if UNITY_EDITOR
            if (GameManager.isApplicationQuit)
                return default;
#endif
            return GameDataManager.Instance.GameDatas.missionDatas.battlepass;
        }
    }

    private CallbackEventObject callbacks;

    public BattlepassData activePass;

#if DATE_PRO_PASS
    //Tính pro pass theo ngày
    public bool isBuyProPass; //đã mua propass chưa
    public long timeStartProPass; //thời gian bắt đầu mua propass
    public int countDayProPass; //số ngày của propass
#else
    //2 loại vip và mua vĩnh viễn
    public enum VipType { None, Vip, VipFull } //không có, vip, vip full
    public VipType vipType;

    public const double TIME_TOTAL = 86400; //24h
    public long lastTimeClaimFreeEntry;
    public System.Action cbReloadRoomUI;
    public bool IsCanClaimFreeEntry
    {
        get
        {
            if (this.lastTimeClaimFreeEntry == 0)
            {
                return this.vipType == VipType.VipFull;
            }
                
            DateTime timeOldFT = DateTime.FromFileTime(this.lastTimeClaimFreeEntry);
            if (timeOldFT == null)
            {
                this.lastTimeClaimFreeEntry = DateTime.Now.ToFileTime();
                return true;
            }
            bool isDiffDate = timeOldFT.Date != DateTime.Now.Date;

            return this.vipType == VipType.VipFull && isDiffDate;
        }
    }
    public void ClaimFreeEntry(BoosterCommodity booster)
    {
        this.lastTimeClaimFreeEntry = DateTime.Now.ToFileTime();

        UserProfile.Instance.AddBooster(booster, "ProFreeEntryRoom", LogSourceWhere.CLAIM_PRO_FREEENTRY);

        this.Save();
    }
#endif

    public BattlepassDatas()
    {
        this.activePass = new BattlepassData();
    }

    public void OpenGame()
    {
#if UNITY_EDITOR && CHEATPASS
        if(activePass.id != -1)
        {
            Debug.Log("Cheat pass end sooner");
            this.vipType = VipType.None; //ban đầu chưa có vip
            ResetData(-1);
        }
#else
        if (activePass.id != 1)
        {
            Debug.Log("reset, reject old pass with id 0, replace with config id 1");
            CreateUser();
        }
#endif
    }

    public void CreateUser()
    {
        this.lastTimeClaimFreeEntry = 0;
        this.vipType = VipType.None; //ban đầu chưa có vip
        ResetData(1);
    }
    public void ResetData(int passID)
    {
        BattlePassConfig config = BattlePassConfigs.Instance.GetPass(passID);
        this.vipType = VipType.None;
        this.lastTimeClaimFreeEntry = 0;

        int idAsset = 0;
        if (this.activePass != null)
            idAsset = BattlePassAssetConfigs.Instance.GetNextLayoutId(this.activePass.idAsset); //random layout kế tiếp
        this.activePass = new BattlepassData(config);
        this.activePass.SetData();
        this.activePass.SetIdAsset(idAsset);

        this.Save();
    }

    public void NextBattlePassData()
    {
        //TODO tạo data battle pass mới

#if UNITY_EDITOR && CHEATPASS
        ResetData(-1); //Temp
#else
        ResetData(1); //Temp
#endif
        //this.Save(); //không dùng resetData thì mở ra (ở reset data đã có lưu)
    }

    public bool IsProPass()
    {
        return this.vipType == VipType.Vip || this.vipType == VipType.VipFull;
    }

    public VipType GetVipType()
    {
        return this.vipType;
    }

    public bool CanBuyVipType(VipType newVipType)
    {
        if (this.vipType != newVipType)
        {
            return (int)this.vipType < (int)newVipType;
        }
        return false;
    }

    public void BuyVip(VipType type)
    {
        if(this.vipType == VipType.None)
        {
            switch (type)
            {
                case VipType.Vip:
                    LogGameAnalytics.Instance.LogEvent(LogAnalyticsEvent.BUYPROPASS);
                    break;
                case VipType.VipFull:
                    LogGameAnalytics.Instance.LogEvent(LogAnalyticsEvent.BUYFULLPROPASS);
                    break;
            }
        }
        else if(this.vipType == VipType.Vip)
        {
            if(type == VipType.VipFull)
                LogGameAnalytics.Instance.LogEvent(LogAnalyticsEvent.UPGRADEFULLPROPASS);
        }



        this.vipType = type;

        if (type == VipType.VipFull)
            cbReloadRoomUI?.Invoke();
        callbackBuyBattlePass?.Invoke(this.IsProPass());
        Save();
    }

    public BattlepassData DoStep(long step = 1)
    {
        //return null;

        if (this.activePass == null || this.activePass.IsOutDate)
            return null;

        bool isComplete = this.activePass.DoStep(step);
        if (isComplete)
        {
            int exceed = (int)(this.activePass.CurrentStep.step - this.activePass.CurrentStep.totalStep);
            ///Remove this bag from list
            ///Move left points to another bag
            ///
            this.activePass.NextStep(exceed);
            this.activePass.isFristShow = true;
            
        }
        callbackProgress?.Invoke(this.activePass.CurrentStep, this.activePass.CurrentIndex);
        callbackReward?.Invoke(this.activePass.CurrentStep);
        this.Save();
        return this.activePass;
    }
    public BattlepassData DoStepWinGame(long step = 1)
    {
        return this.DoStep(vipType == VipType.VipFull ? step * 3 : step);
    }

    public bool IsComplete()
    {
        if (activePass != null)
        {
            return activePass.IsComplete();
        }
        return false;
    }

    public void UpdateFristShowReward()
    {
        if (this.activePass != null)
        {
            this.activePass.isFristShow = false;
            this.Save();
        }
    }

    private void Save()
    {
        GameDataManager.Instance.SaveUserData();
    }
    
    /// <summary>
    /// Lấy số lượng battle pass có thể nhận thưởng
    /// </summary>
    /// <returns></returns>
    public int GetCountBattlePassesCanReward()
    {
        if (this.activePass == null) return 0;
        return this.activePass.GetCountStepCanReward();
    }

    public BattlepassData GetActiveBattlePass()
    {
        return this.activePass;
    }

    public int GetIdAssetActiveBattlePass()
    {
        if (this.activePass != null) 
            return this.activePass.idAsset;
        return 0;
    }

#region Time

    /// <summary>
    /// kiểm tra qua ngày mới chưa để reset deal
    /// </summary>
    /// <param name="totalRemain">Thời gian còn lại của pass để show lên UI, in TotalSecond</param>
    /// <param name="saveData">Chỉ cho bằng true 1 lần khi mở dialog để save lại data, trong vòng update bình thường không cần</param>
    /// <returns></returns>
    public bool IsOutDate(ref double totalRemain)
    {
        double TIME_TOTAL = this.activePass.length;
        double timePassed = DateTime.Now.Subtract(DateTime.FromFileTime(this.activePass.timeStartPass)).TotalSeconds;

        totalRemain = TIME_TOTAL - timePassed;

        if (totalRemain <= 0)
        {
            this.activePass.IsOutDate = true;

            //TODO reset 
            Save();

            return true;
        }

        return false;
    }


    /// <summary>
    /// Nếu activePass outdate và vừa claim reward cuối của pass
    /// Return true
    /// Mặc định sẽ reset data luôn nếu biến bằng true,có thể comment hàm lại và tự gọi 
    /// </summary>
    /// <returns></returns>
    public bool IsCanReset()
    {
        if (this.activePass.IsOutDate)
        {
            for (int i = 0; i < this.activePass.steps.Count; i++)
            {
                if (this.activePass.steps[i].IsCanReward())
                    return false;
            }

#if UNITY_EDITOR && CHEATPASS

            ResetData(-1);
#else
            ResetData(1);
#endif

            this.ResetData(1); //comment hàm này nếu muốn tự gọi reset data
            return true;
        }

        return false;
    }

#endregion
}


[System.Serializable]
public class BattlepassData
{
    
#region Property
    private BattlePassConfig _config;
    public BattlePassConfig Config
    {
        get
        {
            if (this._config == null)
            {
                this._config = BattlePassConfigs.Instance.GetPass(this.id);
            }

            return this._config;
        }
    }
    public string name => this.Config.name;
    public double length => this.Config.length;

    public BattlepassStepData CurrentStep => this.steps[CurrentIndex];

    /// <summary>
    /// Lấy ra step min mà nó chưa claim
    /// </summary>
    public BattlepassStepData CurrentClaimStepData
    {
        get
        {
            if (this.CurrentIndexClaimStep < this.steps.Count)
            {
                return this.steps[this.CurrentIndexClaimStep];
            }
            return null;
        }
    }
    /// <summary>
    /// Lấy ra index thấp nhất
    /// </summary>
    public int CurrentIndexClaimStep
    {
        get
        {
            for (int i = 0; i < this.steps.Count; i ++)
            {
                if (this.steps[i].IsCanReward())
                {
                    return i;
                }
            }
            return this.CurrentIndex;
        }
    }
#endregion

#region Save Data
    public int id;
    public List<BattlepassStepData> steps;

    public int CurrentIndex;
    public bool isFristShow = true;

    public long timeStartPass;

    ///Khi Pass outdate, pass sẽ nằm yên trong data cho đến khi User claim toàn bộ quà sẽ gọi reset pass mới
    ///Nếu IsOutDae = true => Không được DoStep hay PurchaseProbag nữa
    public bool IsOutDate;

    /// <summary>
    /// Id asset dùng để lưu color, layout của battle pass
    /// </summary>
    public int idAsset;

#endregion

    public void PurchaseProPass()
    {
        if (this.IsOutDate)
            return;

        foreach(BattlepassStepData step in this.steps)
        {
            if (step.status == BattlepassStepData.Status.DONE && !step.proReward.isReward)
                step.status = BattlepassStepData.Status.COMPLETE;
        }
    }

    /// <summary>
    /// Tặng free step 0
    /// </summary>
    /// <param name="startStep"></param>
    /// <returns></returns>
    public BattlepassData SetData(int startStep = 1, int tour = -1)
    {
        if (this.IsOutDate)
            return this;

        this.steps = this.steps ?? new List<BattlepassStepData>();

        tour = tour >= 0 ? tour : RoomDatas.Instance.GetRoomUnlockedMax();

        for (int i = 0; i < this.Config.step.Count; i++)
        {
            BattlepassStepData battlepassStep = new BattlepassStepData(tour, _config.step[i]);
            battlepassStep.SetStatus(i < startStep ? BattlepassStepData.Status.COMPLETE : (i == startStep ? 
               BattlepassStepData.Status.DOING : BattlepassStepData.Status.NOT_READY));
            //battlepassStep.SetStatus(BattlepassStepData.Status.NOT_READY);
            this.steps.Add(battlepassStep);
        }

        this.CurrentIndex = startStep; //đưa về step start

        return this;
    }

    public void SetIdAsset(int idAsset)
    {
        this.idAsset = idAsset;  
    }
    public BattlepassData()
    {
        this.CurrentIndex = 1;
        this.isFristShow = true;

        this.timeStartPass = 0;
        this.IsOutDate = false;
    }
    public BattlepassData(BattlePassConfig config)
    {
        this.id = config.id;

        this._config = config;
        this.CurrentIndex = 1;
        this.isFristShow = true;

        
        this.timeStartPass = DateTime.Now.ToFileTime();
        this.IsOutDate = false;
    }

    /// <summary>
    /// Step hiện tại đã unlock
    /// </summary>
    /// <param name="stepData"></param>
    /// <returns></returns>
    public bool IsCurrentStep(BattlepassStepData stepData)
    {
        return this.CurrentStep == stepData;
    }


    /// <summary>
    /// Đã đi qua step đó chưa
    /// </summary>
    /// <param name="stepData"></param>
    /// <returns></returns>
    public bool IsUnlockStep(BattlepassStepData stepData)
    {
        int indexStep = this.steps.IndexOf(stepData);
        return this.CurrentIndex > indexStep; 
    }

    public virtual bool DoStep(long step)
    {
        if (this.IsOutDate)
            return false;

        this.CurrentStep.DoStep(step);

        return this.CurrentStep.status == BattlepassStepData.Status.COMPLETE || this.CurrentStep.status == BattlepassStepData.Status.DONE;
    }

    public virtual BattlepassData NextStep(int stepLeft = 0)
    {
        if (this.IsOutDate)
            return this;

        this.CurrentIndex++;

        if (!IsComplete())
        {
            this.CurrentStep.SetStatus(BattlepassStepData.Status.DOING);
            if (stepLeft > 0)
                this.CurrentStep.DoStep(stepLeft);
        }

        return this;
    }

    public virtual bool IsComplete()
    {
        return (this.CurrentIndex == this.Config.step.Count - 1 &&
            this.CurrentStep.IsComplete()) || this.CurrentIndex >= this.Config.step.Count;
    }

    public int GetCountStepCanReward()
    {
        int count = 0;

        foreach(BattlepassStepData battlepassStep in this.steps)
        {
            if (battlepassStep.IsCanReward())
            {
                count += 1;
            }
        }
        return count;
    }
}


[System.Serializable]
public class BattlepassStepData
{
    [System.Serializable]
    public class StepReward
    {
        public bool isReward;
        public BoosterCommodity reward;
        public BagAmount bag;

        public BattlePassMissionStep stepConfig;
        public BattlePassType type;
        public void Init(BattlePassMissionStep s, bool isPro, int tour)
        {
            this.stepConfig = s;
            this.type = isPro ? BattlePassType.PRO_PASS : BattlePassType.FREE_PASS;

            BoosterCommodity b = s.GetReward(type, tour);
            if(b.type == BoosterType.BAG)
            {
                this.reward = null;
                this.bag = s.GetBag(type, tour);
            }
            else
            {
                this.reward = b;
                this.bag = null;
            }

            this.isReward = false;
        }
        /// <summary>
        /// Trả ra phần thưởng của step này là bag
        /// </summary>
        /// <returns></returns>
        public bool IsRewardBag()
        {
            if (this.bag != null)
            {
                return this.bag.amount > 0;
            }

            return false;
        }

        public BagAmount GetBagData()
        {
            int maxRoom = RoomDatas.Instance.GetRoomUnlockedMax();

            return new BagAmount()
            {
                bagType = this.bag.bagType,
                amount = this.bag.amount,
                tour = maxRoom
            };
        }

        public BoosterCommodity GetBoosterData()
        {
            int maxRoom = RoomDatas.Instance.GetRoomUnlockedMax();
            return this.stepConfig.GetReward(this.type, maxRoom);
        }
    }

    public int passID;
    public int id;
    public int tour;

    public Status status;
    public long preStep;
    public long step;
    public long totalStep;


    public StepReward freeReward;
    public StepReward proReward;


    public bool _hasShowNotify;
    public bool _takenReward;


    private BattlePassMissionStep _config;
    private BattlePassMissionStep Config
    {
        get
        {
            if (this._config == null)
            {
                this._config = BattlePassConfigs.Instance.GetPassStep(this.passID, this.id);
            }

            return this._config;
        }
    }

    public enum Status
    {
        NONE = -1,
        NOT_AVAILABLE, //phải purchase hoặc lên pro pass
        NOT_READY, //chung pass nhưng chưa reach được
        DOING, //đang làm
        COMPLETE, //đã xong, chưa nhận thưởng
        DONE, //đã xong đã nhận thưởng
    }

    public BattlepassStepData()
    {
        this.id = -1;
        this.tour = 0;
        this.step = -1;
        this.totalStep = 0;

        this.status = Status.NONE;

        this.preStep = 0;
        this.step = 0;

        this._hasShowNotify = false;
        this._takenReward = false;
    }
    public BattlepassStepData(int tour,BattlePassMissionStep config)
    {
        this.id = config.id;
        this.tour = tour; //(level)
        this.step = 0; //value target

        this._config = config;

        this.totalStep = config.GetStep(this.tour);


        this.freeReward = new StepReward();
        freeReward.Init(config, false, tour);

        this.proReward = new StepReward();
        proReward.Init(config, true, tour);

        this.preStep = 0;

        this._hasShowNotify = false;
        this._takenReward = false;
    }
    /// <summary>
    /// 
    /// </summary>
    /// <returns>
    /// True: là chưa nhận reward free và pro
    /// </returns>
    public bool IsAllReward()
    {
        if (this.status == Status.COMPLETE)
        {
            if (BattlepassDatas.Instance.IsProPass())
            {
                return !this.freeReward.isReward && !this.proReward.isReward;
            }
        }

        return false;
    }

    public bool IsCanReward()
    {
        if (this.status == Status.COMPLETE)
        {
            if (BattlepassDatas.Instance.IsProPass())
            {
                return !this.freeReward.isReward || !this.proReward.isReward;
            }
            else
            {
                return !this.freeReward.isReward;
            }
        }

        return false;
    }

    public virtual BattlepassStepData SetStep(long step)
    {
        this.step = step;

        return this;
    }

    public virtual BattlepassStepData DoStep(long step)
    {
        if ( this.status == Status.DOING)
        {
            this.preStep = this.step;
            this.step += step;

            if (this.IsComplete() && this._hasShowNotify == false)
            {
                _hasShowNotify = true;

                this.SetStatus(Status.COMPLETE);
            }

            Debug.LogError(string.Format("pass ID {0} preStep {1}, currentStep {2} in Total {3}",
                this.id, this.preStep, this.step,
                this.totalStep));
        }

        return this;
    }

    public virtual BattlepassStepData SetStatus(Status status)
    {
        this.status = status;

        return this;
    }

    public virtual bool IsComplete()
    {
        return this.step >= this.totalStep;
    }


    /// <summary>
    /// Including Reward
    /// </summary>
    /// <returns></returns>
    public virtual BattlepassStepData GetFreeReward()
    {
        if (this.status != Status.COMPLETE)
            return this;

        this.freeReward.isReward = true;

        bool isBuyProPass = BattlepassDatas.Instance.IsProPass();

        //nếu cả 2 gift đều reward => done
        if (isBuyProPass && this.proReward.isReward)
            this.SetStatus(Status.DONE);

        LogGameAnalytics.Instance.LogEvent(string.Format(LogAnalyticsEvent.COMPLETE_PASS_STEP_ID, id), LogParams.BATTLEPASS, id);

        GameDataManager.Instance.SaveUserData();

        BattlepassDatas.Instance.IsCanReset();
        BattlepassDatas.callbackReward?.Invoke(this);


        return this;
    }

    public bool IsProRewardedOrCompleted()
    {
        return (this.status != Status.COMPLETE || this.proReward.isReward);
    }

    public bool IsFreeRewardedOrCompleted()
    {
        return (this.status != Status.COMPLETE || this.freeReward.isReward);
    }

    public virtual BattlepassStepData GetProReward()
    {
        bool isBuyProPass = BattlepassDatas.Instance.IsProPass();
        if (!isBuyProPass)
        {
            return this;
        }
        
        if (this.status != Status.COMPLETE || this.proReward.isReward)
            return this;

        this.proReward.isReward = true;

        //nếu cả 2 gift đều nhận reward => done
        if (this.freeReward.isReward)
            this.SetStatus(Status.DONE);

        GameDataManager.Instance.SaveUserData();

        BattlepassDatas.Instance.IsCanReset();
        BattlepassDatas.callbackReward?.Invoke(this);
        return this;
    }

    public virtual float GetProgressFill()
    {
        return ((float)this.step / (float)this.totalStep);
    }

    public virtual string GetProgress()
    {
        return string.Format("{0}/{1}", GameUtils.FormatMoneyDot(this.step), GameUtils.FormatMoneyDot(this.totalStep));
    }
}
