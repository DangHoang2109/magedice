using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class OpenBagItemFinal : MonoBehaviour
{
    public GameObject goNew;
    public Image imgIconLarge;
    public Image imgIconSmall;
    //public StatsCommon stats;
    public Image imgBG;
    public TextMeshProUGUI txtName;
    public TextMeshProUGUI txtCount;

    private Transform cachedTransform;

    public virtual void ParseData(OpenBagDialog.BagCardModel model)
    {
        if (this.cachedTransform is null)
            this.cachedTransform = this.transform;

        this.gameObject.SetActive(true);

        switch (model.earnType)
        {
            case OpenBagDialog.BagCardModel.EarnType.Booster:
                this.ParseBooster(BoosterConfigs.Instance.GetBooster(model.booster), model.value);
                //this.imgBG.sprite = GameAssetsConfigs.Instance.cardBorderConfig.cardMaterial;
                break;
            case OpenBagDialog.BagCardModel.EarnType.String:
                //this.ParseString(model.stringId, valueGet);
                break;
            case OpenBagDialog.BagCardModel.EarnType.Equipment:
                this.ParseStatsCard(model.equipmentConfig, model.value, model.isNew);
                //this.imgBG.sprite = GameAssetsConfigs.Instance.cardBorderConfig.cardMaterial;
                break;
        }

    }

    private void ParseBooster(BoosterConfig b, long valueGet)
    {
        this.txtName.text = b.name;
        this.txtName.color = ShopCueRef.GetFgColorByRarity( StatManager.Tier.None); ;
        this.txtCount.text = $"+{valueGet}";
        this.txtCount.color = ShopCueRef.GetLevelColorByCardType();

        //this.stats.Show(false);
        this.imgIconSmall.gameObject.SetActive(true);
        this.imgIconLarge.gameObject.SetActive(false);

        this.goNew.gameObject.SetActive(false);

        this.imgIconSmall.sprite = b.spr;

        this.imgBG.sprite = GameAssetsConfigs.Instance.cardBorderConfig.cardMaterial;
    }

    private void ParseStatsCard(ShopStatConfig c, long valueGet, bool isNew)
    {
        this.txtName.text = c.statName;
        this.txtName.color = ShopCueRef.GetFgColorByRarity(c.tier);
        this.txtCount.text = $"x{valueGet}";
        this.txtCount.color = ShopCueRef.GetLevelColorByCardType();

        //this.stats.gameObject.SetActive(true);
        this.imgIconSmall.gameObject.SetActive(false);
        this.imgIconLarge.gameObject.SetActive(true);
        this.imgIconLarge.sprite = c.sprStatItem;

        this.imgBG.sprite = TierAssetConfigs.Instance.GetCardAsset(c.tier).sprCard;

        //StatData data = StatDatas.Instance.GetStat(c.id);
        this.goNew.SetActive(isNew);

        //if (!isNew)
        //{
        //    StatsLevel cur = data.GetStatsByLevel(data.level);
        //    if (cur is null)
        //    {
        //        this.stats.Show(false);
        //    }
        //    else
        //    {
        //        this.stats.ParseData(cur.stats);
        //    }
        //}
        //else
        //{
        //    StatsLevel cur = c.GetStatsStartLevel();
        //    if (cur is null)
        //    {
        //        this.stats.Show(false);
        //    }
        //    else
        //    {
        //        this.stats.ParseData(cur.stats);
        //    }
        //}
    }

    public void SetParent(Transform parent)
    {
        this.cachedTransform.SetParent(parent);
    }

    public void Hide(Transform whereFallback)
    {
        this.gameObject.SetActive(false);
        this.cachedTransform.SetParent(whereFallback);
    }
}
