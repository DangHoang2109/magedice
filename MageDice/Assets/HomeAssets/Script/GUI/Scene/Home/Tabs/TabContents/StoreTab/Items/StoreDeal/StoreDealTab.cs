using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class StoreDealTab : StoreChildTab
{
    [Header("Free item")]
    public StoreDealFreeItem freeItem;

    [Header("Deal card or cue")]
    public StoreDealCardItem[] dealCardItems;

    [Header("Time")]
    public TextMeshProUGUI tmpTime;
    private double timeRemain;

#if UNITY_EDITOR
    protected override void OnValidate()
    {
        //base.OnValidate();
        this.freeItem = this.transform.GetComponentInChildren<StoreDealFreeItem>();
        this.dealCardItems = this.transform.GetComponentsInChildren<StoreDealCardItem>();
    }
#endif

    private void Start()
    {
        ParseData();
    }

    public void ParseData()
    {
        this.freeItem.ParseData();

        if (this.dealCardItems != null)
        {
            int iSlot = 0;

            //parse deal card items
            List<StoreDealCardData> dealCards = StoreDealCardsData.Instance.GetDealCards();
            if (dealCards != null)
            {
                for (int i = 0; i < dealCards.Count; i++)
                {
                    if (iSlot < this.dealCardItems.Length)
                    {
                        this.dealCardItems[iSlot].ParseData(dealCards[i]);
                        iSlot += 1;
                    }
                }
            }
        }
        
    }

    public void ReloadData()
    {
        Debug.LogError("Reload data");
        ParseData();
    }

    private void Update()
    {
        if (StoreDealsData.Instance.IsResetDeals(ref timeRemain))
        {
            //parse lại data
            ReloadData();
        }
        else
        {
            this.tmpTime.SetText(GameUtils.ConvertFloatToTime(GameDefine.TIME_TOTAL_A_DAY - this.timeRemain, "hh'h'mm'm'"));
        }
    }
}
