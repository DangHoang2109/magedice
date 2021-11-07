using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;


namespace Cosina
{
    [System.Serializable]
    public class AdsId
    {
        public string appid = "YOUR_APP_ID";
        public string banner = "YOUR_PLACEMENT_ID";
        public string instertitial = "YOUR_PLACEMENT_ID";
        public string rewardVideo = "YOUR_PLACEMENT_ID";

        public AdsId()
        { }
        public AdsId(string appid, string bannerid, string intersitialid, string rewardid)
        {
            this.appid = appid;
            this.banner = bannerid;
            this.instertitial = intersitialid;
            this.rewardVideo = rewardid;
        }
    }

    public interface AdProvide
    {
        void InitAds();
        bool IsBanner();
        void ShowBanner();
        void RequestBanner();

        void RequestInterstitial();
        bool IsInterstitial();
        void ShowInterstitial();

        void RequestVideoReward();
        bool IsVideoReward();
        void ShowVideoReward(UnityAction<bool> callback = null);
        int CountVideo();

        void Message(string message);
        void Clear();
        
        //log
    }

}
