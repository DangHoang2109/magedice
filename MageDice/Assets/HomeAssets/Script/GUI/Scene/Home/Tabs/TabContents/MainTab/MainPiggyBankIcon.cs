using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MainPiggyBankIcon : BaseIcon
{
    public override void OnClickIcon()
    {
        Debug.Log("Click piggy bank");
        Notification.Instance.ShowNotificationIcon(LanguageManager.GetString("TITLE_COOMINGSOON"));
        base.OnClickIcon();
    }
}
