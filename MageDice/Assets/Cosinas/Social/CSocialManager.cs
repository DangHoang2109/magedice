using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Cosinas.Social
{
    
    public partial class CSocialManager : MonoBehaviour
    {
        private static CSocialManager _instance;
        public static CSocialManager Instance
        {
            get
            {
                return _instance;
            }
        }

        private void Awake()
        {
            _instance = this;
        }
        
        private void Start()
        {
            GameManager.Instance.AddCallbackNetWork(this.OnChangeNetwork);
        }

        private void OnChangeNetwork(bool isInternet)
        {
            if (isInternet)
            {
                this.SocialInit();
            }
        }

        public void SocialInit()
        {
            this.InitFacebook();
            this.InitGameServices();
        }
    }

}