using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using UnityEngine.Events;
#if ADMOB_ADS
using GoogleMobileAds.Api;
using GoogleMobileAds.Api.Mediation.UnityAds;
#endif
namespace Cosina
{
#if ADMOB_ADS
    public class AdmobAds : MonoBehaviour, AdProvide
    {
        private int countVideo;
        private void Start()
        {
            this.InitAds();
        }
        private AdsId GetAdsId()
        {
#if CHEAT
            return AdConfigs.Instance.TEST;
#else
            if (!AdConfigs.Instance.isTest)
            {
#if FIREBASE
                AdsId remoteAds = FireBaseManager.Instance.GetAdsId();
                if (remoteAds != null)
                {
                    return remoteAds;
                }
#endif
                return AdConfigs.Instance.GetAdmodId();
            }
            return AdConfigs.Instance.GetAdmodId();
#endif
        }

        public void InitAds()
        {
            MobileAds.Initialize(this.GetAdsId().appid);

            //this.InitBanner();
            this.InitInterstitial();
            this.InitVideoReward();
            this.countVideo = 0;
        }
        #region BANNER
        private BannerView banner;
        private bool isBanner = false;
        public void InitBanner()
        {
            this.isBanner = false;
            if (this.GetAdsId() == null)
            {
                this.Message("Admob id NULL");
                return;
            }

            if (this.banner != null)
            {
                this.banner.Destroy();
            }
            AdSize adSize = new AdSize(this.bannerSizeToPixels(320), this.bannerSizeToPixels(50));
            this.banner = new BannerView(this.GetAdsId().banner, AdSize.Banner, AdPosition.Top);
            if (this.banner != null)
            {
                this.Message("banner inited");
            }

            this.banner.OnAdLoaded += BannerOnAdLoaded;
#if DEVICE_ID
            string deviceId = AdMobUtility.GetTestDeviceId();
            AdRequest request = new AdRequest.Builder().AddTestDevice(deviceId).Build();
            this.Message("request baner: " + deviceId);
            this.banner.LoadAd(request);
#else

            AdRequest request = new AdRequest.Builder().Build();

            this.banner.LoadAd(request);
#endif
        }
        private int bannerSizeToPixels(int size)
        {
            return size * Mathf.RoundToInt(Screen.dpi / 160);
        }

        public bool IsBanner()
        {
            if (this.banner != null)
            {
                return this.isBanner;
            }
            this.InitBanner();
            return false;
        }

        public void RequestBanner()
        {
            if (this.banner != null)
            {
                if (!this.isBanner)
                {
                    this.InitBanner();
                }
            }
            else
            {
                this.InitBanner();
            }
        }
        public void ShowBanner()
        {
            if (this.banner != null)
            {
                if (this.isBanner)
                {
                    this.banner.Show();
                }
                else
                {
                    this.InitBanner();
                }
            }
            else
            {
                this.InitBanner();
            }

        }
        public void HideBanner()
        {
            if (this.banner != null)
            {
                this.banner.Hide();
                this.banner.Destroy();
            }
        }
        public void BannerOnAdLoaded(object sender, EventArgs args)
        {
            this.isBanner = true;
        }
        #endregion
        #region INTERSTITIAL
        private InterstitialAd interstitial;
        private void InitInterstitial()
        {
            if (this.GetAdsId() == null)
            {
                this.Message("Admob id NULL");
                return;
            }
            if (this.interstitial != null)
            {
                this.interstitial.Destroy();
            }


            this.interstitial = new InterstitialAd(this.GetAdsId().instertitial);
            if (this.interstitial != null)
            {
                this.Message("interstitial inited");
            }

            this.interstitial.OnAdClosed += (delegate (System.Object sender, EventArgs args)
            {
                this.Message("interstitial to close");
                //this.RequestInterstitial();
                this.ReloadInterstitial();

            });
            this.interstitial.OnAdLoaded += (delegate (System.Object sender, EventArgs args)
            {
                this.Message("interstitial Loaded");
            });
            this.interstitial.OnAdFailedToLoad += (delegate (System.Object sender, AdFailedToLoadEventArgs args)
            {
                this.Message("interstitial fail to load: " + args.Message);
            });

#if DEVICE_ID
            string deviceId = AdMobUtility.GetTestDeviceId();
            AdRequest request = new AdRequest.Builder().AddTestDevice(deviceId).Build();
            this.Message("request interstitial: " + deviceId);
            this.interstitial.LoadAd(request);
#else
            AdRequest request = new AdRequest.Builder().Build();
            this.interstitial.LoadAd(request);
#endif
        }
        public bool IsInterstitial()
        {
            if (this.interstitial.IsLoaded())
            {
                return true;
            }
            this.RequestInterstitial();
            return false;
        }

        public void RequestInterstitial()
        {
            if (this.interstitial != null)
            {
                if (!this.interstitial.IsLoaded())
                {
                    this.InitInterstitial();
                }
            }
            else
            {
                this.InitInterstitial();
            }
        }
        public void ShowInterstitial()
        {
            if (this.interstitial != null)
            {
                if (this.interstitial.IsLoaded())
                {
                    this.Message("Show Interstitial");
                    this.interstitial.Show();
                }
                else
                {
                    RequestInterstitial();
                }
            }
            else
            {
                this.InitInterstitial();
            }
        }
        private void ReloadInterstitial()
        {
            this.StartCoroutine(this.OnWaitingLoadInterstitail());
        }
        private IEnumerator OnWaitingLoadInterstitail()
        {
            yield return new WaitForEndOfFrame();
            yield return new WaitForSeconds(0.1f);
            this.RequestInterstitial();
        }
        #endregion

        #region VIDEO REWARD

        private RewardBasedVideoAd rewardVideo;
        private UnityAction<bool> videoCallback;
        public int CountVideo()
        {
            return this.countVideo;
        }
        private void InitVideoReward()
        {
            if (this.rewardVideo != null)
            {
                this.rewardVideo = null;
            }
            this.rewardVideo = RewardBasedVideoAd.Instance;

            if (this.rewardVideo != null)
            {
                this.Message("rewardVideo inited");
            }

            this.rewardVideo.OnAdLoaded += (delegate (System.Object sender, EventArgs args)
            {
                this.Message("Reward video Loaded");
            });

            this.rewardVideo.OnAdFailedToLoad += (delegate (System.Object sender, AdFailedToLoadEventArgs args)
            {
                this.Message("Reward video fail to load");

            });

            this.rewardVideo.OnAdClosed += (delegate (System.Object sender, EventArgs args)
            {
                this.Message("Reward video closed. Shame");
                this.RewardVideo(false);
            });
            this.rewardVideo.OnAdRewarded += (delegate (System.Object sender, Reward reward)
            {
                this.Message("Reward video get rewared");
                this.RewardVideo(true);
            });
            this.rewardVideo.OnAdLeavingApplication += (delegate (System.Object sender, EventArgs args)
            {
                this.Message("Reward video leaving");
            });
            this.RequestVideoReward();
        }

        public void RequestVideoReward()
        {
            if (this.rewardVideo != null)
            {
                if (!this.rewardVideo.IsLoaded())
                {
                    this.Message("Request video reward");
#if DEVICE_ID
                    string deviceId = AdMobUtility.GetTestDeviceId();
                    AdRequest request = new AdRequest.Builder().AddTestDevice(deviceId).Build();

                    this.rewardVideo.LoadAd(request, this.GetAdsId().rewardVideo);
#else
                    AdRequest request = new AdRequest.Builder().Build();

                    this.rewardVideo.LoadAd(request, this.GetAdsId().rewardVideo);
#endif


                }
            }
            else
            {
                this.InitVideoReward();
            }
        }
        public bool IsVideoReward()
        {
            if (this.rewardVideo.IsLoaded())
            {
                return true;
            }
            this.RequestVideoReward();
            return false;
        }


        public void ShowVideoReward(UnityAction<bool> callback = null)
        {
            if (this.rewardVideo != null)
            {
                if (this.rewardVideo.IsLoaded())
                {
                    this.videoCallback = callback;
                    this.Message("Show Video Reward");
                    this.rewardVideo.Show();
                    this.countVideo++;
                }
            }
            else
            {
                this.InitVideoReward();
            }
        }
        private void RewardVideo(bool isReward)
        {
            this.StartCoroutine(this.OnWaitingReward(isReward));
        }
        private IEnumerator OnWaitingReward(bool isReward)
        {
            yield return new WaitForEndOfFrame();
            yield return new WaitForSeconds(0.2f);
            if (this.videoCallback != null)
            {
                this.Message("Reward Video: " + isReward);
                this.videoCallback.Invoke(isReward);
                this.videoCallback = null;

            }
            yield return new WaitForEndOfFrame();
            this.RequestVideoReward();
        }
        private void ReloadVideoReward()
        {
            this.InitVideoReward();
        }
        #endregion

        public void Message(string message)
        {
            GameManager.Instance.LogMessage(string.Format("ADMOB ADS: {0}", message));

        }
        public void Clear()
        {

        }
    }
#endif
    
}
