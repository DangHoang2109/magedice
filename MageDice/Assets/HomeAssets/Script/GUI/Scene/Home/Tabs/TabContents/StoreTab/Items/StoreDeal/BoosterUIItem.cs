using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BoosterUIItem : MonoBehaviour
{
    [Header("Booster")]
    public IBooster booster;

    [Header("Icon")]
    public Image imgIcon;

    public virtual void ShowBooster(BoosterCommodity booster)
    {
        this.booster.ParseBooster(booster);
        Sprite sprIcon = SpriteIconValueConfigs.Instance.GetSprite(booster.type, booster.GetValue());
        if (sprIcon != null) this.imgIcon.sprite = sprIcon;
    }

    public virtual void ShowBooster(BoosterType type, long value, bool rejectIcon)
    {
        this.booster.ParseBooster(new BoosterCommodity(type, value));

        if (rejectIcon && type == BoosterType.COIN && value < 1000)
            value = 1000;

        Sprite sprIcon = SpriteIconValueConfigs.Instance.GetSprite(type, value);
        if (sprIcon != null) this.imgIcon.sprite = sprIcon;
    }
}
