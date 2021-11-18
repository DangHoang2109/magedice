using System.Collections;
using System.Collections.Generic;
using Coffee.UIEffects;
using DG.Tweening;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class CollectCuePanel : MonoBehaviour
{
    private static readonly Color COL_TRANSPARENT = new Color(1f, 1f, 1f, 0f);
    private static readonly Vector3 VEC_SCALE_BIG = new Vector3(5f, 5f, 5f);

    public OpenBagDialog hostBag;
    public CollectCueDialog hostCollect;
    
    [Header("Cue fx")]
    public Image imgCue;
    public GlowImage glowCue;
    public Image smoke1;
    public Image smoke2;
    
    [Header("Getting cue fx")]
    public TextMeshProUGUI txtYouGot;
    public TextMeshProUGUI txtFxYouGot;
    public TextMeshProUGUI txtstatName;
    public TextMeshProUGUI txtFxstatName;

    public ParticleSystem fxFirefly;
    
    private CanvasGroup cg;
    //private UIShiny shinyCue;


    private System.Action callShowButtonsAds;
    private System.Action<bool> callSetBlockByAds;

    private StatData StatDataCurrent;

    private bool isWatch;
    
    /// <param name="callShowBtnAds">show button watch, then button reject</param>
    /// <param name="callSetBlockByAds">set state "is blocking by ads"</param>
    public void Init(System.Action callShowBtnAds, System.Action<bool> callSetBlockByAds)
    {
        this.callShowButtonsAds = callShowBtnAds;
        this.callSetBlockByAds = callSetBlockByAds;
        
        if (this.cg == null)
        {
            this.cg = this.GetComponent<CanvasGroup>();
            this.cg.alpha = 0f;
        }
    }
    
    public void ParseCue(StatData cData, bool isWatch)
    {
        if (this.cg == null)
        {
            this.cg = this.GetComponent<CanvasGroup>();
            this.cg.alpha = 0f;
        }
        
        if (cData == null)
        {
            Debug.LogException(new System.Exception("CollectCuePanel ParseCue ERROR! cData NULL!"));
            return;
        }
        ShopStatConfig c = cData.config;
        if (c == null)
        {
            Debug.LogException(new System.Exception("CollectCuePanel ParseCue ERROR! model.equipmentConfig not linked to CueConfig!"));
            return;
        }

        this.isWatch = isWatch;
        this.glowCue.sprite = this.imgCue.sprite = c.sprStatItem;
        Color col = ColorCommon.GetBgColorByRarity(c.tier);
        this.glowCue.glowColor = col;
        col.a = 0.3f;
        this.smoke1.color = this.smoke2.color = col;
        
        
        this.txtstatName.text = c.statName;
        if (!isWatch)
        {
            this.txtYouGot.text = LanguageManager.GetString("CUE_YOUGOT", LanguageCategory.Feature);
            
            // earn here
            StatManager.Instance.WinCue(cData);

            this.StatDataCurrent = null;
        }
        else
        {
            this.txtYouGot.text = LanguageManager.GetString("CUE_YOUWILLGET", LanguageCategory.Feature);
            if (this.callShowButtonsAds == null)
            {
                Debug.LogError("CollectCuePanel ParseCue not initialized properly");
                this.callSetBlockByAds?.Invoke(false);
                if (this.hostBag != null)
                    this.hostBag.txtLogBlockByAd.text = "unblock by ad: CollectCuePanel ParseCue";
                else if(this.hostCollect != null)
                    this.hostCollect.txtLogBlockByAd.text = "unblock by ad: CollectCuePanel ParseCue";
                return;
            }
            this.callShowButtonsAds();

            this.StatDataCurrent = cData;
        }
    }
    
    public void ParseCueNotEarn(StatData cData)
    {
        this.isWatch = false;
        if (this.cg == null)
        {
            this.cg = this.GetComponent<CanvasGroup>();
            this.cg.alpha = 0f;
        }
        
        if (cData == null)
        {
            Debug.LogException(new System.Exception("CollectCuePanel ParseCue ERROR! cData NULL!"));
            return;
        }
        ShopStatConfig c = cData.config;
        if (c == null)
        {
            Debug.LogException(new System.Exception("CollectCuePanel ParseCue ERROR! model.equipmentConfig not linked to CueConfig!"));
            return;
        }

        this.glowCue.sprite = this.imgCue.sprite = c.sprStatItem;
        Color col = ColorCommon.GetBgColorByRarity(c.tier);
        this.glowCue.glowColor = col;
        col.a = 0.3f;
        this.smoke1.color = this.smoke2.color = col;
        
        
        this.txtstatName.text = c.statName;
        if (!isWatch)
        {
            this.txtYouGot.text = LanguageManager.GetString("CUE_YOUGOT", LanguageCategory.Feature);
            this.StatDataCurrent = null;
        }
    }

    public void StartAnimate()
    {
        DOTween.Kill(this, true);
        this.cg.alpha = 1f;
        
        if (!this.gameObject.activeSelf)
        {
            this.gameObject.SetActive(true);            
        }
        this.fxFirefly.Play();
        this.imgCue.color = Color.white;
        
        if (!this.isWatch)
        {
            this.AnimateGotCue();
        }
        else
        {
            this.AnimateWillGetCue();
        }
    }

    private void AnimateGotCue()
    {
        this.txtYouGot.color = COL_TRANSPARENT;
        this.txtFxYouGot.color = COL_TRANSPARENT;
        this.txtstatName.color = COL_TRANSPARENT;
        this.txtFxstatName.color = COL_TRANSPARENT;

        this.txtFxYouGot.text = this.txtYouGot.text;
        this.txtFxstatName.text = this.txtstatName.text;

        DOTween.Sequence()
            .AppendInterval(0.000001f)
            .AppendCallback(this.OnSetBlock)
            .AppendInterval(0.5f)
            .AppendCallback(this.AnimateGotCue1)
            .AppendInterval(0.2f)
            .AppendCallback(this.AnimateGotCue2)
            .AppendInterval(0.4f)
            .AppendCallback(this.AnimateGotCue2End)
            .AppendInterval(0.4f)
            .OnComplete(this.StopBlockByAnimation)
            .SetId(this);
    }

    private void OnSetBlock()
    {
        this.callSetBlockByAds?.Invoke(true);
        if (this.hostBag != null)
            this.hostBag.txtLogBlockByAd.text = "block by ad: CollectCuePanel AnimateGotCue";
        else if(this.hostCollect != null)
            this.hostCollect.txtLogBlockByAd.text = "block by ad: CollectCuePanel AnimateGotCue";
    }

    private void AnimateGotCue1()
    {
        SoundManager.Instance.Play("sfx_pellet_start");
        this.txtYouGot.transform.localScale = VEC_SCALE_BIG;
        
            
        DOTween.Sequence()
            .Append(this.txtYouGot.DOFade(1f, 0.4f))
            .Join(this.txtYouGot.transform.DOScale(1f, 0.4f))
            .OnComplete(this.AnimateGotCue1End).SetId(this);
    }

    private void AnimateGotCue1End()
    {
        SoundManager.Instance.Play("sfx_pellet_end");
        this.txtFxYouGot.color = Color.white;
        this.txtFxYouGot.transform.localScale = Vector3.one;
        DOTween.Sequence()
            .Append(this.txtFxYouGot.transform.DOScale(5f, 0.4f))
            .Join(this.txtFxYouGot.DOFade(0f, 0.4f))
            .SetId(this);
    }
    
    private void AnimateGotCue2()
    {
        SoundManager.Instance.Play("sfx_pellet_start");
        
        this.txtstatName.transform.localScale = VEC_SCALE_BIG;
        
        DOTween.Sequence()
            .Append(this.txtstatName.transform.DOScale(1f, 0.4f))
            .Join(this.txtstatName.DOFade(1f, 0.4f))
            .SetId(this);
    }

    private void AnimateGotCue2End()
    {
        SoundManager.Instance.Play("sfx_pellet_end");
        
        this.txtFxstatName.color = Color.white;
        this.txtFxstatName.transform.localScale = Vector3.one;

        DOTween.Sequence()
            .Append(this.txtFxstatName.transform.DOScale(5f, 0.4f))
            .Join(this.txtFxstatName.DOFade(0f, 0.4f))
            .SetId(this);
    }

    private void PlaySoundEnd()
    {
        SoundManager.Instance.Play("sfx_pellet_end");
    }

    private void StopBlockByAnimation()
    {
        this.callSetBlockByAds?.Invoke(false);
        
        if (this.hostBag != null)
            this.hostBag.txtLogBlockByAd.text = "unblock by ad: CollectCuePanel StopBlockByAnimation";
        else if(this.hostCollect != null)
            this.hostCollect.txtLogBlockByAd.text = "unblock by ad: CollectCuePanel StopBlockByAnimation";
    }

    private void AnimateWillGetCue()
    {
        this.callSetBlockByAds?.Invoke(true);
        if (this.hostBag != null)
            this.hostBag.txtLogBlockByAd.text = "block by ad: CollectCuePanel AnimateWilLGetCue";
        else if(this.hostCollect != null)
            this.hostCollect.txtLogBlockByAd.text = "block by ad: CollectCuePanel AnimateWilLGetCue";
        
        
        this.txtYouGot.color = COL_TRANSPARENT;
        this.txtFxYouGot.color = COL_TRANSPARENT;
        this.txtstatName.color = COL_TRANSPARENT;
        this.txtFxstatName.color = COL_TRANSPARENT;
        
        this.txtYouGot.transform.localScale = this.txtstatName.transform.localScale = Vector3.one;

        DOTween.Sequence()
            .AppendInterval(0.7f)
            .AppendCallback(() =>
            {
                SoundManager.Instance.Play("sfx_pellet_end");
                this.txtYouGot.color = Color.yellow;
            })
            .Append(this.txtYouGot.DOColor(Color.white, 0.4f).SetEase(Ease.InCubic))
            .AppendCallback(() =>
            {
                SoundManager.Instance.Play("sfx_pellet_end");
                this.txtstatName.color = Color.yellow;
            })
            .Append(this.txtstatName.DOColor(Color.white, 0.4f).SetEase(Ease.InCubic))
            .SetId(this);
    }

    public void OnAdsWatched()
    {
        this.txtYouGot.text = LanguageManager.GetString("CUE_YOUGOT", LanguageCategory.Feature);
        this.AnimateGotCue();

        if (this.StatDataCurrent == null)
        {
            Debug.LogException(new System.Exception("CollectCuePanel OnAdsWatched ERROR! StatDataCurrent NULL!"));
            return;
        }
        StatManager.Instance.WinCue(this.StatDataCurrent);
    }

    public void OnAdsRejected()
    {
        this.txtYouGot.text = LanguageManager.GetString("CUE_YOUREJECT", LanguageCategory.Feature);

        this.imgCue.DOColor(Color.black, 0.7f)
            .SetEase(Ease.InCubic).SetId(this);
        
        this.fxFirefly.Stop();
    }
    
    public void StartHide()
    {
        DOTween.Kill(this, true);
        this.cg.DOFade(0f, 0.5f).SetId(this);
        
        this.fxFirefly.Stop();
    }
}
