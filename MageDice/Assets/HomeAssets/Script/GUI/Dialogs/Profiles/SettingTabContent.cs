using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Events;

public class SettingTabContent : TabContent
{
    public CustomToogle toggleMusic;
    public CustomToogle toggleSound;
    public CustomToogle toggleFacebook;
    public TextMeshProUGUI txtMusic;
    public TextMeshProUGUI txtSound;
    public TextMeshProUGUI txtFacebook;
    public override void OnShow(int index, object data = null, UnityAction callback = null)
    {
        base.OnShow(index, data, callback);
        this.toggleMusic.IsOn = SoundManager.Instance.isMusic;
        this.toggleSound.IsOn = SoundManager.Instance.isSound;
        this.toggleFacebook.IsOn = UserDatas.Instance.info.facebook.isLogin;
        this.txtMusic.text = string.Format("{0} {1}", LanguageManager.GetString("SETTING_MUSIC", LanguageCategory.Feature), LanguageManager.GetString(this.toggleMusic.IsOn ? "DES_ON" : "DES_OFF"))  ; //"Music {0}", this.toggleMusic.IsOn ? "On" : "Off"
        this.txtSound.text = string.Format("{0} {1}", LanguageManager.GetString("SETTING_SOUND", LanguageCategory.Feature), LanguageManager.GetString(this.toggleSound.IsOn ? "DES_ON" : "DES_OFF")); // string.Format("Sound {0}", this.toggleSound.IsOn ? "On" : "Off");
        this.txtFacebook.text = string.Format("{0} {1}", LanguageManager.GetString("SETTING_FACEBOOK", LanguageCategory.Feature), LanguageManager.GetString(!this.toggleFacebook.IsOn ? "DES_LOGIN" : "DES_LOGOUT"));  //string.Format("Facebook : {0}", this.toggleFacebook.IsOn ? "Logout" : "Login");
    }

    protected override void AnimationHide()
    {
        base.AnimationHide();
        this.OnCompleteHide();
    }

    public void ClickMusic(bool isOn)
    {
        Debug.LogError("MUSIC: " + isOn);
        SoundManager.Instance.OnChangeMusic(isOn);
        this.txtMusic.text = string.Format("{0} {1}", LanguageManager.GetString("SETTING_MUSIC", LanguageCategory.Feature), LanguageManager.GetString(this.toggleMusic.IsOn ? "DES_ON" : "DES_OFF")); //"Music {0}", this.toggleMusic.IsOn ? "On" : "Off"

    }

    public void ClickSound(bool isOn)
    {
        Debug.LogError("SOUND: " + isOn);
        SoundManager.Instance.OnChangeSounds(isOn);
        this.txtSound.text = string.Format("{0} {1}", LanguageManager.GetString("SETTING_SOUND", LanguageCategory.Feature), LanguageManager.GetString(this.toggleSound.IsOn ? "DES_ON" : "DES_OFF")); // string.Format("Sound {0}", this.toggleSound.IsOn ? "On" : "Off");

    }

    public void ClickLoginFacebook()
    {
        if (!UserDatas.Instance.info.facebook.isLogin)
        {
            LoadingManager.Instance.ShowLoading(true);
            FacebookManager.Instance.Login((success) =>
            {
                LoadingManager.Instance.ShowLoading(false);
                this.toggleFacebook.IsOn = FacebookManager.Instance.IsLoginFacebook();
                this.txtFacebook.text = string.Format("{0} {1}", LanguageManager.GetString("SETTING_FACEBOOK", LanguageCategory.Feature), LanguageManager.GetString(!this.toggleFacebook.IsOn ? "DES_LOGIN" : "DES_LOGOUT"));  //string.Format("Facebook : {0}", this.toggleFacebook.IsOn ? "Logout" : "Login");
            });
        }
        else
        {
            FacebookManager.Instance.Logout();
            this.toggleFacebook.IsOn = false;
            this.txtFacebook.text = string.Format("{0} {1}", LanguageManager.GetString("SETTING_FACEBOOK", LanguageCategory.Feature), LanguageManager.GetString(!this.toggleFacebook.IsOn ? "DES_LOGIN" : "DES_LOGOUT"));  //string.Format("Facebook : {0}", this.toggleFacebook.IsOn ? "Logout" : "Login");

        }
    }

    public void ClickLoginFacebook(bool isOn)
    {
        
    }

    public void ClickLikeFanpage()
    {
        Application.OpenURL(SocialDefine.FAN_PAGE);
    }

    public void ClickLanguage()
    {
        GameManager.Instance.OnShowDialogWithSorting<LanguageDialogs>("Home/GUI/Dialogs/Profiles/LanguageDialogs",
            PopupSortingType.OnTopBar);
    }

    public void ClickTermsOfService()
    {
        Application.OpenURL(SocialDefine.TERNS_OF_SERVICE);

    }

    public void ClickPolicy()
    {
        Application.OpenURL(SocialDefine.POLICY);

    }

    public void ClickSupport()
    {
        Application.OpenURL(SocialDefine.SUPPORT);

    }
}
