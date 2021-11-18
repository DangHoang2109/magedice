using Cosina.Components;
using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class StoreTabContent : TabContent
{
    [Header("Ver layout")]
    public VerticalLayoutGroup verticalLayout; 

    [Header("Child tabs")]
    public StoreChildTab[] childTabs;

    [Header("Scroll")]
    public ScrollRectEx scroll;
    public RectTransform rectScroll;

    public Dictionary<StoreTabName, StoreChildTab> dicChildTabs;

    //for move tab
    private Dictionary<StoreTabName, float> disPosItems;

#if UNITY_EDITOR
    private void OnValidate()
    {
        this.childTabs = this.GetComponentsInChildren<StoreChildTab>();
        this.rectScroll = this.scroll.GetComponent<RectTransform>();
    }
#endif

    protected override void OnEnable()
    {
        base.OnEnable();
        if (this.dicChildTabs == null)
        {
            this.dicChildTabs = new Dictionary<StoreTabName, StoreChildTab>();
            if (this.childTabs != null)
            {
                foreach (StoreChildTab childTab in this.childTabs)
                {   
                    this.dicChildTabs.Add(childTab.storeTab, childTab);
                    if (childTab.storeTab == StoreTabName.SPECIAL_OFFER)
                    {
                        StorePackageTab storePackageTab = (childTab as StorePackageTab);
                        if (storePackageTab!=null)
                            storePackageTab.callbackParseSpecial += ReEnableLayoutGroup;
                    }
                }
            }
        }
        ReEnableLayoutGroup();
    }

    public void ReEnableLayoutGroup()
    {
        StartCoroutine(IeReEnableVerLayout());
    }

    private IEnumerator IeReEnableVerLayout()
    {
        this.verticalLayout.enabled = false;
        yield return new WaitForEndOfFrame();
        this.verticalLayout.enabled = true;
    }

    [ContextMenu("ForceReset")]
    public void ForceResetDeal()
    {
        StoreDealsData.Instance.ForceResetDeal();
        StorePackagesData.Instance.ForceResetDeal();
    }

    public override void OnShow(int index, object data = null, UnityAction callback = null)
    {
        base.OnShow(index, data, callback);
        _CalPosItems();
        ShowAnimByRect();

        //LogGameAnalytics.Instance.LogEvent(LogAnalyticsEvent.OPEN_SHOP);
    }

    public override void OnHide(int index, object data = null, UnityAction callback = null)
    {
        base.OnHide(index, data, callback);
        //foreach (StoreChildTab childTab in this.childTabs)
        //{
        //    childTab.OnHideTab();
        //}
    }

    /// <summary>
    /// Tính vị trí của các tab
    /// </summary>
    private void _CalPosItems()
    {
        if (this.disPosItems== null)
        {
            this.disPosItems = new Dictionary<StoreTabName, float>();
            RectTransform rectPanel = this.panel.GetComponent<RectTransform>();
            float posMax = this.scroll.content.rect.height - rectPanel.rect.height / 2;
            if (this.childTabs != null)
            {
                foreach (StoreChildTab childTab in this.childTabs)
                {
                    float posY = rectPanel.rect.height / 2 - childTab.GetRectSize().y / 2
                - childTab.transform.localPosition.y;

                    if (posY > posMax) posY = posMax;
                    this.disPosItems.Add(childTab.storeTab, posY);
                }
            }
        }
    }

    public void MoveToTab(StoreTabName tab, float speed = 25f) //speed: 20 ~ 50
    {
        if (this.disPosItems != null)
        {
            if (this.disPosItems.ContainsKey(tab))
            {
                float posY = this.disPosItems[tab];
                float time = Mathf.Abs(((this.scroll.content.localPosition.y - posY) / speed)* Time.deltaTime);

                DOTween.Kill(this + "Move");
                this.scroll.content.DOLocalMoveY(posY, time)
                .OnComplete(() => { scroll.velocity = Vector2.zero;})
                .SetId(this + "Move");
            }
        }
    }

    private void ShowAnimByRect()
    {
        //show anim tab nằm trong viewport
        foreach (StoreChildTab childTab in this.childTabs)
        {
            //gửi rect viewport
            childTab.OnShowTab(this.scroll.viewport, this.rectScroll);
        }
    }
}
