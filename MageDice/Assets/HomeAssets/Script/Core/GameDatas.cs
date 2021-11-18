using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class GameDatas
{
    public UserDatas userDatas;
    public RoomDatas roomDatas;
    public BagSlotDatas bagDatas;
    public StoreDatas storeDatas;
    public UserBehaviorDatas behaviorDatas;
    //public MissionDatas missionDatas;
    public StatDatas cueDatas;

    public GameDatas()
    {
        this.userDatas = new UserDatas();
        this.roomDatas = new RoomDatas();
        this.bagDatas = new BagSlotDatas();
        this.storeDatas = new StoreDatas();
        this.behaviorDatas = new UserBehaviorDatas();

        //this.missionDatas = new MissionDatas();

        //this.eventDatas = new UserTournamentDatas();

    }
    /// <summary>
    /// Gọi khi user lần đầu vào game
    /// Tạo 1 user
    /// </summary>
    public void CreateUser()
    {
        this.userDatas.CreateUser();
        this.roomDatas.CreateUser();
        this.bagDatas.CreateUser();
        this.storeDatas.CreateUser();
        this.behaviorDatas.CreateUser();

        //this.missionDatas.CreateUser();
    }

    /// <summary>
    /// Gọi khi mở game
    /// </summary>
    public void OpenGame()
    {
        //this.missionDatas.OpenGame();
        this.userDatas.OpenGame();
        this.storeDatas.OpenGame();
    }
}
