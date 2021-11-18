using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class BagInfoPanel : MonoBehaviour
{
    [Header("Bag")]
    public Image imgBag;
    public TextMeshProUGUI tmpName;

    [Header("Loot")]
    public CardNumberItem cardNumberItem;
    public Transform tranLoots;
    public TextMeshProUGUI tmpBonus;
    private Dictionary<CardType, CardNumberItem> dicCardNumbers;

    public void ParseBag(BagType bagType, int tourId, string nameBag)
    {
        //Bag asset
        BagAssetConfig bagAsset = BagAssetConfigs.Instance.GetBagAsset(bagType);
        GiftBagPerTourConfig giftBagConfig = GiftBagConfigs.Instance.GetGiftBag(bagType, tourId);

        if (bagAsset != null)
        {
            this.imgBag.sprite = bagAsset.sprBag;
            this.tmpName.SetText(string.Format("{0} - {1}", bagAsset.name, nameBag!= null? nameBag: string.Format("TOUR {0}", tourId)));
        }

        if (giftBagConfig != null)
        {
            //parse number of cards of bar
            if (this.dicCardNumbers == null) this.dicCardNumbers = new Dictionary<CardType, CardNumberItem>();
            foreach (CardNumberItem cardNumber in this.dicCardNumbers.Values)
            {
                cardNumber.gameObject.SetActive(false);
            }

            foreach (CardAmount card in giftBagConfig.cardAmounts)
            {
                if (!this.dicCardNumbers.ContainsKey(card.cardType))
                {
                    this.dicCardNumbers.Add(card.cardType, Instantiate<CardNumberItem>(this.cardNumberItem, this.tranLoots));
                }
                this.dicCardNumbers[card.cardType].gameObject.SetActive(true);
                this.dicCardNumbers[card.cardType].ParseCardNumber(card);
            }
        } 
    }
}

