using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;
#if FIREBASE_MESSAGING
using System;
using Firebase;
using Firebase.Messaging;
#endif

namespace Cosinas.Firebase
{
    
    public partial class CFirebaseManager
    {
        private string topic = "TestTopic";
        public void InitMessaging()
        {
#if FIREBASE_MESSAGING
            Debug.LogError("FirebaseManager.InitializeMessaging");
            FirebaseMessaging.MessageReceived += OnMessageReceived;
            FirebaseMessaging.TokenReceived += OnTokenReceived;
            FirebaseMessaging.SubscribeAsync(topic).ContinueWith(task =>
            {
                LogTaskCompletion(task, "SubscribeAsync");
            });

            // This will display the prompt to request permission to receive
            // notifications if the prompt has not already been displayed before. (If
            // the user already responded to the prompt, their decision is cached by
            // the OS and can be changed in the OS settings).
            FirebaseMessaging.RequestPermissionAsync().ContinueWith(task =>
            {
                LogTaskCompletion(task, "RequestPermissionAsync");
            });

            ToggleTokenOnInit();
#endif
        }


#if FIREBASE_MESSAGING
        public virtual void OnMessageReceived(object sender, MessageReceivedEventArgs e)
        {
            Debug.Log("FirebaseManager.OnMessageReceived Received a new message");
            var notification = e.Message.Notification;
            if (notification != null)
            {
                Debug.Log("FirebaseManager.OnMessageReceived title: " + notification.Title);
                Debug.Log("FirebaseManager.OnMessageReceived body: " + notification.Body);
                Debug.Log("FirebaseManager.OnMessageReceived body: " + notification.Icon);
            }
            if (e.Message.From.Length > 0)
            {
                Debug.Log("FirebaseManager.OnMessageReceived from: " + e.Message.From);
            }
            if (e.Message.Link != null)
            {
                Debug.Log("FirebaseManager.OnMessageReceived link: " + e.Message.Link.ToString());
            }
            if (e.Message.Data.Count > 0)
            {
                Debug.Log("FirebaseManager.OnMessageReceived data:");
                foreach (KeyValuePair<string, string> iter in e.Message.Data)
                {
                    Debug.Log("  " + iter.Key + ": " + iter.Value);
                }
            }
        }

        public virtual void OnTokenReceived(object sender, TokenReceivedEventArgs token)
        {
            Debug.Log("FirebaseManager.OnTokenReceived Received Registration Token: " + token.Token);
        }
#endif

        public void ToggleTokenOnInit()
        {
#if FIREBASE_MESSAGING
            bool tokenRegistration = FirebaseMessaging.TokenRegistrationOnInitEnabled;
            //FirebaseMessaging.TokenRegistrationOnInitEnabled = newValue;
            Debug.LogError("FirebaseManager.ToggleTokenOnInit Set TokenRegistrationOnInitEnabled to ");
            if (!tokenRegistration)
            {
                FirebaseMessaging.TokenRegistrationOnInitEnabled = true;
            }
#endif
        }

#if FIREBASE_MESSAGING
        protected bool LogTaskCompletion(Task task, string operation)
        {
            if (task is null)
            {
                throw new ArgumentNullException(nameof(task));
            }

            bool complete = false;
            if (task.IsCanceled)
            {
                Debug.Log("FirebaseManager " + operation + " canceled.");
            }
            else if (task.IsFaulted)
            {
                Debug.Log("FirebaseManager " + operation + " uncounted an error.");
                foreach (Exception exception in task.Exception.Flatten().InnerExceptions)
                {
                    string errorCode = "";
                    Debug.Log("FirebaseManager " + errorCode + exception.ToString());
                }
            }
            else if (task.IsCompleted)
            {
                Debug.Log("FirebaseManager " + operation + " completed");
                complete = true;
            }
            return complete;
        }
#endif
    }
}
