using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MainRankIcon : BaseIcon
{
    public override void OnClickIcon()
    {
        Debug.Log("Click rank");
        Notification.Instance.ShowNotificationIcon(LanguageManager.GetString("TITLE_COOMINGSOON"));
        base.OnClickIcon();
    }
}
