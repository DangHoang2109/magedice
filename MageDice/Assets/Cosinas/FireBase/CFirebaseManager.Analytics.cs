using System.Collections;
using System.Collections.Generic;
using UnityEngine;
#if FIREBASE_ANALYTICS
using Firebase.Analytics;
#endif
namespace Cosinas.Firebase
{
    public partial class CFirebaseManager
    {
        public void InitFirebaseAnalytics()
        {
        
#if FIREBASE_ANALYTICS
            FirebaseAnalytics.SetAnalyticsCollectionEnabled(true);
            Debug.LogError("Set user properties.");
            // Set the user's sign up method.
            FirebaseAnalytics.SetUserProperty(
                FirebaseAnalytics.UserPropertySignUpMethod,
                "Google");
            // Set the user ID.
            FirebaseAnalytics.SetUserId(SystemInfo.deviceUniqueIdentifier);
            AnalyticsLogin();
#endif
        }
        public void AnalyticsLogin()
        {
            UserBehaviorDatas.Instance.OpenApp();
#if FIREBASE_ANALYTICS
            // Log an event with no parameters.
            Debug.LogError("Logging a login event.");
            FirebaseAnalytics.LogEvent(FirebaseAnalytics.EventLogin);
#endif
        }

        public void AnalyticsAppOpen()
        {
#if FIREBASE_ANALYTICS
            // Log an event with no parameters.
            Debug.Log("Logging a login event.");
            FirebaseAnalytics.LogEvent(FirebaseAnalytics.EventAppOpen);
#endif
        }
        
        public void SetUserId(string userId)
        {
#if FIREBASE_ANALYTICS
            FirebaseAnalytics.SetUserId(userId);
#endif
        }

        public void SetUserProperty(string propertyName, string userId)
        {
#if FIREBASE_ANALYTICS
            FirebaseAnalytics.SetUserProperty(propertyName, userId);
#endif
        }

        public void LogEvent(string name, string parameterName, long parameterValue)
        {
#if FIREBASE_ANALYTICS
            FirebaseAnalytics.LogEvent(name, parameterName, parameterValue);
#endif
        }
        public void LogEvent(string name, string parameterName, int parameterValue)
        {
#if FIREBASE_ANALYTICS
            FirebaseAnalytics.LogEvent(name, parameterName, parameterValue);
#endif
        }
        public void LogEvent(string name)
        {
#if FIREBASE_ANALYTICS
            FirebaseAnalytics.LogEvent(name);
#endif
        }
        public void LogEvent(string name, string parameterName, string parameterValue)
        {
#if FIREBASE_ANALYTICS
            FirebaseAnalytics.LogEvent(name, parameterName, parameterValue);
#endif
        }
        public void LogEvent(string name, string parameterName, double parameterValue)
        {
#if FIREBASE_ANALYTICS
            FirebaseAnalytics.LogEvent(name, parameterName, parameterValue);
#endif
        }

        public void LogEvent(string nameEvent, Dictionary<string, string> values)
        {
#if FIREBASE_ANALYTICS
            List<Parameter> parameters = new List<Parameter>();
            foreach (var pair in values)
            {
                parameters.Add(new Parameter(pair.Key, pair.Value.ToString()));
            }
            
            FirebaseAnalytics.LogEvent(nameEvent, parameters.ToArray());
#endif
        }

        public void LogEvent(string nameEvent, Dictionary<string, object> values)
        {
#if FIREBASE_ANALYTICS
            Dictionary<string, string> parameters = new Dictionary<string, string>();
            foreach (var pair in values)
            {
                if (pair.Value != null) parameters[pair.Key] = pair.Value.ToString();
                else parameters[pair.Key] = string.Empty;
            }

            LogEvent(nameEvent, parameters);
#endif
        }
        
        public void AnalyticsLevelUp(int level, string parameterCharacter)
        {
#if FIREBASE_ANALYTICS
            // Log an event with multiple parameters.
            Debug.Log("Logging a level up event.");
            FirebaseAnalytics.LogEvent(
              FirebaseAnalytics.EventLevelUp,
              new Parameter(FirebaseAnalytics.ParameterLevel, level),
              new Parameter(FirebaseAnalytics.ParameterCharacter, parameterCharacter));
#endif
        }
    }
}
