using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RewardItemUI : MonoBehaviour
{
    public CardUIItem card;
    public BagUIItem bag;
    public BoosterUIItem booster;

    public CardUIItem Card => card;
    public BagUIItem Bag => bag;
    public BoosterUIItem Booster => booster;

    public void DisableAll()
    {
        card.gameObject.SetActive(false);
        bag.gameObject.SetActive(false);
        booster.gameObject.SetActive(false);
    }
    public void Show(object data, string name)
    {
        this.gameObject.SetActive(true);

        if (data is CardAmount)
            ShowCard(data as CardAmount, name);

        else if (data is BagAmount)
            ShowBag(data as BagAmount, name);

        else if (data is BoosterCommodity)
            ShowBooster(data as BoosterCommodity, true);
    }
    public void ShowCard(CardAmount cardAmount, string name)
    {
        DisableAll();
        card.gameObject.SetActive(true);

        card.ShowCard(cardAmount, name);
    }
    public void ShowBag(BagAmount bagAmount, string name)
    {
        DisableAll();
        bag.gameObject.SetActive(true);

        bag.ShowBag(bagAmount, name);
    }
    public void ShowBooster(BoosterCommodity boosterAmount, bool dynamicIcon)
    {
        DisableAll();
        booster.gameObject.SetActive(true);

        booster.ShowBooster(boosterAmount, dynamicIcon);
    }
}
