using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cosina;

[CreateAssetMenu(menuName = "AdConfigs")]
public class AdConfigs : ScriptableObject
{
    [Header("ID ADMOB ANDROID")]
    public AdsId ANDROID;
    [Header("ID ADMOB IOS")]
    public AdsId IOS;
    [Header("ID ADMOB TEST")]
    public AdsId TEST;
    [Header("UNITY ANDROID APP ID")]
    public string ANDROID_APP_ID;
    [Header("UNITY IOS APP ID")]
    public string IOS_APP_ID;
    [Header("TEST MODE")]
    public bool isTest;

    

    private static AdConfigs _instance;
   
    public static AdConfigs Instance
    {
        get
        {
            if(_instance == null)
            {
                _instance = Resources.Load<AdConfigs>("Games/Configs/AdConfigs");
            }
            return _instance;
        }
    }
    public AdsId GetAdmodId()
    {
        if(this.isTest)
        {
            return this.TEST;
        }
#if UNITY_ANDROID
        return this.ANDROID;
#elif UNITY_IOS
         return this.IOS;
#else
        return this.TEST;
#endif
        return this.TEST;
    }
    public string GetUnityAdsID()
    {
#if UNITY_ANDROID
        return this.ANDROID_APP_ID;
#elif UNITY_IOS
         return this.IOS_APP_ID;
#else
        return this.ANDROID_APP_ID;
#endif
        return this.ANDROID_APP_ID;
    }
}

