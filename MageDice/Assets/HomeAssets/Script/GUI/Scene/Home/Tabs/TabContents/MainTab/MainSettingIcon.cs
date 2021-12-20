using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MainSettingIcon : IconNoti
{
    public override void OnClickIcon()
    {
        GameManager.Instance.OnShowDialogWithSorting<UserProfileDialog>("Home/GUI/UserProfile/UserProfileDialog", PopupSortingType.OnTopBar,1 );
        
        base.OnClickIcon();
    }
}
