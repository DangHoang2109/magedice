using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class MainFreeBagIcon : BaseIcon
{
    public Transform tranFree;
    public Transform tranWait;
    public TextMeshProUGUI tmpTime;

    private double timeRemain;
    private double timeWait;
    private bool isFree;

    public override void OnClickIcon()
    {
        base.OnClickIcon();

        if (this.isFree)
        {
            this.OpenFreeBag();
        }
        else
        {
            Notification.Instance.ShowNotificationIcon(string.Format(LanguageManager.GetString("DES_WAIT"), GameUtils.ConvertFloatToTime(this.timeWait, "mm'm'ss's'")));
        }
    }

    private void OpenFreeBag()
    {
        int currentTour = RoomDatas.Instance.GetRoomUnlockedMax();
        //MainBagSlots.OpenBagNow(BagType.FREE_BAG, currentTour, "MainFreeBagSlot");
        //BagSlotDatas.Instance.bagFree.OpenBag();
    }

    private void Update()
    {
        //this.isFree = BagSlotDatas.Instance.bagFree.IsReadyToOpen(ref timeRemain);
        //this.tranFree.gameObject.SetActive(isFree);
        //this.tranWait.gameObject.SetActive(!isFree);
        //if (!isFree)
        //{
        //    this.timeWait = BagFreeData.TOTAL_TIME_WAIT - timeRemain;
        //    this.tmpTime.text = string.Format(LanguageManager.GetString("DES_WAIT"), GameUtils.ConvertFloatToTime(this.timeWait)); //"Wait: {0}"
        //}
    }

}
