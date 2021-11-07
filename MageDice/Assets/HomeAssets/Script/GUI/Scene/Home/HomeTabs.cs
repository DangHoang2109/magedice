using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class HomeTabs : MonoSingleton<HomeTabs>
{
    [Header("Tab and page view")]
    public TabAndPageView tabAndPage;

    [Header("Tab UIs")]
    public HomeBottomTabUI[] tabUis;

    [Header("Block raycast")]
    public Transform blockRaycast;

    [Header("Canvas group")]
    public CanvasGroup canMid;
    public CanvasGroup canTop;
    public CanvasGroup canBottom;


    public static bool isShowPass = false;

    public Transform tfButtonMission;
    public Transform tfIconPlayGame;

    public UnityAction<HomeTabName> eventChangedTab;

    public override void Init()
    {
        base.Init();
    }

    public static void AssignCbAnniPass()
    {
        HomeTabs.isShowPass = true;
    }
    public void ShowFxCollectPass()
    {
        StartCoroutine(ieWait(() =>
        {
            FxHelper.Instance.ShowFxCollectBySprite(GameAssetsConfigs.Instance.sprPointBattePass, tfIconPlayGame, tfButtonMission);
            HomeTabs.isShowPass = false;
        }, 0.5f));

    }
    public IEnumerator ieWait(System.Action callback, float time = -1f)
    {
        if(time <= 0)
            callback?.Invoke();
        else
        {
            yield return new WaitForSeconds(time);
            callback?.Invoke();
        }

    }

#if UNITY_EDITOR
    private void OnValidate()
    {
        this.tabUis = this.GetComponentsInChildren<HomeBottomTabUI>();
    }
#endif

    public void Start()
    {
        this.tabAndPage.Init(OnChangeTab);
        this.SetBlockInput(false);

        if (HomeTabs.isShowPass)
            ShowFxCollectPass();

        //callbackAddPointBattlePass?.Invoke();
    }

    public void SetBlockInput(bool block)
    {
        this.blockRaycast.gameObject.SetActive(block);
    }

    private void OnChangeTab(int indexTab)
    {
        if (this.tabUis != null)
        {
            foreach (HomeBottomTabUI tabUI in this.tabUis)
            {
                tabUI.ShowSelectTab(indexTab);
            }
        }

        //event changed tab
        HomeTabName tabName = (HomeTabName)indexTab;
        this.eventChangedTab?.Invoke(tabName);
    }

    public void MoveToTab(HomeTabName tabName)
    {
        this.tabAndPage.LerpToPage((int)tabName);
    }

    public T GetTabContent<T>(HomeTabName tabName) where T: TabContent
    {
        TabContent tabContent = this.tabAndPage.tabBase.GetTabContent((int)tabName);
        if (tabContent!=null)
            return (T)tabContent;
        return null;
    }

    public T GetTab<T>(HomeTabName tabName) where T : Tab
    {
        Tab tab = this.tabAndPage.tabBase.GetTab((int)tabName);
        if (tab != null)
            return (T)tab;
        return null;
    }


    public void ShowLayout(LayoutHome layout, bool isOn)
    {
        CanvasGroup canvasGroup = null;
        switch (layout)
        {
            case LayoutHome.LayoutTop:
                canvasGroup = this.canTop;
                break;
            case LayoutHome.LayoutMid:
                canvasGroup = this.canMid;
                break;
            case LayoutHome.LayoutBottom:
                canvasGroup = this.canBottom;
                break;
        }
        if (canvasGroup!=null) canvasGroup.alpha = isOn ? 1f : 0f;
    }

    public enum LayoutHome
    {
        LayoutTop,
        LayoutMid,
        LayoutBottom
    }
}



public enum HomeTabName //sắp xếp theo index tab
{
    SOCIAL =0,
    STORE =1,
    MAIN =2,
    SHOP_CUE =3,     //LINEUP =3,
    EVENT =4
}
