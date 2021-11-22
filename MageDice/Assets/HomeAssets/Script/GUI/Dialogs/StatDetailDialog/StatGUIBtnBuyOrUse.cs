using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class StatGUIBtnBuyOrUse : MonoBehaviour
{
    public GameObject goBuy;
    public IBooster uiBooster;
    
    public GameObject goUse;
    public GameObject goUsing;

    public void ParseDisable()
    {
        this.goBuy.SetActive(false);
        this.goUse.SetActive(false);
        this.goUsing.SetActive(false);
    }
    
    public void ParseToBuy(long price, StatManager.UnlockType type)
    {
        this.goBuy.SetActive(true);
        this.goUse.SetActive(false);
        this.goUsing.SetActive(false);

        BoosterType boosterType;
        switch (type)
        {
            case StatManager.UnlockType.Coin:
                boosterType = BoosterType.COIN;
                break;
            case StatManager.UnlockType.Cash:
                boosterType = BoosterType.CASH;
                break;
            default:
                boosterType = BoosterType.NONE;
                Debug.LogException(new System.Exception("ShopCueBtnBuy: type not supported: " + type.ToString()));
                return;
        }
        this.uiBooster.ParseBooster(new BoosterCommodity(boosterType, price));
    }

    public void ParseCueBought(bool isUsing)
    {
        this.goBuy.SetActive(false);
        
        this.goUse.SetActive(!isUsing);
        this.goUsing.SetActive(isUsing);
    }
}
