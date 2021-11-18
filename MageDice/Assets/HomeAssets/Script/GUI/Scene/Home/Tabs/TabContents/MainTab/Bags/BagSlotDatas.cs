using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Thời gian đợi, trạng thái của slot bag
/// </summary>
[System.Serializable]
public class BagSlotDatas 
{
    /// <summary>
    /// Có đợi thời gian để mở bag không, True: có thể mở ngay, False: phải đợi thời gian để mở
    /// </summary>
    public bool NO_WAIT_OPEN_BAG = true;

    public static BagSlotDatas Instance
    {
        get
        {
            return GameDataManager.Instance.GameDatas.bagDatas;
        }
    }
    public BagSlotDatas()
    {
        this.bagSlots = new List<BagSlotData>();
        this.bagFree = new BagFreeData();
    }

    public void CreateUser()
    {
        if (this.bagSlots == null) this.bagSlots = new List<BagSlotData>();
        for (int i = 0; i < COUNT_BAG; i++)
        {
            this.bagSlots.Add(new BagSlotData(i));
        }
        SaveData();
    }

    public void SaveData()
    {
        GameDataManager.Instance.SaveUserData();
    }

    public bool IsNoWaitOpenBag()
    {
        if (RemoteConfigsManager.IsLoadRemoteConfig)
        {
            NO_WAIT_OPEN_BAG = RemoteConfigsManager.IsOpenBag;
            this.SaveData();
        }
        return NO_WAIT_OPEN_BAG;
    }

    #region Bag slots

    public List<BagSlotData> bagSlots;

    private const int COUNT_BAG = 4;

    public List<BagSlotData> GetBagSlots()
    {
        //Force cho mở ngay
        if (IsNoWaitOpenBag())
        {
            foreach(BagSlotData bagSlot in this.bagSlots)
            {
                if (bagSlot!=null)
                {
                    if (bagSlot.state!= BagSlotState.NONE && bagSlot.state!= BagSlotState.READY_OPEN)
                    {
                        bagSlot.state = BagSlotState.READY_OPEN;
                    }
                }         
            }
        }

        return this.bagSlots;
    }

    public BagSlotData GetBagSlot(int id)
    {       
        BagSlotData bagSlot = this.bagSlots.Find(x => x.id == id);

        //Force cho mở ngay
        if (IsNoWaitOpenBag())
        {
            if (bagSlot != null)
            {
                if (bagSlot.state != BagSlotState.NONE && bagSlot.state != BagSlotState.READY_OPEN)
                {
                    bagSlot.state = BagSlotState.READY_OPEN;
                }
            }
        }

        return bagSlot;
    }

    public bool IsFullSlots()
    {
        foreach(BagSlotData bagSlot in this.bagSlots)
        {
            if (!bagSlot.IsExistBag()) return false;
        }
        return true;
    }

    /// <summary>
    /// True nếu toàn bộ slot có bag ngoài này đều là loại bag này
    /// </summary>
    /// <param name="bagType"></param>
    /// <returns></returns>
    private bool IsAllSlotThisBag(BagType bagType, bool isFasleIfAllSlotEmpty = true)
    {
        int slotEmpty = 0;
        for (int i = 0; i < this.bagSlots.Count; i++)
        {
            if (this.bagSlots[i].IsExistBag())
            {
                if (this.bagSlots[i].type != bagType)
                    return false;
            }
            else
                slotEmpty++;
        }

        if (slotEmpty == this.bagSlots.Count && isFasleIfAllSlotEmpty)
            return false;

        return true;
    }

    /// <summary>
    /// Hiện mở bag ngay lúc end game
    /// </summary>
    /// <param name="bagType">loại bag</param>
    /// <param name="tourId">tour</param>
    /// <returns></returns>
    public bool IsCanOpenBagEndGame(BagType bagType, int tourId)
    {
        if (!IsFullSlots())
            return false;

        Debug.Log($"test bag {bagType}");
        ///bag xịn hơn bag trắng thì hỏi
        if ((int)bagType > (int)BagType.PLATINUM_BAG)
            return true;

        ///check bag đỏ
        ///Nếu toàn bộ slot ngoài này full xanh => hỏi với rate 75%
        if (bagType == BagType.GOLD_BAG)
        {
            if (IsAllSlotThisBag(BagType.SILVER_BAG))
                if (UnityEngine.Random.value <= 0.75f)
                {
                    Debug.Log($"test bag Full all blue bag, rate < 0.75");
                    return true;
                }
        }

        //bag xanh dương và free thì luôn skip
        return false;
    }

    /// <summary>
    /// Nhận bag
    /// </summary>
    /// <param name="bagType">Kiểu bag</param>
    /// <param name="name">Nhận nó ở tour nào => tên bag = tên bag - name</param>
    /// <param name="where">Dung de log user profile booster</param>
    /// <param name="where">Dùng để log ở User profile booster</param>
    /// <param name="amount">Số lượng bag nhận</param>
    /// <returns></returns>
    public bool CollectBag(BagType bagType, string name,string from, string where, int tour ,int amount = 1)
    {
        bool collected = false;
        for(int i=0; i< amount; i++)
        {
            foreach (BagSlotData bagSlot in this.bagSlots)
            {
                if (!bagSlot.IsExistBag())
                {
                    //Debug.LogError("add bag " + bagType);
                    bagSlot.ParseBag(bagType, name);
                    bagSlot.tourID = tour;
                    collected = true;
                    UserProfile.Instance.AddBooster(new BoosterCommodity(BoosterType.BAG, 1),from, where);
                    break;
                }
            }
        }
        SaveData();
        return collected;
    }


    /// <summary>
    /// Nhận bag, VỚI THỜI GIAN ĐƯỢC CUSTOM
    /// </summary>
    /// <param name="bagType">Kiểu bag</param>
    /// <param name="name">Nhận nó ở tour nào => tên bag = tên bag - name</param>
    /// <param name="where">Dung de log user profile booster</param>
    /// <param name="where">Dùng để log ở User profile booster</param>
    /// <param name="amount">Số lượng bag nhận</param>
    /// <returns></returns>
    public bool CollectBag(float timeWait, BoosterCommodity bstSkip,BagType bagType, string name, string from, string where, int tour, int amount = 1)
    {
        bool collected = false;
        for (int i = 0; i < amount; i++)
        {
            foreach (BagSlotData bagSlot in this.bagSlots)
            {
                if (!bagSlot.IsExistBag())
                {
                    //Debug.LogError("add bag " + bagType);
                    bagSlot.ParseBag(bagType, name, timeWait, bstSkip);
                    bagSlot.tourID = tour;
                    collected = true;
                    UserProfile.Instance.AddBooster(new BoosterCommodity(BoosterType.BAG, 1), from, where);
                    break;
                }
            }
        }
        SaveData();
        return collected;
    }

    public bool CollectBag(BagAmount bagAmount, string name,string from, string where)
    {
        return CollectBag(bagAmount.bagType, name, from, where, bagAmount.tour, bagAmount.amount);
    }

    public bool IsAnotherBeingOpenned(int id)
    {
        foreach (BagSlotData slot in this.bagSlots)
        {
            if (slot.id != id)
            {
                if (slot.state == BagSlotState.WAITTING || slot.state == BagSlotState.READY_OPEN)
                {
                    return true;
                }
            }
        }
        return false;
    }

    #endregion

    #region Bag free
    public BagFreeData bagFree;

    #endregion
}

[System.Serializable]
public class BagSlotData
{
    public int id;
    public string name;
    public BagType type;
    public BagSlotState state;
    public BoosterCommodity bstUnlockMax;
    public BoosterCommodity bstUnlock;
    public int tourID;

    public double TOTAL_TIME_WAIT;  //Total time for watting
    public long timeStartWaitting;
    private double ratioTimeRemain;

    public BagSlotData() { }

    public BagSlotData(int id)
    {
        this.state = BagSlotState.NONE;
        this.id = id;
    }

    public void ParseBag(BagType bagType, string name)
    {
        BagConfig bagSlot = BagConfigs.Instance.GetBag(bagType);
        if (bagSlot != null)
        {
            this.type = bagType;
            this.name = name;

            //check có cần đợi không
            if (BagSlotDatas.Instance.IsNoWaitOpenBag())
            {
                this.state = BagSlotState.READY_OPEN;
            }
            else
            {
                this.state = BagSlotState.EXIST;
            }
            
            this.TOTAL_TIME_WAIT = bagSlot.totalTimeWait;
            this.bstUnlockMax = new BoosterCommodity(bagSlot.bstOpenNow.type, bagSlot.bstOpenNow.GetValue());
            this.bstUnlock = new BoosterCommodity(bagSlot.bstOpenNow.type, bagSlot.bstOpenNow.GetValue());
            SaveData();
        }
    }

    public void ParseBag(BagType bagType, string name, float timeWait, BoosterCommodity bstUnlock)
    {
        BagConfig bagSlot = BagConfigs.Instance.GetBag(bagType);
        if (bagSlot != null)
        {
            this.type = bagType;
            this.name = name;

            //check có cần đợi không
            if (BagSlotDatas.Instance.IsNoWaitOpenBag())
            {
                this.state = BagSlotState.READY_OPEN;
            }
            else
            {
                this.state = timeWait <= 0 ? BagSlotState.READY_OPEN : BagSlotState.EXIST;
            }

            this.TOTAL_TIME_WAIT = timeWait;
            this.bstUnlockMax = new BoosterCommodity(bstUnlock.type, bstUnlock.GetValue());
            this.bstUnlock = new BoosterCommodity(bstUnlock.type, bstUnlock.GetValue());
            SaveData();
        }
    }

    public BagSlotData ChangeState(BagSlotState state)
    {
        this.state = state;
        if (this.state == BagSlotState.WAITTING) this.timeStartWaitting = DateTime.Now.ToFileTime();
        SaveData();
        return this;
    }

    public BagSlotData OpenBag()
    {
        this.state = BagSlotState.NONE;
        SaveData();
        return this;
    }

    public bool IsReadyToOpen(ref double totalRemain)
    {
        totalRemain = 0;
        if (this.state == BagSlotState.WAITTING)
        {
            DateTime oldDate = DateTime.FromFileTime(this.timeStartWaitting);
            DateTime newDate = DateTime.Now;
            TimeSpan difference = newDate.Subtract(oldDate);
            totalRemain = difference.TotalSeconds;

            if (totalRemain >= TOTAL_TIME_WAIT || TOTAL_TIME_WAIT == 0)
            {
                this.state = BagSlotState.READY_OPEN;
                SaveData();
                return true;
            }
            else
            {
                ratioTimeRemain = (1 - totalRemain / TOTAL_TIME_WAIT);

                if (ratioTimeRemain > 1f) ratioTimeRemain = 1f;
                else if (ratioTimeRemain < 0f) ratioTimeRemain = 0f;

                this.bstUnlock.Set(this.bstUnlockMax.GetValue()==0? 0 : (long)(this.bstUnlockMax.GetValue() * ratioTimeRemain + 1));
            }
        }   
        return false;
    }

    public BagSlotData ReduceTime(double timeReduce)
    {
        this.TOTAL_TIME_WAIT -= timeReduce;
        if (this.TOTAL_TIME_WAIT < 0) this.TOTAL_TIME_WAIT = 0;
        double timeRemain = 0f;
        IsReadyToOpen(ref timeRemain);
        SaveData();
        return this;
    }
    public bool IsExistBag()
    {
        return this.state != BagSlotState.NONE;
    }

    private void SaveData()
    {
        GameDataManager.Instance.SaveUserData();
    }
}


[System.Serializable]
public class BagFreeData
{
    //4 phút nhận được 1 bag free4
    //đang đợi nhận bag => bắt watch video

    public bool isFree; //nếu free thì có thể mở ngay
    public const double TOTAL_TIME_WAIT = GameDefine.TIME_FREE_BAG; 
    public long timeStartWaitting;

    public BagFreeData()
    {
        this.isFree = true;
    }

    public bool IsReadyToOpen(ref double totalRemain)
    {
        if (!this.isFree)
        {
            DateTime oldDate = DateTime.FromFileTime(this.timeStartWaitting);
            DateTime newDate = DateTime.Now;
            TimeSpan difference = newDate.Subtract(oldDate);
            totalRemain = difference.TotalSeconds;
            if (totalRemain >= TOTAL_TIME_WAIT)
            {
                this.isFree = true;
                SaveData();
            }
        }
        return this.isFree;
    }

    public void OpenBag()
    {
        this.isFree = false;
        this.timeStartWaitting = DateTime.Now.ToFileTime();
        SaveData();
    }

    private void SaveData()
    {
        GameDataManager.Instance.SaveUserData();
    }
}