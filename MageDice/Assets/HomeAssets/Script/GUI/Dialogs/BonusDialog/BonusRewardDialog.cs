using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class BonusRewardDialog : BaseSortingDialog
{
    public TabBase tabbaseReward;

    public override void OnShow(object data = null, UnityAction callback = null)
    {
        base.OnShow(data, callback);
        this.tabbaseReward.Init();
    }
}
