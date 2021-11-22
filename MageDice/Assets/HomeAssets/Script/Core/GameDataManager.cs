using System.Collections;
using System.Collections.Generic;
using Cosinas.Firebase;
using UnityEngine;

public class GameDataManager : MonoSingleton<GameDataManager>
{
    private GameDatas gameDatas;
    private UserBoosters boosters;

    public GameDatas GameDatas
    {
        get
        {
            if(this.gameDatas == null)
            { }
            return this.gameDatas;
        }
    }
    public UserBoosters Boosters
    {
        get
        {
            if(this.boosters == null)
            {

            }
            return this.boosters;
        }
    }

    public YieldInstruction _waitFor5s;
    private Coroutine _saving = null;
    /// <summary>
    /// Bắt đầu load thông tin user
    /// </summary>
    /// <returns></returns>
    public IEnumerator OnLoadData()
    {
        yield return new WaitForEndOfFrame();
        this._waitFor5s = new WaitForSeconds(5f);
        ///Load user info
        this.LoadUserData();
        while (this.gameDatas == null)
        {

            yield break;
        }
        yield return new WaitForEndOfFrame();
        ///Load tiếp booster
        ///
        this.LoadBoosterData();
        while (this.boosters == null)
        {

            yield break;
        }
        yield return new WaitForEndOfFrame();
        this.OpenGame();
    }
    /// <summary>
    /// Load thông tin user
    /// </summary>
    private void LoadUserData()
    {
        try
        {
            if (PlayerPrefs.HasKey(GameDefine.USER_INFO_DATA))
            {
                string jsonData = PlayerPrefs.GetString(GameDefine.USER_INFO_DATA);
                if (!string.IsNullOrEmpty(jsonData))
                {
                    this.gameDatas = JsonUtility.FromJson<GameDatas>(jsonData);
                    this.gameDatas.ParseDataNotFirstTime();
                }
                else
                {
                    Debug.LogError("CAN NOT PARSE USER DATA: " + jsonData);
                    return;
                }
            }
            else
            {
                //Create New User;
                this.CreateUser();

            }
        }
        catch(System.Exception e)
        {
            Debug.LogException(e);
        }
    }
    /// <summary>
    /// Lưu thông tin user data
    /// </summary>
    public void SaveUserData()
    {
        string jsonData = JsonUtility.ToJson(this.gameDatas);
        PlayerPrefs.SetString(GameDefine.USER_INFO_DATA, jsonData);
        if (this._saving == null)
        {
            _saving = this.StartCoroutine(this.DelaySaveData());
        }
    }
    /// <summary>
    /// Load thông tin booster
    /// </summary>
    private void LoadBoosterData()
    {
        try
        {
            if (PlayerPrefs.HasKey(GameDefine.USER_BOOSTER_DATA))
            {
                string jsonData = PlayerPrefs.GetString(GameDefine.USER_BOOSTER_DATA);
                if (!string.IsNullOrEmpty(jsonData))
                {
                    this.boosters = JsonUtility.FromJson<UserBoosters>(jsonData);
                }
                else
                {
                    Debug.LogError("CAN NOT PARSE BOOSTER DATA: " + jsonData);
                    return;
                }
            }
            // else
            // {
            // // CreateUser() had initialized boosters   
            // }
        }
        catch(System.Exception e)
        {
            Debug.LogException(e);
        }
    }
    /// <summary>
    /// Lưu thông tin booster
    /// </summary>
    public void SaveBoosterData()
    {
        string jsonData = JsonUtility.ToJson(this.boosters);
        PlayerPrefs.SetString(GameDefine.USER_BOOSTER_DATA, jsonData);
        if (this._saving == null)
        {
            this.StartCoroutine(this.DelaySaveData());
        }
    }
    private IEnumerator DelaySaveData()
    {
        yield return _waitFor5s;
        PlayerPrefs.Save();
        this._saving = null;
    }

    private void OnApplicationFocus(bool focus)
    {
        if (!focus)
        {
            this.ForceSave();
        }
    }

    private void OnApplicationQuit()
    {
        PlayerPrefs.Save();
        this._saving = null;
    }

    private void ForceSave()
    {
        PlayerPrefs.Save();
        if (this._saving != null)
        {
            this.StopCoroutine(this._saving);
            this._saving = null;
           
        }
    }
    /// <summary>
    /// Tạo mới user
    /// </summary>
    private void CreateUser()
    {
        this.gameDatas = new GameDatas();
        this.gameDatas.ParseDataFirstTime();

        this.boosters = new UserBoosters();
        this.gameDatas.CreateUser();
        this.boosters.CreateUser();

        this.SaveUserData();
        this.SaveBoosterData();
    }

    public void OpenGame()
    {
        LanguageManager.Instance.OnLoadData();

        this.gameDatas.OpenGame();
    }

    private const string FIREBASE_DATABASE = "users";
    private void SaveDataToFirebase()
    {
        GameDatabases data = new GameDatabases(this.gameDatas, this.boosters);
        string jsonData = JsonUtility.ToJson(data);
        CFirebaseManager.Instance.SaveDatabase(FIREBASE_DATABASE, jsonData);
        
    }

    public void LoadDataFromFirebase()
    {
        CFirebaseManager.Instance.GetUserData(FIREBASE_DATABASE, (success, datas) =>
        {
            if (success)
            {
                string jsonData = datas.ToString();
                if (string.IsNullOrEmpty(jsonData))
                {
                    GameDatabases data = JsonUtility.FromJson<GameDatabases>(jsonData);
                }
            }
        });
    }
    
    #region Calendar
    public void AddCallendars()
    {
        new System.Globalization.GregorianCalendar();
        new System.Globalization.PersianCalendar();
        new System.Globalization.UmAlQuraCalendar();
        new System.Globalization.ThaiBuddhistCalendar();
    }
    
    #endregion Calendar
}

[System.Serializable]
public class GameDatabases
{
    public GameDatas gamedatas;
    public UserBoosters boosters;

    public GameDatabases()
    {
        this.gamedatas = new GameDatas();
        this.boosters = new UserBoosters();
    }

    public GameDatabases(GameDatas gamedatas, UserBoosters boosters)
    {
        this.gamedatas = gamedatas;
        this.boosters = boosters;
    }
}
