using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class RateGameDialog : BaseSortingDialog
{
    public static bool isShowRate; //show rate ở home hay không

    public override void OnShow(object data = null, UnityAction callback = null)
    {
        base.OnShow(data, callback);
        isShowRate = false;
    }

    public void OnClick4Stars()
    {
        this.OnCloseDialog();
        SoundManager.Instance.PlayButtonClick();
    }


    public void OnClick5Stars()
    {
#if UNITY_ANDROID
        Application.OpenURL(string.Format("https://play.google.com/store/apps/details?id={0}", Application.identifier));
#else
        //TODO IOS
#endif
        RateGameDialog.UserRate();
        this.OnCloseDialog();
        SoundManager.Instance.PlayButtonClick();
    }

    public static void ShowRateInHome(bool showRate)
    {
        isShowRate = showRate;
    }

    public static bool IsUserRated()
    {
        return PlayerPrefs.GetInt(GameDefine.USER_RATED, 0) == 1;
    }

    public static void UserRate()
    {
        PlayerPrefs.SetInt(GameDefine.USER_RATED, 1);
    }

#if UNITY_EDITOR
    [UnityEditor.MenuItem("Test/Dialogs/Rate game")]
    private static void TestShow()
    {
        GameManager.Instance.OnShowDialogWithSorting<RateGameDialog>("Home/GUI/Dialogs/RateGame/RateGameDialog", PopupSortingType.OnTopBar);
    }
#endif
}
