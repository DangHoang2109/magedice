using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MissionMiniGame : MissionTarget
{
    public override void DoMisison(MissionData data, long step)
    {
        base.DoMisison(data, step);

        HomeTabs.Instance.MoveToTab(HomeTabName.EVENT);
    }
}
