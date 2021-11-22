using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class OpenBagItemFinal : MonoBehaviour
{
    public Image imgIconLarge;
    public Image imgIconSmall;
    
    public Image imgIconCueTail;
    public Image imgIconCueHead;
    
    public Image imgBG;
    public TextMeshProUGUI txtName;
    public TextMeshProUGUI txtCount;

    private Transform cachedTransform;
    private Sprite sprBGDefault;
    
    

    public virtual void ParseData(OpenBagDialog.BagCardModel model)
    {
        if (this.cachedTransform is null)
        {
            this.cachedTransform = this.transform;
            this.sprBGDefault = this.imgIconLarge.sprite;
            this.txtName.color = Color.white;
        }
        
        this.gameObject.SetActive(true);
        
        switch (model.earnType)
        {
            case OpenBagDialog.BagCardModel.EarnType.Booster:
                this.ParseBooster(BoosterConfigs.Instance.GetBooster(model.booster), model.value);
                //this.imgBG.sprite = GameAssetsConfigs.Instance.cardBorderConfig.cardMaterial;
                break;
            // case OpenBagDialog.BagCardModel.EarnType.String:
            //     //this.ParseString(model.stringId, valueGet);
            //     break;
            case OpenBagDialog.BagCardModel.EarnType.CueCard:
                this.ParseCard(model.equipmentConfig, model.value);
                //this.imgBG.sprite = GameAssetsConfigs.Instance.cardBorderConfig.cardMaterial;
                break;
            case OpenBagDialog.BagCardModel.EarnType.Cue:
                this.ParseCue(model.equipmentConfig);
                break;
        }

    }

    private void ParseBooster(BoosterConfig b, long valueGet)
    {
        this.txtName.text = b.name;
        this.txtCount.text = $"+{valueGet}";
        this.txtCount.color = new Color(0.1f, 0.1f, 0.1f, 1f);;
        this.txtCount.outlineColor = Color.white;
        
        //this.imgIconLarge.sprite = GameAssetsConfigs.Instance.cardBorderConfig.portraitGeneric;
        
        this.imgIconSmall.gameObject.SetActive(true);
        this.imgIconSmall.sprite = b.spr;
        
        this.imgIconCueTail.gameObject.SetActive(false);
        this.imgIconCueHead.gameObject.SetActive(false);

        this.imgBG.color = Color.white;
    }

    private void ParseCard(StatData c, long valueGet)
    {
        ShopStatConfig config = c.config;
        if (config == null)
        {
            Debug.LogException(new System.Exception("OpenBagItemFinal ParseCard error: config NULL"
                                                    + $" -id: {("NULL")}"));
            this.txtName.text = "NULL";
            return;
        }
        
        this.txtName.text = config.statName;
        //this.txtName.color = ColorCommon.GetFgColorByRarity(config.tier);
        this.txtCount.text = $"x{valueGet}";
        this.txtCount.color = Color.white;
        this.txtCount.outlineColor = Color.black;
        
        this.imgIconLarge.sprite = this.sprBGDefault;
        this.imgIconSmall.gameObject.SetActive(false);
        
        this.imgIconCueTail.gameObject.SetActive(true);
        this.imgIconCueHead.gameObject.SetActive(true);
        this.imgIconCueHead.sprite = this.imgIconCueTail.sprite = config.sprStatItem;
        
        this.imgBG.color = ColorCommon.GetBgColorByRarity(config.tier);
    }
    private void ParseCue(StatData c)
    {
        ShopStatConfig config = c.config;
        if (config == null)
        {
            Debug.LogException(new System.Exception("OpenBagItemFinal ParseCard error: config NULL"
                                                    + $" -id: {("NULL")}"));
            this.txtName.text = "NULL";
            return;
        }
        
        this.txtName.text = config.statName;
        //this.txtName.color = ColorCommon.GetFgColorByRarity(config.tier);
        this.txtCount.text = "A Cue!";
        this.txtCount.color = Color.white;
        this.txtCount.outlineColor = Color.black;
        
        this.imgIconLarge.sprite = this.sprBGDefault;
        this.imgIconSmall.gameObject.SetActive(false);
        
        this.imgIconCueTail.gameObject.SetActive(true);
        this.imgIconCueHead.gameObject.SetActive(true);
        this.imgIconCueHead.sprite = this.imgIconCueTail.sprite = config.sprStatItem;
        
        this.imgBG.color = ColorCommon.GetBgColorByRarity(config.tier);
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
