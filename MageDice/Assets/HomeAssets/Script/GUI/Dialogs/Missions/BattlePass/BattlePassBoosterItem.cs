using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BattlePassBoosterItem : BattlePassItem
{
    public TextCurrency money;
    public TextResize txtResize;

    public override void ParseData(BattlepassStepData stepDatas, BattlepassStepData.StepReward datas, bool isBattlePro, bool isUnlock)
    {
        base.ParseData(stepDatas, datas,isBattlePro, isUnlock);

        BoosterCommodity booster = datas.GetBoosterData();

        this.money.ParseData(booster);
        this.txtResize.UpdateText();
        if (booster.type == BoosterType.COIN)
        {
            this.imgIcon.sprite = SpriteIconValueConfigs.Instance.GetSprite(booster.type, 3800);
        }

        if (booster.type == BoosterType.CASH)
        {
            this.imgIcon.sprite = SpriteIconValueConfigs.Instance.GetSprite(booster.type, 150);
        }

        //this.money.ParseData(datas.reward);
        //this.txtResize.UpdateText();
        //if (datas.reward.type == BoosterType.COIN)
        //{
        //    this.imgIcon.sprite = SpriteIconValueConfigs.Instance.GetSprite(datas.reward.type, 3800);
        //}

        //if (datas.reward.type == BoosterType.CASH)
        //{
        //    this.imgIcon.sprite = SpriteIconValueConfigs.Instance.GetSprite(datas.reward.type, 150);
        //}
    }

}
