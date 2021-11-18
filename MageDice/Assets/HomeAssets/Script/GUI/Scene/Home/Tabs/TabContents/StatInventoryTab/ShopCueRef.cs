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
}
