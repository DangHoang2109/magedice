using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StoreBagTab : StoreChildTab
{
    [Header("Bags")]
    public StoreBagItem[] bagItems;

#if UNITY_EDITOR
    protected override void OnValidate()
    {
        Debug.Log("on validate");
        base.OnValidate();
        this.bagItems = this.GetComponentsInChildren<StoreBagItem>();
    }
#endif

    private void Start()
    {
        ParseBag();
    }

    public void ParseBag()
    {
        //parse bag
        List<StoreBoosterConfig> bags = StoreConfigs.Instance.GetBagsShowInStore();
        if (bags != null)
        {
            for (int i = 0; i < bags.Count; i++)
            {
                if (i < this.bagItems.Length)
                {
                    this.bagItems[i].ParseConfig(bags[i]);
                }
            }
        }
    }
}
