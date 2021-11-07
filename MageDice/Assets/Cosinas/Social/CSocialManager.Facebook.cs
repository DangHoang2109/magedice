using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
#if FACEBOOK
using Facebook.Unity;
#endif

namespace Cosinas.Social
{
   
    public partial class CSocialManager
    {
        public static event Action OnLoginFacebookSuccessed = delegate { };
        public static event Action OnLoginFacebookFailed = delegate { };
        public static event Action OnLogoutFacebookCompleted = delegate { };

        public static event Action OnShareFacebookComplete = delegate { };
        public static event Action OnShareFacebookError = delegate { };

        public static event Action OnInitFacebookCompleted = delegate { };

        public static event Action OnGetInfoFacebookUserCompleted = delegate { };

        public static event Action OnGetPictureFacebook = delegate { };
#if FACEBOOK
        public static event Action<IAppRequestResult> OnAppRequest = delegate { };
        public static event Action<IGraphResult> OnGetPlayerInfo = delegate { };
        public static event Action<IGraphResult> OnGetFriends = delegate { };
        public static event Action<IGraphResult> OnGetInvitableFriends = delegate { };
        public static event Action<IGraphResult> OnLoadImageWithFbId = delegate { };
        public static event Action<IGraphResult> OnGetScoresCallback = delegate { };
#endif
        public Dictionary<string, object> _dicFacebookUserDetails = new Dictionary<string, object>();
        private Texture2D _fbAvatarTexture = null;
        private string strQueryFields;
        public string facebookUserId = "";
        
        public bool EnableFacebook()
        {
#if FACEBOOK
            return true;
#else
            return false;
#endif
        }

        public bool IsInitFacebook()
        {
#if FACEBOOK
            return FB.IsInitialized;
#else
            return false;
#endif
        }
        public bool IsLoginFacebook()
        {
#if FACEBOOK
            return FB.IsLoggedIn;
#else
            return false;
#endif
        }

        public Sprite GetAvatarFacebook()
        {
            if (this._fbAvatarTexture != null)
            {
                return  Sprite.Create(_fbAvatarTexture, new Rect(0, 0, _fbAvatarTexture.width, _fbAvatarTexture.height), new Vector2());

            }

            return null;
        }

        public string ConvertAvatarToBase64()
        {
            if (this._fbAvatarTexture != null)
            {
                byte[] bytes = this._fbAvatarTexture.EncodeToPNG();
                return System.Convert.ToBase64String(bytes);
            }
            return string.Empty;
        }
        
#if FACEBOOK
        public AccessToken GetFacebookAccessToken()
        {
            return AccessToken.CurrentAccessToken;
        }
#endif
        public bool HasFacebookAccessTokenString()
        {
#if FACEBOOK
            return FB.IsInitialized && FB.IsLoggedIn && AccessToken.CurrentAccessToken != null
                && !string.IsNullOrEmpty(AccessToken.CurrentAccessToken.TokenString);
#else
            return false;
#endif
        }

        public string GetFacebookAccessTokenString()
        {
#if FACEBOOK
            if (HasFacebookAccessTokenString()) return AccessToken.CurrentAccessToken.TokenString;
#endif
            return string.Empty;
        }


        public void InitFacebook()
        {
#if FACEBOOK
           this.strQueryFields =  "id,email, name,first_name,last_name,picture.width(120).height(120)";
            
#if UNITY_ANDROID || UNITY_IOS
            if (!FB.IsInitialized)
            {
                // Initialize the Facebook SDK
                FB.Init(HandleInitFacebookCompleted, HandleHideUnity);
            }
            else
            {
                // Already initialized, signal an app activation App Event
                FB.ActivateApp();
            }
#endif
#endif
        }
        
#if FACEBOOK
        private void HandleInitFacebookCompleted()
        {
            if (FB.IsInitialized)
            {
                // Signal an app activation App Event
                FB.ActivateApp();
                // Continue with Facebook SDK
                // ...
                OnInitFacebookCompleted?.Invoke();
                Debug.LogError("INIT FACEBOOK SUCCESS");
                
            }
            else
            {
                Debug.LogError("Failed to Initialize the Facebook SDK");
            }
        }
#endif
        private void HandleHideUnity(bool isGameShown)
        {
            if (!isGameShown)
            {
                // Pause the game - we will need to hide
                Time.timeScale = 0;
            }
            else
            {
                // Resume the game - we're getting focus again
                Time.timeScale = 1;
            }
        }
        public void LogoutFB()
        {
#if FACEBOOK
            FB.LogOut();
#endif
        }
        
        /// <summary>
        /// Logins the Facebook.
        /// </summary>
        /// <param name="isGetInfo">If set to <c>true</c> is get info.</param>
        public void LoginFacebook()
        {
            if (!this.IsInitFacebook())
            {
                return;
            }
            List<string> facebookPermisions =  new List<string>() { "public_profile", "email", "user_friends" };
            LoginFacebook(facebookPermisions);
        }

        /// <summary>
        /// Logins the Facebook.
        /// </summary>
        /// <param name="permissions">Permissions.</param>
        /// <param name="isGetInfo">If set to <c>true</c> is get info.</param>
        public void LoginFacebook(List<string> permissions)
        {
            if (!this.IsInitFacebook())
            {
                return;
            }
#if FACEBOOK
	        FB.LogInWithReadPermissions(permissions, this.HandleLoginFacebookCallback);
#endif
        }
        
        
#if FACEBOOK
        /// <summary>
        /// Handles the result.
        /// </summary>
        /// <param name="result">Result.</param>
        protected void HandleLoginFacebookCallback(IResult result)
        {
           
            if (result == null)
            {
                if (OnLoginFacebookFailed != null)
                {
                    OnLoginFacebookFailed();
                    Debug.LogError("LOGIN FB FAIL");
                }
                return;
            }

            // Some platforms return the empty string instead of null.
			if (!string.IsNullOrEmpty(result.Error) || result.Cancelled || string.IsNullOrEmpty(result.RawResult))
            {
				if (OnLoginFacebookFailed != null)
                {
                    OnLoginFacebookFailed();
                    Debug.LogError("LOGIN FB FAIL");
                }
            }
            else if (!string.IsNullOrEmpty(result.RawResult))
            {
                if (IsLoginFacebook())
                {
                    AccessToken aToken = AccessToken.CurrentAccessToken;
                    if(aToken != null) facebookUserId = aToken.UserId;
                }
                OnLoginFacebookSuccessed?.Invoke();
                GetInfoFacebookUser ();
                Debug.LogError("LOGIN FB SUCCESS");
            }
            else
            {
                OnLoginFacebookFailed();
                Debug.LogError("LOGIN FB FAIL");
            }
        }

        private void GetInfoFacebookUser_CallBack(IGraphResult result)
        {
            _dicFacebookUserDetails = (Dictionary<string, object>)result.ResultDictionary;
            OnGetInfoFacebookUserCompleted?.Invoke();
            GetPlayerPicture();
        }
#endif

        public void GetInfoFacebookUser()
        {
#if FACEBOOK
            FB.API("/me?fields=" + strQueryFields, HttpMethod.GET, GetInfoFacebookUser_CallBack, new Dictionary<string, string>() { });
#endif
        }
        
        
#region App Request
        public void AppRequest(List<string> listSend)
        {
#if FACEBOOK
            List<string> recipient = new List<string>();
            string title, message, data = string.Empty;
            title = "Game!";
            message = "Best game is here. Check it out!";
            recipient = new List<string>();
            recipient = listSend;
            FB.AppRequest(
                message,
                recipient,
                null,
                null,
                null,
                data,
                title,
                HandleAppRequestCallback
            );
#endif
        }

#if FACEBOOK
        private static void HandleAppRequestCallback(IAppRequestResult result)
        {
            if(OnAppRequest != null)
            {
                OnAppRequest(result);
            }

        }
#endif
#endregion


#region PlayerInfo
        // Once a player successfully logs in, we can welcome them by showing their name
        // and profile picture on the home screen of the game. This information is returned
        // via the /me/ endpoint for the current player. We'll call this endpoint via the
        // SDK and use the results to personalize the home screen.
        //
        // Make a Graph API GET call to /me/ to retrieve a player's information
        // See: https://developers.facebook.com/docs/graph-api/reference/user/
        public void GetPlayerInfo()
        {
#if FACEBOOK
            string queryString = "/me?fields=id,first_name,picture.width(120).height(120)";
            FB.API(queryString, HttpMethod.GET, HandleGetPlayerInfoCallback);
#endif
        }
        // In the above request it takes two network calls to fetch the player's profile picture.
        // If we ONLY needed the player's profile picture, we can accomplish this in one call with the /me/picture endpoint.
        //
        // Make a Graph API GET call to /me/picture to retrieve a players profile picture in one call
        // See: https://developers.facebook.com/docs/graph-api/reference/user/picture/
        public void GetPlayerPicture()
        {
#if FACEBOOK
            FB.API(GetPictureQuery("me", 200, 200), HttpMethod.GET, HandleGetPlayerPicture);
#endif
        }

#if FACEBOOK

        private void HandleGetPlayerInfoCallback(IGraphResult result)
        {
            if(OnGetPlayerInfo != null)
            {
                OnGetPlayerInfo(result);
                Debug.LogError("GET PLAYER INFO SUCCESS: " + result.ResultList.Count);
            }
            Debug.LogError("GET PLAYER INFO FAIL");
        }

        private void HandleGetPlayerPicture(IGraphResult result)
        {
            if (string.IsNullOrEmpty(result.Error) && result.Texture != null)
            {
                _fbAvatarTexture = result.Texture;
                OnGetPictureFacebook?.Invoke();
            }
        }
#endif
#endregion

#region Share

 public void ShareFacebookLink(string linkStore, string nameGame, string linkImageShareFB)
        {
#if FACEBOOK
            string description = "Let's me play this " + nameGame + "!";
            FB.ShareLink(new Uri(linkStore), nameGame, description, new System.Uri(linkImageShareFB), ShareFacebookLink_Callback);
#endif
        }

#if FACEBOOK
	    protected void ShareFacebookLink_Callback(IShareResult result)
	    {
		    if (string.IsNullOrEmpty(result.Error) && !result.Cancelled)
		    {
                OnShareFacebookComplete?.Invoke();
		    }
	    }
#endif

        public void ShareFacebook(string linkStore, string nameGame, string linkImageShareFB)
        {
#if FACEBOOK
            FB.FeedShare(
                "",
                new System.Uri(linkStore),
                nameGame,
                "Share game " + nameGame,
                "Let's me play this " + nameGame + "!",
	            new System.Uri(linkImageShareFB),
                "",
                CallBackShare);
#endif
        }
        public void ShareFacebook(string linkStore, string nameGame, string linkImageShareFB, string linkDescription)
        {
#if FACEBOOK
            FB.FeedShare(
                "",
                new System.Uri(linkStore),
                nameGame,
                "Share game " + nameGame,
                "Join with me and play " + nameGame + ". " + linkDescription,
                new System.Uri(linkImageShareFB),
                "",
                CallBackShare);
#endif
        }

#if FACEBOOK
        private void CallBackShare(IShareResult result)
        {
            if ("".Equals(result) || result.Cancelled)
            {
                if (OnShareFacebookError != null)
                    OnShareFacebookError();
            }
            else
            {
                if (OnShareFacebookComplete != null)
                    OnShareFacebookComplete();
            }
        }
#endif

#endregion

#region Friends

        public void GetFriends()
        {
#if FACEBOOK
            string queryString = "/me/friends?fields=id,first_name,picture.width(128).height(128)&limit=100";
            FB.API(queryString, HttpMethod.GET, HandleGetFriendsCallback);
#endif
        }
        
        
        public void GetInvitableFriends()
        {
#if FACEBOOK
            string queryString = "/me/invitable_friends?fields=id,first_name,picture.width(128).height(128)&limit=25";
            FB.API(queryString, HttpMethod.GET, HandleGetInvitableFriendsCallback);
#endif
        }

#if FACEBOOK
        private void HandleGetFriendsCallback(IGraphResult result)
        {
            if(OnGetFriends != null)
            {
                OnGetFriends(result);
            }
        }

        private void HandleGetInvitableFriendsCallback(IGraphResult result)
        {
            if (OnGetInvitableFriends != null)
            {
                OnGetInvitableFriends(result);
            }
        }
#endif
        private void LoadFriendImgFromID(string userID, Action<Texture> callback)
        {
#if FACEBOOK
            /*FB.API(GraphUtil.GetPictureQuery(userID, 128, 128),
                   HttpMethod.GET,
                   delegate (IGraphResult result)
                   {
                       if (result.Error != null)
                       {
                           Debug.LogError(result.Error + ": for friend " + userID);
                           return;
                       }
                       if (result.Texture == null)
                       {
                           Debug.Log("LoadFriendImg: No Texture returned");
                           return;
                       }
                       callback(result.Texture);
                   });*/
#endif
        }
#endregion

#region Load Image
        public void LoadImageWithFbId(string facebookId)
        {
#if FACEBOOK
            //FB.API(GraphUtil.GetPictureQuery(facebookId, 32, 32), HttpMethod.GET, HandleLoadImageWithFbId);
#endif
        }

#if FACEBOOK
        private void HandleLoadImageWithFbId(IGraphResult result)
        {
            if(OnLoadImageWithFbId != null)
            {
                OnLoadImageWithFbId(result);
            }
        }
#endif
        public string GetPictureQuery(string facebookID, int? width = null, int? height = null, string type = null, bool onlyURL = false)
        {
            string query = string.Format("/{0}/picture", facebookID);
            string param = width != null ? "&width=" + width.ToString() : "";
            param += height != null ? "&height=" + height.ToString() : "";
            param += type != null ? "&type=" + type : "";
            if (onlyURL) param += "&redirect=false";
            if (param != "") query += ("?g" + param);
            return query;
        }

#endregion

    }

}