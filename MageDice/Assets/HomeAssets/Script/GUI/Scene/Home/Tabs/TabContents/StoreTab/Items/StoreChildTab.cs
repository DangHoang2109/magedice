using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StoreChildTab : MonoBehaviour
{
    public StoreTabName storeTab;

    public RectTransform rect;

    [Header("Store items, for animation")]
    public StoreItem[] items; 

#if UNITY_EDITOR
    protected virtual void OnValidate()
    {
        this.rect = this.GetComponent<RectTransform>();
        this.items = this.GetComponentsInChildren<StoreItem>();
    }
#endif

    public Vector3 GetPosTop()
    {
        return rect.localPosition + new Vector3(0, rect.sizeDelta.y / 2);
    }

    public Vector2 GetRectSize()
    {
        return rect.sizeDelta;
    }

    public virtual void OnShowTab()
    {
        //animation show tab
        if (this.items != null) 
        {
            Sequence seq = DOTween.Sequence();
            foreach(StoreItem item in this.items)
            {
                seq.Append(item.AnimShow());
                seq.AppendInterval(0.02f);
            }
            seq.SetId(this);
        }
    }

    /// <summary>
    /// show anim những item giao với rect
    /// </summary>
    /// <param name="rectTrans"></param>
    public virtual void OnShowTab(RectTransform rectView, RectTransform rectScroll)
    {
        if (this.items != null)
        {
            Sequence seq = DOTween.Sequence();
            foreach (StoreItem item in this.items)
            {
                //kiểm tra item có nằm trong rect không
                //Debug.Log("<color=yellow> Rect view </color>" + rectTrans.rect);
                Vector3 posInScroll = rectScroll.InverseTransformPoint(item.transform.position);
                //Debug.Log("<color=blue> Rect item </color>" + posInScroll);

                if (rectView.rect.Contains(posInScroll))
                {
                    seq.Append(item.AnimShow());
                    seq.AppendInterval(0.02f);
                }             
            }
            seq.SetId(this);
        }
    }

    public virtual void OnHideTab()
    {
        //animation hide tab
        if (this.items != null)
        {
            Sequence seq = DOTween.Sequence();
            foreach (StoreItem item in this.items)
            {
                seq.Append(item.AnimHide());
                seq.AppendInterval(0.05f);
            }
            seq.SetId(this);
        }
    }

    public virtual void OnClear()
    {
        DOTween.Kill(this);
    }
}
