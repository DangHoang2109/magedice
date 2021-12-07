using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class TabBase : MonoBehaviour
{
    public List<Tab> tabs;
    public List<TabContent> contents;
    protected TabContent current;
    protected int tabCurrentIndex = -1;


    protected int nextTab = 0;
    protected object nextData = null;

    protected bool _isInitialized = false;

    public CallbackEventInt changeTabEvent = new CallbackEventInt();

    [SerializeField]
    private bool isCallInitAtStart = false;

    protected virtual void Start()
    {
        if(isCallInitAtStart)
            this.Init();
    }
    public virtual void Init()
    {
        foreach (Tab tab in this.tabs)
        {
            tab.Init(this);
        }
        this._isInitialized = true;

        this.ChangeTab(nextTab, nextData);
    }
    
    public virtual void ChangeTab(int index, object data = null)
    {
        if(!this._isInitialized)
        {
            this.nextTab = index;
            this.nextData = data;
            return;
        }

        if(this.tabCurrentIndex == index)
        {
            return;
        }
        
        if(this.current != null)
        {
            this.current.OnHide(index);
        }
        Tab tab = this.tabs.Find(x => x.tabIndex == index);
        if(tab != null)
        {
            tab.isOn = true;
        }
        TabContent content = this.contents.Find(x => x.TabIndex == index);
        if(content != null)
        {
            content.gameObject.SetActive(true);
            content.OnShow(this.tabCurrentIndex, data);
            this.current = content;
        }
        this.tabCurrentIndex = index;

        this.changeTabEvent?.Invoke(index);
    }

    public TabContent GetTabContent(int index)
    {
        if (this.contents != null)
        {
            if (index < this.contents.Count)
            {
                return this.contents[index];
            }
        }
        return null;
    }

    public Tab GetTab(int index)
    {
        if (this.tabs != null)
        {
            if (index < this.tabs.Count)
            {
                return this.tabs[index];
            }
        }
        return null;
    }
}
