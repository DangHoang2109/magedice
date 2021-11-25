using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;
using Cosina.Components;
using TMPro;

public class OpenBagItemUpper : MonoBehaviour
{
    private static readonly Color COLOR_TRANSPARENT_WHITE = new Color(1f, 1f, 1f, 0f);

    public enum CardDisplayType
    {
        Common,
        StatsCardOld,
        StatsCardNew,
        SpecialString,
    }

    public OpenBagDialog host;
    public OpenBagBag bag;

    [Header("Linkers")]
    public CanvasGroup cg;
    public StatCardItemDisplay displayer;


    [Space(10f)]
    public TextMeshProUGUI txtRank;
    public TextMeshProUGUI txtName;
    public TextMeshProUGUI txtCount;

    public TextMeshProUGUI txtNameBig;
    public TextMeshProUGUI txtNewUnlocked;
    public TextMeshProUGUI txtNewUnlockedFX;
    private Transform transNameBig;
    private Transform transNewUnlocked;
    private Transform transNewUnlockedFX;

    [Space(10f)]
    public CanvasGroup cgAcceptWatch;
    public Button btnAcceptWatch;
    public CanvasGroup cgRejectWatch;
    public Button btnRejectWatch;

    public ParticleSystem fxCardGlare;
    //private ParticleSystem.MainModule fxCardGlareMain;
    public ParticleSystem fxCardGlare2;
    public ParticleSystem fxFlashAds;
    public ParticleSystem fxBolt;

    [Header("Linker FXs")]
    private bool _;
    /// <summary>
    /// (Reactangle) this also fades out
    /// </summary>
    public Image imgFXRectOut;
    /// <summary>
    /// (Eclipse) this also fades out
    /// </summary>
    public Image imgFXEclipseOut;
    /// <summary>
    /// this also fades in
    /// </summary>
    public Image imgFxRectIn;



    [Header("Stats")]

    public float durationAllOut;

    [Space(5f)]
    public Vector3 scaleFXZoomOutStart;
    public PairTweenVector statFxZoomOut;
    public Vector3 scaleFXZoomInStart;
    public PairTweenVector statFxZoomIn;

    private CardDisplayType displayType;
    private Transform cachedTransform;

    private BagType bagType;
    private OpenBagDialog.BagCardModel modelCurrent;

    //private bool isNeedWatch;
    private BoosterConfig boosterCurrent;
    private ShopStatConfig ShopStatConfigCurrent;
    private long valueCurrent;

    private bool isBlockingByAds;
    public bool IsBlockingByAds => this.isBlockingByAds;

    public void Init()
    {
        this.cachedTransform = this.transform;

        this.transNameBig = this.txtNameBig.transform;
        this.transNewUnlocked = this.txtNewUnlocked.transform;
        this.transNewUnlockedFX = this.txtNewUnlockedFX.transform;

        //this.fxCardGlareMain = this.fxCardGlare.main;
    }

    public virtual void ParseData(BagType bag, OpenBagDialog.BagCardModel model, Vector3 position)
    {
#if K_TEST_BAG_ADS
        model.isWatch = true;
#endif

        this.bagType = bag;
        this.modelCurrent = model;
        this.cachedTransform.position = position;
        this.txtNameBig.gameObject.SetActive(false);
        this.txtNewUnlocked.gameObject.SetActive(false);
        this.txtNewUnlockedFX.gameObject.SetActive(false);


        this.boosterCurrent = null;
        this.ShopStatConfigCurrent = null;

        // #if UNITY_EDITOR
        // model.isWatch = true;// test is watch
        // #endif

        if (!(model.isWatch))
        {
            this.valueCurrent = 0;
            switch (model.earnType)
            {
                case OpenBagDialog.BagCardModel.EarnType.Booster:
                    this.ParseBoosterAndEarn(BoosterConfigs.Instance.GetBooster(model.booster), model.value);
                    break;
                case OpenBagDialog.BagCardModel.EarnType.String:
                    //this.ParseStringAndEarn(model.stringId, valueGet);
                    break;
                case OpenBagDialog.BagCardModel.EarnType.Equipment:
                    this.ParseStatsCardAndEarn(model.equipmentConfig, model.value);
                    break;
            }
        }
        else
        {
            //this.fxCardColored.StartAnimate();
            this.valueCurrent = model.value;
            this.isBlockingByAds = true;
            switch (model.earnType)
            {
                case OpenBagDialog.BagCardModel.EarnType.Booster:
                    this.ParseBooster(BoosterConfigs.Instance.GetBooster(model.booster), model.value);
                    break;
                case OpenBagDialog.BagCardModel.EarnType.String:
                    //this.ParseString(model.stringId, valueGet);
                    break;
                case OpenBagDialog.BagCardModel.EarnType.Equipment:
                    this.ParseStatsCard(model.equipmentConfig, model.value);
                    break;
            }
        }


    }

    private void ParseBoosterAndEarn(BoosterConfig b, long value)
    {
        this.txtRank.gameObject.SetActive(false);
        this.txtName.text = b.name;
        this.txtName.gameObject.SetActive(true);
        this.txtCount.text = $"+{value}";
        this.txtCount.gameObject.SetActive(true);

        this.displayer.ParseData(b.type);


        //this.imgFXRectOut.gameObject.SetActive(true);
        //this.imgFxRectIn.gameObject.SetActive(true);
        this.imgFXRectOut.color = this.imgFxRectIn.color = Color.cyan;
        this.imgFXEclipseOut.gameObject.SetActive(false);

        this.displayType = CardDisplayType.Common;

        // earn here
        UserProfile.Instance.AddBooster(b.type, value, this.bagType.ToString(),
            LogSourceWhere.OPEN_BAG);
        Invoker.Invoke(GameDataManager.Instance.SaveBoosterData);
    }
    //
    // // private void ParseStringAndEarn(int stringId, long value)
    // // {
    // //     // TODO! separate common string vs special strings
    // //     
    // //     this.imgBGMaterial.sprite = null;
    // //     
    // //     this.imgIconMaterial.gameObject.SetActive(false);
    // //     this.cardMaterial.SetActive(true);
    // //     this.cardStats.SetActive(false);
    // //     
    // //     this.txtRank.gameObject.SetActive(false);
    // //     this.txtName.text = $"String {stringId}";
    // //     this.txtCount.text = $"+{value}";
    // //
    // //     this.panelNew.gameObject.SetActive(false);
    // //     this.imgFXRectOut.gameObject.SetActive(false);
    // //     this.imgFXEclipseOut.gameObject.SetActive(true);
    // //     this.imgFXEclipseOut.color = this.imgFxRectIn.color = Color.cyan;
    // //     this.imgBg.sprite = this.dicBGCards[StatsItemRarity.COMMON].sprBG;
    // //     
    // //     this.displayType = CardDisplayType.SpecialString;
    // // }
    //
    private void ParseStatsCardAndEarn(ShopStatConfig c, long valueGet)
    {

        if (c != null)
        {
            this.displayer.ParseForBag(c, (int)valueGet, this.durationAllOut);

            //this.imgFXRectOut.gameObject.SetActive(true);
            //this.imgFxRectIn.gameObject.SetActive(true);
            this.imgFXEclipseOut.gameObject.SetActive(false);
            this.txtRank.color = this.imgFXRectOut.color
                = this.imgFxRectIn.color = ShopCueRef.GetBgColorByRarity(c.tier);


            StatData data = StatDatas.Instance.GetStat(c.id);

            bool isUnlock = data != null;

            if (isUnlock)
            {
                this.displayType = CardDisplayType.StatsCardOld;


                this.txtRank.gameObject.SetActive(true);
                this.txtRank.text = c.tier.ToString();
                this.txtName.gameObject.SetActive(true);
                this.txtName.text = c.statName;
                this.txtCount.gameObject.SetActive(true);
                this.txtCount.text = $"x{valueGet}";
            }
            else
            {
                this.displayType = CardDisplayType.StatsCardNew;

                this.txtRank.gameObject.SetActive(false);
                this.txtName.gameObject.SetActive(false);
                this.txtCount.gameObject.SetActive(false);

                this.txtNameBig.text = c.statName;
            }

            // earn here
            StatManager.Instance.AddCard(data, valueGet);

            //MissionDatas.Instance.DoStep(MissionID.COLLECT_CARD, (int)valueGet);

            Invoker.Invoke(GameDataManager.Instance.SaveBoosterData);
        }
        else
        {
            this.txtName.text = "No Config!";
            Debug.LogError("CONFIG NULL");
        }
    }


    private void ParseBooster(BoosterConfig b, long value)
    {
        this.boosterCurrent = b;

        this.txtRank.gameObject.SetActive(false);
        this.txtName.text = b.name;
        this.txtName.gameObject.SetActive(true);
        this.txtCount.text = $"+{value}";
        this.txtCount.gameObject.SetActive(true);

        this.displayer.ParseData(b.type);


        //this.imgFXRectOut.gameObject.SetActive(true);
        //this.imgFxRectIn.gameObject.SetActive(true);
        this.imgFXRectOut.color = this.imgFxRectIn.color = Color.cyan;
        this.imgFXEclipseOut.gameObject.SetActive(false);

        this.displayType = CardDisplayType.Common;

        // earn here
        // UserProfile.Instance.AddBooster(b.type, value, this.bagType.ToString(),LogSourceWhere.OPEN_BAG);
        // Invoker.Invoke(GameDataManager.Instance.SaveBoosterData);

    }
    //
    // // private void ParseString(int stringId, long value)
    // // {
    // //     // TODO! separate common string vs special strings
    // //     
    // //     this.imgBGMaterial.sprite = null;
    // //     
    // //     this.imgIconMaterial.gameObject.SetActive(false);
    // //     this.cardMaterial.SetActive(true);
    // //     this.cardStats.SetActive(false);
    // //     
    // //     this.txtRank.gameObject.SetActive(false);
    // //     this.txtName.text = $"String {stringId}";
    // //     this.txtCount.text = $"+{value}";
    // //
    // //     this.panelNew.gameObject.SetActive(false);
    // //     this.imgFXRectOut.gameObject.SetActive(false);
    // //     this.imgFXEclipseOut.gameObject.SetActive(true);
    // //     this.imgFXEclipseOut.color = this.imgFxRectIn.color = Color.cyan;
    // //     this.imgBg.sprite = this.dicBGCards[StatsItemRarity.COMMON].sprBG;
    // //     
    // //     this.displayType = CardDisplayType.SpecialString;
    // // }
    //
    private void ParseStatsCard(ShopStatConfig c, long valueGet)
    {
        if (c != null)
        {
            this.ShopStatConfigCurrent = c;
            this.displayer.ParseForBag(c, (int)valueGet, this.durationAllOut);


            //this.imgFXRectOut.gameObject.SetActive(true);
            //this.imgFxRectIn.gameObject.SetActive(true);
            this.imgFXEclipseOut.gameObject.SetActive(false);
            this.txtRank.color = this.imgFXRectOut.color
                = this.imgFxRectIn.color = ShopCueRef.GetBgColorByRarity(c.tier);


            StatData data = StatDatas.Instance.GetStat(c.id);

            bool isUnlock = data != null;

            if (isUnlock)
            {
                this.displayType = CardDisplayType.StatsCardOld;


                this.txtRank.gameObject.SetActive(true);
                this.txtRank.text = c.tier.ToString();
                this.txtName.gameObject.SetActive(true);
                this.txtName.text = c.statName;
                this.txtCount.gameObject.SetActive(true);
                this.txtCount.text = $"x{valueGet}";
            }
            else
            {
                this.displayType = CardDisplayType.StatsCardNew;

                this.txtRank.gameObject.SetActive(false);
                this.txtName.gameObject.SetActive(false);
                this.txtCount.gameObject.SetActive(false);

                this.txtNameBig.text = c.statName;
            }

            //Invoker.Invoke(GameDataManager.Instance.SaveBoosterData);
        }
        else
        {
            this.txtName.text = "No Config!";
            Debug.LogError("CONFIG NULL");
        }
    }

    private void ShowWatchButton()
    {

        DOTween.Kill(this.btnAcceptWatch);
        this.cgRejectWatch.alpha = 0f;
        this.btnRejectWatch.image.raycastTarget = false;
        DOTween.Sequence()
            .Append(this.cgAcceptWatch.DOFade(1f, 0.5f))
            .AppendInterval(2f)
            .AppendCallback(this.ShowRejectButton).SetId(this.btnAcceptWatch);
        this.btnAcceptWatch.image.raycastTarget = true;
    }

    private void ShowRejectButton()
    {
        this.cgRejectWatch.DOFade(1f, 0.5f).SetId(this.btnAcceptWatch);
        this.btnRejectWatch.image.raycastTarget = true;
    }

    public void OnClickClaimWithAds()
    {
        if (this.boosterCurrent == null && this.ShopStatConfigCurrent == null)
        {
            Debug.LogError("OpenBagItemUpper OnClickClaimWithAds error: no current");
            return;
        }

        AdsManager.Instance.ShowVideoReward(LogAdsVideoWhere.OPEN_BAG_TAKE_ITEM, this.OnWatchAdsFinished);
    }

    public void OnClickRejectAds()
    {
        if (this.boosterCurrent == null && this.ShopStatConfigCurrent == null)
        {
            Debug.LogError("OpenBagItemUpper OnClickClaimWithAds error: no current");
            return;
        }

        SoundManager.Instance.Play("snd_step");

        this.HideBtnWatch(this.OnDoneAfterRejectAds);

        this.fxCardGlare.Stop();

        // this.fxCardGlare.DOKill();
        // DOTween.To(() => 1f,
        //     this.SetGlareFade,
        //     0f,
        //     0.4f).SetId(this.fxCardGlare);
    }


    private void OnDoneAfterRejectAds()
    {
        this.isBlockingByAds = false;
        this.host.OnClickStep();
    }

    // private void SetGlareFade(float x)
    // {
    //     var g = this.fxCardGlareMain.startColor;
    //     var col = g.color;
    //     col.a = x;
    //     g.color = col;
    //     this.fxCardGlareMain.startColor = col;
    // }

    private void HideBtnWatch(TweenCallback callback)
    {
        DOTween.Kill(this.btnAcceptWatch);
        this.cgAcceptWatch.DOFade(0f, 0.5f)
            .SetId(this.btnAcceptWatch);
        this.btnAcceptWatch.image.raycastTarget = false;
        if (callback != null)
        {
            this.cgRejectWatch.DOFade(0f, 0.5f)
                .OnComplete(callback)
                .SetId(this.btnAcceptWatch);
        }
        else
        {
            this.cgRejectWatch.DOFade(0f, 0.5f)
                .SetId(this.btnAcceptWatch);
            this.isBlockingByAds = false;
        }

        this.btnRejectWatch.image.raycastTarget = false;
    }


    // earn here
    private void OnWatchAdsFinished(bool isComplete)
    {
        if (isComplete)
        {
            SoundManager.Instance.Play("sfx_pellet_start");

            this.modelCurrent.isWatch = false;
            this.fxCardGlare.Stop();
            this.fxFlashAds.Stop();
            this.fxCardGlare2.Play();
            this.fxBolt.Play();
            if (this.boosterCurrent != null)
            {
                UserProfile.Instance.AddBooster(this.boosterCurrent.type, this.valueCurrent, this.bagType.ToString(),
                    LogSourceWhere.OPEN_BAG);
                this.boosterCurrent = null;
                Invoker.Invoke(GameDataManager.Instance.SaveBoosterData);
            }
            else if (this.ShopStatConfigCurrent != null)
            {
                StatManager.Instance.AddCard(StatDatas.Instance.GetStat(ShopStatConfigCurrent.id), valueCurrent);
                //StatDatas.Instance.CollectCard(this.ShopStatConfigCurrent, (int)this.valueCurrent);
                this.ShopStatConfigCurrent = null;

                if (this.displayType == CardDisplayType.StatsCardNew)
                {
                    this.ShowSpecialName();
                }
                Invoker.Invoke(GameDataManager.Instance.SaveBoosterData);
            }
            this.valueCurrent = 0;
            this.HideBtnWatch(null);
        }
        else
        {
            // do nothing
        }
    }

    public void StartAnimate()
    {
        this.fxCardGlare.Stop();
        this.fxFlashAds.Stop();
        if (!this.gameObject.activeSelf)
        {
            this.gameObject.SetActive(true);
        }
        DOTween.Kill(this);

        Debug.Log("b_ " + "OpenBag".WrapColor("magenta") + $" CardUpper -display {this.displayType} -isAds {this.isBlockingByAds}");

        this.cg.alpha = 1f;

        switch (this.displayType)
        {
            case CardDisplayType.Common:
            case CardDisplayType.StatsCardOld:
                {
                    this.imgFXRectOut.transform.localScale = this.scaleFXZoomOutStart;
                    this.imgFXRectOut.transform
                        .DOScale(this.statFxZoomOut.value, this.statFxZoomOut.duration).SetId(this);
                    this.imgFXRectOut
                        .DOFade(0f, this.statFxZoomOut.duration).SetId(this);

                    if (isBlockingByAds)
                    {
                        //this.fxCardGlareMain.startColor = Color.white;
                        this.fxCardGlare2.Stop();
                        this.fxBolt.Stop();
                        this.fxCardGlare.Play();
                        this.fxFlashAds.Play();
                        SoundManager.Instance.Play("sfx_pellet_end");
                        this.ShowWatchButton();
                        Invoker.Invoke(this.bag.HideBagAndCounter, 0.1f);
                    }
                }
                break;
            case CardDisplayType.StatsCardNew:
                {
                    this.imgFXRectOut.transform.localScale = this.scaleFXZoomOutStart;
                    if (isBlockingByAds)
                    {
                        //this.fxCardGlareMain.startColor = Color.white;
                        this.fxCardGlare2.Stop();
                        this.fxBolt.Stop();
                        this.fxCardGlare.Play();
                        this.fxFlashAds.Play();
                        SoundManager.Instance.Play("sfx_pellet_end");
                        this.imgFXRectOut.transform
                            .DOScale(this.statFxZoomOut.value, this.statFxZoomOut.duration).SetId(this)
                            .OnComplete(ShowWatchButton);
                    }
                    else
                    {
                        this.imgFXRectOut.transform
                            .DOScale(this.statFxZoomOut.value, this.statFxZoomOut.duration).SetId(this)
                            .OnComplete(ShowSpecialName);
                    }

                    this.imgFXRectOut
                        .DOFade(0f, this.statFxZoomOut.duration).SetId(this);
                }
                break;
            case CardDisplayType.SpecialString:
                {
                    this.imgFXEclipseOut.transform.localScale = this.scaleFXZoomOutStart;
                    this.imgFXEclipseOut.transform
                        .DOScale(this.statFxZoomOut.value, this.statFxZoomOut.duration).SetId(this);
                    this.imgFXEclipseOut
                        .DOFade(0f, this.statFxZoomOut.duration).SetId(this);
                }
                break;
        }



        this.imgFxRectIn.transform.localScale = this.scaleFXZoomInStart;
        this.imgFxRectIn.transform
            .DOScale(this.statFxZoomIn.value, this.statFxZoomIn.duration).SetId(this);
        this.imgFxRectIn
            .DOFade(0f, this.statFxZoomIn.duration).SetId(this);
    }

    private void ShowSpecialName()
    {
        this.txtNameBig.gameObject.SetActive(true);
        this.txtNameBig.color = COLOR_TRANSPARENT_WHITE;
        this.txtNameBig.DOColor(Color.white, this.statFxZoomOut.duration / 2f)
            .SetId(this)
            .OnComplete(this.ShowSpecialUnlocked);

        this.transNameBig.localScale = Vector3.one * 5f;
        this.transNameBig.DOScale(1f, this.statFxZoomOut.duration / 2f)
            .SetId(this);
    }

    private void ShowSpecialUnlocked()
    {
        this.txtNewUnlocked.gameObject.SetActive(true);
        this.txtNewUnlocked.color = COLOR_TRANSPARENT_WHITE;
        this.txtNewUnlocked.DOColor(Color.white, this.statFxZoomOut.duration / 2f)
            .SetId(this)
            .OnComplete(this.ShowSpecialUnlocked2);

        this.transNewUnlocked.localScale = Vector3.one * 5f;
        this.transNewUnlocked.DOScale(1f, this.statFxZoomOut.duration / 2f)
            .SetId(this);
    }
    private void ShowSpecialUnlocked2()
    {
        this.txtNewUnlockedFX.gameObject.SetActive(true);
        this.txtNewUnlockedFX.color = Color.white;
        this.txtNewUnlockedFX.DOColor(COLOR_TRANSPARENT_WHITE, this.statFxZoomOut.duration)
            .SetId(this);

        this.transNewUnlockedFX.localScale = Vector3.one;
        this.transNewUnlockedFX.DOScale(5f, this.statFxZoomOut.duration * 2f)
            .SetId(this);
    }

    public void StartHide()
    {
        DOTween.Kill(this, true);
        this.cg.DOFade(0f, this.durationAllOut).SetId(this);
    }
}
