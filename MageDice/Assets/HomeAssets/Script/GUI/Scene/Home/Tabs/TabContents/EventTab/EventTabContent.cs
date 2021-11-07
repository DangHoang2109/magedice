using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class EventTabContent : TabContent
{
    public enum EventIndex
    {
        QUICK_PLAY = 0,
        WHEEL = 1,
        TOURNAMENT = 2
    }

    public List<BaseIcon> icons;

#if UNITY_EDITOR
    private void OnValidate()
    {
        icons = new List<BaseIcon>(this.GetComponentsInChildren<BaseIcon>());
    }
#endif
    public override void OnShow(int index, object data = null, UnityAction callback = null)
    {
        base.OnShow(index, data, callback);

        //icons[(int)EventIndex.TOURNAMENT].gameObject.SetActive(RoomDatas.Instance.GetRoomUnlockedMax() >= 3);

        if (this.icons != null)
        {
            foreach (BaseIcon icon in this.icons)
            {
                icon?.OnShow();
            }
        }  
    }

    public override void OnHide(int index, object data = null, UnityAction callback = null)
    {
        //GameManager.Instance.CloseDialog<WheelDialog>();
        base.OnHide(index, data, callback);
    }

    public void ClickIcon(EventIndex e){
        if ((int)e <= this.icons.Count)
            this.icons[(int)e].OnClickIcon();
    }


}
