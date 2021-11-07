using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[System.Serializable]
public class RoomDatas 
{
    public static Action<int> callbackUnlockRoom;

    public static RoomDatas Instance
    {
        get
        {
            return GameDataManager.Instance.GameDatas.roomDatas;
        }
    }

    public List<RoomData> datas;

    public RoomDatas()
    {
        this.datas = new List<RoomData>();
    }

    public void CreateUser()
    {
        List<RoomConfig> rooms = RoomConfigs.Instance.GetRooms();
        if (rooms != null)
        {
            foreach (RoomConfig room in rooms)
            {
                this.datas.Add(new RoomData(room.id));
            }
        }
        this.UnlockRoom(-1); //room id -1 = room practicle
        this.UnlockRoom(0);//room id = 0: tutorial first AI
        this.UnlockRoom(1);//room id = 1: newyork

    }
    
    public List<RoomData> GetRooms()
    {
        return this.datas;
    }

    public RoomData GetRoom(int id)
    {
        return this.datas.Find(x => x.id == id);
    }

    public int GetNumTour()
    {
        return GetRoomUnlockedMax() + 1;
    }

    public int GetMaxRoomID()
    {
        if (this.datas != null)
        {
            for (int i = this.datas.Count - 1; i >= 0; i--)
            {
                 return this.datas[i].id;
            }
        }
        return -1;
    }
    public int GetRoomUnlockedMax()
    {
        if (this.datas != null)
        {
            for (int i = this.datas.Count - 1; i >= 0; i--)
            {
                if (this.datas[i].unlocked) return this.datas[i].id;
            }
        }
        return -1;
    }

    public bool IsUnlockedRoom(int id)
    {
        RoomData room = this.GetRoom(id);
        if (room != null) return room.unlocked;
        return false;
    }

    public void UnlockRoom(int id)
    {
        this.GetRoom(id)?.UnlockRoom();
        callbackUnlockRoom?.Invoke(id);

        LogGameAnalytics.Instance.LogEvent(string.Format(LogAnalyticsEvent.UNLOCK_ROOM_WITH_ID, id), LogParams.ROOM_ID, id);
        
        SaveData();
    }

    public void AddPoint(int id, int value)
    {
        this.GetRoom(id)?.AddPoint(value);
        SaveData();
    }

    public void SetPoint(int id, int value)
    {
        this.GetRoom(id)?.SetPoint(value);
        SaveData();
    }


    private void SaveData()
    {
        GameDataManager.Instance.SaveUserData();
    }

#if UNITY_EDITOR
    [UnityEditor.MenuItem("Test/Trophy/+50 room 1")]
    private static void TestFullTrophyRoom1()
    {
        RoomDatas.Instance.AddPoint(1, 50);
    }
#endif
}

[System.Serializable]
public class RoomData
{
    public int id;
    public bool unlocked; //đã được unlock hay chưa
    public int point; //điểm

    public RoomData(int id)
    {
        this.id = id;
        this.unlocked = false;
        this.point = 0;
    }

    public void AddPoint(int value)
    {
        this.point += value;
        if (this.point <= 0) this.point = 0;
    }

    public void SetPoint(int value)
    {
        this.point = value;
    }

    public void UnlockRoom()
    {
        this.unlocked = true;
    }

}



