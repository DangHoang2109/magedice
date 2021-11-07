using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using UnityEngine.Events;

public class BoosterTopUI : MonoBehaviour
{
    [SerializeField]
    private TextCurrency txtCoin;

    [SerializeField]
    private BoosterType type;

    public TextCurrency TxtCoin { get => txtCoin; set => txtCoin = value; }

#if UNITY_EDITOR
    private void OnValidate()
    {
        this.txtCoin = this.GetComponentInChildren<TextCurrency>();
    }
#endif

    protected virtual void OnEnable()
    {
        UserProfile.Instance.AddCallbackBooster(type, this.OnChangeValue);
    }
    protected virtual void OnDisable()
    {
#if UNITY_EDITOR
        if (GameManager.isApplicationQuit)
            return;
#endif

        UserProfile.Instance.RemoveCallbackBooster(type, this.OnChangeValue);
    }

    private void OnChangeValue(BoosterCommodity booster)
    {
        if (booster != null)
        {
            this.ParseValue(booster.GetValue());
        }         
    }

    protected virtual void ParseValue(long coins)
    {
        Debug.Log("<color=yellow>Parse booster value </color>" + coins);
        this.TxtCoin.ParseData(coins);
    }

    public void OnClick()
    {
        switch (this.type)
        {
            case BoosterType.CASH:
                HomeTabs.Instance.MoveToTab(HomeTabName.STORE);
                //HomeTabs.Instance.GetTabContent<StoreTabContent>(HomeTabName.STORE)?.MoveToTab(StoreTabName.CASHS);
                break;
            case BoosterType.COIN:
                HomeTabs.Instance.MoveToTab(HomeTabName.STORE);
                //HomeTabs.Instance.GetTabContent<StoreTabContent>(HomeTabName.STORE)?.MoveToTab(StoreTabName.COINS);
                break;
        }
        SoundManager.Instance.PlayButtonClick();
    }

    public Transform GetTranIcon()
    {
        return this.txtCoin.imgIcon.transform;
    }
}
