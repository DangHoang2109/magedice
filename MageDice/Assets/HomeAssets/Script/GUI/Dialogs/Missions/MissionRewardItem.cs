using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MissionRewardItem : TextCurrency
{
    public override void ParseData(BoosterCommodity booster)
    {
        base.ParseData(booster);
    }

    public void ParseValueText(long value)
    {
        UpdateText(value);
    }
}
