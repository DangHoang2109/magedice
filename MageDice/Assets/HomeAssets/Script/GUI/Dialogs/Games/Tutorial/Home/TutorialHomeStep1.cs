using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class TutorialHomeStep1 : MonoBehaviour
{
    
    private RoomTourItem roomItemFirst;

    private RoomTourItem RoomItemFirst
    {
        get
        {
            var roomContainers = FindObjectOfType<SelectRoomPageView>();
            if (roomContainers == null)
            {
                Debug.LogException(new System.Exception($"TutorialHome1 RoomItem exception! -roomContainers NULL -step {TutorialDatas.TUTORIAL_PHASE}\n"));
                return null;
            }

            if (roomContainers.dicRooms == null)
            {
                Debug.LogException(new System.Exception($"TutorialHome1 RoomItem exception! -roomContainers -dicRooms NULL -step {TutorialDatas.TUTORIAL_PHASE}\n"));
                return null;
            }
            if (roomContainers.dicRooms.Count == 0)
            {
                Debug.LogException(new System.Exception($"TutorialHome1 RoomItem exception! -roomContainers -dicRooms EMPTY -step {TutorialDatas.TUTORIAL_PHASE}\n"));
                return null;
            }
            if (!roomContainers.dicRooms.ContainsKey(1))
            {
                Debug.LogException(new System.Exception($"TutorialHome1 RoomItem exception! -roomContainers -dicRooms NOT contain -id 1 -step {TutorialDatas.TUTORIAL_PHASE}\n"));
                return null;
            }
            
            this.roomItemFirst = roomContainers.dicRooms[1] as RoomTourItem;
            if (!roomContainers.dicRooms.ContainsKey(1))
            {
                Debug.LogException(new System.Exception($"TutorialHome1 RoomItem exception! failed to cast to RoomTourItem -roomContainers.dicRooms[1] {(roomContainers.dicRooms[1] == null? "NULL" : roomContainers.dicRooms[1].ToString())} -step {TutorialDatas.TUTORIAL_PHASE}\n"));
                return null;
            }

            return roomItemFirst;
        }
    
    }


    //private MainBagSlot _bagSlot;

    //private MainBagSlot BagSlot
    //{
    //    get
    //    {
    //        if (_bagSlot == null)
    //        {
    //            _bagSlot = MainBagSlots.Instance.bagSlots.FirstOrDefault(x=> x.IsHaveBag());
    //        }
    //        return _bagSlot;
    //    }
    //}
    private IEnumerator Start()
    {
        yield return new WaitForSeconds(0.2f);
        this.TutorialOpenBag();
    }

    public void TutorialOpenBag()
    {
        //if (this.BagSlot != null)
        //{
        //    RectTransform r = this.BagSlot.transform as RectTransform;
            
        //    TutorialHandClickDialog dialog = GameManager.Instance.OnShowDialogWithSorting<TutorialHandClickDialog>("Games/GUI/Tutorials/TutorialHandClickDialog", PopupSortingType.OnTopBar );
        //    dialog?.OnTutorial(r.position, r.rect.size);
        //    dialog?.MoveHand(0);

        //    LogGameAnalytics.Instance.LogEvent(LogAnalyticsEvent.TUTORIAL_STEP_TUTBOX);
        //}
    }

    public void TutorialClickRoom()
    {
        if (this.RoomItemFirst != null)
        {
            RectTransform r = this.RoomItemFirst.tranBtPlay as RectTransform;
            TutorialHandClickDialog dialog = GameManager.Instance.OnShowDialogWithSorting<TutorialHandClickDialog>("Games/GUI/Tutorials/TutorialHandClickDialog", PopupSortingType.OnTopBar );
            dialog?.OnTutorial(r.position, r.rect.size);
            dialog?.MoveHand(0);

            this.RoomItemFirst.onClickPlay += this.JoinPlayTutorial;
            this.roomItemFirst.isBlockPlayByTutorial = true;
        }
        ///Complete Done Tutorial
        TutorialDatas.TUTORIAL_PHASE = TutorialDatas.TUT_PHASE_FINAL;
        LogGameAnalytics.Instance.LogEvent(LogAnalyticsEvent.TUTORIAL_STEP_DONE);

    }

    private void JoinPlayTutorial()
    {
        JoinGameHelper.Instance.JoinTutorialFristGameAI(1);
        
    }
}
