using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class TabContent : MonoBehaviour
{
    [SerializeField]
    protected int tabIndex;
    public Transform panel;
    protected object data;
    public int TabIndex => this.tabIndex;
    protected bool isShow = false;

    protected virtual void Start()
    {
        this.OnInit();
    }

    protected virtual void OnEnable()
    {}

    protected virtual void OnDisable()
    {}

    public virtual void OnInit()
    { }
    public virtual void OnShow(int index, object data = null, UnityAction callback = null)
    {
        this.data = data;
        this.panel.gameObject.SetActive(true);
        this.AnimationShow();
    }
    protected virtual void AnimationShow()
    { }
    protected virtual void OnCompleteShow()
    { }
    public virtual void OnHide(int index, object data = null, UnityAction callback = null)
    {
        this.AnimationHide();
    }
    protected virtual void AnimationHide()
    {
    }
    protected virtual void OnCompleteHide()
    {
        this.panel.gameObject.SetActive(false);
    }
    public virtual void Clear()
    {
        this.panel.gameObject.SetActive(false);
    }
}
