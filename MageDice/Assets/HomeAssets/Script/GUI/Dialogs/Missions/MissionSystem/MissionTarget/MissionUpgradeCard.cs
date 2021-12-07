using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MissionUpgradeCard : MissionTarget
{
#if LITE
#else
    public override void DoMisison(MissionData data, long step)
    {
        base.DoMisison(data, step);

        if (Random.value < 0.5f)
        {
            HomeTabs.Instance.MoveToTab(HomeTabName.MAIN);
            SelectRoomDialog selectRoomDialog = GameManager.Instance.OnShowDialogWithSorting<SelectRoomDialog>("GUI/Dialogs/HomeScene/SelectRoom/SelectRoomDialog", PopupSortingType.BellowBottomBar);

        }
        else
            HomeTabs.Instance.MoveToTab(HomeTabName.STORE);
    }
#endif
}
