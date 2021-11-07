using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cosinas.Social;
using UnityEngine.Events;

public class FacebookManager : MonoSingleton<FacebookManager>
{
    public void RegisterCallback()
    {
        CSocialManager.OnLoginFacebookSuccessed += this.OnLoginFacebookSuccess;
        CSocialManager.OnLoginFacebookFailed += this.OnLoginFacebookFail;
        CSocialManager.OnGetInfoFacebookUserCompleted += this.OnGetUserInfoSuccess;
        CSocialManager.OnGetPictureFacebook += this.OnGetAvatarSuccess;
    }

    public void UnRegisterCallback()
    {
        CSocialManager.OnLoginFacebookSuccessed -=  this.OnLoginFacebookSuccess;
        CSocialManager.OnLoginFacebookFailed -= this.OnLoginFacebookFail;
        CSocialManager.OnGetInfoFacebookUserCompleted -=  this.OnGetUserInfoSuccess;
        CSocialManager.OnGetPictureFacebook -=  this.OnGetAvatarSuccess;
    }

    public bool IsLoginFacebook()
    {
        return CSocialManager.Instance.IsLoginFacebook();
    }

    private UnityAction<bool> callbackLogin;
    public void Login(UnityAction<bool> callbackLogin)
    {
        this.callbackLogin = callbackLogin;
        #if UNITY_EDITOR
         UserDatas.Instance.info.facebook.LoginFacebook("user_id_1111", "user_nickname_111");
        this.OnLoginFacebookSuccess();
        return;
        #endif
        
        CSocialManager.Instance.LoginFacebook();
    }

    public void Logout()
    {
        CSocialManager.Instance.LogoutFB();
        UserDatas.Instance.info.facebook.LogoutFacebook();
    }

    public void OnLoginFacebookSuccess()
    {
        this.callbackLogin?.Invoke(true);
    }
    public void OnLoginFacebookFail()
    {
        this.callbackLogin?.Invoke(true);
    }

    public void OnGetUserInfoSuccess()
    {
        string id = string.Empty;
        string nickname = String.Empty;
        Dictionary<string, object> facebookUserDetails = this.GetFacebookUserDatails();
        if (facebookUserDetails != null)
        {
            if (facebookUserDetails.Count > 0)
            {
                if (facebookUserDetails.ContainsKey("id"))
                {
                    id = facebookUserDetails["id"].ToString();
                }

                if (facebookUserDetails.ContainsKey("name"))
                {
                    nickname = facebookUserDetails["name"].ToString();
                }

                if (!string.IsNullOrEmpty(id) && !string.IsNullOrEmpty(nickname))
                {
                    UserDatas.Instance.info.facebook.LoginFacebook(id, nickname);
                    UserDatas.Instance.info.ChangeName(nickname);
                }
            }
        }
    }

    private Dictionary<string, object> GetFacebookUserDatails()
    {
        return CSocialManager.Instance._dicFacebookUserDetails;
    }
    public void OnGetAvatarSuccess()
    {
        string avatar = CSocialManager.Instance.ConvertAvatarToBase64();
        if (!string.IsNullOrEmpty(avatar))
        {
            UserDatas.Instance.info.facebook.SetAvatar(avatar);
        }
    }

    public Sprite GetAvatarFacebook()
    {
        return CSocialManager.Instance.GetAvatarFacebook();
    }
}
