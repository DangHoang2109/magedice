using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class TabAndPageView : MonoBehaviour
{
    [Header("Tab base")]
    public TabBase tabBase;

    [Header("Page view")]
    public PageView pageView;

    private UnityAction<int> callbackChangeTab;

    private void OnEnable()
    {
        this.pageView.changePageEvent.AddListener(OnChangePage);
        this.tabBase.changeTabEvent.AddListener(OnChangeTab);
    }
    private void Start()
    {
        this.tabBase.Init();
    }
    public void Init(UnityAction<int> callback = null)
    {
        this.callbackChangeTab = callback;

        this.pageView.startingPage = (int)HomeTabName.MAIN;
        this.pageView._Start();
        this.OnChangePage(this.pageView.startingPage);
    }

    private void OnDisable()
    {
        this.pageView.changePageEvent.RemoveListener(OnChangePage);
        this.tabBase.changeTabEvent.RemoveListener(OnChangeTab);
    }

    private void OnChangePage(int indexPage)
    {
        //Debug.LogError("Change page " + indexPage);
        this.tabBase.ChangeTab(indexPage);
        this.callbackChangeTab?.Invoke(indexPage);
    }

    private void OnChangeTab(int indexTab)
    {
        //Debug.LogError("Chagne tab " + indexTab);
        this.pageView.SetPage(indexTab);
        this.callbackChangeTab?.Invoke(indexTab);
    }

    public void LerpToPage(int id)
    {
        this.pageView.LerpToPage(id);
    }

    public void ChangeTab(int id)
    {
        this.tabBase.ChangeTab(id);
    }
}
