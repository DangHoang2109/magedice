using System.Collections.Generic;
using System.Linq;
using Cosina.Components;
using DG.Tweening;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;
using Random = UnityEngine.Random;

/// <summary>
/// Earn things in: OpenBagItemUpper -> ParseBooster & ParseStatsCard
/// <br></br> Watch ads in: OpenBagItemUpper -> ParseBooster & ParseStatCard
/// </summary>
public class OpenBagDialog : BaseSortingDialog
{
    [System.Serializable]
    public class BagCardModel
    {
        public enum EarnType
        {
            None = 0,
            Booster = 1,
            CueCard = 2,
            Cue = 3,
        }

        /// <summary>
        /// Booster | String | Equipment
        /// </summary>
        public EarnType earnType;
        /// <summary>
        /// booster you earn
        /// </summary>
        public BoosterType booster;
        /// <summary>
        /// card or cue you earn
        /// </summary>
        public StatData equipmentConfig;
        /// <summary>
        /// value you earn
        /// </summary>
        public long value;
        /// <summary>
        /// need watch ads?
        /// </summary>
        public bool isWatch = false;
    }
    public class BagModel
    {
        public BagType typeBag;
        public List<BagCardModel> items;

        /// <summary>
        /// Nếu user ko coi reward ads thì có show inter hay ko?
        /// 3 bag đầu thì ko có reward ads nên ko show inter
        /// Các bag mua IAP ko có show inter
        /// </summary>
        public bool isShowInterAtEnd;

        public BagModel()
        {
            this.items = new List<BagCardModel>();
            this.isShowInterAtEnd = false;
        }
    }
    
    [Header("Logs")]
    public Text txtLogBlockByAd;
    public Text txtLogBlockByAnim;
    
    [Header("Panels Play")]
    public Transform transTop;
    public Transform transMid;
    public Transform transBottom;

    [Header("Components Play")]
    public OpenBagBag bag;
    public OpenBagItemCounter counter;
    public OpenBagItemUpper cardPlay;



    private List<BagModel> models;
    private int indexCard = 0;
    private int indexBag = 0;


    [Header("Result")]
    public RectTransform transTxtYouGot;
    
    public GameObject goResult;
    public Transform transResult;
    public Transform transCache;
    [Space(10f)]
    public OpenBagItemFinal pfCardResult;
    private Stack<OpenBagItemFinal> cardsPool;
    private List<OpenBagItemFinal> cardsResult;


    private bool isBlockingByAds;
    private bool isInitialized = false;

    private bool isNeedShowInterstitial = false;
    
    private void FirstRun()
    {
        this.bag.Init(this.transTop, transMid);
        this.counter.Init();
        this.cardPlay.Init();
        
        this.cardsPool = new Stack<OpenBagItemFinal>();
        this.cardsResult = new List<OpenBagItemFinal>();
        
        this.canvasGroup.alpha = 0f;
        
        this.isInitialized = true;
        
    }
    
    public override void OnShow(object data = null, UnityAction callback = null)
    {
        if(!this.isInitialized)
            this.FirstRun();

        base.OnShow(data, callback);
        this.ParseData();
        this.AnimateBagFall();
    }

    public void DisableInterstitial()
    {
        this.isNeedShowInterstitial = false;
    }

    private void ParseData()
    {
        if (data is BagModel m)
        {
            this.models = new List<BagModel>() {m};

            this.isNeedShowInterstitial = m.isShowInterAtEnd;
        }
        else if (data is List<BagModel> b)
        {
            this.models = b;
            
            this.isNeedShowInterstitial = b[0].isShowInterAtEnd;
        }
        else
        {
            Debug.LogError( $"{"OpenBagDialog".WrapColor("red")} ParseData exception: data is not model or list model, it is: {data?.GetType().Name??"NULL"}" );
            return;
        }

        //do mission openbag

        this.indexBag = 0;
        this.ParseNextBag();


    }

    private void ParseNextBag()
    {
        if (this.indexBag >= this.models.Count)
        {
            Debug.LogError( $"{"OpenBagDialog".WrapColor("red")} ParseBag exception: -indexBag {indexBag} -bagsCount {this.models.Count}" );
            this.ClickCloseDialog();
            return;
        }

        this.indexCard = 0;
        
        this.bag.gameObject.SetActive(true);
        this.bag.ParseData(this.models[this.indexBag].typeBag);
        this.bag.CachedTransform.position = this.transTop.position + Vector3.up * 5f;
        
        this.counter.gameObject.SetActive(true);
        this.counter.Parse(this.models[this.indexBag].typeBag, this.models[this.indexBag].items.Count);
    }
    
    protected override void AnimationShow()
    {
        this.canvasGroup.DOFade(1f, this.transitionTime)
            .OnComplete(this.OnCompleteShow).SetId(this);
    }

    private void AnimateBagFall()
    {
        this.bag.StartFallDown(this.transTop.position + Vector3.up * 200f, this.transBottom.position)
            .OnComplete(this.OnBagFallComplete);
        Invoker.Invoke(this.PlayBagSound, this.bag.durationFall * 0.8f);
    }


    private void PlayBagSound()
    {
        SoundManager.Instance.Play("snd_BoxDrop2",
            SoundManager.VOLUME_SOUND * 1.2f);
    }

    protected override void AnimationHide()
    {
        this.canvasGroup.DOFade(0f, this.transitionTime)
            .OnComplete(this.OnCompleteHide).SetId(this);
    }

    protected override void OnCompleteHide()
    {
        DOTween.Kill(this);
        base.OnCompleteHide();
        
        this.ReturnAllCards();
        
        //check tutorial
        if (TutorialDatas.TUTORIAL_PHASE == TutorialDatas.DONE_PHASE_AI)
        {
            TutorialDatas.TUTORIAL_PHASE = TutorialDatas.TUT_PHASE_FINAL;
            //Hướng dẫn click find room
            //step này không quan trọng, xem như đã complete tutorial
            TutorialHomeStep1 nextStep = FindObjectOfType<TutorialHomeStep1>();
            if (nextStep != null)
            {
                nextStep.TutorialClickRoom();
            }
        }
    }

    private void OnBagFallComplete()
    {
        Debug.Log($"{"OpenBagDialog".WrapColor("red")} OnBagFallComplete");
    }

    public void OnClickStep()
    {
        if (this.isBlockingByAds || !this.cardPlay.CheckClickable())
        {
            Debug.Log("OpenBagDialog".WrapColor("magenta")
                      + " OnClickStep failed: " + "ads".WrapColor("yellow"));
            return;
        }
            
        
        switch (this.bag.Step)
        {
            case OpenBagBag.BagAppearStep.None:
                this.CheckShowBagAgain();
                break;
            case OpenBagBag.BagAppearStep.FallingDown:
                //this.bag.SkipAnimation(); // no longer let this be skipped
                break;
            case OpenBagBag.BagAppearStep.Waiting:
                if(this.indexCard > 0)
                    this.cardPlay.StartHide();
            
                if (indexCard >= this.models[this.indexBag].items.Count)
                {
                    this.ShowResult();
                }
                else
                {
                    this.StartOpenNextCard();
                }
                break;
            case OpenBagBag.BagAppearStep.Opening:
                this.bag.SkipAnimation();
                break;
        }
    }

    private void StartOpenNextCard()
    {
        this.counter.UpdateCount(this.models[this.indexBag].items.Count - this.indexCard - 1);

        var itemModel = this.models[this.indexBag].items[this.indexCard];
        this.isBlockingByAds = itemModel.isWatch;
        if (itemModel.earnType == BagCardModel.EarnType.Cue)
        {
            StatData c = itemModel.equipmentConfig;
            this.bag.StartOpenBag(OpenBagItemUpper.CardDisplayType.Cue)
                .OnComplete(this.OnShowCue);
#if UNITY_EDITOR
            Debug.Log("b_ " + "OpenBag".WrapColor("yellow")
                            + $" StartOpenNextCard {"new".WrapColor("yellow")} -isAds {this.isBlockingByAds}" );
#else
            Debug.Log($"b_ OpenBag StartOpenNextCard new -isAds {this.isBlockingByAds}");
#endif
            SoundManager.Instance.Play("snd_RandomCard");
            return;
        }
#if UNITY_EDITOR
        Debug.Log("b_ " + "OpenBag".WrapColor("yellow")
                        + $" StartOpenNextCard -isAds {this.isBlockingByAds}" );
#else
        Debug.Log($"b_ OpenBag StartOpenNextCard -isAds {this.isBlockingByAds}" );
#endif
        
        this.bag.StartOpenBag(OpenBagItemUpper.CardDisplayType.Common)
            .OnComplete(this.OnShowCard);

        SoundManager.Instance.Play("snd_OpenCard");
    }


    private void OnShowCard()
    {
        this.isBlockingByAds = false;
        this.cardPlay.ParseData(this.models[this.indexBag].typeBag, this.models[this.indexBag].items[this.indexCard], this.transTop.position);
        this.cardPlay.StartAnimate();
        ++this.indexCard;
    }
    private void OnShowCue()
    {
        
        LogGameAnalytics.Instance.LogEvent(LogAnalyticsEvent.CUE_UNLOCKED, LogParams.STAT_ITEM_ID, this.models[this.indexBag].items[this.indexCard].equipmentConfig.id);  //unlock trong bag
        this.isBlockingByAds = false;
        this.cardPlay.ParseData(this.models[this.indexBag].typeBag, this.models[this.indexBag].items[this.indexCard], this.transMid.position);
        this.cardPlay.StartAnimate();
        ++this.indexCard;
        SoundManager.Instance.Play("snd_ShowCard");
    }
    
    public void ShowResult()
    {
        this.bag.Done();
        this.cardPlay.StartHide();
        this.counter.gameObject.SetActive(false);
        this.bag.gameObject.SetActive(true);
        this.goResult.SetActive(true);

        var curCards = this.models[this.indexBag].items;
        // remove items which still have ads available (I mean, the player didn't watch on that item)
        curCards.RemoveAll(x => x.isWatch);
        this.ShowResult(curCards);
        SoundManager.Instance.Play("snd_reward");
    }

    private void ShowResult(List<BagCardModel> curCards)
    {
        for (int i = 0; i < curCards.Count; ++i)
        {
            OpenBagItemFinal itemFinal = this.PrepareCardResult();
            if (itemFinal != null)
            {
                itemFinal.ParseData(curCards[i]);
                //Vector3 oldScale = itemFinal.transform.lossyScale;
                //itemFinal.transform.localScale = Vector3.zero;
                //itemFinal.transform.DOScale(oldScale, 0.2f);
            }      
        }
        this.transResult.localScale = Vector3.zero;
        this.transResult.DOScale(Vector3.one, 0.2f);

        if(curCards.Count <= 6)
        {
            this.transTxtYouGot.anchoredPosition = new Vector2(0, 256f);
        }
        else if (curCards.Count <= 9)
        {
            this.transTxtYouGot.anchoredPosition = new Vector2(0, 382f);
        }
        else
        {
            this.transTxtYouGot.anchoredPosition = new Vector2(0, 471f);
        }
    }

    private OpenBagItemFinal PrepareCardResult()
    {
        OpenBagItemFinal result;
        if (!this.cardsPool.Any())
        {
            result = GameObject.Instantiate(this.pfCardResult, this.transResult);          
        }
        else
        {
            result = this.cardsPool.Pop();
            result.SetParent(this.transResult);
        }
        Vector3 oldScale = result.transform.localScale;
        this.cardsResult.Add(result);
        return result;
    }

    private void ReturnAllCards()
    {
        for (int i = 0; i < this.cardsResult.Count; ++i)
        {
            this.cardsResult[i].Hide(this.transCache);
            this.cardsPool.Push(this.cardsResult[i]);
        }
        this.cardsResult.Clear();
        
        this.goResult.SetActive(false);
    }

    public void CheckShowBagAgain()
    {
        if ((++this.indexBag) >= this.models.Count)
        {
            if (this.isNeedShowInterstitial)
            {
                AdsManager.Instance.ShowInterstitial(LogAdsInterstitialWhere.OPEN_BOX);
            }
            this.ClickCloseDialog();
        }
        else
        {
            this.ReturnAllCards();
            this.ParseNextBag();
            this.AnimateBagFall();
        }
    }
    
#if UNITY_EDITOR
    

    [UnityEditor.MenuItem("Test/Dialogs/Bag/Demo many cases")]
    public static void TestManyCase()
    {
        if (!Application.isPlaying)
        {
            UnityEditor.EditorUtility.DisplayDialog("Ehhh", "Hey bro, please run this when the game is running!", "Yes sir");
            return;
        }

        var bagType = System.Enum.GetValues(typeof(BagType)).Cast<BagType>()
            .OrderBy(elem => System.Guid.NewGuid()).FirstOrDefault();
        BagModel m = new BagModel()
        {
            typeBag = bagType,
            items = new List<BagCardModel>()
            {
                new BagCardModel()
                {
                    earnType = BagCardModel.EarnType.Booster,
                    booster = BoosterType.CASH,
                    value = 1000,
                },
                new BagCardModel()
                {
                    earnType = BagCardModel.EarnType.Booster,
                    booster = BoosterType.COIN,
                    value = 1000,
                },
                new BagCardModel()
                {
                    earnType = BagCardModel.EarnType.Cue,
                    equipmentConfig = StatManager.Instance.GetDatasByKind_Simple(StatManager.Kind.NotUnlocked).GetRandom(),
                    value = 1,
                },
                new BagCardModel()
                {
                    earnType = BagCardModel.EarnType.Cue,
                    equipmentConfig = StatManager.Instance.GetDatasByKind_Simple(StatManager.Kind.NotUnlocked).GetRandom(),
                    value = 1,
                    isWatch =  true,
                },
                new BagCardModel()
                {
                    earnType = BagCardModel.EarnType.CueCard,
                    equipmentConfig = StatManager.Instance.GetDatasByKind_Simple(StatManager.Kind.NotUnlocked).GetRandom(),
                    value = Random.Range(1, 11),
                },
                new BagCardModel()
                {
                    earnType = BagCardModel.EarnType.CueCard,
                    equipmentConfig = StatManager.Instance.GetDatasByKind_Simple(StatManager.Kind.NotUnlocked).GetRandom(),
                    value = Random.Range(1, 11),
                    isWatch = true,
                },
                new BagCardModel()
                {
                    earnType = BagCardModel.EarnType.Booster,
                    booster = BoosterType.CASH,
                    value = 1000,
                },
                new BagCardModel()
                {
                    earnType = BagCardModel.EarnType.Booster,
                    booster = BoosterType.COIN,
                    value = 1000,
                    isWatch = true
                },
            }
        };

        m.items.Shuffle();
        
        Invoker.Invoke(() =>
        {
            GameManager.Instance.OnShowDialogWithSorting<OpenBagDialog>(
                "Home/GUI/Dialogs/OpenBag/OpenBag",
                PopupSortingType.OnTopBar,
                m);
        }, 0.5f);
    }
    
    [UnityEditor.MenuItem("Test/Dialogs/Bag/Demo cards for strange non-country cue")]
    public static void TestCardsStrange()
    {
        if (!Application.isPlaying)
        {
            UnityEditor.EditorUtility.DisplayDialog("Ehhh", "Hey bro, please run this when the game is running!", "Yes sir");
            return;
        }

        var bagType = System.Enum.GetValues(typeof(BagType)).Cast<BagType>()
            .OrderBy(elem => System.Guid.NewGuid()).FirstOrDefault();
        var collection = StatManager.Instance.GetDatasByKind_Simple(StatManager.Kind.NotUnlocked);
        BagModel m = new BagModel()
        {
            typeBag = bagType,
            items = new List<BagCardModel>()
        };
        for (int i = 0; i < 9; ++i)
        {
            var bm = new BagCardModel()
            {
                earnType = BagCardModel.EarnType.CueCard,
                equipmentConfig = collection.GetRandom(),
                value = Random.Range(1, 11),
                isWatch = Random.Range(0, 2) == 0,
            };
            collection.Remove(bm.equipmentConfig);
            m.items.Add(bm);
        }
        
        Invoker.Invoke(() =>
        {
            GameManager.Instance.OnShowDialogWithSorting<OpenBagDialog>(
                "Home/GUI/Dialogs/OpenBag/OpenBag",
                PopupSortingType.OnTopBar,
                m);
        }, 0.5f);
    }
    [UnityEditor.MenuItem("Test/Dialogs/Bag/Demo get cues  %&#B")]
    public static void TestGetCues()
    {
        if (!Application.isPlaying)
        {
            UnityEditor.EditorUtility.DisplayDialog("Ehhh", "Hey bro, please run this when the game is running!", "Yes sir");
            return;
        }

        var bagType = System.Enum.GetValues(typeof(BagType)).Cast<BagType>()
            .OrderBy(elem => System.Guid.NewGuid()).FirstOrDefault();
        BagModel m = new BagModel()
        {
            typeBag = bagType,
            items = new List<BagCardModel>()
            {
                new BagCardModel()
                {
                    earnType = BagCardModel.EarnType.Cue,
                    equipmentConfig = StatManager.Instance.GetDatasByKind_Simple(StatManager.Kind.NotUnlocked).GetRandom(),
                    isWatch = false,
                },
                new BagCardModel()
                {
                    earnType = BagCardModel.EarnType.Cue,
                    equipmentConfig = StatManager.Instance.GetDatasByKind_Simple(StatManager.Kind.NotUnlocked).GetRandom(),
                    isWatch = false,
                },
                new BagCardModel()
                {
                    earnType = BagCardModel.EarnType.Cue,
                    equipmentConfig = StatManager.Instance.GetDatasByKind_Simple(StatManager.Kind.NotUnlocked).GetRandom(),
                    isWatch = false,
                },
                new BagCardModel()
                {
                    earnType = BagCardModel.EarnType.Cue,
                    equipmentConfig = StatManager.Instance.GetDatasByKind_Simple(StatManager.Kind.NotUnlocked).GetRandom(),
                    isWatch = false,
                },
                new BagCardModel()
                {
                    earnType = BagCardModel.EarnType.Cue,
                    equipmentConfig = StatManager.Instance.GetDatasByKind_Simple(StatManager.Kind.NotUnlocked).GetRandom(),
                    isWatch = true,
                },
                new BagCardModel()
                {
                    earnType = BagCardModel.EarnType.Cue,
                    equipmentConfig = StatManager.Instance.GetDatasByKind_Simple(StatManager.Kind.NotUnlocked).GetRandom(),
                    isWatch = true,
                },
                new BagCardModel()
                {
                    earnType = BagCardModel.EarnType.Cue,
                    equipmentConfig = StatManager.Instance.GetDatasByKind_Simple(StatManager.Kind.NotUnlocked).GetRandom(),
                    isWatch = true,
                },
                new BagCardModel()
                {
                    earnType = BagCardModel.EarnType.Cue,
                    equipmentConfig = StatManager.Instance.GetDatasByKind_Simple(StatManager.Kind.NotUnlocked).GetRandom(),
                    isWatch = true,
                },
            }
        };

        m.items.Shuffle();
        
        Invoker.Invoke(() =>
        {
            GameManager.Instance.OnShowDialogWithSorting<OpenBagDialog>(
                "Home/GUI/Dialogs/OpenBag/OpenBag",
                PopupSortingType.OnTopBar,
                m);
        }, 0.5f);
    }
    
    
#endif
}
