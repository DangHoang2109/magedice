using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MissionOpenBox : MissionTarget
{
#if LITE
#else
    public override void DoMisison(MissionData data, long step)
    {
        base.DoMisison(data, step);

        HomeTabs.Instance.MoveToTab(HomeTabName.MAIN);

    }
#endif
}
