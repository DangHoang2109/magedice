using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class SpecialOfferItem : StorePackageItem
{
    [Header("Time")]
    public TextMeshProUGUI tmpTime;

    private double timeRemain;

    protected StoreSpecialPackageConfig specialConfig;

    public System.Action cbCloseDialog;

    public override void ParseConfig(StoreItemConfig config)
    {
        //TODO parse special
        this.specialConfig = config as StoreSpecialPackageConfig;

        base.ParseConfig(config);  

        if (this.specialConfig != null)
        {
            this.tmpName.text = this.specialConfig.Title; //TODO language
            this.tmpContains.text = this.specialConfig.name; //TODO langue
        }
    }

    private void Update()
    {
        //TODO update time
        if (StoreSpecialData.Instance.IsStillTimeOfSpeacial(ref this.timeRemain))
        {
            this.tmpTime.text = GameUtils.ConvertFloatToTime(timeRemain, "dd'd'hh'h'mm'm'");
        }
        else
        {
            //TODO show time of speacial
            cbCloseDialog?.Invoke();
        }
    }

    public override void OnClickBuy()
    {
        base.OnClickBuy();
    }

    protected override void BuySuccess()
    {
        base.BuySuccess();
        StoreSpecialData.Instance.BuySuccess();
        cbCloseDialog?.Invoke();
    }
}
