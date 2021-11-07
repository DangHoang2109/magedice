using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AdsBannerSample : MonoBehaviour
{
    private RectTransform rect => this.transform as RectTransform;
    #if UNITY_EDITOR
    private void OnValidate()
    {
        //throw new NotImplementedException();
        this.ShowSampleBanner();
    }
    #endif

    private void Start()
    {
        
    }

    private void ShowSampleBanner()
    {
        float x = 350f * Screen.dpi / 160f;
        float y = 50 * Screen.dpi / 160f;
        this.rect.sizeDelta = new Vector2(x, y);
    }
}
