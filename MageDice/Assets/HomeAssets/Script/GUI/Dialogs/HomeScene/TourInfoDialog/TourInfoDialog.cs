using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class TourInfoDialog : BaseSortingDialog
{
    public TextMeshProUGUI tmpTitle;

    //OLD LevelCaps
    //[Header("Level cap")]
    //public List<CardLevelCapItem> levelCaps;

    //[Header("Bag reward")]
    //public List<BagRewardCardItem> bagRewards;

#if UNITY_EDITOR
    protected override void OnValidate()
    {
        base.OnValidate();

        //OLD LevelCaps
        //this.levelCaps = new List<CardLevelCapItem>();
        //CardLevelCapItem[] caps = this.GetComponentsInChildren<CardLevelCapItem>();
        //foreach(CardLevelCapItem cap in caps)
        //{
        //    this.levelCaps.Add(cap);
        //}

        //this.bagRewards = new List<BagRewardCardItem>();
        //BagRewardCardItem[] bags = this.GetComponentsInChildren<BagRewardCardItem>();
        //foreach(BagRewardCardItem bag in bags)
        //{
        //    this.bagRewards.Add(bag);
        //}
    }
#endif

    public void ParseConfig(RoomConfig roomConfig)
    {
        if (roomConfig != null)
        {
            this.tmpTitle.SetText(string.Format(LanguageManager.GetString("TOURINFO_TITLE", LanguageCategory.Games), roomConfig.id));

            //List<BagType> bagCanRewards = roomConfig.GetBagsCanReward();

            int indexTour = roomConfig.id;

            //if (bagCanRewards != null)
            //{
            //    for (int i = 0; i < this.bagRewards.Count; i++)
            //    {
            //        this.bagRewards[i].gameObject.SetActive(i < bagCanRewards.Count);
            //        if (i < bagCanRewards.Count)
            //        {
            //            this.bagRewards[i].ParseBag(bagCanRewards[i], indexTour);
            //        }
            //    }
            //}
        }
        else Debug.LogError("Room config is NULL");
    }
}
