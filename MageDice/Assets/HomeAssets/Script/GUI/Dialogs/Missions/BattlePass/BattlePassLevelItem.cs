using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class BattlePassLevelItem : MonoBehaviour
{
    [Header("Panel left")]
    public Transform panel;
    public GameObject panelReward;
    public Transform panelLeft;
    public Transform panelRight;

    [Header("Prefab")]
    public BattlePassBoosterItem boosterItemPrefab;
    public BattlePassBagItem bagItemPrefab;

    [Header("Current")]
    public GameObject goCurrent;

    private bool onShow;
    private BattlepassStepData stepData;
    private BattlepassData battlePass;
    private List<BattlePassItem> items;
    UnityAction<BattlepassStepData.StepReward, Transform> callbackPreview;

    private enum BattlePassGiftType { BOOSTERITEM, BAGITEM };
    private Dictionary<BattlePassGiftType, BattlePassItem> dicLeftItems;
    private Dictionary<BattlePassGiftType, BattlePassItem> dicRightItems;

    private Coroutine coroutineParseData;

    private void OnEnable()
    {
        BattlepassDatas.callbackReward += this.OnCollectReward;
    }

    private void OnDisable()
    {
        BattlepassDatas.callbackReward -= this.OnCollectReward;
        if (this.coroutineParseData != null)
        {
            this.StopCoroutine(this.coroutineParseData);
            this.coroutineParseData = null;
        }
    }
    private void OnCollectReward(BattlepassStepData stepData)
    {
        ParseData(this.battlePass, this.stepData, callbackPreview);
    }

    public void ParseData(BattlepassData battlePass, BattlepassStepData stepData, UnityAction<BattlepassStepData.StepReward, Transform> callbackPreview)
    {
        this.callbackPreview = callbackPreview;

        if (this.gameObject.activeInHierarchy && battlePass != null && stepData != null)
        {
            this.coroutineParseData = StartCoroutine(IeParseData(battlePass, stepData, callbackPreview));
        }
    }

    private IEnumerator IeParseData(BattlepassData battlePass, BattlepassStepData stepData, UnityAction<BattlepassStepData.StepReward, Transform> callbackPreview)
    {
        Clear();
        yield return new WaitForEndOfFrame();

        this.items = new List<BattlePassItem>();
        if (this.dicLeftItems == null) this.dicLeftItems = new Dictionary<BattlePassGiftType, BattlePassItem>();
        if (this.dicRightItems == null) this.dicRightItems = new Dictionary<BattlePassGiftType, BattlePassItem>();


        this.battlePass = battlePass;
        this.stepData = stepData;

        if (this.battlePass != null && this.stepData != null)
        {
            bool isCurrentStep = this.battlePass.IsCurrentStep(this.stepData);
            this.goCurrent?.SetActive(isCurrentStep);

            bool isAllReward = this.stepData.IsAllReward();
            this.panelReward?.SetActive(isAllReward);

            bool isComplete = this.stepData.IsComplete();
            //Debug.Log(string.Format("<color=green>Step :</color> {0}, is compelete: {1}", stepData.id, isComplete));
            bool isUnlock = this.battlePass.IsUnlockStep(this.stepData);


            //parse item left
            //show panel reward không?
            bool isShowRewardFree = !this.stepData.freeReward.isReward && isComplete
                && !isAllReward && isUnlock;
            ParseItem(this.stepData.freeReward, false, isUnlock, isShowRewardFree, callbackPreview);

            //parse item right
            //show panel reward không?
            bool isShowRewardPro = !this.stepData.proReward.isReward && isComplete
                && !isAllReward && BattlepassDatas.Instance.IsProPass() && isUnlock;
            ParseItem(this.stepData.proReward, true, BattlepassDatas.Instance.IsProPass() && isUnlock, isShowRewardPro, callbackPreview);
        }
    }


    private BattlePassBagItem GetBagItem(bool isRight)
    {
        Transform tranParent = isRight ? this.panelRight : this.panelLeft; //pro item luôn nằm ở bên phải
        Dictionary<BattlePassGiftType, BattlePassItem> dicItems = isRight ? this.dicRightItems : this.dicLeftItems;
        if (dicItems.ContainsKey(BattlePassGiftType.BAGITEM))
        {
            return dicItems[BattlePassGiftType.BAGITEM] as BattlePassBagItem;
        }
        BattlePassBagItem bagItem = Instantiate(this.bagItemPrefab, tranParent);
        dicItems.Add(BattlePassGiftType.BAGITEM, bagItem);
        return bagItem;
    }

    private BattlePassBoosterItem GetBoosterItem(bool isRight)
    {
        Transform tranParent = isRight ? this.panelRight : this.panelLeft; //pro item luôn nằm ở bên phải
        Dictionary<BattlePassGiftType, BattlePassItem> dicItems = isRight ? this.dicRightItems : this.dicLeftItems;
        if (dicItems.ContainsKey(BattlePassGiftType.BOOSTERITEM))
        {
            return dicItems[BattlePassGiftType.BOOSTERITEM] as BattlePassBoosterItem;
        }
        BattlePassBoosterItem bagItem = Instantiate(this.boosterItemPrefab, tranParent);
        dicItems.Add(BattlePassGiftType.BOOSTERITEM, bagItem);
        return bagItem;
    }

    private void ParseItem(BattlepassStepData.StepReward reward, bool isProItem, bool isUnlock, bool showReward, UnityAction<BattlepassStepData.StepReward, Transform> callbackPreview)
    {
        if (reward.IsRewardBag())
        {
            BattlePassBagItem bagItem = GetBagItem(isRight: isProItem);
            bagItem.gameObject.SetActive(true);
            bagItem.ParseData(this.stepData, reward, isProItem, isUnlock);
            bagItem.SetReward(showReward);
            bagItem.SetCallbackPreview(callbackPreview);
            this.items.Add(bagItem);
        }
        else
        {
            BattlePassBoosterItem boosterItem = GetBoosterItem(isRight: isProItem);
            boosterItem.gameObject.SetActive(true);
            boosterItem.ParseData(this.stepData, reward, isProItem, isUnlock);
            boosterItem.SetReward(showReward);
            boosterItem.SetCallbackPreview(callbackPreview);
            this.items.Add(boosterItem);
        }
    }

    private void Clear()
    {
        if (this.dicLeftItems != null)
        {
            foreach (var item in this.dicLeftItems)
            {
                item.Value.gameObject.SetActive(false);
            }
        }
        if (this.dicRightItems != null)
        {
            foreach (var item in this.dicRightItems)
            {
                item.Value.gameObject.SetActive(false);
            }
        }
    }
}
