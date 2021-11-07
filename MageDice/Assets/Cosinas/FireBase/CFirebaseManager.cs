using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

#if FIREBASE_ANALYTICS || FIREBASE_REMOTE_CONFIG || FIREBASE_MESSAGING || FIREBASE_AUTH
using Firebase;
using Firebase.Extensions;
#endif

namespace Cosinas.Firebase
{
    
    public partial class CFirebaseManager : MonoBehaviour
    {
        private static CFirebaseManager _instance;

        public static CFirebaseManager Instance
        {
            get
            {
                return _instance;
            }
        }
        private void Awake()
        {
            _instance = this;
            this.OnEnableAuth();
        }

        
        private void Start()
        {
            GameManager.Instance.AddCallbackNetWork(this.OnChangeNetword);
        }
        private void OnChangeNetword(bool isInternet)
        {
            if (isInternet)
            {
                this.InitFirebase();
            }
        }

        private bool _firebaseInitialized = false;
        public bool FirebaseInitialized
        {
            get { return _firebaseInitialized; }
            set { _firebaseInitialized = value; }
        }
#if FIREBASE_ANALYTICS || FIREBASE_REMOTE_CONFIG || FIREBASE_MESSAGING || FIREBASE_AUTH
        private DependencyStatus _dependencyStatus;
        private FirebaseApp _app;
#endif

        private void InitFirebase()
        {
            if (this.FirebaseInitialized)
            {
                return;
            }
#if FIREBASE_ANALYTICS || FIREBASE_REMOTE_CONFIG || FIREBASE_MESSAGING || FIREBASE_AUTH
            FirebaseApp.CheckAndFixDependenciesAsync().ContinueWithOnMainThread(task =>
            {
                _dependencyStatus = task.Result;

                if (_dependencyStatus == DependencyStatus.Available)
                {
                    this._firebaseInitialized = true;
                    this._app = FirebaseApp.DefaultInstance;

#if FIREBASE_ANALYTICS
                    try
                    {
                        this.InitFirebaseAnalytics();
                    }
                    catch (Exception e)
                    {
                        Debug.LogError(e);
                    }
#endif
                    
#if FIREBASE_REMOTE_CONFIG
                    try
                    {
                        this.InitRemoteConfigs();
                    }
                    catch (Exception e)
                    {
                        Debug.LogError(e);
                    }
#endif
#if FIREBASE_MESSAGING
                    try
                    {
                        this.InitMessaging();
                    }
                    catch (Exception e)
                    {
                        Debug.LogError(e);
                    }
#endif
#if FIREBASE_AUTH
                    try
                    {
                        this.InitAuth();
                    }
                    catch (Exception e)
                    {
                        Debug.LogError(e);
                    }
#endif
                }
                

                else
                {
                    this._firebaseInitialized = false;
                    Debug.LogError("@LOG Could not resolve all Firebase dependencies: " + _dependencyStatus);
                }
               
            });
#endif
        }
    }
}
