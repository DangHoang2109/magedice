using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEngine;

public static class RemoteConfigsManager
{
    private static bool isLoadRemoteConfig = false;

    public static bool IsLoadRemoteConfig
    {
        get
        {
            return isLoadRemoteConfig;
        }
        set
        {
            isLoadRemoteConfig = value;
        }
    }

    #region UserType

    /// <summary>
    /// Label user from their audice in firebase
    /// currently we get:
    /// 0: user never paid IAP and rarely paid gem
    /// 1: user paid gem only or rarely use IAP
    /// 2: user paid a lot, both gem and IAP
    /// 
    /// User type will be 0 in the first session. As we not have enough data of
    /// this user to segment him
    /// </summary>
    ///

    private static int userType;

    private const string KEY_USER_TYPE = "Key_User_Type";
    public static int UserType
    {
        get
        {
            if (isLoadRemoteConfig)
            {
                ///Nếu load được config của firebase
                return userType;
            }
            else
            {
                if (PlayerPrefs.HasKey(KEY_USER_TYPE))
                {
                    userType = PlayerPrefs.GetInt(KEY_USER_TYPE);
                    return userType;
                }
            }

            return 0;
        }
        set
        {
            userType = value;
            PlayerPrefs.SetInt(KEY_USER_TYPE, userType);
            PlayerPrefs.Save();
        }
    }

    #endregion

    #region Count Ads

    

    private static int countAds;
    private const string KEY_COUNT_ADS = "Key_Count_Ads";
    public static int CountAds
    {
        get
        {
            if (isLoadRemoteConfig)
            {
                ///Nếu load được config của firebase
                return countAds;
            }
            else
            {
                if (PlayerPrefs.HasKey(KEY_COUNT_ADS))
                {
                    countAds = PlayerPrefs.GetInt(KEY_COUNT_ADS);
                    return countAds;
                }
            }

            return 5;
        }
        set
        {
            countAds = value;
            PlayerPrefs.SetInt(KEY_COUNT_ADS, countAds);
            PlayerPrefs.Save();
        }
    }
    
    #endregion

    #region Open Bag Now
    /// <summary>
    /// Remote open bag now
    /// True = 1: Open Now
    /// False = 0: Waiting time
    /// </summary>
    private static bool isOpenBag;

    private const string KEY_OPEN_BAG = "Key_Open_Bag";

    public static bool IsOpenBag
    {
        get
        {
            if (isLoadRemoteConfig)
            {
                return isOpenBag;
            }
            else
            {
                if (PlayerPrefs.HasKey(KEY_OPEN_BAG))
                {
                    isOpenBag = PlayerPrefs.GetInt(KEY_OPEN_BAG) != 0;
                    return isOpenBag;
                }
            }

            return false;
        }
        set
        {
            isOpenBag = value;
            if (isOpenBag)
            {
                PlayerPrefs.SetInt(KEY_OPEN_BAG, 1);
            }
            else
            {
                PlayerPrefs.SetInt(KEY_OPEN_BAG, 0);
            }
            PlayerPrefs.Save();
        }
    }

    #endregion

    #region Tut Coach

    private static bool isSkipCoach;

    public static bool IsSkipCoach
    {
        get => isSkipCoach;
        set => isSkipCoach = value;
    }

    #endregion

}

