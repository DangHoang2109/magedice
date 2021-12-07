using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MissionWinGame : MissionTarget
{
#if LITE
#else
    public override void DoMisison(MissionData data, long step)
    {
        base.DoMisison(data, step);

        HomeTabs.Instance.MoveToTab(HomeTabName.MAIN);
        SelectRoomDialog selectRoomDialog = GameManager.Instance.OnShowDialogWithSorting<SelectRoomDialog>("GUI/Dialogs/HomeScene/SelectRoom/SelectRoomDialog", PopupSortingType.BellowBottomBar);

    }
#endif
}
