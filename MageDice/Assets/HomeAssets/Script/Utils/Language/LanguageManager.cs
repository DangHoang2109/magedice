using System.Collections;
using System.Collections.Generic;
using I2.Loc;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public enum LanguageDefine
{
    English,
    Vietnamese,
    Indonesian,
    Persian,
    Tagal,
    Arabic,
    Portuguese,
    Gujarati,
    Urdu,
    Japanese,
    Korean,
    Chinese,
    Spanish,
    Taiwan,
    Thai,
    German,
}
public enum LanguageCategory
{
    Common,
    Feature,
    Tips,
    StatUpgrade,
    Games,
    Tutorial,
    Character,
    MissionPass
}
public class LanguageManager : MonoSingleton<LanguageManager>
{
    public LanguageDefine Language = LanguageDefine.English;

    //#if LANGUGE
    //public Language Language = Language.English;
 
    public event System.Action OnLanguageReloaded;

    public override void Init()
    {
        base.Init();
    }
    public void OnLoadData(UnityAction callback = null)
    {

        //gen lại user language mà không cần clear data
#if UNITY_EDITOR
        if (UserLanguageData.Instance == null)
        {
            UserDatas.Instance.languages = new UserLanguageData();
            UserDatas.Instance.languages.NewUser();
        }
#endif


        this.ChangeLanguage(UserLanguageData.Instance.Language);
        if (callback != null)
        {
            callback.Invoke();
        }
    }
    public void ChangeLanguage(LanguageDefine language)
    {
        this.Language = language;
        LocalizationManager.CurrentLanguage = this.Language.ToString();

        this.OnLanguageReloaded?.Invoke();
        this.SaveLanguage(Language);
    }
    public void SetCallback_UpdateLanguage(System.Action ac)
    {
        this.OnLanguageReloaded += ac;
    }
    public void RemoveCallback_UpdateLanguage(System.Action ac)
    {
        this.OnLanguageReloaded -= ac;
    }
    private void SaveLanguage(LanguageDefine language)
    {
        UserLanguageData.Instance.ChangeLanguage(language);
    }

    public static string GetString(string key, LanguageCategory category = LanguageCategory.Common)
    {
        string termText = LocalizationManager.GetTranslation(string.Format("{0}/{1}", category.ToString(), key));
        if (string.IsNullOrEmpty(termText))
        {
            return key;
        }
        return termText;
    }
    public bool IsLanguage(string language)
    {
        return LocalizationManager.CurrentLanguage.Equals(language);
    }
    public List<string> AllLanguage()
    {
        return LocalizationManager.GetAllLanguages();
    }

//    /// <summary>
//    /// Get ra tên đã dịch của một item
//    /// </summary>
//    /// <param name="mapID">map của item này. NÊN TRỪ MAPID ĐI MỘT VÌ VẤN ĐỀ CONFIG</param>
//    /// <param name="foodID">id của item này</param>
//    /// <param name="itemType">loại item DEVICE hay FOOD</param>
//    /// <returns></returns>
//    public string GetItemTranslated_Name(int mapID, int foodID, FoodItemType itemType)
//    {
//        string queryFormat = string.Format("{0}_{1}_MAP_{2}", itemType, foodID, mapID);
//        string result = this.GetString(queryFormat, LanguageCategory.Food);

//        if (string.IsNullOrEmpty(result)) return string.Format("Item {0} {1} of map {2}", itemType, foodID, mapID);
//        else return result;
//    }

//    /// <summary>
//    /// Get ra description của một item
//    /// Description thường chỉ show trong upgrade dialog, nên các level index 0 (level 1) không có description
//    /// </summary>
//    /// <param name="mapID">map của item này</param>
//    /// <param name="foodID">id của item này</param>
//    /// <param name="level">level cần get description</param>
//    /// <param name="itemType">loại item DEVICE hay FOOD</param>
//    /// <returns></returns>
//    public string GetItemTranslated_Description(int mapID, int foodID, int level, FoodItemType itemType)
//    {
//        if(level == 0) return string.Format("Description of Item {0} {1} of map {2}", itemType, foodID, mapID);

//        string queryFormat = string.Format("{0}_{1}_UPGRADE_{2}_MAP_{3}_DESCR", itemType, foodID, level, mapID);
//        string result = this.GetString(queryFormat, LanguageCategory.Food);

//        if (string.IsNullOrEmpty(result)) return string.Format("Description of Item {0} {1} of map {2}", itemType, foodID, mapID);
//        else return result;
//    }

//#endif
}

