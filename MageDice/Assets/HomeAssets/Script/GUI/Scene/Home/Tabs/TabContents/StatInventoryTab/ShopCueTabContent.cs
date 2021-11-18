using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class ShopCueTabContent : TabContent
{
    public enum TabName
    {
        None = 0,
        NowOwn = 1,
        Own = 2,
    }

    
    [Header("Children")]
    public GameObject goTabs;
    public GameObject goPanels;
    
    [Header("for first run")]
    public TabBase tabControlEachTier;

    [Header("Update tab color")]
    public TextMeshProUGUI[] txtTabLabels;
    private Color colGrey = new Color(1f, 1f, 1f, 0.7f);
    
    [Header("On buying cue")]
    public CuesKindTab tabOwn;
    [Space(10f)]
    public Image imgGlareOwn1;
    public Image imgGlareOwn2;

    private Tween tweenFxBuy;
    private Color colTrans = new Color(1f, 1f, 1f, 0f);
    
    public override void OnShow(int index, object data = null, UnityAction callback = null)
    {
        // first run
        if(this.tweenFxBuy == null)
        {
            this.InitAnimation();
            this.tabControlEachTier.changeTabEvent.RemoveListener(this.OnTabChanged);
            this.tabControlEachTier.changeTabEvent.AddListener(this.OnTabChanged);
            this.tabControlEachTier.ChangeTab(0);
            
            this.goPanels.SetActive(true);
            this.goTabs.SetActive(true);
        }
        
        base.OnShow(index, data, callback);
    }

    public void MoveToTab(TabName tab)
    {
        if ((int) tab > (int)TabName.Own)
        {
            Debug.LogException(new System.Exception("ShopCueTabContent MoveToTab error! Sub tab out of range: " + tab.ToString()));
            return;
        }
        
        if (tab != TabName.None)
        {
            this.tabControlEachTier.ChangeTab(((int) tab) - 1);
        }
    }
    
    private void InitAnimation()
    {
        this.tweenFxBuy = DOTween.Sequence()
            .AppendCallback(this.ResetFx)
            .Append(this.imgGlareOwn1.DOColor(Color.white, 0.25f))
            .Join(this.imgGlareOwn2.DOColor(Color.white, 0.25f))
            .Append(this.imgGlareOwn1.DOColor(this.colTrans, 1f))
            .Join(this.imgGlareOwn2.DOColor(this.colTrans, 1f))
            .SetAutoKill(false).Pause().SetId(this);
    }
    
    // animation open tab
    private void OnTabChanged(int index)
    {
        for (int i = 0; i < this.txtTabLabels.Length; ++i)
        {
            if (i == index)
            {
                this.txtTabLabels[i].color = Color.green;
            }
            else
            {
                this.txtTabLabels[i].color = this.colGrey;
            }
        }
    }
    
    // animation buy cue
    public void OnACueBought()
    {
        this.tabOwn.NeedToRefresh = true;
        if (this.gameObject.activeInHierarchy)
        {
            this.tweenFxBuy.Restart();
        }
    }

    private void ResetFx()
    {
        this.imgGlareOwn1.color = this.imgGlareOwn2.color = this.colTrans;
    }

    public override void OnHide(int index, object data = null, UnityAction callback = null)
    {
        base.OnHide(index, data, callback);

        if (this.tweenFxBuy != null)
        {
            this.tweenFxBuy.Complete();
        }
    }
}
