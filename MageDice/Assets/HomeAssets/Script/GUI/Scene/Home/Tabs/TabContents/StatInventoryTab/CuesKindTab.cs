using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Cosina.Components;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class CuesKindTab : ShopCuesTab
{
    [Header("stat")]
    public StatManager.Kind kind;

    public override void OnInit()
    {
        base.OnInit();
    }

    public override void OnShow(int index, object data = null, UnityAction callback = null)
    {
        if(this._isShowing)
        {
            return;
        }
        this._isShowing = true;
        Invoker.Invoke(() => this._isShowing = false, 0.00001f);

        base.OnShow(index, data, callback);

        if (this._isFirstShow)
        {
            this.ParseData();

            this.recyclerView.Init(
                callParseItem: this.ParseItem,
                funcGetCountItems: this.GetCount,
                funcTakeItemFromStock: this.TakeItemFromPool,
                callReturnItemToStock: this.ReturnItemToPool
            );
            
            StatManager.Instance.OnCueGained -= OnACueGained;
            StatManager.Instance.OnCueGained += OnACueGained;
            this.NeedToRefresh = false;
            this._isFirstShow = false;
        }
        else
        {
            if (this.NeedToRefresh)
            {
                this.Refresh();
            }
        }

    }

    private void ParseData()
    {
        this._datas = StatManager.Instance.GetDatasByKind_Complex(this.kind)
            .OrderByDescending(StatManager.CueValueSelector).ToList();
    }

    
    private void ParseItem(RectTransform r, int index)
    {
        r.GetComponent<ShopCueItem>()
            .ParseData(this._datas[index], index, false)
            .SetRemoveAfterBuy(null)
            .SetHost(this);
    }
    
    private void OnACueGained(StatData _)
    {
        if(this.gameObject.activeInHierarchy)
        {
            this.Refresh();
        }
        else
        {
            this.NeedToRefresh = true;
        }
    }

    private void Refresh()
    {
        this.ParseData();
        this.recyclerView.OnCollectionChanged(false);
        this.NeedToRefresh = false;
    }

    protected override void AnimationHide()
    {
        base.AnimationHide();
        this.OnCompleteHide();
    }

    private void OnDestroy()
    {
#if UNITY_EDITOR
        if (GameManager.isApplicationQuit)
            return;
#endif

        StatManager.Instance.OnCueGained -= OnACueGained;
    }
}
