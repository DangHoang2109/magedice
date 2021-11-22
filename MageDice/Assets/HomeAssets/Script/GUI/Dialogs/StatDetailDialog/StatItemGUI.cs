using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class StatItemGUI : MonoBehaviour
{
    //public Image imgFrame;
    public Image imgStat;
    public Image imgRarity;
    public TextMeshProUGUI tmpRarity;
    public TextMeshProUGUI tmpStatName;

    
    public void ParseData(StatData c)
    {
        //this.imgFrame.color = ShopCueRef.Instance.GetColorTier(c.config?.tier ?? CueManager.Tier.Standard)
        //    ?.colorLight?? Color.white;
        if (tmpStatName != null)
            tmpStatName.SetText(c.config.statName);

        if (imgRarity != null)
            imgRarity.color = TierAssetConfigs.Instance.GetCardAsset(c.config.tier).color;
        if (tmpRarity != null)
            tmpRarity.SetText(TierAssetConfigs.Instance.GetCardAsset(c.config.tier).name) ;

        this.imgStat.sprite = c.config?.sprStatItem;
    }
}
