using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using Cosina;
#if ONESOFT
using FalconSDK.Advertising;
#endif
public class AdsManager : MonoSingleton<AdsManager>
{
    public const float TIME_INTERSTITIAL = 30;
    public List<AdProvide> adProvides;
    private bool isInitAds = false;
    private float timeInterstitial = 0;
#if UNITY_EDITOR
    private void OnValidate()
    {
        //if(this.admob == null)
        //{
        //    GameObject obj = new GameObject("admob");
        //    obj.transform.SetParent(this.transform);
        //    this.admob = obj.AddComponent<AdmobFactory>();
        //}
    }
#endif
    public override void Init()
    {
        base.Init();
        this.adProvides = new List<AdProvide>();
        if (!this.isInitAds)
        {
            this.StartCoroutine(this.InitAds());
        }
    }
    private void OnDisable()
    {
    }
    private void Update()
    {
        if (this.timeInterstitial > 0)
        {
            this.timeInterstitial -= Time.deltaTime;
        }
    }
    private IEnumerator InitAds()
    {
        this.timeInterstitial = 15.0f;
        this.LoadAds();
        yield return new WaitForEndOfFrame();

    }
    private void LoadAds()
    {
        if (this.isInitAds)
        {
            return;
        }
        //Debug.LogError("INIT ADS: " + this.isInitAds);
#if ADMOB_ADS
        AdmobAds admobAds = new GameObject().AddComponent<AdmobAds>();
        if (admobAds != null)
        {
            admobAds.transform.SetParent(this.transform);
            admobAds.name = "Admob Ads";
            //admobAds.InitAds();
            this.adProvides.Add(admobAds);
        }
#endif
#if UNITY_ADS

        UnityAds unityads = new GameObject().AddComponent<UnityAds>();
        if (unityads != null)
        {
            unityads.transform.SetParent(this.transform);
            unityads.name = "Unity Ads";
            //admobAds.InitAds();
            this.adProvides.Add(unityads);

        }
#endif
        this.isInitAds = true;
    }

    private void OnChangeNetwork(bool isNetwork)
    {
        if (!this.isInitAds && isNetwork)//chua init ads && network
        {
            //
        }
    }
    
    #region BANNER

    public void ShowBanner()
    {
#if ONESOFT && EXISTED_IRON_SOURCE
        IronSourceManager.Instance.ShowBanner();
#endif
    }

    public void HideBanner()
    {
#if ONESOFT && EXISTED_IRON_SOURCE
        IronSourceManager.Instance.HideBanner();
#endif
    }

    #endregion


    #region INTERSTITIAL
    public AdProvide IsInterstitial()
    {

        foreach (AdProvide ads in this.adProvides)
        {
            if (ads.IsInterstitial())
            {
                return ads;
            }
        }
        return null;
    }
    public void ShowInterstitial(string where, UnityAction callback = null)
    {
        #if !ONESOFT
        if (!this.isInitAds)
        {
            this.LoadAds();
            return;
        }
        #endif
        if (UserBehaviorDatas.Instance.IsPurchaseUser())
        {
            return;
        }
        if (this.timeInterstitial > 0)
        {
            return;
        }

        AdProvide ad = this.IsInterstitial();
        if (ad != null)
        {
            ad.ShowInterstitial();
            this.timeInterstitial = this.GetTimeInterstitialAds();
            return;
        }

    }

    private float GetTimeInterstitialAds()
    {
        return TIME_INTERSTITIAL;
    }

    #endregion

    /// <summary>
    /// check video reward available
    /// </summary>
    public bool IsReward()
    {
        return this.IsVideoReward() != null;
    }

    private AdProvide IsVideoReward()
    {
        foreach (AdProvide ads in this.adProvides)
        {
            if (ads.IsVideoReward())
            {
                return ads;
            }
        }
        return null;
    }
    /// <summary>
    /// Show video reward, if video is showed will callback true when completed, or false on other cases
    /// <br></br> no internet -> no callback, but displays a dialog let the player know
    /// <br></br> auto prevent fast-clicking (rapidly request ads) 
    /// </summary>
    public void ShowVideoReward(string where, UnityAction<bool> callback = null)
    {
#if !ONESOFT
        if (!this.isInitAds)
        {
            this.LoadAds();
            return;
        }
#endif
#if UNITY_EDITOR
        if (callback != null)
        {
            callback.Invoke(true);
        }
        return;
#endif
        AdProvide ad = this.IsVideoReward();
        if (ad == null)//khong co ads, request
        {
            this.StartCoroutine(this.RequestVideoAds(where, callback));
        }
        else
        {
            ad.ShowVideoReward(callback);
            UserBehaviorDatas.Instance.WatchAds();
        }
    }


    private IEnumerator RequestVideoAds(string where, UnityAction<bool> callback = null)
    {
        LoadingManager.Instance.ShowLoading(true);
        yield return new WaitForEndOfFrame();
        foreach (AdProvide ad in this.adProvides)
        {
            ad.RequestVideoReward();
        }
        yield return new WaitForSeconds(2.0f);
        LoadingManager.Instance.ShowLoading(false);
        AdProvide reward = this.IsVideoReward();
        if (reward != null)
        {
            reward.ShowVideoReward(callback);
            UserBehaviorDatas.Instance.WatchAds();
        }
        else
        {
            /*LanguageManager.Instance.ShowCommonError(LanguageManager.Instance.GetString("DES_NOADS", LanguageCategory.Commons))
                .SetButtonOk().SetButtonYes().SetButtonNo();*/

        }
    }
}

public static class LogAdsInterstitialWhere
{
    public const string GAME_STANDARD = "Game_Standard";
    public const string GAME_QUICK_FIRE = "Game_Quick_Fire";
    public const string END_GAME = "End_Game";
    public const string OPEN_BOX = "Open_Box";
}

public static class LogAdsVideoWhere
{
    public const string DOUBLE_WIN_GAME = "Double_Win_Game";
    public const string FREE_COIN_HOME = "Free_Coin_Home";
    public const string REDUCE_TIME_BAG = "Reduce_Time_Bag";
    public const string WATCH_DEAL_SHOP = "Watch_Deal_Shop";
    public const string OPEN_BAG_TAKE_ITEM = "Open_Bag_Take_Item";
    public const string PLAY_BULLEYE_ADS = "Play_bulleye_ads";
    public const string PLAY_WHEEL_ADS = "Play_wheel_ads";

}
