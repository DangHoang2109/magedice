using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class StatCardItemDisplay : MonoBehaviour
{
    [Header("Linkers")]
    public StatCardTag tagCard;
    public StatGUIUpgrade lvl; //StatsCardLevelV2

    [Space(5f)]
    public TextMeshProUGUI txtName;

    public Image imgPortrait;
    public Image imgIconSmall; //for booster
    public Image imgIconBig; //for dice
    public Image imgBorder;

    private string itemName;

    [Header("Stats")]
    public Vector4 coorStatCard;
    public Vector4 coorMaterialCard;


    private RectTransform rectTransPortrait;
    private Transform cachedTransform;
    public Transform CachedTransform => cachedTransform;

    private bool isInitialized = false;
    private void CheckInit()
    {
        if (isInitialized)
            return;

        this.rectTransPortrait = this.imgPortrait.GetComponent<RectTransform>();
        this.cachedTransform = this.transform;
    }

    public void ParseData(ShopStatConfig c, bool isNewWithNonUnlocked = true)
    {
        this.CheckInit();

        this.imgBorder.color = ShopCueRef.GetBgColorByRarity(c.tier);
        this.SetInternalName(c.statName, ShopCueRef.GetFgColorByRarity(c.tier));

        this.ApplyPortraitStat(
            c.sprStatItem);
        StatData data = StatDatas.Instance.GetStat(c.id);
        bool isUnlock = data != null && StatManager.CheckKind(data.kind, StatManager.Kind.Unlocked);
        if (isUnlock)
        {
            if (this.tagCard != null)
            {
                this.tagCard.ParseData(StatManager.Instance.IsUsing(c.id)
                    ? StatCardTag.TagType.Equipped
                    : StatCardTag.TagType.None);
            }

            //StatsLevel cur = data.GetStatsByLevel(data.level);
            //if (cur != null && c.type != StatsCardType.CHARACTER)
            //{
            //    this.stats.ParseData(cur.stats);
            //}
            //else
            //{
            //    this.stats.Show(false);
            //}

            StatItemStats next = data.NextStats;
            if (next != null)
            {
                this.lvl.ParseCueBought(data);
                //this.lvl.ParseData(data.level, data.card, next.card,
                //    ShopCueRef.GetLevelColorByCardType(c.type));
                //this.lvl.DoFillTo(data.card + countEarn, duration, Ease.Linear);
            }
            else
            {
                this.lvl.ParseCueToBuy(data);
                //if (data.card == 0)
                //{
                //    this.lvl.ParseData(data.level,
                //        ShopCueRef.GetLevelColorByCardType(c.type));
                //}
                //else
                //{
                //    this.lvl.ParseDataMaxed(data.level, data.card,
                //        ShopCueRef.GetLevelColorByCardType(c.type));
                //}
            }
        }
        else
        {
            if (this.tagCard != null)
            {
                this.tagCard.ParseData(isNewWithNonUnlocked
                    ? StatCardTag.TagType.New
                    : StatCardTag.TagType.None);
            }

            //StatsLevel cur = c.GetStatsStartLevel();
            //int curLevel = 0;
            //if (cur != null)
            //{
            //    curLevel = cur.level;
            //    if (c.type != StatsCardType.CHARACTER)
            //    {
            //        this.stats.ParseData(cur.stats);
            //    }
            //    else
            //    {
            //        this.stats.Show(false);
            //    }
            //}
            //else
            //{
            //    this.stats.Show(false);
            //}
            this.lvl.ParseCueBought(data);

            //StatsLevel next = c.GetStatsByLevel(curLevel + 1);
            //if (next != null)
            //{
            //    this.lvl.ParseData(curLevel, countEarn, next.card,
            //        ShopCueRef.GetLevelColorByCardType(c.type));
            //}
            //else
            //{
            //    this.lvl.ParseData(curLevel,
            //        ShopCueRef.GetLevelColorByCardType(c.type));
            //}
        }
    }

    public void ParseForBag(ShopStatConfig c, int countEarn, in float duration, bool isNewWithNonUnlocked = true)
    {
        this.CheckInit();

        this.imgBorder.sprite = TierAssetConfigs.Instance.GetCardAsset(c.tier).sprCard;
        this.SetInternalName(c.statName, ShopCueRef.GetFgColorByRarity(c.tier));

        this.ApplyPortraitStat(
            c.sprStatItem);
        StatData data = StatDatas.Instance.GetStat(c.id);
        bool isUnlock = data != null && StatManager.CheckKind(data.kind, StatManager.Kind.Unlocked);
        this.lvl.gameObject.SetActive(true);

        if (isUnlock)
        {
            if (this.tagCard != null)
            {
                this.tagCard.ParseData(StatManager.Instance.IsUsing(c.id)
                    ? StatCardTag.TagType.Equipped
                    : StatCardTag.TagType.None);
            }

            StatItemStats next = data.NextStats;
            if (next != null)
            {
                this.lvl.ParseCueBought(data);
                this.lvl.DoFillTo(data.cards + countEarn, duration);
            }
            else
            {
                this.lvl.ParseCueToBuy(data);
            }
        }
        else
        {
            if (this.tagCard != null)
            {
                this.tagCard.ParseData(isNewWithNonUnlocked
                    ? StatCardTag.TagType.New
                    : StatCardTag.TagType.None);
            }
            this.lvl.ParseCueBought(data);

            //StatsLevel cur = c.GetStatsStartLevel();
            //int curLevel = 0;
            //if (cur != null)
            //{
            //    curLevel = cur.level;
            //    if (c.type != StatsCardType.CHARACTER)
            //    {
            //        this.stats.ParseData(cur.stats);
            //    }
            //    else
            //    {
            //        this.stats.Show(false);
            //    }
            //}
            //else
            //{
            //    this.stats.Show(false);
            //}

            //StatsLevel next = c.GetStatsByLevel(curLevel + 1);
            //if (next != null)
            //{
            //    this.lvl.ParseData(curLevel, countEarn, next.card,
            //        ShopCueRef.GetLevelColorByCardType(c.type));
            //}
            //else
            //{
            //    this.lvl.ParseData(curLevel,
            //        ShopCueRef.GetLevelColorByCardType(c.type));
            //}
        }
        //
        // // earn here
        // StatDatas.Instance.CollectCard(c, countEarn);
    }

    public void ParseData(StatData d, bool needCapCount = false)
    {
        this.CheckInit();

        if (d == null)
        {
            Debug.LogError("NULL HERE");
#if UNITY_EDITOR
            UnityEditor.EditorUtility.DisplayDialog("hey", "Bac oi cai nay null roi, xem log", "OK");
#endif
            return;
        }

        ShopStatConfig c = d.config;
        this.ApplyPortraitStat(
            c.sprStatItem);

        if (this.tagCard != null)
        {
            this.tagCard.ParseData(StatManager.Instance.IsUsing(c.id)
                ? StatCardTag.TagType.Equipped
                : StatCardTag.TagType.None);
        }

        //if (c.type == StatsCardType.CHARACTER)
        //{
        //    this.imgBorder.color = ShopCueRef.GetBgColorByRarity(StatsItemRarity.CHARACTER);
        //    this.SetInternalName(c.name, ShopCueRef.GetFgColorByRarity(StatsItemRarity.CHARACTER));

        //    this.stats.Show(false);
        //}
        //else
        //{
        //    this.imgBorder.color = ShopCueRef.GetBgColorByRarity(c.tier);
        //    this.SetInternalName(c.name, ShopCueRef.GetFgColorByRarity(c.tier));

        //    StatsLevel cur = d.GetStatsByLevel(d.level);
        //    if (cur != null)
        //    {
        //        this.stats.ParseData(cur.stats);
        //    }
        //    else
        //    {
        //        this.stats.Show(false);
        //    }
        //}

        this.imgBorder.sprite = TierAssetConfigs.Instance.GetCardAsset(c.tier).sprCard;
        this.SetInternalName(c.statName, ShopCueRef.GetFgColorByRarity(c.tier));

        //StatsLevel cur = d.GetStatsByLevel(d.level);
        //if (cur != null)
        //{
        //    this.stats.ParseData(cur.stats);
        //}
        //else
        //{
        //    this.stats.Show(false);
        //}
        this.lvl.ParseCueBought(d);

        //StatsLevel next = d.GetStatsByLevel(d.level + 1);
        //if (next != null)
        //{
        //    this.lvl.ParseCueBought(d);

        //    if (d.card > next.card && needCapCount)
        //    {
        //        this.lvl.ParseData(d.level, next.card, next.card,
        //            ShopCueRef.GetLevelColorByCardType(c.type));
        //    }
        //    else
        //    {
        //        this.lvl.ParseData(d.level, d.card, next.card,
        //            ShopCueRef.GetLevelColorByCardType(c.type));
        //    }
        //}
        //else
        //{
        //    this.lvl.ParseDataMaxed(d.level, d.card,
        //        ShopCueRef.GetLevelColorByCardType(c.type));
        //}
    }

    public void ParseData(BoosterType bt)
    {
        this.CheckInit();

        var bc = BoosterConfigs.Instance.GetBooster(bt);
        this.ApplyPortraitMaterial(
            bc.spr);
        this.SetInternalMaterialValue(bc.name, 0);

        if (this.tagCard != null)
            this.tagCard.Show(false);
        //this.stats.Show(false);
        this.lvl.gameObject.SetActive(false);
    }

    public string GetName()
    {
        return this.itemName;
    }


    private void SetInternalName(in string n, in Color col)
    {
        this.itemName = n;

        if (this.txtName != null)
        {
            this.txtName.text = n;
            this.txtName.color = col;
        }
    }

    private void SetInternalMaterialValue(in string n, in int value)
    {
        this.itemName = n;

        if (this.txtName != null)
        {
            this.txtName.text = value.ToString();
            this.txtName.color = Color.white;
        }
    }

    private void ApplyPortraitStat(Sprite icon)
    {
        if (this.imgIconSmall != null)
            this.imgIconSmall.gameObject.SetActive(false);

        if (this.imgIconBig != null)
        {
            this.imgIconBig.gameObject.SetActive(true);
            this.imgIconBig.sprite = icon;
        }

        //this.imgPortrait.sprite = background;

        //this.rectTransPortrait.anchoredPosition = new Vector2(coorMaterialCard.x, coorMaterialCard.y);
        //this.rectTransPortrait.sizeDelta = new Vector2(coorMaterialCard.z, coorMaterialCard.w);

        //this.rectTransPortrait.anchoredPosition = new Vector2(coorStatCard.x, coorStatCard.y);
        //this.rectTransPortrait.sizeDelta = new Vector2(coorStatCard.z, coorStatCard.w);

        //this.imgBorder.sprite = GameAssetsConfigs.Instance.cardBorderConfig.cardStats;
    }
    private void ApplyPortraitMaterial(Sprite icon)
    {
        if (this.imgIconBig != null)
            this.imgIconBig.gameObject.SetActive(false);

        if (this.imgIconSmall != null)
        {
            this.imgIconSmall.gameObject.SetActive(true);
            this.imgIconSmall.sprite = icon;
        }
        //this.imgPortrait.sprite = background;
        //this.rectTransPortrait.anchoredPosition = new Vector2(coorMaterialCard.x, coorMaterialCard.y);
        //this.rectTransPortrait.sizeDelta = new Vector2(coorMaterialCard.z, coorMaterialCard.w);

        this.imgBorder.color = Color.white;
        this.imgBorder.sprite = GameAssetsConfigs.Instance.cardBorderConfig.cardMaterial;
    }

}
