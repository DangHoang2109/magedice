using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class FindingOppPlayer : FindingPlayer
{
    [Header("Rolling")]
    public Transform tranRoll;

    //public Sprite[] sprAvatars;
    private int indexAvt; //tăng index để chạy finding

    public override void ParseData(PlayerModel model)
    {
        base.ParseData(model);
        this.tmpTrophy.text = "--";
        this.txtNickname.text = "----";
        this.tranRoll.localPosition = new Vector3(0, 160f);
        ShowAvatar(false); //ẩn avatar
    }

    public Sequence ShowRollingV2(float timeSpin = 3f)
    {
        //thời gian quay 1 vòng
        float timePer = 0.8f;
        int timeLoop = timeSpin > timePer ? (int)(timeSpin / timePer) : 1;
        this.tranRoll.localPosition = new Vector3(0, 120f);

        Sequence seq = DOTween.Sequence();
        seq.Append(this.tranRoll.DOLocalMoveY(-120f, timePer)
            .SetEase(Ease.Linear)
            .SetLoops(timeLoop)
            .OnComplete(() => { this.tranRoll.localPosition = Vector3.zero; }));

        //seq.Append(this.OffBtn(timeSpin - timeLoop * timePer));
        seq.Append(
           this.tranRoll.DOLocalMoveY(-120f * (timeSpin - timeLoop * timePer), timeSpin - timeLoop * timePer)
           .SetEase(Ease.Linear)
           .OnComplete(() => { this.tranRoll.localPosition = Vector3.zero; })
           );

        return seq;
    }
}
