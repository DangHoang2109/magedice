using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class StoreItem : MonoBehaviour
{
    [Header("Rectransform")]
    public RectTransform rectTrans;

    [Header("Button")]
    public IBooster bstPrice;

#if UNITY_EDITOR
    private void OnValidate()
    {
        this.rectTrans = this.GetComponent<RectTransform>();
    }
#endif

    public virtual void ParseConfig(StoreItemConfig config)
    {
    }

    public virtual Tween AnimShow()
    {
        this.transform.localScale = Vector3.zero;
        return this.transform.DOScale(1, 0.2f).SetEase(Ease.Linear).SetId(this);
    }

    public virtual Tween AnimHide()
    {
        return this.transform.DOScale(0, 0.2f).SetEase(Ease.Linear).SetId(this);
    }

    public void ShowPriceText(string prize)
    {
        this.bstPrice.tmpValue.SetText(prize);
        this.bstPrice.imgBooster.enabled = false;
    }

    public void ShowPrice(BoosterCommodity bst)
    {
        this.bstPrice.imgBooster.enabled = true;
        this.bstPrice.ParseBooster(bst);
    }

    public virtual void OnClickBuy()
    {
        //Debug.LogError("Onclick buy");
        SoundManager.Instance.PlayButtonClick();

    }

    protected virtual void BuySuccess()
    {
        SoundManager.Instance.Play("snd_buy_success");

    }
}
