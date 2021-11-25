using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StoreCashTab : StoreChildTab
{
    [Header("Cashs")]
    public StoreCashItem[] cashItems;

#if UNITY_EDITOR
    protected override void OnValidate()
    {
        base.OnValidate();
        this.cashItems = this.GetComponentsInChildren<StoreCashItem>();
    }
#endif

    private void Start()
    {
        ParseConfig();
    }

    public void ParseConfig()
    {
        List<StoreCashConfig> cashConfigs = StoreConfigs.Instance.GetBetterPlaceCashs();
        if (cashConfigs != null)
        {
            for (int i = 0; i < cashConfigs.Count; i++)
            {
                if (i < this.cashItems.Length)
                {
                    this.cashItems[i].ParseConfig(cashConfigs[i]);
                }
            }
        }
    }
}
