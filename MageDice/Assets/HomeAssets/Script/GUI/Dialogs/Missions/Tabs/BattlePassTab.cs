using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BattlePassTab : MissionBaseTab
{
    protected override void OnEnable()
    {
        base.OnEnable();
        BattlepassDatas.callbackProgress += this.DoProgress;
        BattlepassDatas.callbackReward += this.OnRewardBattlePass;
        BattlepassDatas.callbackBuyBattlePass += this.OnBuyBattlePass;
        ParseCountBattlePassCanReward();

    }

    protected override void OnDisable()
    {
        base.OnDisable();
        BattlepassDatas.callbackProgress -= this.DoProgress;
        BattlepassDatas.callbackReward -= this.OnRewardBattlePass;        
        BattlepassDatas.callbackBuyBattlePass -= this.OnBuyBattlePass;
    }

    private void OnRewardBattlePass(BattlepassStepData step)
    {
        ParseCountBattlePassCanReward();
    }

    private void DoProgress(BattlepassStepData stepData, int level)
    {
        ParseCountBattlePassCanReward();
    }

    private void OnBuyBattlePass(bool isBuyProPass)
    {
        ParseCountBattlePassCanReward();
    }

    /// <summary>
    /// Parse số lượng battle pass có thể nhận
    /// </summary>
    private void ParseCountBattlePassCanReward()
    {
        //TODO parse số lượng battle pass có thể nhận
        this.numNoti.ShowNumNoti(BattlepassDatas.Instance.GetCountBattlePassesCanReward());
    }
}
