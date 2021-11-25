using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class ShopCueRef : MonoSingleton<ShopCueRef>
{
    [System.Serializable]
    public class TierColor
    {
        public StatManager.Tier tier;
        public Color colorLight;
        public Color colorDark;
    }
    
    [Header("prefab item"), SerializeField]
    private ShopCueItem prefab;
    // available item for using
    private Stack<ShopCueItem> stack;

    [Header("colors for tier"), SerializeField]
    private TierColor[] tierColors;
    private Dictionary<StatManager.Tier, TierColor> dicColor;

    [Header("more colors")]
    public Color colStatCurrent = Color.green;
    public Color colStatNext = Color.yellow;
    public Color colStatMax = new Color(0.66f, 0.66f, 0.66f, 0.55f);
    public Color black = Color.black;
    public Color transparent = Color.clear;
    

    public void Prepare()
    {
        this.stack = new Stack<ShopCueItem>();
        if (this.tierColors != null && this.tierColors.Length != 0)
            this.dicColor = this.tierColors.ToDictionary(x => x.tier, x => x);
    }

    public ShopCueItem RequestItem()
    {
        if (this.stack.Count != 0)
        {
            return this.stack.Pop();
        }

        return Instantiate(this.prefab);
    }
    
    public void ReturnItem(ShopCueItem item)
    {
        item.transform.SetParent(this.transform);
        this.stack.Push(item);
    }

    public TierColor GetColorTier(StatManager.Tier tier)
    {
        if (this.dicColor == null || !this.dicColor.ContainsKey(tier))
            return null;

        return this.dicColor[tier];
    }


    public static Color GetFgColorByRarity(StatManager.Tier rarity)
    {
        switch (rarity)
        {
            case StatManager.Tier.Rare:
                return new Color(0.49f, 0.29f, 0.082f, 1f);

            case StatManager.Tier.Legendary:
                return new Color(0.424f, 0.098f, 0.608f, 1f);

            case StatManager.Tier.Standard:
                return new Color(0.05f, 0.353f, 0.58f, 1f);


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
                return new Color(0.796f, 0.33f, 1f, 1f);

            case StatManager.Tier.Standard:
                return new Color(0.2f, 0.651f, 1f, 1f);


            default:
                return new Color(0.877f, 0.898f, 0.94f, 1f);
        }
    }

    public static Color GetLevelColorByCardType()
    {
        return Color.white;
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
