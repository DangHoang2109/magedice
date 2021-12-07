using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MissionPurchase : MissionTarget
{

    public override void DoMisison(MissionData data, long step = 1)
    {
        base.DoMisison(data, step);

        HomeTabs.Instance.MoveToTab(HomeTabName.STORE);
    }

}
