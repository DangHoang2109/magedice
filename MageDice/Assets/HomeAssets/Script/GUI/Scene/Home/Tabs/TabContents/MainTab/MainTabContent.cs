using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class MainTabContent : TabContent
{
    [Header("For animation show")]
    public Transform tranRewardTop;  //scale ra
    public Transform tranFeatureTop; //đi từ trên xuống
    public Transform tranLeft;       //đi từ trái qua
    public Transform tranRight;      //đi từ phải qua
    public Transform btPlay;         //scale ra
    //bag  hiển thị từng cái 1


    [Header("Rooms")]
    public SelectRoomPageView roomPageView; //rooms - anim scale to ra

    private SelectRoomDialog selectRoomDialog;

    // added by mr K
    public System.Action onClickPlay;


    public override void OnShow(int index, object data = null, UnityAction callback = null)
    {
        base.OnShow(index, data, callback);

        //effect show tab
        this.tranRewardTop.localScale = Vector3.zero;
        this.tranLeft.localPosition = new Vector3(-150, 0);
        this.tranRight.localPosition = new Vector3(150, 0);
        this.tranFeatureTop.localPosition = new Vector3(0, 70);

        DOTween.Kill(this);
        Sequence seq = DOTween.Sequence();
        seq.Join(this.tranRewardTop.DOScale(1, 0.2f).SetEase(Ease.Linear));
        seq.Join(this.tranLeft.DOLocalMoveX(0, 0.2f).SetEase(Ease.Linear));
        seq.Join(this.tranRight.DOLocalMoveX(0, 0.2f).SetEase(Ease.Linear));
        seq.Join(this.tranFeatureTop.DOLocalMoveY(0, 0.2f).SetEase(Ease.Linear));
        seq.SetId(this);
    }


    #region Select Room Page View

    /// <summary>
    /// Lấy transform bt play current room (for tut)
    /// </summary>
    /// <returns></returns>
    public Transform GetTranBtPlayCurRoom()
    {
        RoomItem curRoomItem = this.roomPageView.GetCurRoomItem();
        if (curRoomItem != null)
        {
            return curRoomItem.tranBtPlay;
        }
        return null;
    }

    #endregion

    #region Select Room Dialog
    public void SetSelecetRoomDialog(SelectRoomDialog selectRoom)
    {
        this.selectRoomDialog = selectRoom;
    }

    public override void OnHide(int index, object data = null, UnityAction callback = null)
    {
        base.OnHide(index, data, callback);
        this.selectRoomDialog?.OnHide();
    }
    #endregion
}
