using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MissionTakeFreeCard : MissionTarget
{
#if LITE
#else
    public override void DoMisison(MissionData data, long step = 1)
    {
        HomeTabs.Instance.MoveToTab(HomeTabName.STORE);
        HomeTabs.Instance.GetTabContent<StoreTabContent>(HomeTabName.STORE)?.MoveToTab(StoreTabName.DAILY_DEALS);

        SoundManager.Instance.PlayButtonClick();
    }
    public override string GetPlay(MissionData data)
    {
        return "Take";
    }
#endif
}
