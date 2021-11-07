using System.Collections;
using System.Collections.Generic;
using Cosina.Components;
using UnityEngine;
using UnityEngine.EventSystems;

public class CanvasBlocker : MonoSingleton<CanvasBlocker>, IPointerDownHandler
{
    public override void Init()
    {
        base.Init();
        this.gameObject.SetActive(false);
        this.enabled = false;
    }

    public void SetActive(bool isEnable)
    {
        this.gameObject.SetActive(isEnable);
    }


    public void OnPointerDown(PointerEventData eventData)
    {
        Debug.Log("BLOCKED".WrapColor("red") + " a click!");
    }
}
