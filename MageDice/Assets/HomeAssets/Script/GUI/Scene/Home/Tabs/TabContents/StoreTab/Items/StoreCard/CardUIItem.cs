using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class CardUIItem : MonoBehaviour
{
    [Header("Card")]
    public TextMeshProUGUI tmpName;
    public Image imgCard;

    [Header("Value")]
    public Transform tranAmount;
    public TextMeshProUGUI tmpAmount;

    public void ShowCard(CardAmount cardAmount, string name)
    {
        TierAssetConfig bagAsset = TierAssetConfigs.Instance.GetCardAsset(cardAmount.tier);
        if (bagAsset != null)
        {
            this.imgCard.sprite = bagAsset.sprBackIcon;

            if (tmpName != null)
            {
                if (string.IsNullOrEmpty(name))
                    this.tmpName.SetText(string.Format(("{0}"), bagAsset.name));
                else
                    this.tmpName.SetText(string.Format(("{0} - {1}"), bagAsset.name, name));
            }
        }
        this.tmpAmount.SetText(string.Format("x{0}", cardAmount.amount));
    }

    public void ShowBagWithNameTour(CardAmount bagAmount)
    {
        string nameTour = string.Format("TOUR {0}", RoomDatas.Instance.GetNumTour());
        ShowCard(bagAmount, nameTour);
    }
    public void ShowCardWithName(CardAmount bagAmount)
    {
        ShowCard(bagAmount, "");
    }
    public void OnTranAmount(bool isOn)
    {
        this.tranAmount.gameObject.SetActive(isOn);
    }
}
