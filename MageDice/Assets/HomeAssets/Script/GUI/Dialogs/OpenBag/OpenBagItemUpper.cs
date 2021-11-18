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
        Card,
        CardNewCue,
        Cue,
        Box,
    }

    public OpenBagDialog host;
    public OpenBagBag bag;
    
    [Header("Linkers")]
    public CanvasGroup cg;
    public StatCardItemDisplay displayer;
    public CueUnlock partUnlock;
    public CollectCuePanel displayerCue;
    
    [Space(10f)]
    public TextMeshProUGUI txtRank;
    public TextMeshProUGUI txtName;
    public TextMeshProUGUI txtCount;

    //public TextMeshProUGUI txtNameBig;
    // public TextMeshProUGUI txtNewUnlocked;
    // public TextMeshProUGUI txtNewUnlockedFX;
    //private Transform transNameBig;
    // private Transform transNewUnlocked;
    // private Transform transNewUnlockedFX;

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
    
    
    /// <summary>
    /// (Rectangle) this also fades out
    /// </summary>
    [Header("Linker FXs")]
    public Image imgFXRectOut;
    /// <summary>
    /// (Eclipse) this also fades out
    /// </summary>
    public Image imgFXEclipseOut;
    /// <summary>
    /// this also fades in
    /// </summary>
    public Image imgFxRectIn;

    [Space(5f)]
    public Image imgFxFlash;
    

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
    
    private BoosterConfig boosterCurrent;
    private StatData StatDataCurrent;
    private long valueCurrent;
    
    private bool isBlockingByAds = false;
    private bool isBlockingByAnim = false;


    public void Init()
    {
        this.cachedTransform = this.transform;

        //this.transNameBig = this.txtNameBig.transform;
        // this.transNewUnlocked = this.txtNewUnlocked.transform;
        // this.transNewUnlockedFX = this.txtNewUnlockedFX.transform;

        this.displayer.gameObject.SetActive(false);
        this.partUnlock.gameObject.SetActive(false);
        this.displayerCue.Init(this.ShowWatchButton, this.SetBlockingByAds);
    }

    public virtual void ParseData(BagType bag, OpenBagDialog.BagCardModel model, Vector3 position)
    {
        DOTween.Kill(this, true);
        
#if K_TEST_BAG_ADS
        model.isWatch = true;
#endif

        this.bagType = bag;
        this.modelCurrent = model;

        if (model.earnType == OpenBagDialog.BagCardModel.EarnType.Cue)
        {
            this.displayType = CardDisplayType.Cue;

            if (model.isWatch)
            {
                this.StatDataCurrent = model.equipmentConfig;
            }
            this.displayerCue.ParseCue(model.equipmentConfig, model.isWatch);
            return;
        }
        
        
        this.cachedTransform.position = position;
        
        this.boosterCurrent = null;
        this.StatDataCurrent = null;
        
        if (!(model.isWatch))
        {
            this.valueCurrent = 0;
            switch (model.earnType)
            {
                case OpenBagDialog.BagCardModel.EarnType.Booster:
                    this.ParseBoosterAndEarn(BoosterConfigs.Instance.GetBooster(model.booster), model.value);
                    break;
                case OpenBagDialog.BagCardModel.EarnType.CueCard:
                    StatData StatData = model.equipmentConfig;
                    this.ParseStatsCardAndEarn(StatData, model.value);
                    break;
            }
        }
        else
        {
            this.valueCurrent = model.value;
            this.isBlockingByAds = true;
            this.host.txtLogBlockByAd.text = "block by ad: OpenBagItemUpper ParseData";
            
            switch (model.earnType)
            {
                case OpenBagDialog.BagCardModel.EarnType.Booster:
                    this.ParseBooster(BoosterConfigs.Instance.GetBooster(model.booster), model.value);
                    break;
                case OpenBagDialog.BagCardModel.EarnType.CueCard:
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
    
        this.displayer.gameObject.SetActive(true);
        this.displayer.ParseData(b.type);
        
        this.imgFXRectOut.color = this.imgFxRectIn.color = Color.white;
        this.imgFXEclipseOut.gameObject.SetActive(false);
    
        this.displayType = CardDisplayType.Common;

        // earn here
        UserProfile.Instance.AddBooster(b.type, value, this.bagType.ToString(),
            LogSourceWhere.OPEN_BAG);
    }
    
    /// <summary>
    /// for card, not for cue
    /// </summary>
    /// <param name="c"></param>
    /// <param name="valueGet"></param>
    private void ParseStatsCardAndEarn(StatData c, long valueGet)
    {
        if (c != null)
        {
            this.displayer.gameObject.SetActive(true);
            this.displayer.ParseStatData(c, (int)valueGet);
            
            this.imgFXRectOut.gameObject.SetActive(true);
            this.imgFxRectIn.gameObject.SetActive(true);
            this.imgFXEclipseOut.gameObject.SetActive(false);
            this.txtRank.color = ColorCommon.GetBgColorByRarity(c.config.tier);
            this.imgFXRectOut.color = this.imgFxRectIn.color = Color.white;
            
            this.displayType = CardDisplayType.Card;
                
                
            this.txtRank.gameObject.SetActive(true);
            this.txtRank.text = c.config.tier.ToString();
            this.txtName.gameObject.SetActive(true);
            this.txtName.text = c.config.statName;
            this.txtCount.gameObject.SetActive(true);
            this.txtCount.text = $"x{valueGet}";
            
            
            if (c.level == 0 && c.RequirementCard != 0 && valueGet != 0)
            {
                this.isBlockingByAnim = true;
                this.host.txtLogBlockByAnim.text = "block by anim: OpenBagItemUpper ParseStatsCardAndEarn";
                this.partUnlock.ParseCue(c);


                // earn here
                StatManager.Instance.AddCard(c, valueGet);

                this.StatDataCurrent = c;
                this.partUnlock.AnimateRemoveCoverInBag(
                    valueGet > int.MaxValue ? int.MaxValue : (int)valueGet,
                    0.7f,
                    this.OnFxUnlockCueStarting,
                    this.OnAnimateRemovedCoverComplete);
            }
            else
            {
                // earn here
                StatManager.Instance.AddCard(c, valueGet); 
            }
            
            this.displayer.FillToCurrentCard(c, 1f);

            //mission
            //MissionDatas.Instance.DoStep(MissionID.COLLECT_CARD, (int)valueGet);
        }
        else
        {
            this.txtName.text = "No Config!";
            Debug.LogError("CONFIG NULL");
        }
    }

    private void OnFxUnlockCueStarting()
    {
        this.displayType = CardDisplayType.CardNewCue;
        
        this.displayerCue.ParseCueNotEarn(this.StatDataCurrent);
        
        SoundManager.Instance.Play("sfx_glare_mid");
        
        this.imgFxFlash.color = COLOR_TRANSPARENT_WHITE;
        this.imgFxFlash.DOFade(1f, 0.1f)
            .OnComplete(this.OnFxUnlockCueStarted).SetId(this);
    }

    private void OnFxUnlockCueStarted()
    {
        this.imgFxFlash.DOColor(COLOR_TRANSPARENT_WHITE, 0.9f)
            .SetEase(Ease.InQuart).SetId(this);
        this.displayerCue.StartAnimate();
    }

    private void OnAnimateRemovedCoverComplete()
    {
        this.isBlockingByAnim = false;
        this.host.txtLogBlockByAnim.text = "unblock by anim: OpenBagItemUpper OnAnimateRemovedCoverComplete";
    }
    
    private void ParseBooster(BoosterConfig b, long value)
    {
        this.boosterCurrent = b;
        
        this.txtRank.gameObject.SetActive(false);
        this.txtName.text = b.name;
        this.txtName.gameObject.SetActive(true);
        this.txtCount.text = $"+{value}";
        this.txtCount.gameObject.SetActive(true);
    
        this.displayer.gameObject.SetActive(true);
        this.displayer.ParseData(b.type);
        
        this.imgFXRectOut.color = this.imgFxRectIn.color = Color.cyan;
        this.imgFXEclipseOut.gameObject.SetActive(false);
    
        this.displayType = CardDisplayType.Common;
    }
    
    private void ParseStatsCard(StatData c, long valueGet)
    { 
        if (c != null)
        {
            this.StatDataCurrent = c;
            
            this.displayer.gameObject.SetActive(true);
            this.displayer.ParseStatData(c, (int)valueGet);
            
            
            this.imgFXRectOut.gameObject.SetActive(true);
            this.imgFxRectIn.gameObject.SetActive(true);
            this.imgFXEclipseOut.gameObject.SetActive(false);
            this.txtRank.color =  ColorCommon.GetBgColorByRarity(c.config.tier);
            this.imgFXRectOut.color = this.imgFxRectIn.color = Color.white;
            
            this.displayType = CardDisplayType.Card;
            this.txtRank.gameObject.SetActive(true);
            this.txtRank.text = c.config.tier.ToString();
            this.txtName.gameObject.SetActive(true);
            this.txtName.text = c.config.statName;
            this.txtCount.gameObject.SetActive(true);
            this.txtCount.text = $"x{valueGet}";
        }
        else
        {
            this.txtName.text = "No data!";
            Debug.LogError("DATA NULL");
        }
    }
    
    private void ShowWatchButton()
    {

        DOTween.Kill(this.btnAcceptWatch, true);
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
        this.host.DisableInterstitial();
        
        if (this.boosterCurrent == null && this.StatDataCurrent == null)
        {
            Debug.LogError("OpenBagItemUpper OnClickClaimWithAds error: no current");
            return;
        }
        
        AdsManager.Instance.ShowVideoReward(LogAdsVideoWhere.OPEN_BAG_TAKE_ITEM, this.OnWatchAdsFinished);
    }

    public void OnClickRejectAds()
    {
        if (this.boosterCurrent == null && this.StatDataCurrent == null)
        {
            Debug.LogError("OpenBagItemUpper OnClickClaimWithAds error: no current");
            return;
        }
        
        SoundManager.Instance.Play("snd_step");

        if (this.displayType == CardDisplayType.Cue)
        {
            this.displayerCue.OnAdsRejected();
        }
        this.HideBtnWatch(this.OnDoneAfterRejectAds);

        this.fxCardGlare.Stop();
    }
    
    
    private void OnDoneAfterRejectAds()
    {
        this.isBlockingByAds = false;
        this.host.txtLogBlockByAd.text = "unblock by ad: OpenBagItemUpper OnDoneAfterRejectAds";
        this.host.OnClickStep();
    }

    private void HideBtnWatch(TweenCallback callback)
    {
        DOTween.Kill(this.btnAcceptWatch, true);
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
            this.host.txtLogBlockByAd.text = "unblock by ad: OpenBagItemUpper HideBtnWatch";
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
            
            if (this.boosterCurrent != null)
            {
                this.fxCardGlare.Stop();
                this.fxFlashAds.Stop();
                this.fxCardGlare2.Play();
                this.fxBolt.Play();
                
                UserProfile.Instance.AddBooster(this.boosterCurrent.type, this.valueCurrent,
                    this.bagType.ToString(), LogSourceWhere.OPEN_BAG);
                this.boosterCurrent = null;
            }
            else if (this.StatDataCurrent != null)
            {
                switch (this.displayType)
                {
                    case CardDisplayType.Card:
                        this.fxCardGlare.Stop();
                        this.fxFlashAds.Stop();
                        this.fxCardGlare2.Play();
                        this.fxBolt.Play();
                        
                        if (this.StatDataCurrent.level == 0 && this.StatDataCurrent.RequirementCard != 0 &&
                            this.valueCurrent != 0)
                        {
                            this.isBlockingByAnim = true;
                            this.host.txtLogBlockByAnim.text = "block by anim: OpenBagItemUpper OnWatchAdsFinished";
                            
                            this.partUnlock.ParseCue(this.StatDataCurrent);
            
                            // earn here
                            StatManager.Instance.AddCard(this.StatDataCurrent, this.valueCurrent);  

                            this.partUnlock.AnimateRemoveCoverInBag(
                                this.valueCurrent > int.MaxValue ? int.MaxValue : (int)this.valueCurrent,
                                0.7f,
                                this.OnFxUnlockCueStarting,
                                this.OnAnimateRemovedCoverComplete);
                        }
                        else
                        {
                            // earn here
                            StatManager.Instance.AddCard(this.StatDataCurrent, this.valueCurrent);   
                        }
                        //mision
                        //MissionDatas.Instance.DoStep(MissionID.COLLECT_CARD, (int)this.valueCurrent);
                        this.displayer.FillToCurrentCard(this.StatDataCurrent, 1f);
                        break;

                    case CardDisplayType.Cue:
                        this.displayerCue.OnAdsWatched();
                        break;
                }
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
        DOTween.Kill(this, true);

        if (this.displayType == CardDisplayType.Cue)
        {
            this.displayerCue.StartAnimate();
            return;
        }
        
        if (!this.gameObject.activeSelf)
        {
            this.gameObject.SetActive(true);            
        }
        
        Debug.Log("b_ " + "OpenBag".WrapColor("magenta") + $" CardUpper -display {this.displayType} -isAds {this.isBlockingByAds}" );
        
        this.cg.alpha = 1f;

        switch (this.displayType)
        {
            case CardDisplayType.Common:
            case CardDisplayType.Card:
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
            case CardDisplayType.Box:
            {
                Debug.LogError("OpenBagItemUpper: Box not supported!");
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

    private void SetBlockingByAds(bool isBlock)
    {
        this.isBlockingByAds = isBlock;
        // log the blocker origin from the source
    }
    
    public bool CheckClickable()
    {
        return !this.isBlockingByAds && !this.isBlockingByAnim;
    }
    
    public void StartHide()
    {
        DOTween.Kill(this, true);

        switch (this.displayType)
        {
            case CardDisplayType.Cue:
                this.displayerCue.StartHide();
                break;
            case CardDisplayType.CardNewCue:
                this.displayerCue.StartHide();
                goto default;
                break;
            default:
                this.fxBolt.Stop();
                this.cg.DOFade(0f, this.durationAllOut).OnComplete(this.OnHideComplete).SetId(this);
                break;
        }
    }

    private void OnHideComplete()
    {
        this.displayer.gameObject.SetActive(false);
        this.partUnlock.Hide();
    }
}
