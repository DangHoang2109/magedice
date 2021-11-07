using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MainMailIcon : IconNoti
{
    public override void OnClickIcon()
    {
        Debug.Log("Click mail");
        Notification.Instance.ShowNotificationIcon(LanguageManager.GetString("TITLE_COOMINGSOON"));
        base.OnClickIcon();
    }
}
