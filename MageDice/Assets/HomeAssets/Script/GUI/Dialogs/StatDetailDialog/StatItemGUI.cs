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
    public TextMeshProUGUI tmpStatLevel;

    public Image imgBg;
    
    public void ParseData(StatData c)
    {
        if (tmpStatName != null)
            tmpStatName.SetText(c.config.statName);

        if (tmpStatLevel != null)
            tmpStatLevel.SetText(c.IsMaxLevel ? "MAX" : $"LV.{c.level}");

        TierAssetConfig config = TierAssetConfigs.Instance.GetCardAsset(c.config.tier);
        if (imgRarity != null)
            imgRarity.color = config.color;

        if (tmpRarity != null)
            tmpRarity.SetText(config.name) ;

        if (imgBg != null)
            imgBg.sprite = config.sprCard;

        this.imgStat.sprite = c.config?.sprStatItem;
    }
}
