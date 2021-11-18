using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StoreCoinTab : StoreChildTab
{
    [Header("Coins")]
    public StoreBoosterItem[] coinItems;

#if UNITY_EDITOR
    protected override void OnValidate()
    {
        base.OnValidate();
        this.coinItems = this.GetComponentsInChildren<StoreBoosterItem>();
    }
#endif

    private void Start()
    {
        ParseConfig();
    }

    public void ParseConfig()
    {
        Debug.Log("edit");

        //List<StoreBoosterConfig> cashConfigs = StoreConfigs.Instance.GetBetterPlaceCoins();
        //if (cashConfigs != null)
        //{
        //    for (int i = 0; i < cashConfigs.Count; i++)
        //    {
        //        if (i < this.coinItems.Length)
        //        {
        //            this.coinItems[i].ParseConfig(cashConfigs[i]);
        //        }
        //    }
        //}
    }
}
