using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OfflineBonusTabContent : TabContent
{
    protected override void AnimationHide()
    {
        base.AnimationHide();
        this.OnCompleteHide();
        this.isShow = false;
    }
}
