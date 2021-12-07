using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MissionTab : MissionBaseTab
{
    protected override void OnEnable()
    {
        base.OnEnable();
        BattlepassDatas.callbackProgress += this.DoProgress;
    }

    protected override void OnDisable()
    {
        base.OnDisable();
        BattlepassDatas.callbackProgress -= this.DoProgress;
    }

    protected override void Start()
    {
        ParseCountMissionCompleted();
    }

    /// <summary>
    /// Cũng chính là callback reward mission
    /// </summary>
    /// <param name="stepData"></param>
    /// <param name="level"></param>
    private void DoProgress(BattlepassStepData stepData, int level)
    {
        ParseCountMissionCompleted();
    }

    private void ParseCountMissionCompleted()
    {
        //TODO parse số lượng mission có thể nhận
        this.numNoti.ShowNumNoti(MissionDatas.Instance.GetCountMissionCanReward());
    }

}
