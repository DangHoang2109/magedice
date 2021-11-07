using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StoreBottomTab : HomeBottomTabUI
{
    private double timeRemain;
    //private StoreDealFreeData.FreeDealStatus dealStatus;

    private bool onNoti;

    private void Update()
    {
        //this.dealStatus = StoreDatas.Instance.storeDeals.dealFreeData.IsFreeDeal(ref timeRemain);
        //this.ShowHighLight(this.dealStatus == StoreDealFreeData.FreeDealStatus.FREE);

        
        //if (this.dealStatus == StoreDealFreeData.FreeDealStatus.WATCH)
        //{
        //    if (!this.onNoti)
        //    {
        //        this.onNoti = true;
        //        this.ShowNotiText(LanguageManager.GetString("TITLE_FREE"));
        //    }          
        //}
        //else
        //{
        //    if (this.onNoti)
        //    {
        //        this.onNoti = false;
        //        this.notiText.HideNoti();
        //    }  
        //}
            
    }
}
