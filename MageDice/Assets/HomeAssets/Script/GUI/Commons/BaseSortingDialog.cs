using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Canvas))]
[RequireComponent(typeof(GraphicRaycaster))]
public class BaseSortingDialog : BaseDialog
{
    [Header("Sorting order")]
    public Canvas canvas;
    public GraphicRaycaster raycaster;

#if UNITY_EDITOR
    protected override void OnValidate()
    {
        base.OnValidate();
        this.canvas = this.GetComponent<Canvas>();
        this.canvas.overrideSorting = true;
        this.raycaster = this.GetComponent<GraphicRaycaster>();
    }
#endif

    public void SetSortingOrder(int sorting)
    {
        this.canvas.overrideSorting = true;
        this.canvas.sortingOrder = sorting;
        this.canvas.sortingLayerName = LayerName.Popup;
    }
}
