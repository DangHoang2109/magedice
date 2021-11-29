using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class NeedMoreGemDialog : BaseSortingDialog
{
    public StoreCashItem cashPrefab;
    public Transform panelCash;

    public override void OnShow(object data = null, UnityAction callback = null)
    {
        base.OnShow(data, callback);
    }

    public void ParseData(BoosterCommodity current)
    {
        List<StoreCashConfig> cashConfigs = StoreConfigs.Instance.GetCashOption_ByNeed(current.GetValue()); //StoreConfigs.Instance.GetCashs();
        int max = cashConfigs.Count > 3 ? 3 : cashConfigs.Count;
        for (int i = 0; i < max; i++)
        {
            StoreCashItem item = Instantiate(this.cashPrefab, this.panelCash);
            item?.ParseConfig(cashConfigs[i]);
        }
    }

    protected override void OnCompleteHide()
    {
        for (int i = 0; i < this.panelCash.childCount; i++)
        {
            Destroy(this.panelCash.GetChild(i).gameObject);
        }
        
        base.OnCompleteHide();
    }
}
