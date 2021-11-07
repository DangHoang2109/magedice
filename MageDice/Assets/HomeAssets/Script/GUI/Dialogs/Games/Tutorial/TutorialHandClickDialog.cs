using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class TutorialHandClickDialog : TutorialClickDialog
{
    
    [SerializeField]
    protected Image imgHand;

    private RectTransform transHand;
    // [Tooltip("distance in world coordinate")]
    // public float distanceMoveHand;

    public RectTransform TransHand
    {
        get
        {
            if (this.transHand == null)
                this.transHand = this.imgHand.rectTransform;
            return this.transHand;
        }
    }

    private bool isHandMoving = false;
    //private bool isBlockAllRaycast = false;

    private bool isBlockAllRaycast = false;
    /// <summary>
    /// auto reset to false if you call OnTutorial
    /// </summary>
    public bool IsBlockAllRaycast
    {
        get
        {
            if (this.imgboder != null)
            {
                return this.isBlockAllRaycast;
            }
            return false;
        }

        set
        {
            if (this.imgboder != null)
            {
                this.isBlockAllRaycast = value;
                if (value)
                {
                    this.imgboder.raycastTarget = true;
                }
            }
        }
    }

    public override void OnTutorial(Vector3 pos, Vector3 size, UnityAction callback = null)
    {
        if (this.imgboder != null)
        {
            this.imgboder.raycastTarget = false;
        }
        
        base.OnTutorial(pos, size, callback);
    }

    /// <summary>
    /// must be called after OnTutorial
    /// </summary>
    /// <param name="time">near 0 mean instantly</param>
    public void MoveHand(float time)
    {
        if(time < 0.001f)
        {
            this.TransHand.position = this.rectBt.position;
        }
        else
        {
            this.isHandMoving = true;
            if(this.imgboder != null)
            {
                this.imgboder.raycastTarget = true;
            }
            this.transHand.DOKill(false);
            this.TransHand.DOMove(this.rectBt.position, time)
                .OnComplete(this.OnHandMoved);
        }
    }

    private void OnHandMoved()
    {
        this.isHandMoving = false;
        if (!this.isBlockAllRaycast && this.imgboder != null)
        {
            this.imgboder.raycastTarget = false;
        }
    }

    protected override void Update()
    {
        if (!this.isHandMoving)
        {
            base.Update();
        }
    }
    protected override void OnCompleteShow()
    {
        if (this.callbackShow != null)
        {
            var bk = this.callbackShow;
            this.callbackShow = null;
            bk.Invoke();
        }
        
        this.OnShowed?.Invoke();
    }
}
