using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
using DG.Tweening;
public class SkillPerkIconItem : MonoBehaviour
{
    public Image imgLight;

    public Image imgIcon;
    public TextMeshProUGUI tmpPerkName;
    public TextMeshProUGUI tmpPerkLevel;
    public Image imgBgIcon;


    public TextMeshProUGUI tmpTitleLevel;

    public int _id;
    public int ID => _id;

    public void ParseData(PerkData data)
    {
        SkillPerkAsset asset = data.Asset;

        this._id = data.id;

        this.imgBgIcon.sprite = asset.sprBG;
        this.imgIcon.sprite = asset.sprIcon;

        this.tmpTitleLevel.color = asset.colorTitle;

        this.tmpPerkName.color = asset.colorText;
        this.tmpPerkLevel.color = asset.colorText;

        this.tmpPerkName.SetText(asset.name);
        this.tmpPerkLevel.SetText(data.IsMax ? "Max" : (data.currentUpgradeStep + 1).ToString());
    }
    public void UpdateData(PerkData data)
    {
        this.tmpPerkLevel.SetText(data.IsMax ? "Max" : (data.currentUpgradeStep + 1).ToString());
    }

    public Tween DoAnimateUpgrade(float time = 0.2f)
    {
        Sequence seq = DOTween.Sequence();
        seq.SetId(this);
        seq.AppendCallback(() => SetLight(true));
        seq.AppendInterval(time);
        seq.AppendCallback(() => SetLight(false));
        return seq;
    }
    public void SetLight(bool isShow)
    {
        imgLight.gameObject.SetActive(isShow);
    }
    public Tween DoAnimateRandomComplete(float time = 0.2f)
    {
        return DoAnimateUpgrade(time)
            .SetLoops(4, LoopType.Yoyo)
            .OnComplete(()=> SetLight(false));
    }
}
