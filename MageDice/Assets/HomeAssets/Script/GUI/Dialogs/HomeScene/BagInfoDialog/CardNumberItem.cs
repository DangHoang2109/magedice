using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class CardNumberItem : MonoBehaviour
{
    public Image imgCard;
    public TextMeshProUGUI tmpName;
    public TextMeshProUGUI tmpNumber;

    private CardAssetConfig cardAsset;

    public void ParseCardNumber(CardAmount cardNumber)
    {
        Debug.Log("edit");
        //cardAsset = CardAssetConfigs.Instance.GetCardAsset(cardNumber.cardType);
        //if (cardAsset != null)
        //{
        //    this.imgCard.sprite = cardAsset.sprCard;
        //    this.tmpName.SetText(cardAsset.name);
        //    this.tmpNumber.SetText(string.Format("X{0}", cardNumber.amount));
        //}
    }

    public void ChangeColorNameText()
    {
        if (cardAsset != null)
        {
            this.tmpName.color = cardAsset.color;
        } 
    }
}
