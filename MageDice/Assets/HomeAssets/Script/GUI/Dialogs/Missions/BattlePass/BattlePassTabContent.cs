using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class BattlePassTabContent : TabContent
{
    [Header("Battle pass")]
    public BattlePassProgress progress;
    public RectTransform panelFree;
    public RectTransform panelPro;

    public BattlePassBagItem bagItemPrefab;
    public BattlePassBoosterItem boosterItemPrefab;

    [Header("Battle pass level")]
    public Transform panelLevel;
    public BattlePassLevelItem levelPrefab;
    public BattlePassLevelProgress leveProgress;

    //[Header("Bubble preview bag")]
    //public BagPreviewBubble bagPreview;

    private List<BattlePassLevelItem> items;

    [Header("Mid icon")]
    public BattlePassMidIcon[] midIcons;

    [Header("Layout")]
    public Image imgTopBg;
    public Image imgDownBg;


    private bool onShow;
    private Coroutine coroutineParseItems;

#if UNITY_EDITOR
    private void OnValidate()
    {
        this.midIcons = this.GetComponentsInChildren<BattlePassMidIcon>();
    }
#endif
    protected override void OnEnable()
    {
        base.OnEnable();
        BattlepassDatas.callbackReward += this.OnRewardBattlePass;
        BattlepassDatas.callbackProgress += this.DoProgress;
        BattlepassDatas.callbackBuyBattlePass += this.OnBuyBattlePass;
    }

    protected override void OnDisable()
    {
        base.OnDisable();
        BattlepassDatas.callbackReward -= this.OnRewardBattlePass;
        BattlepassDatas.callbackProgress -= this.DoProgress;
        BattlepassDatas.callbackBuyBattlePass -= this.OnBuyBattlePass;
        if (this.coroutineParseItems != null)
        {
            StopCoroutine(this.coroutineParseItems);
            this.coroutineParseItems = null;
        }
    }

    protected override void AnimationHide()
    {
        base.AnimationHide();
        this.OnCompleteHide();
    }

    protected override void AnimationShow()
    {
        base.AnimationShow();
        OnCompleteShow();
    }


    public override void OnShow(int index, object data = null, UnityAction callback = null)
    {
        base.OnShow(index, data, callback);
        this.onShow = true;
        this.ParseData();
    }

    public override void OnHide(int index, object data = null, UnityAction callback = null)
    {
        base.OnHide(index, data, callback);
        this.onShow = false;
    }

    private void OnRewardBattlePass(BattlepassStepData step)
    {
        if (this.onShow)
        {
            BattlepassData battlePass = BattlepassDatas.Instance.activePass;
            this.progress.SetScroll(battlePass.CurrentIndexClaimStep);
            this.UpdateBackground(battlePass);
        }
    }

    private void DoProgress(BattlepassStepData stepData, int level)
    {
        if (this.onShow)
            this.ParseData();
    }
    private void OnBuyBattlePass(bool isBuyProPass)
    {
        if (this.onShow)
            this.ParseData();
    }

    private IEnumerator ParseItems(BattlepassData battlePass)
    {
        if (this.items == null)
            this.items = new List<BattlePassLevelItem>();

        for (int i = 0; i < this.items.Count; i++)
        {
            items[i].gameObject.SetActive(false);
        }

        for (int i = 0; i < battlePass.steps.Count; i++)
        {
            BattlePassLevelItem tempItem;

            //check đã có đủ số lượng chưa, có cần init thêm nữa không
            if (this.items.Count > i)
            {
                tempItem = this.items[i];
            }
            else
            {
                tempItem = Instantiate(this.levelPrefab, this.panelLevel);
                this.items.Add(tempItem);
            }

            if (tempItem != null)
            {
                tempItem.gameObject.SetActive(true);
                yield return new WaitForEndOfFrame();
                tempItem.ParseData(battlePass, battlePass.steps[i], ShowPreview);
            }
            yield return new WaitForEndOfFrame();
        }
    }

    public void ParseData()
    {
        BattlepassData battlePass = BattlepassDatas.Instance.activePass;
        if (battlePass == null)
        {
            Debug.LogError("CAN LOAD BATTLE PASSS");
            return;
        }

        if (!this.gameObject.activeInHierarchy) return;

        this.coroutineParseItems = StartCoroutine(ParseItems(battlePass));

        Debug.LogError("CURRENT LV PASS: " + battlePass.CurrentIndex + " CURRENT CLAIN: " + battlePass.CurrentIndexClaimStep);

        //parse mid icon
        for (int i = 0; i < this.midIcons.Length; i++)
        {
            this.midIcons[i].ShowNumber(i + 1);
            this.midIcons[i].ShowCurrent(i == battlePass.CurrentIndexClaimStep);
        }

        this.UpdateBackground(battlePass);

        this.leveProgress.ParseData(battlePass.CurrentStep, battlePass.CurrentIndex);

        Debug.Log("<color=yellow>Parse data </color>");
    }


    /// <summary>
    /// Hiển thị cho những thằng chưa reward hoặc đã được reward xem phần quà nó nhận là gì
    /// </summary>
    /// <param name="stepReward"> phần quà </param>
    /// <param name="transform"> vị trí show </param>
    private void ShowPreview(BattlepassStepData.StepReward stepReward, Transform tranReward)
    {
        //Debug.Log("<color=blue>Show preview</color>");

        //if (stepReward.IsRewardBag())
        //{
        //    BagAmount bagAmount = stepReward.GetBagData();
        //    //BagConfig bagConfig = BagConfigs.Instance.GetBag(bagAmount.bagType);

        //    GiftBagPerTourConfig bagConfig = GiftBagConfigs.Instance.GetGiftBag(bagAmount.bagType, bagAmount.tour);
        //    if (bagConfig != null)
        //    {
        //        this.bagPreview.ShowBubble(tranReward.position);
        //        this.bagPreview.ParseBagContains(bagConfig);

        //        if (stepReward.type == BattlePassType.PRO_PASS)
        //            this.bagPreview.ParseGemAmount(bagConfig.valueGem.max);
        //    }
        //}
    }

    private void UpdateBackground(BattlepassData battlePass)
    {
        //animation progress move
        this.progress.SetBackGround(battlePass.CurrentIndex);
        this.progress.SetScroll(battlePass.CurrentIndexClaimStep);
    }


    public void ParseLayout(BattlePassAssetConfig battlePassAsset)
    {
        this.imgTopBg.color = battlePassAsset.color1;
        this.imgDownBg.color = battlePassAsset.color3;
    }

}
