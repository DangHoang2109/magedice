using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MainPlayIcon : BaseIcon
{
    public MainTabContent mainTab;

    public override void OnClickIcon()
    {
        this.mainTab.onClickPlay?.Invoke();
        
        SelectRoomDialog selectRoomDialog = GameManager.Instance.OnShowDialogWithSorting<SelectRoomDialog>("Home/GUI/Dialogs/HomeScene/SelectRoom/SelectRoomDialog", PopupSortingType.BellowBottomBar);
        this.mainTab.SetSelecetRoomDialog(selectRoomDialog);
        base.OnClickIcon();
    }
}
