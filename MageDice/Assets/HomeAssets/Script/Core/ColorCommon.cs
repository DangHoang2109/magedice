using System;
using System.Collections;
using System.Collections.Generic;
using Coffee.UIEffects;
using UnityEngine;

public static class ColorCommon
{
    public static Color GetFgColorByRarity(StatManager.Tier rarity)
    {
        switch (rarity)
        {
            case StatManager.Tier.None:
            case StatManager.Tier.Standard:
                return new Color(0.05f, 0.353f, 0.58f, 1f);

            //case StatManager.Tier.Country:
            //    return Color.blue;
            case StatManager.Tier.Rare:
                return new Color(0.49f, 0.29f, 0.082f, 1f);

            case StatManager.Tier.Legendary:
                return new Color(0.424f, 0.098f, 0.608f, 1f);

            default:
                return Color.white;
        }
    }


    public static Color GetBgColorByRarity(StatManager.Tier rarity)
    {
        switch (rarity)
        {
            case StatManager.Tier.Rare:
                return new Color(0.965f, 0.471f, 0f, 1f);

            case StatManager.Tier.Legendary:
                //return new Color(0.75f, 0.72f, 0.01f, 1f);
                return Color.yellow;

            case StatManager.Tier.Event:
                return new Color(0.796f, 0.33f, 1f, 1f);

            //case StatManager.Tier.Country:
            //    return new Color(0.2f, 0.651f, 1f, 1f);


            default:
                return new Color(0.4f, 0.4f, 0.4f, 1f);
        }
    }


    public static Color ColorTagNew
    => new Color(0.8f, 0.15f, 0f);
    public static Color ColorTagEquipped
    => new Color(1f, 0.5f, 0f);
    
    public static Color ColorProgressLightBlue
    => new Color(0f, 0.6f, 0.9f);
    public static Color ColorProgressLightGreen
    => new Color(0.3f, 0.73f, 0f);
}
