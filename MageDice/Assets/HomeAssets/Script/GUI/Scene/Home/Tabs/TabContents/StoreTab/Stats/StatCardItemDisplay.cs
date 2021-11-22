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
    
    //OLD
    //public StatsCommon stats; 

    [Space(5f)]
    public TextMeshProUGUI txtName;

    public Image imgPortrait;
    private Sprite sprBGDefault;
    public Image imgBorder;

    [Header("Booster")]
    public Transform tranBooster;
    public Image imgIconSmall;


    [Header("Cue")]
    public CueCardDisplay cueDisplay;
    public StatsCardLevelV2 lvl => cueDisplay.lvl;

    [Header("Count")]
    public TextMeshProUGUI tmpCount;

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
        if(isInitialized)
            return;

        this.rectTransPortrait = this.imgPortrait.GetComponent<RectTransform>();
        this.cachedTransform = this.transform;
        
        
        this.sprBGDefault = this.imgPortrait.sprite;

        this.isInitialized = true;
    }

    public void ParseStatData(StatData c, int count)
    {
        Debug.Log("<color=yellow>Parse cue</color>");
        
        
        this.CheckInit();

        this.imgIconSmall.gameObject.SetActive(false);
        this.cueDisplay.gameObject.SetActive(true);

        this.tmpCount.text = string.Format("x{0}", count);

        //parse boder theo cue, đổi màu card
        this.SetColorBg(ColorCommon.GetBgColorByRarity(c.config.tier), ColorCommon.GetBgColorByRarity(c.config.tier));

        //parse name
        this.SetInternalName(c.config.statName, Color.white);

        //TODO parse cue
        this.cueDisplay.ParseCue(c);
 
    }
    public void FillToCurrentCard(StatData c, in float duration)
    {
        this.lvl.DoFillTo(c.cards, duration, Ease.InSine);
    }

    public void ParseStatDataAndShowTag(StatData c, int count, bool isNewWithNonUnlocked = true)
    {
        ParseStatData(c, count);

        bool isUnlock = c.kind != StatManager.Kind.NotUnlocked;

        if (isUnlock)
        {
            //hiển thị tag card cue đang dùng hay không
            if (this.tagCard != null)
            {
                this.tagCard.ParseData(StatDatas.Instance.CurrentStatId.Contains(c.id)
                    ? StatCardTag.TagType.Equipped
                    : StatCardTag.TagType.None);
            }
            return;
        }

        //hiển thị tag card cue new
        if (this.tagCard != null)
        {
            this.tagCard.ParseData(!isUnlock
                ? StatCardTag.TagType.New
                : StatCardTag.TagType.None);
        }
    }

    public void ParseData(BoosterType bt)
    {
        this.CheckInit();

        this.tranBooster.gameObject.SetActive(true);
        this.cueDisplay.gameObject.SetActive(false);
        this.tmpCount.text = "";

        //TODO parse booster

        this.imgBorder.color = Color.white;
        //this.imgPortrait.sprite = GameAssetsConfigs.Instance.cardBorderConfig.portraitGeneric;
        this.imgPortrait.color = Color.white;
        
        BoosterConfig boosterConfig = GameAssetsConfigs.Instance.boosters.GetBooster(bt);
        if (boosterConfig != null)
        {
            this.txtName.text = boosterConfig.name;
            this.imgIconSmall.gameObject.SetActive(true);
            this.imgIconSmall.sprite = boosterConfig.spr;
        }
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

    private void ApplyPortraitStat(Sprite spr)
    {
        if (this.imgIconSmall != null)
            this.imgIconSmall.gameObject.SetActive(false);
        this.imgPortrait.sprite = spr;
        this.rectTransPortrait.anchoredPosition = new Vector2(coorStatCard.x, coorStatCard.y);
        this.rectTransPortrait.sizeDelta = new Vector2(coorStatCard.z, coorStatCard.w);

        //this.imgBorder.sprite = GameAssetsConfigs.Instance.cardBorderConfig.cardStats;
    }
    private void ApplyPortraitMaterial(Sprite background, Sprite icon)
    {
        if (this.imgIconSmall != null)
        {
            this.imgIconSmall.gameObject.SetActive(true);
            this.imgIconSmall.sprite = icon;
        }
        this.imgPortrait.sprite = background;
        this.rectTransPortrait.anchoredPosition = new Vector2(coorMaterialCard.x, coorMaterialCard.y);
        this.rectTransPortrait.sizeDelta = new Vector2(coorMaterialCard.z, coorMaterialCard.w);
        
        this.imgBorder.color = Color.white;
        //this.imgBorder.sprite = GameAssetsConfigs.Instance.cardBorderConfig.cardMaterial;
    }

    /// <summary>
    /// Set color card
    /// </summary>
    /// <param name="colorBoder">phía ngoài</param>
    /// <param name="colorPortrait">phía trong</param>
    public void SetColorBg(Color colorBoder, Color colorPortrait)
    {
        this.imgBorder.color = colorBoder;
        this.imgPortrait.sprite = this.sprBGDefault;
        this.imgPortrait.color = colorPortrait;
    }
}
