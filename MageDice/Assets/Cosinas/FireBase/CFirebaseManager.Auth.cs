using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.Threading.Tasks;

#if FIREBASE_AUTH
using Firebase;
using Firebase.Auth;
using Firebase.Extensions;
using Firebase.Database;
#endif

namespace Cosinas.Firebase
{
    public partial class CFirebaseManager
    {
#if FIREBASE_AUTH
        protected FirebaseAuth _auth;
        protected FirebaseUser _user;
        protected DatabaseReference _referenceDatabase;
        protected Dictionary<string, FirebaseUser> _userByAuth = new Dictionary<string, FirebaseUser>();
        private bool fetchingToken = false;
#endif
        public void InitAuth()
        {
#if FIREBASE_AUTH
            _auth = FirebaseAuth.DefaultInstance;
            _auth.StateChanged += AuthStateChanged;
            _auth.IdTokenChanged += IdTokenChanged;
            _referenceDatabase = FirebaseDatabase.DefaultInstance.RootReference;
#endif
        }
        
        protected void OnEnableAuth()
        {
            Cosinas.Social.CSocialManager.OnInitFacebookCompleted += SocialManager_OnInitFacebookCompleted;
            Cosinas.Social.CSocialManager.OnLoginFacebookSuccessed += SocialManager_OnLoginFacebookCompleted;

        }

        public void OnDisableAuth()
        {
            Cosinas.Social.CSocialManager.OnInitFacebookCompleted -= SocialManager_OnInitFacebookCompleted;
            Cosinas.Social.CSocialManager.OnLoginFacebookSuccessed -= SocialManager_OnLoginFacebookCompleted;

        }

        private void SocialManager_OnInitFacebookCompleted()
        {
            Debug.LogError("SocialManager_OnInitFacebookCompleted");
            if (Cosinas.Social.CSocialManager.Instance.IsLoginFacebook())
            {
                SignInWithFacebook(Cosinas.Social.CSocialManager.Instance.GetFacebookAccessTokenString(), false);
            }
        }
        private void SocialManager_OnLoginFacebookCompleted()
        {
            Debug.LogError("SocialManager_OnLoginFacebookCompleted");
            if (Cosinas.Social.CSocialManager.Instance.IsLoginFacebook())
            {
                //Kiểm tra nếu login firebase rồi thì k login nữa
                if (!UserDatas.Instance.info.isLoginFirebase)
                {
                    SignInWithFacebook(Cosinas.Social.CSocialManager.Instance.GetFacebookAccessTokenString(), false);
                }
            }
        }
        public void SignInWithFacebook(string accessToken, bool callDelegate = true)
        {
#if FIREBASE_AUTH
            try
            {
                if (!string.IsNullOrEmpty(accessToken))
                {
                    Debug.LogError("FirebaseManager.SignInWithFacebook");
#if FIREBASE_AUTH
                    Credential credential = FacebookAuthProvider.GetCredential(accessToken);
                    //_auth.SignInWithCredentialAsync(credential).ContinueWith(task =>
                    _auth.SignInWithCredentialAsync(credential).ContinueWithOnMainThread(task =>
                    {
                        if (task.IsCanceled)
                        {
                            Debug.LogError("@LOG F4A FirebaseManager.SignInWithFacebook/SignInWithCredentialAsync was canceled.");
                            //if (callDelegate) OnLoginFacebookCompleted?.Invoke(false, "SignInWithFacebook was canceled");
                            return;
                        }
                        else if (task.IsFaulted)
                        {
                            Debug.LogError("@LOG F4A FirebaseManager.SignInWithFacebook/SignInWithCredentialAsync encountered an error: " + task.Exception);
                            //if (callDelegate) OnLoginFacebookCompleted?.Invoke(false, "SignInWithFacebook " + task.Exception.ToString());
                            return;
                        }
                        else
                        {
                            _user = task.Result;
                            UserDatas.Instance.info.LoginFirebase(_user.UserId);
                            Debug.LogFormat("@LOG F4A FirebaseManager.SignInWithFacebook User signed in successfully: {0} ({1})", _user.DisplayName, _user.UserId);
                            //if (callDelegate) OnLoginFacebookCompleted?.Invoke(true, string.Empty);
                        }
                    });
#endif
                }
            }
            catch (Exception ex)
            {
                Debug.LogError("FirebaseManager.SignInWithFacebook/SignInWithCredentialAsync encountered an error: " + ex);
                //if (callDelegate) OnLoginFacebookCompleted?.Invoke(false, ex.Message);
            }
#endif
        }
        
        public bool IsSignIn()
        {
#if FIREBASE_AUTH
            return _auth != null && _auth.CurrentUser != null && _user != null && !string.IsNullOrEmpty(_user.UserId);
#else
            return false;
#endif
        }
        public void LogoutWithFacebook()
        {
#if FIREBASE_AUTH
            if(_auth != null && _auth.CurrentUser != null) _auth.SignOut();
            _user = null;
#endif
        }
#if FIREBASE_AUTH
        void AuthStateChanged(object sender, System.EventArgs eventArgs)
        {
            FirebaseAuth senderAuth = sender as FirebaseAuth;
            if (senderAuth != null) _userByAuth.TryGetValue(senderAuth.App.Name, out _user);
            if (senderAuth == _auth && senderAuth.CurrentUser != _user)
            {
                bool signedIn = _user != senderAuth.CurrentUser && senderAuth.CurrentUser != null;
                if (!signedIn && _user != null)
                {
                    Debug.LogError("Signed out " + _user.UserId);
                }
                _user = senderAuth.CurrentUser;
                _userByAuth[senderAuth.App.Name] = _user;
                if (signedIn)
                {
                    Debug.LogError("AuthStateChanged Signed in " + _user.UserId);

                    //displayName = user.DisplayName ?? "";
                    //DisplayDetailedUserInfo(user, 1);
                }
            }
        }

        // Track ID token changes.
        void IdTokenChanged(object sender, System.EventArgs eventArgs)
        {
            FirebaseAuth senderAuth = sender as FirebaseAuth;
            if (senderAuth == _auth && senderAuth.CurrentUser != null && !fetchingToken)
            {
                senderAuth.CurrentUser.TokenAsync(false).ContinueWithOnMainThread(
                  task => Debug.LogError(String.Format("Token[0:8] = {0}", task.Result.Substring(0, 8))));
            }
        }

        public void GetUserToken()
        {
            if (_auth.CurrentUser == null)
            {
                Debug.LogError("Not signed in, unable to get token.");
                return;
            }
            Debug.LogError("Fetching user token");
            fetchingToken = true;
            _auth.CurrentUser.TokenAsync(false).ContinueWithOnMainThread(task => {
                fetchingToken = false;
                if (LogTaskCompletion(task, "User token fetch"))
                {
                    Debug.LogError("Token = " + task.Result);
                }
            });
        }
#endif
        
        public void GetUserData(string databaseName, System.Action<bool, object> callBack)
        {
#if FIREBASE_AUTH
            if (IsSignIn())
            {
                GetUserData(databaseName, _user.UserId, callBack);
            }
            else
            {
                callBack?.Invoke(false, string.Empty);
            }
#endif
        }
        
        private void GetUserData(string databaseName, string uid, System.Action<bool, string> callBack)
        {
#if FIREBASE_AUTH
            DatabaseReference data = _referenceDatabase.Child(databaseName).Child(uid);
            if (data != null)
            {
                data.GetValueAsync().ContinueWithOnMainThread(task =>
                {
                    try
                    {
                        if (task == null || task.IsFaulted || task.IsCanceled)
                        {
                            callBack?.Invoke(false, string.Empty);
                        }
                        else if (task.IsCompleted)
                        {
                            // Do something with snapshot...
                            DataSnapshot snapshot = task.Result;
                            if (snapshot != null)
                            {
                                string jsonData = snapshot.GetRawJsonValue();
                                if (!string.IsNullOrEmpty(jsonData))
                                {
                                    callBack?.Invoke(true, jsonData);
                                }
                                else
                                {
                                    callBack?.Invoke(false, string.Empty);
                                }
                            }
                        }
                        else
                        {
                            callBack?.Invoke(false, string.Empty);
                        }
                    }
                    catch (Exception ex)
                    {
                        Debug.Log("FirebaseManager.GetUserData ex:" + ex);
                        callBack?.Invoke(false, string.Empty);
                    }
                });
            }
            else
            {
                callBack?.Invoke(false, string.Empty);
            }
#endif
        }
        
        public void SaveDatabase(string databaseName, string json, System.Action<bool> callBack = null)
        {
#if FIREBASE_AUTH
            if (_referenceDatabase == null) return;
            if (!IsSignIn()) return;

            Debug.LogFormat("FirebaseManager.SaveDatabase User signed in successfully: {0} ({1})", _user.DisplayName, _user.UserId);
            var uid = _user.UserId;

            this._referenceDatabase.Child(databaseName).Child(uid).SetRawJsonValueAsync(json).ContinueWithOnMainThread(task =>
            {
                if (task.IsCompleted)
                {
                    Debug.LogError("SUCCESS UPDATE DATA");
                    callBack?.Invoke(true);
                }
                else
                {
                    Debug.LogError("ERROR UPlOAD DATA");
                    callBack?.Invoke(false);
                }
                
            });
#endif
        }

    }
}
