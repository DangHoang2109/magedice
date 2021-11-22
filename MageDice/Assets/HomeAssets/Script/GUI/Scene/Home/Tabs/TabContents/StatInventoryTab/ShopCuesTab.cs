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
    public Transform tfContent;
    //public RecyclerViewUIVertical recyclerView;
    //public int countItemAppearanceFix = 6;
    
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
    protected void ParseItem(RectTransform r, int index)
    {
        if(index >= this._datas.Count)
        {
            ReturnItemToPool(r);
            return;
        }
        r.GetComponent<ShopCueItem>()
            .ParseData(this._datas[index], index, false)
            .SetRemoveAfterBuy(null)
            .SetHost(this);
    }
    protected void ParseItem(ShopCueItem r, int index)
    {
        if (index >= this._datas.Count)
        {
            ReturnItemToPool(r.transform as RectTransform);
            return;
        }
        r.ParseData(this._datas[index], index, false)
            .SetRemoveAfterBuy(null)
            .SetHost(this);
    }
    protected void ParseAllItem()
    {
        int continueIndex = 0;
        List<ShopCueItem> existedChild = new List<ShopCueItem>(this.tfContent.GetComponentsInChildren<ShopCueItem>());
        for (int i = continueIndex; i < existedChild.Count; i++)
        {
            this.ParseItem(existedChild[i], i);
        }

        continueIndex = existedChild.Count;

        if(continueIndex < this._datas.Count)
        {
            for (int i = continueIndex; i < _datas.Count; i++)
            {
                RectTransform r = TakeItemFromPool();
                r.SetParent(this.tfContent);
                r.SetAsLastSibling();
                this.ParseItem(r, i);
            }
        }
    }
    
#if UNITY_EDITOR
    [ContextMenu("Force Update Scroll State")]
    private void ForceUpdateScrollState()
    {
        //this.OnScrollChanged(this.scrollRect.normalizedPosition);
    }

#endif
}
