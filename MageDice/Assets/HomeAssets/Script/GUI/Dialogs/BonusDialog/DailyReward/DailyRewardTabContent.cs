using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public class DailyRewardTabContent : TabContent
{
    [Header("Sprite Item")]
    [SerializeField] private Sprite bgItemStick;
    [SerializeField] private Sprite bgItemToDay;
    [SerializeField] private Sprite bgItemNormal;

    [Header("Item")]
    public DailyRewardItem prefab;
    public DailyRewardItem stickItem;
    public Transform tfContent;
    public ScrollRect scroll;

    public override void OnInit()
    {
        base.OnInit();
        ParseData();
    }

    private void ParseData()
    {
        UserDailyRewardData DailyReward = UserBonusData.Instance.dailyReward;

        this.stickItem.ParseData(DailyReward.StickData, DailyReward.dataPass.Count -1 , bgItemStick);

        int currentDayIndex = DailyReward.currentDay;

        List<DailyRewardDayData> data = DailyReward.dataPass;
        for (int i = 0; i < data.Count -1; i++)
        {
            DailyRewardItem item = Instantiate(prefab, tfContent);
            item.ParseData(data[i], i, i == currentDayIndex ? bgItemToDay : bgItemNormal);
        }
    }

#if UNITY_EDITOR
    [ContextMenu("Add 1 days")]
    private void AddDay1()
    {

    }
    [ContextMenu("Add 3 days")]
    private void AddDay3()
    {

    }
    [ContextMenu("Add 7 days")]
    private void AddDay7()
    {

    }
    [ContextMenu("Add 14 days")]
    private void AddDay14()
    {

    }
    [ContextMenu("Add 30 days")]
    private void AddDay30()
    {

    }
#endif

    protected override void AnimationHide()
    {
        base.AnimationHide();
        this.OnCompleteHide();
        this.isShow = false;
    }
}
