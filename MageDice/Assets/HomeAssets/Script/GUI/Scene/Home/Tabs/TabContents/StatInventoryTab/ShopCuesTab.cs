using System;
using System.Collections;
using System.Collections.Generic;
using Cosina.Components;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;


public class ShopCuesTab : TabContent
{
    [Header("outer linker")]
    public ShopCueTabContent theHolder;
    
    public bool NeedToRefresh { get; set; }
    
    [Header("recycler view")]
    public RecyclerViewUIVertical recyclerView;
    public int countItemAppearanceFix = 6;
    
    protected List<StatData> _datas;

    protected bool _isFirstShow = true;
    
    /// <summary>
    /// prevent double show
    /// </summary>
    protected bool _isShowing = false;
    
    protected int GetCount()
    {
        if (this._datas == null)
            return 0;
        return this._datas.Count;
    }

    protected RectTransform TakeItemFromPool()
    {
        return ShopCueRef.Instance.RequestItem().GetComponent<RectTransform>();
    }

    protected void ReturnItemToPool(RectTransform r)
    {
        ShopCueRef.Instance.ReturnItem(r.GetComponent<ShopCueItem>());
    }

    
    
#if UNITY_EDITOR
    [ContextMenu("Force Update Scroll State")]
    private void ForceUpdateScrollState()
    {
        //this.OnScrollChanged(this.scrollRect.normalizedPosition);
    }

#endif
}
