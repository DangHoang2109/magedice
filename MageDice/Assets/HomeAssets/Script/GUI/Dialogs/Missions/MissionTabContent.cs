using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class MissionTabContent : TabContent
{
    [Header("Battle pass progress")]
    public BattlePassLevelProgress battlePassProgress;

    public MissionItem missionPrefab;
    public RectTransform panelMisison;
    private List<MissionItem> items;

    public override void OnShow(int index, object data = null, UnityAction callback = null)
    {
        base.OnShow(index, data, callback);
        this.ParseData();
    }

    protected override void AnimationHide()
    {
        base.AnimationHide();
        this.OnCompleteHide();
    }

    protected override void AnimationShow()
    {
        base.AnimationShow();
        OnCompleteShow();
    }

    private void ParseData()
    {
        if (this.items == null)
            this.items = new List<MissionItem>();
        foreach(MissionItem item in this.items)
        {
            item.gameObject.SetActive(false);
        }

        List<MissionData> missions = MissionDatas.Instance.GetMissionDatas();
        if (missions != null)
        {
            for (int i = 0; i < missions.Count; i++)
            {
                MissionData m = missions[i];
                MissionItem item;
                if (i < this.items.Count)
                {
                    item = this.items[i];
                }
                else
                {
                    item = Instantiate(this.missionPrefab, this.panelMisison);
                    this.items.Add(item);
                }

                /*if (m.status == MissionData.MissionStatus.COMPLETE || m.status == MissionData.MissionStatus.DONE)
                {
                   continue;
                }*/

                if (item != null)
                {
                    item.gameObject.SetActive(true);
                    item.ParseData(m, this.OnCollectMission);
                    item.transform.SetSiblingIndex(i);           
                }
            }
        }
       

        /*float height = 230 + missions.Count * 190;
        this.panelMisison.sizeDelta = new Vector2(this.panelMisison.sizeDelta.x, height);
        this.isParseData = true;*/
    }

    private void OnCollectMission(MissionItem item)
    {
        //TODO animation collect battle pass
        this.battlePassProgress.AnimCollectPoint();
        FxHelper.Instance.ShowFxCollectBySprite(GameAssetsConfigs.Instance.sprPointBattePass, item.rewardPointBag.transform, this.battlePassProgress.tranIcon, () =>
        {
            item.transform.SetSiblingIndex(this.items.Count - 1);
        });
    }
}
