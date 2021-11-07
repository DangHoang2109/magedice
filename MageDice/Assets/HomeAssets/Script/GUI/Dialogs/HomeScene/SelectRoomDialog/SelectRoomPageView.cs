using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class SelectRoomPageView : MonoBehaviour
{
    [Header("Page view")]
    public PageView pageView;
    public ScrollRect scroll;

    [Header("Room item")]
    public RoomItem roomItemPrefab;
    public Transform tranItems;

    public List<RoomItem> roomItems;


    [System.NonSerialized]
    public Dictionary<int, RoomItem> dicRooms;
    [System.NonSerialized]
    public int currentRoom;
    [System.NonSerialized]
    public bool unlockRoom;

    //Event
    public EventObject<int> EventChangeRoom
    {
        get
        {
            if (this.eventChangeRoom == null) this.eventChangeRoom = new EventObject<int>();
            return this.eventChangeRoom;
        }
    }

    public EventObject<int> EventSellectRoom
    {
        get
        {
            if (this.eventSellectRoom == null) this.eventSellectRoom = new EventObject<int>();
            return this.eventSellectRoom;
        }
    }

    private EventObject<int> eventChangeRoom;
    private EventObject<int> eventSellectRoom;

#if UNITY_EDITOR
    private void OnValidate()
    {
        RoomItem[] rooms = this.tranItems.GetComponentsInChildren<RoomItem>();
        if (rooms != null)
        {
            this.roomItems = new List<RoomItem>();
            foreach (RoomItem room in rooms)
            {
                this.roomItems.Add(room);
            }
        }
    }
#endif

    private void OnEnable()
    {
        if (this.dicRooms == null)
        {
            this.dicRooms = new Dictionary<int, RoomItem>();
            if (this.roomItems != null)
            {
                foreach (RoomItem room in this.roomItems)
                {
                    if (!this.dicRooms.ContainsKey(room.RoomId))
                    {
                        this.dicRooms.Add(room.RoomId, room);
                    }
                }
            }
        }
    }

    private void OnDisable()
    {
        this.pageView.changePageEvent.RemoveAllListeners();
        UserProfile.Instance.RemoveCallbackBooster(BoosterType.CUP, OnChangeBoosterCup);
    }

    private void Start()
    {
        ParseRooms();

    }

    private void OnChangeBoosterCup(BoosterCommodity booster)
    {
        if (booster != null)
        {
            unlockRoom = false;
            List<RoomData> roomDatas = RoomDatas.Instance.GetRooms();
            if (roomDatas != null)
            {
                foreach (RoomData roomData in roomDatas)
                {
                    RoomConfig roomConfig = RoomConfigs.Instance.GetRoom(roomData.id);
                    if (roomConfig != null)
                    {
                        if (roomData.id !=  GameDefine.ROOM_FRIST_AI && roomData.id !=  GameDefine.ROOM_PRACTICLE && !roomData.unlocked && booster.GetValue() >= roomConfig.unlock.GetValue())
                        {
                            //Unlock room data
                            RoomDatas.Instance.UnlockRoom(roomData.id);
                            this.currentRoom = roomData.id;
                            unlockRoom = true;
                        }
                    }
                }

                if (unlockRoom)
                {
                    this.scroll.enabled = false;
                    this.pageView.LerpToPage(this.currentRoom);
                }
            }
        }
    }

    private void ParseRooms()
    {
        Debug.Log("parse room");
        this.currentRoom = 0;
        unlockRoom = false;
        List<RoomConfig> roomConfigs = RoomConfigs.Instance.GetRooms();
        if (roomConfigs != null)
        {
            foreach (RoomConfig roomConfig in roomConfigs)
            {
                if (roomConfig.id == GameDefine.ROOM_FRIST_AI || roomConfig.id == GameDefine.ROOM_PRACTICLE) //bỏ qua parse room coach
                    continue;

                RoomItem room;
                if (this.dicRooms.ContainsKey(roomConfig.id))
                {
                    room = this.dicRooms[roomConfig.id];
                }
                else
                {
                    Debug.Log("instante room");
                    room = Instantiate<RoomItem>(this.roomItemPrefab, this.tranItems);
                    this.dicRooms.Add(roomConfig.id, room);
                }
                room.gameObject.SetActive(true);
                //check data xem đã unlock chưa
                RoomData roomData = RoomDatas.Instance.GetRoom(roomConfig.id);
                room.ParseConfig(roomConfig, roomData);
                if (roomData.unlocked) this.currentRoom = roomConfig.id;
            }
        }
        //BattlepassDatas.Instance.cbReloadRoomUI = () => GetCurRoomItem().ReloadRoom();

        StartCoroutine(IeStart());
    }

    private IEnumerator IeStart()
    {
        yield return new WaitForEndOfFrame();
        yield return new WaitForSeconds(0.1f);
        //Debug.LogError("Start page view");
        this.pageView.startingPage = this.currentRoom;
        this.pageView.changePageEvent.AddListener(ChangePageRoom);
        this.pageView._Start();

        yield return new WaitForEndOfFrame();
        UserProfile.Instance.AddCallbackBooster(BoosterType.CUP, OnChangeBoosterCup); //call back cup : for check unlock
    }

    private void ChangePageRoom(int id)
    {
        //effect unlock room
        if (this.unlockRoom) 
        {
            if (this.dicRooms != null)
            {
                if (this.dicRooms.ContainsKey(this.currentRoom))
                {
                    HomeTabs.Instance.SetBlockInput(true); //Không cho chuyển tab
                    RoomItem roomItem = this.dicRooms[this.currentRoom];
                    roomItem.ShowEffectUnlock(() => {
                        this.scroll.enabled = true;
                        HomeTabs.Instance.SetBlockInput(false); //cho chuyển tab
                    });
                }
            };
            this.unlockRoom = false;
        }
    }
    public RoomItem GetCurRoomItem()
    {
        if (this.dicRooms != null)
        {
            if (this.dicRooms.ContainsKey(this.currentRoom))
            {
                Debug.Log("get room");
                return this.dicRooms[this.currentRoom];
            }
        }
        return null;
    }
}
