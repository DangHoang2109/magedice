using System.Collections;
using System.Collections.Generic;
using UnityEngine;
#if UNITY_ADS
using UnityEngine.Advertisements;
#endif
using UnityEngine.Events;

namespace Cosina
{
    public class UnityAds : MonoBehaviour
#if UNITY_ADS
        , AdProvide, IUnityAdsListener
#endif
    {
#if UNITY_ADS
        private int countVideo;

        private string gameId = "3304760";
        private bool testMode = true;
        private void Start()
        {
            this.InitAds();
        }
        public void Clear()
        {

        }
        public void SetAppId(string id, bool testMode)
        {
            this.gameId = id;
            this.testMode = testMode;
        }

        public void InitAds()
        {
            try
            {

                if (!Advertisement.isSupported)
                {
                    Debug.LogError("Unity Ads is not supported on the current runtime platform.");
                }
                else if (Advertisement.isInitialized)
                {
                    Debug.LogError("Unity Ads is already initialized.");
                }
                else if (string.IsNullOrEmpty(this.gameId))
                {
                    Debug.LogError("The game ID value is not set. A valid game ID is required to initialize Unity Ads.");
                }
                else
                {
                    Advertisement.AddListener(this);
                    Debug.LogError("UNITY ADS ID: " + this.gameId);
                    Advertisement.Initialize(this.gameId, false);
                }
            }
            catch (System.Exception e)
            {
                Debug.LogException(e);
            }
        }

        public bool IsBanner()
        {
            return false;
        }
        public void RequestBanner()
        {

        }
        public void ShowBanner()
        {

        }




        public bool IsInterstitial()
        {
            return Advertisement.IsReady();
        }

        public void RequestInterstitial()
        {

        }

        public void ShowInterstitial()
        {
            string placement = "interstitial";
            if (Advertisement.IsReady(placement))
            {
                Advertisement.Show(placement);
                this.Message("Show Interstital");

            }
            else
            {
                Debug.Log("Interstitial ad not ready at the moment! Please try again later!");
                this.Message("Interstitial ad not ready at the moment! Please try again later!");
            }
        }

        private UnityAction<bool> rewardCallback;
        public int CountVideo()
        {
            return this.countVideo;
        }
        public bool IsVideoReward()
        {
            return Advertisement.IsReady("rewardedVideo");
        }
        public void RequestVideoReward()
        {

        }
        public void ShowVideoReward(UnityAction<bool> callback = null)
        {
            this.rewardCallback = callback;
            string placement = "rewardedVideo";
            if (Advertisement.IsReady(placement))
            {
                Advertisement.Show(placement);
                this.Message("Show Video Reward " + placement);

            }
            else
            {
                Debug.Log("Rewarded video is not ready at the moment! Please try again later!");
                this.Message("Rewarded video is not ready at the moment! Please try again later!");

            }
        }


        public void OnUnityAdsDidFinish(string placementId, ShowResult showResult)
        {
            string log = string.Format("Placement {0}, results {1}", placementId, showResult.ToString());
            this.Message(log);
            if (placementId.Equals("rewardedVideo"))
            {
                // Define conditional logic for each ad completion status:
                if (showResult == ShowResult.Finished)
                {
                    // Reward the user for watching the ad to completion.

                    this.RewardVideo(true);
                    return;
                }
                else if (showResult == ShowResult.Skipped)
                {
                    // Do not reward the user for skipping the ad.
                    this.RewardVideo(false);
                    return;
                }
                else if (showResult == ShowResult.Failed)
                {
                    Debug.LogWarning("The ad did not finish due to an error.");
                    this.RewardVideo(false);
                    return;
                }

                this.RewardVideo(false);
            }

        }

        private void RewardVideo(bool isReward)
        {
            this.StartCoroutine(this.OnWaitingRewardVideo(isReward));
        }
        private IEnumerator OnWaitingRewardVideo(bool isReward)
        {
            yield return new WaitForEndOfFrame();
            if (this.rewardCallback != null)
            {
                this.Message("Reward Video " + isReward);
                this.rewardCallback.Invoke(isReward);
                this.rewardCallback = null;
            }
        }
        public void OnUnityAdsReady(string placementId)
        {
        }

        public void OnUnityAdsDidError(string message)
        {
        }

        public void OnUnityAdsDidStart(string placementId)
        {
        }


        public void Message(string message)
        {
            GameManager.Instance.LogMessage(string.Format("UNITY ADS: {0}", message));

        }

#endif
    }

}
