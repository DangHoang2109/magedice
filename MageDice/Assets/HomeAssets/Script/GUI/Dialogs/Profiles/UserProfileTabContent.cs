using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Events;

public class UserProfileTabContent : TabContent
{
    public ProfileBasicInfo info;
    //public ProfileCardView lineUp; //(OLD)
    public UserCareerOverview career;
    
    public override void OnShow(int index, object data = null, UnityAction callback = null)
    {
        base.OnShow(index, data, callback);
        if (this.isShow)
        {
            return;
        }
        long trophy = UserBoosters.Instance.GetBoosterCommodity(BoosterType.CUP).GetValue();
        long coinEarned = UserDatas.Instance.careers.coinEarned;
        long coin = UserBoosters.Instance.GetBoosterCommodity(BoosterType.COIN).GetValue();
        this.info?.ParseData(JoinGameHelper.GetStandardMainUser(),trophy, coinEarned, coin );

        //OLD
        //this.lineUp?.ParseData(StatsDatas.Instance.GetLineupById(1));
        this.career?.ParseData(UserDatas.Instance.careers);
        this.isShow = true;

    }

    protected override void AnimationHide()
    {
        base.AnimationHide();
        this.OnCompleteHide();
        this.isShow = false;
    }
}
