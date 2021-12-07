using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class BagUIItem : MonoBehaviour
{
    [Header("Bag")]
    public TextMeshProUGUI tmpName;
    public Image imgBag;

    [Header("Value")]
    public Transform tranAmount;
    public TextMeshProUGUI tmpAmount;

    public void ShowBag(BagAmount bagAmount, string bagName)
    {
        BagAssetConfig bagAsset = BagAssetConfigs.Instance.GetBagAsset(bagAmount.bagType);
        if (bagAsset != null)
        {
            this.imgBag.sprite = bagAsset.sprBag;

            if(tmpName != null)
                this.tmpName.SetText(string.Format(("{0} - {1}"),bagAsset.name, bagName));
        }
        this.tmpAmount.SetText(string.Format("x{0}", bagAmount.amount));
    }

    public void ShowBagWithNameTour(BagAmount bagAmount)
    {
        string nameTour = string.Format("TOUR {0}", RoomDatas.Instance.GetNumTour());
        ShowBag(bagAmount, nameTour);
    }

    public void OnTranAmount(bool isOn)
    {
        this.tranAmount.gameObject.SetActive(isOn);
    }
}
