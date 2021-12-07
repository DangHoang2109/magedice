using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MissionClaimDaily : MissionTarget
{
    public override void DoMisison(MissionData data, long step)
    {
        base.DoMisison(data, step);

        //TODO
        //GameManager.Instance.OnShowDialog<DailyRewardDialog>("GUI/Dialogs/DailyRewards/DailyRewardDialog");
        SoundManager.Instance.PlayButtonClick();
    }
}
