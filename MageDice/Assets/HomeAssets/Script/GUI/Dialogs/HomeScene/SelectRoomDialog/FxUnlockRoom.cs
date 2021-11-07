using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class FxUnlockRoom : MonoBehaviour
{
    [Header("Locks")]
    //public Image imgBg;
    public CanvasGroup cgBg;
    public Image imgLock;
    public CanvasGroup canPieces;
    public Image[] imgPieces;

    [Header("Particle")]
    public Transform[] tranPars;

    public void ShowEffect(int roomID, Vector3 posLock, UnityAction callback = null)
    {
        //Room bg (không dùng)
        //RoomAssetConfig roomAsset = RoomAssetConfigs.Instance.GetRoomAsset(roomID);
        //this.imgBg.sprite = roomAsset.sprBg;  

        this.imgLock.transform.localPosition = this.transform.InverseTransformPoint(posLock);
        this.imgLock.transform.localScale = new Vector3(0.3f, 0.3f);
        this.imgLock.enabled = false;

        this.canPieces.alpha = 0f;
        this.cgBg.alpha = 0f;

        Sequence seq = DOTween.Sequence();
        seq.AppendInterval(0.3f);
        seq.AppendCallback(() => {
            this.imgLock.enabled = true;
            this.tranPars[0].gameObject.SetActive(true);
            SoundManager.Instance.Play("snd_finding");
            });  //par random
        seq.Append(this.imgLock.transform.DOLocalMoveX(0, 0.5f));
        seq.Join(this.imgLock.transform.DOLocalMoveY(0, 0.5f).SetEase(Ease.OutBack));
        seq.Join(this.imgLock.transform.DOScale(Vector3.one, 1f));
        seq.Join(this.cgBg.DOFade(1f, 1f));
        seq.AppendCallback(()=> {
            this.tranPars[0].gameObject.SetActive(false);
            this.tranPars[1].gameObject.SetActive(true);
            SoundManager.Instance.Play("snd_RandomCard");
        }); //par hút vào
        seq.Append(this.imgLock.transform.DOPunchPosition(new Vector3(20, 20), 2f, 30).SetEase(Ease.Linear));
        seq.Join(this.tranPars[1].DOLocalRotate(new Vector3(0, 0, 180), 2f, RotateMode.FastBeyond360).SetEase(Ease.Linear)); //quay par hút vào
        seq.AppendCallback(() => {
            this.imgLock.enabled = false;
            this.canPieces.alpha = 1f;
            this.tranPars[0].gameObject.SetActive(false);
            this.tranPars[1].gameObject.SetActive(false);
        });
        seq.Join(this.cgBg.DOFade(0f, 0.5f));
        seq.AppendCallback(() =>
        {
            callback?.Invoke();
            this.tranPars[2].gameObject.SetActive(true); //par tỏa ra
            SoundManager.Instance.Play("snd_OpenCard");
        }
        );
        seq.Join(this.imgPieces[0].transform.DOLocalMove(new Vector3(-100, 100), 0.8f).SetEase(Ease.OutBack));
        seq.Join(this.imgPieces[1].transform.DOLocalMove(new Vector3(100, -100), 0.8f).SetEase(Ease.OutBack));
        seq.Join(this.canPieces.DOFade(0, 1f));
        seq.AppendInterval(1f);
        seq.SetId(this);
        seq.OnComplete(() =>
        {
            Destroy(this.gameObject);
        });
    }
}
