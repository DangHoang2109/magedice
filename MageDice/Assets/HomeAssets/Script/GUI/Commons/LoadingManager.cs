﻿using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;
using DG.Tweening;
using TMPro;

public class LoadingManager : MonoSingleton<LoadingManager>
{
    public static UnityAction<float> callbackProgress;
    public Image imgLoadingScene;
    public GameObject panelLoading;

    public Image imgProgress;
    public TextMeshProUGUI txtProgress;
    public TextMeshProUGUI txtTip;

    public override void Init()
    {
        base.Init();
        callbackProgress += this.ProgressLoading;
    }

    private void ProgressLoading(float progress)
    {
        this.imgProgress.fillAmount = progress;
        this.txtProgress.text = string.Format("{0}%", (int)(progress * 100.0f));
    }

    public void LoadScene(bool isShow, UnityAction callback = null)
    {
        this.imgLoadingScene.gameObject.SetActive(true);
        if (isShow)
        {
            this.txtTip.text = LanguageManager.GetString(string.Format("TIP_{0}", TipConfigs.Instance.GetRandomTipIndex()), LanguageCategory.Tips); //TipConfigs.Instance.GetRandomTip()
        }
        float fade = isShow ? 1 : 0;
        Sequence seq = DOTween.Sequence();
        seq.Join(this.imgLoadingScene.DOFade(fade, 0.5f).SetEase(Ease.Linear));
        seq.OnComplete(() =>
        {
            if (callback != null)
            {
                callback.Invoke();
            }
            if(!isShow)
            {
                this.imgLoadingScene.gameObject.SetActive(false);
                this.imgProgress.fillAmount = 0.1f;
            }
        });
        
    }
    public void ShowLoading(bool isShow, UnityAction callback = null)
    {
        this.panelLoading.SetActive(isShow);
    }
}
