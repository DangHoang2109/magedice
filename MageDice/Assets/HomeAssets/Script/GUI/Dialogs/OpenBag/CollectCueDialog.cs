using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class CollectCueDialog : BaseSortingDialog
{
    public GameObject goBlocker;
    
    public Button btnWatch;
    private CanvasGroup cgWatch;
    public Button btnReject;
    private CanvasGroup cgReject;
    
    public CollectCuePanel main;

    [Header("log")]
    public Text txtLogBlockByAd;
    
    private bool isInitialized = false;

    private void Init()
    {
        this.cgWatch = this.btnWatch.GetComponent<CanvasGroup>();
        this.cgReject = this.btnReject.GetComponent<CanvasGroup>();
        
        this.main.Init(this.ShowButtonWatch, this.SetBlockRaycastByAds);
        
        this.isInitialized = true;
    }
    
    /// <param name="cData">data of the cue the game give tot he player</param>
    /// <param name="isWatch">is need to watch to earn (not supported yet)</param>
    public void ParseData(StatData cData, bool isWatch)
    {
        if (!this.isInitialized)
        {
            this.Init();
        }

        if (!isWatch)
        {
            this.cgWatch.alpha = this.cgReject.alpha = 0f;
        }
        
        this.main.ParseCue(cData, isWatch);
        this.main.StartAnimate();
    }

    private void SetBlockRaycastByAds(bool isBlock)
    {
        this.goBlocker.SetActive(isBlock);
    }
    
    private void ShowButtonWatch()
    {
        DOTween.Kill(this.btnReject);
        
        this.cgWatch.alpha = 1f;
        this.btnWatch.image.raycastTarget = true;
        
        this.cgReject.alpha = 0f;
        this.btnReject.image.raycastTarget = false;
        
        DOTween.Sequence()
            .AppendInterval(2f)
            .AppendCallback(this.ShowButtonReject).SetId(this.btnReject);
    }

    private void ShowButtonReject()
    {
        this.cgReject.DOFade(1f, 1f).SetId(this.btnReject);
        this.btnReject.image.raycastTarget = true;
    }

    public void OnClickWatch()
    {
        AdsManager.Instance.ShowVideoReward(LogAdsVideoWhere.OPEN_BAG_TAKE_ITEM,
            this.OnWatchAdsFinished);
    }
    
    // earn here
    private void OnWatchAdsFinished(bool isComplete)
    {
        if (isComplete)
        {
            SoundManager.Instance.Play("sfx_pellet_start");
            
            this.main.OnAdsWatched();
            this.HideButtons();
        }
        else
        {
            // do nothing
        }
    }
    
    public void OnClickReject()
    {
        SoundManager.Instance.Play("snd_step");
        
        this.main.OnAdsRejected();
        this.HideButtons();
    }

    private void HideButtons()
    {
        this.SetBlockRaycastByAds(false);

        this.cgWatch.DOFade(0f, 1f).SetId(this.btnReject);
        this.btnWatch.image.raycastTarget = false;
        this.cgReject.DOFade(0f, 1f).SetId(this.btnReject);
        this.btnReject.image.raycastTarget = false;
    }
    
    
    #if UNITY_EDITOR
    [UnityEditor.MenuItem("Test/Dialogs/Get a random cue!")]
    public static void Demo()
    {
        var d = GameManager.Instance.OnShowDialogWithSorting<CollectCueDialog>(
            "Home/GUI/Dialogs/OpenBag/CollectCue",
            PopupSortingType.CenterBottomAndTopBar);
        d.ParseData(StatManager.Instance.GetDatasByKind_Simple(StatManager.Kind.NotUnlocked).GetRandom(),
            true);
    }
    #endif
}
