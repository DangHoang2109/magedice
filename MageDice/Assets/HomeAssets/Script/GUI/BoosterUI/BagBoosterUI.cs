using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;


public class BagBoosterUI : MonoBehaviour
{
    public Image imgBooster;
    public TextMeshProUGUI tmpValue;
    public BagAmount bag;

#if UNITY_EDITOR
    protected virtual void OnValidate()
    {
        if (this.tmpValue == null)
            this.tmpValue = this.transform.GetComponentInChildren<TextMeshProUGUI>();
        if (this.imgBooster == null)
            this.imgBooster = this.transform.GetComponentInChildren<Image>();
    }
#endif

    public virtual void ParseBag(BagAmount bag)
    {
        this.bag = bag;
        if (this.bag != null)
        {
            this.imgBooster.sprite = BagAssetConfigs.Instance.GetBagAsset(bag.bagType).sprBag;
            SetValueText(GameUtils.FormatMoneyDot(this.bag.amount));
        }
        this.gameObject.SetActive(this.bag != null);

    }
    protected virtual void SetValueText(string text, string prefix = "")
    {
        this.tmpValue.SetText($"{prefix}{text}");
    }
}
