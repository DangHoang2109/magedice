using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class StoreCardBoosterItem : StoreOneCardItem
{
    [Header("Booster")]
    public IBooster booster;

    public void ParseBooster(BoosterCommodity booster)
    {
        //get asset for store => icon

        this.booster.ParseBooster(booster);

        Sprite spriteIcon = SpriteIconValueConfigs.Instance.GetSprite(booster.type, booster.GetValue());
        if (spriteIcon != null)
        {
            this.imgIcon.sprite = spriteIcon;
        }
    }
}
