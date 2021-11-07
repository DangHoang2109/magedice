using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;
using System;
#if FIREBASE_REMOTE_CONFIG
using Firebase.RemoteConfig;
using Firebase.Extensions;
#endif
namespace Cosinas.Firebase
{
    public partial class CFirebaseManager
    {
        public void InitRemoteConfigs()
        {
#if FIREBASE_REMOTE_CONFIG
            this.FetchDataAsync();
#endif
        }
        
#if FIREBASE_REMOTE_CONFIG

        public Task FetchDataAsync()
        {
            Debug.LogError("@LOG FetchDataAsync");
            Task fetchTask = FirebaseRemoteConfig.DefaultInstance.FetchAsync(System.TimeSpan.Zero);
            return fetchTask.ContinueWithOnMainThread(FetchComplete);
        }

        void FetchComplete(System.Threading.Tasks.Task fetchTask)
        {
            if (fetchTask.IsCanceled)
            {
                Debug.LogError("Fetch canceled.");
            }
            else if (fetchTask.IsFaulted)
            {
                Debug.LogError("Fetch encountered an error.");
            }
            else if (fetchTask.IsCompleted)
            {
                Debug.LogError("Fetch completed successfully!");

            }
            
            FirebaseRemoteConfig remoteConfig = FirebaseRemoteConfig.DefaultInstance;
            var info = remoteConfig.Info;
            switch (info.LastFetchStatus)
            {
                case LastFetchStatus.Success:
                    remoteConfig.ActivateAsync().ContinueWithOnMainThread((task) =>
                    {
                        Debug.LogError(string.Format("Remote data loaded and ready (last fetch time {0}).",
                            info.FetchTime));
                        string gameConfig = remoteConfig.GetValue("configs").StringValue;
                        Debug.LogError("CONFIGS:" + gameConfig);
                        //RemoteConfigsManager.ParseRemoteConfig(gameConfig);
                    });
                    return;
                case LastFetchStatus.Failure:
                    switch (info.LastFetchFailureReason)
                    {
                        case FetchFailureReason.Error:
                            Debug.LogError("Fetch failed for unknown reason");
                            break;
                        case FetchFailureReason.Throttled:
                            Debug.Log("Fetch throttled until " + info.ThrottledEndTime);
                            break;
                    }
                    return;
                case LastFetchStatus.Pending:
                    Debug.LogError("Latest Fetch call still pending.");
                    return;
            }

        }
#endif
        
    }
}
