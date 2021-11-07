using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;

[RequireComponent(typeof(BoxCollider2D))]
public class BaseBubble : MonoBehaviour
{
    public BoxCollider2D boxCol;

    private bool onShow;

#if UNITY_EDITOR
    protected virtual void OnValidate()
    {
        this.boxCol = this.GetComponentInChildren<BoxCollider2D>();
        RectTransform rect = this.GetComponent<RectTransform>();

        //auto calculate box collider
        this.boxCol.size = rect.sizeDelta;
        this.boxCol.offset = new Vector2((0.5f - rect.pivot.x) * rect.sizeDelta.x, (0.5f - rect.pivot.y) * rect.sizeDelta.y);
    }
#endif

    private void Update()
    {
        //raycast when mouse down
        if (onShow)
        {
            if (Input.GetMouseButtonDown(0))
            {
                //check input rayscast object khong
                if (checkTouch(Input.mousePosition) != this.boxCol.gameObject)
                {
                    HideBubble();
                }
            }
        }
    }

    GameObject checkTouch(Vector3 pos)
    {
        Vector3 wp = Camera.main.ScreenToWorldPoint(pos);
        Vector2 touchPos = new Vector2(wp.x, wp.y);
        GameObject ObjPointer = null;
        if (Physics2D.OverlapPoint(touchPos))
            ObjPointer = Physics2D.OverlapPoint(touchPos).gameObject;

        return ObjPointer;

    }

    private void OnDisable()
    {
        DOTween.Kill(this);
    }

    /// <summary>
    /// Được gọi để show
    /// </summary>
    public virtual void ShowBubble(Vector3 pos, UnityAction callback= null)
    {
        //TODO show bubble
        this.onShow = false;
        this.gameObject.SetActive(true);
        this.gameObject.transform.position = pos;
        this.gameObject.transform.localScale = Vector3.zero;
        DOTween.Kill(this);
        this.transform.DOScale(1f, 0.2f).OnComplete(() => {
            this.onShow = true;
            callback?.Invoke();
            }).SetId(this);
    }

    /// <summary>
    /// Event trigger mouse click exit
    /// </summary>
    public virtual void HideBubble()
    {
        this.onShow = false;
        //TODO hide bubble
        this.transform.DOScale(0f, 0.2f).OnComplete(() =>
        {     
            this.gameObject.SetActive(false);
        }   
        ).SetId(this); 
    }
}
