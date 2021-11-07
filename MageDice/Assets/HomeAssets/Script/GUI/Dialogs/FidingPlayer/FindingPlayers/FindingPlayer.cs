using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class FindingPlayer : PlayerCommon
{
    [Header("Player")]
    public Transform panel;
    public TextMeshProUGUI tmpTrophy;


    [Header("Money")]
    public Transform tranMoney;
    public TextCurrency txtMoney;

    protected virtual void OnEnable()
    {
        this.tranMoney.localScale = Vector3.zero;
    }

    public void ShowAvatar(bool isOn = true)
    {
        this.imgAvatar.enabled = isOn;
    }

    public void ShowTrophy(int trophy)
    {
        this.tmpTrophy.text = trophy.ToString();
    }


    public void ShowInfo()
    {
        if (this.model != null)
        {
            //show nickname, avatar
            SetNickname(this.model.info.nickname);
            SetAvatar(this.model.info.SprAvatar);
            ShowAvatar();

            StandardPlayer standardModel = this.model as StandardPlayer;
            if (standardModel != null)
            {
                //trophy
                ShowTrophy(standardModel.trophy);
            }
        } 
    }

    public Sequence ShowMoney(long money, float time=0.5f)
    {
        Debug.Log("<color=yellow>Money: </color>"+ money);
        this.txtMoney.tmpValue.text = money.ToString();
        Sequence seq = DOTween.Sequence();
        seq.AppendCallback(()=>this.tranMoney.gameObject.SetActive(false));
        seq.AppendInterval(time / 3f);//delay để update size
        seq.AppendCallback(() => this.tranMoney.gameObject.SetActive(true));
        seq.Append(this.tranMoney.DOScale(1f, time));
        return seq;
    }

    public virtual Tween TakeAllMoney(long playerMoney, float time=0.5f)
    {
        return this.txtMoney.AddValueAnimtion(playerMoney, 0, time);
    }

    public virtual Tween HidePrize()
    {
        return this.tranMoney.DOScale(0f, 0.3f).SetEase(Ease.OutBack);
    }
}
