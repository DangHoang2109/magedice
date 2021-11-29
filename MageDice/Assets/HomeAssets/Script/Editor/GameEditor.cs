using System;
using System.Threading;
using System.Threading.Tasks;
using Cosina.Components;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.UI;
using Object = UnityEngine.Object;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
public static class GameEditor
{

    #region Data

    [MenuItem("Tools/Clear data")]
    private static void ClearData()
    {
        Debug.Log("Create data");
        PlayerPrefs.DeleteAll();
    }

    #endregion


    #region Cheat Booster

    [MenuItem("Cheats/Boosters/Add 50 cup")]
    private static void CheatAdd50Cup()
    {
        UserProfile.Instance.AddBooster(new BoosterCommodity(BoosterType.CUP, 50), "cheat","cheat 50 cups");
    }

    [MenuItem("Cheats/Boosters/Add full cup")]
    private static void CheatAddFullCup()
    {
        UserProfile.Instance.AddBooster(new BoosterCommodity(BoosterType.CUP, 5000), "cheat", "cheat 5000 cups");
    }

    [MenuItem("Cheats/Boosters/Add 1000 coin")]
    private static void CheatAdd1000Coin()
    {
        UserProfile.Instance.AddBooster(new BoosterCommodity(BoosterType.COIN, 1000), "cheat","cheat 1000 coins");
    }
    [MenuItem("Cheats/Boosters/Add 1G coins")]
    private static void CheatAdd1GCoin()
    {
        UserProfile.Instance.AddBooster(new BoosterCommodity(BoosterType.COIN, 1000000000), "cheat","cheat 1000 coins");
    }

    [MenuItem("Cheats/Boosters/Add 100 cash")]
    private static void CheatAdd100Cash()
    {
        UserProfile.Instance.AddBooster(new BoosterCommodity(BoosterType.CASH, 100), "cheat","cheat 100 cashs");
    }
    [MenuItem("Cheats/Boosters/Add 1G cash")]
    private static void CheatAdd1GCash()
    {
        UserProfile.Instance.AddBooster(new BoosterCommodity(BoosterType.CASH, 1000000000), "cheat", "cheat 1g cash");
    }

    [MenuItem("Cheats/Boosters/Sub 100 coin")]
    private static void CheatSub100Coin()
    {
        UserProfile.Instance.UseBooster(new BoosterCommodity(BoosterType.COIN, 100), "cheat","cheat 100 cashs");
    }

    
    [MenuItem("Cheats/Boosters/Sub 100 cash")]
    private static void CheatSub100Cash()
    {
        UserProfile.Instance.UseBooster(new BoosterCommodity(BoosterType.CASH, 100), "cheat","cheat 100 cashs");
    }
    
    [MenuItem("Cheats/Boosters/Sub All cash")]
    private static void CheatSubAllCash()
    {
        UserProfile.Instance.UseBooster(UserBoosters.Instance.GetBoosterCommodity(BoosterType.CASH), "cheat","cheat 100 cashs");
    }
    
    #endregion

    #region Cheat bags

    //[MenuItem("Cheats/Bags/Add 1 silver bag")]
    //private static void Add1SilverBag()
    //{
    //    if (!BagSlotDatas.Instance.CollectBag(BagType.SILVER_BAG, "Tour 1","cheat", "Cheat", 1))
    //        Debug.LogError("Full bags");
    //}

    //[MenuItem("Cheats/Bags/Add 1 king bag")]
    //private static void Add1KingBag()
    //{
    //    if (!BagSlotDatas.Instance.CollectBag(BagType.KING_BAG, "Cheat","cheat", "Cheat", 1))
    //        Debug.LogError("Full bags");
    //}

    #endregion

    #region Scenes

    [MenuItem("Scenes/Loading #1")]
    static void LoadSplashScene()
    {
        UnityEditor.SceneManagement.EditorSceneManager.SaveCurrentModifiedScenesIfUserWantsTo();
        UnityEditor.SceneManagement.EditorSceneManager.OpenScene("Assets/HomeAssets/Scenes/LoadingScene.unity");
    }

    [MenuItem("Scenes/Home #2")]
    static void LoadHomeScene()
    {
        UnityEditor.SceneManagement.EditorSceneManager.SaveCurrentModifiedScenesIfUserWantsTo();
        UnityEditor.SceneManagement.EditorSceneManager.OpenScene("Assets/HomeAssets/Scenes/HomeScene.unity");
    }

    [MenuItem("Scenes/Game #3")]
    static void LoadGameScene()
    {
        UnityEditor.SceneManagement.EditorSceneManager.SaveCurrentModifiedScenesIfUserWantsTo();
        UnityEditor.SceneManagement.EditorSceneManager.OpenScene("Assets/Pools/Scenes/PoolGame.unity");
    }

    const string settingKey = "StartWithSpecificScene";
    const string firstScenePath1 = "Assets/HomeAssets/Scenes/LoadingScene.unity";
    static int sceneBeforeStartIndex = 0;

    [MenuItem("Scenes/Play Game %#r")]
    static void PlayGame()
    {
        if (EditorApplication.isPlaying)
        {
            EditorApplication.isPlaying = false;
            return;
        }

        var activeScene = UnityEngine.SceneManagement.SceneManager.GetActiveScene();
        sceneBeforeStartIndex = activeScene.buildIndex;

        EditorSceneManager.SaveCurrentModifiedScenesIfUserWantsTo();
        EditorSceneManager.OpenScene(firstScenePath1);
        SaveScenePath();
        EditorApplication.isPlaying = true;
    }

    static void SaveScenePath()
    {
        EditorPrefs.SetInt(settingKey, sceneBeforeStartIndex);
        Debug.Log(sceneBeforeStartIndex);
    }

    #endregion

    [MenuItem("Test/Set Time 10%")]
    static void SetTimeScaleOneTenth()
    {
        Time.timeScale = 0.1f;
    }
    [MenuItem("Test/Set Time 100%")]
    static void SetTimeScaleNormal()
    {
        Time.timeScale = 1f;
    }
    [MenuItem("Info/Shadow map resolution")]
    static void LogShadowMapResolution()
    {
        foreach (var light in Object.FindObjectsOfType<Light>())
        {
            Debug.Log(light.shadowCustomResolution);
        }
        
    }
    [MenuItem("Tools/Capture")]
    static void Capture()
    {
        Texture2D tex = CosinaExtension.MakeBlur(CosinaExtension.CaptureMainCamera(500, 4),
            4);
        // Texture2D tex = CosinaExtension.MakeBlur(CosinaExtension.CaptureMainCamera(250, 500),
        //     0);
        //Texture2D tex = CosinaExtension.CaptureMainCamera(250, 500);
        


        Canvas cv = GameObject.FindObjectOfType<Canvas>();
        GameObject o = cv.transform.Find("o")?.gameObject;
        if (o == null)
        {
            o = new GameObject("o", typeof(RawImage));
            o.transform.SetParent(cv.transform);
            RectTransform r = o.transform as RectTransform;
            r.anchorMin = Vector2.zero;
            r.anchorMax = Vector2.one;
            r.sizeDelta = Vector2.zero;

            r.localScale = Vector3.one;
            r.anchoredPosition = Vector2.zero;
        }

        o.GetComponent<RawImage>().texture = tex;

    }

    

}
