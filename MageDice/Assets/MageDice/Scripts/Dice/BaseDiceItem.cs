using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
public class BaseDiceItem : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler,IPointerClickHandler
{

    private BaseDiceData data;
    public T GetData<T>() where T : BaseDiceData
    {
        return this.data as T;
    }

    [Header("UI")]
    public Image imgFront;

    public virtual void SetData<T>(T data) where T :BaseDiceData
    {
        this.data = data;
        this.interactState = STATE.IDDLE;
        Display();
    }
    public virtual void Display()
    {
        this.imgFront.sprite = this.data.Front;
    }

    #region Interact handler
    public enum STATE
    {
        DISABLE = 0,
        BLOCKING,
        IDDLE,
        DRAGING
    }
    public STATE interactState;
    protected Vector3 pointCurrent;
    protected Vector3 pointNew;

    protected Vector3 posStart;

    public virtual void OnBeginDrag(PointerEventData eventData)
    {
        if (interactState != STATE.IDDLE)
            return;

        this.interactState = STATE.DRAGING;
        OnCustomBeginDrag(eventData);
    }

    public virtual void OnDrag(PointerEventData eventData)
    {
        if (interactState == STATE.DRAGING)
        {
            OnCustomDrag(eventData);
        }
    }

    public virtual void OnEndDrag(PointerEventData eventData)
    {
        OnCustomEndDrag(eventData);
        this.interactState = STATE.IDDLE;
    }
    public void OnPointerClick(PointerEventData eventData)
    {
        OnCustomPointerClick(eventData);
    }
    protected virtual void OnCustomBeginDrag(PointerEventData eventData)
    {
        posStart = this.transform.position;
        pointCurrent = eventData.position;//Camera.main.ScreenToWorldPoint(eventData.position);
        //Debug.Log($"begin drag {posStart} {pointCurrent}");

    }
    protected virtual void OnCustomDrag(PointerEventData eventData)
    {
        pointNew = eventData.position;// Camera.main.ScreenToWorldPoint(eventData.position);
        //this.transform.position += pointNew - pointCurrent;
        this.transform.position = pointNew;
        pointCurrent = pointNew;

    }
    protected virtual void OnCustomEndDrag(PointerEventData eventData)
    {
    }
    protected virtual void OnCustomPointerClick(PointerEventData eventData)
    {
    }
    #endregion Interact handler
}
