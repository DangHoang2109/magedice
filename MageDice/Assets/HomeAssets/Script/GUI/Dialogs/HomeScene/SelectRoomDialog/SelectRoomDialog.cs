using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class SelectRoomDialog : BaseSortingDialog
{
    [Header("Background")]
    public Image[] imgBgs;

    [Header("Rooms")]
    public SelectRoomPageView roomPageView;

    //[Header("Bootom")]
    //public Transform tranBottom;
    //private int idShowBottom;

    public override void OnShow(object data = null, UnityAction callback = null)
    {
        base.OnShow(data, callback);


        //ParseConfigs();
        //StartCoroutine(IeShowBottom());
    }

    private void OnDisable()
    {

    }

#region BOTTOM
    //private IEnumerator IeShowBottom()
    //{
    //    yield return new WaitForSeconds(15f);
    //    ShowBottom(this.idShowBottom, false);
    //    this.idShowBottom += 1;
    //    if (this.idShowBottom >= this.tranBottom.childCount) this.idShowBottom = 0;
    //    yield return new WaitForSeconds(0.2f);
    //    ShowBottom(this.idShowBottom, true);
    //    StartCoroutine(IeShowBottom());
    //}

    //private void ShowBottom(int id, bool onShow)
    //{
    //    if (id < this.tranBottom.childCount)
    //    {
    //        Transform tranBot = this.tranBottom.GetChild(id);
    //        if (tranBot != null)
    //        {
    //            tranBot.gameObject.SetActive(onShow);
    //        }
            
    //    }
    //}
#endregion
}
