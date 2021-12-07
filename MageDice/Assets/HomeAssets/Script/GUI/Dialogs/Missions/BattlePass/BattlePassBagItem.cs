using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class BattlePassBagItem : BattlePassItem
{
    public TextMeshProUGUI txtBagName;

    public TextMeshProUGUI txtBagTour;

    public TextMeshProUGUI txtBagAmount;
    public TextResize txtResize;

    public override void ParseData(BattlepassStepData stepDatas, BattlepassStepData.StepReward datas, bool isBattlePro, bool isUnlock)
    {
        base.ParseData(stepDatas, datas, isBattlePro, isUnlock);

        BagAmount bag = datas.GetBagData();

        this.imgIcon.sprite = GameAssetsConfigs.Instance.bagAsset.GetBagAsset(bag.bagType)?.sprBag;
        this.txtBagName.text = $"{bag.bagType.ToString()}";
        this.txtBagTour.text = $"Tour {bag.tour}";
        this.txtBagAmount.gameObject.SetActive(bag.amount > 1);
        this.txtBagAmount.text = $"X{bag.amount}";
        this.txtResize.UpdateText();

    }
}
