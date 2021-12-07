using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class BattlePassItem : MonoBehaviour
{
    
    public Image imgIcon;

    public Transform tranPreview;
    public GameObject panelReward;
    public GameObject panelLock;
    public GameObject panelRewarded;
    protected BattlepassStepData stepDatas;
    protected BattlepassStepData.StepReward stepReward;
    public bool isBattlePassPro;
    protected bool isUnlock;
    public BattlepassStepData.StepReward StepReward=> this.stepReward;


    private UnityAction<BattlepassStepData.StepReward, Transform> callbackPreview;

    public void SetCallbackPreview(UnityAction<BattlepassStepData.StepReward, Transform> callback)
    {
        this.callbackPreview = callback;
    }


    private void OnEnable()
    {
        BattlepassDatas.callbackBuyBattlePass += this.OnBuyBattlePass;

    }

    private void OnDisable()
    {
        BattlepassDatas.callbackBuyBattlePass -= this.OnBuyBattlePass;
    }

    private void OnBuyBattlePass(bool isBuyProPass)
    {
        this.isUnlock = isBuyProPass;
        this.panelLock.SetActive(!isBuyProPass);
    }
    public virtual void ParseData(BattlepassStepData stepDatas, BattlepassStepData.StepReward datas, bool isBattlePro, bool isUnlock)
    {
        this.stepDatas = stepDatas;
        this.stepReward = datas;
        this.isUnlock = isUnlock;
        this.isBattlePassPro = isBattlePro;
        this.panelLock.SetActive(!this.isUnlock);
        this.UpdateData(datas);
    }

    public void SetReward(bool isReward)
    {
        this.panelReward.SetActive(isReward);
    }

    public void UpdateData(BattlepassStepData.StepReward datas)
    {
        this.panelReward.SetActive(false);
        this.panelRewarded.SetActive(false);
        if (this.stepDatas.IsComplete() && this.isUnlock)
        {
            if (datas.isReward)
            {
                this.panelRewarded.SetActive(true);
            }
        }
    }

    public virtual void Claimed()
    {
        
    }

    public virtual void ClickClaim()
    {
        Debug.Log("Click claim");

        if (!this.isUnlock)
        {
            Debug.LogError("CAN NOT CLAIM");

            //TODO show preview reward
            OnCallbackPreview();
            return;
        }

       // this.stepDatas = this.stepDatas.GetReward(this.isBattlePassPro);

       if (this.isBattlePassPro == false)
       {
            //chưa reward mới đc nhận
            if (!this.stepDatas.IsFreeRewardedOrCompleted())
            {
                //free
                CollectReward(this.stepDatas.freeReward, false);
                this.stepDatas = this.stepDatas.GetFreeReward();
                //show Fx parse data
                this.ParseData(this.stepDatas, this.stepDatas.freeReward, this.isBattlePassPro, this.isUnlock);
            }
            else
            {
                OnCallbackPreview();
            }    
       }
       else
       {
            //chưa reward pro thì mới mới đc nhận
            if (!this.stepDatas.IsProRewardedOrCompleted())
            {
                //pro
                CollectReward(this.stepDatas.proReward, true);
                this.stepDatas = this.stepDatas.GetProReward();
                //show fx
                this.ParseData(this.stepDatas, this.stepDatas.proReward, this.isBattlePassPro, this.isUnlock);
            }
            else
            {
                OnCallbackPreview();
            }            
       }
    }

    private void OnCallbackPreview()
    {
        this.callbackPreview?.Invoke(this.isBattlePassPro == false ? this.stepDatas.freeReward : this.stepDatas.proReward, this.transform);
    }

    /// <summary>
    /// Code step reward
    /// </summary>
    /// <param name="stepReward"></param>
    /// <param name="isPro"></param>
    private void CollectReward(BattlepassStepData.StepReward stepReward ,bool isPro)
    {
        string where = isPro ? LogSourceWhere.BATTLE_PASS_PRO : LogSourceWhere.BATTLE_PASS_FREE;
        if (stepReward.IsRewardBag())
        {
            MainBagSlots.OpenBagNow(new List<BagAmount>() { stepReward.GetBagData() }, where);
            Debug.Log(string.Format("Reward from Pass {0} : {1} {2} tour {3}", this.stepDatas.id, stepReward.bag.amount, stepReward.bag.bagType, stepReward.bag.tour));
        }
        else
        {
            BoosterCommodity booster = stepReward.GetBoosterData();
            UserProfile.Instance.AddBooster(
                booster: booster,
                from: string.Format("Reward from Battle Pass {0}", this.stepDatas.id),
                where: where);
            //fx reward booster
            FxHelper.Instance.ShowFxCollectBooster(booster, this.transform);
            Debug.Log(string.Format("Reward from Pass {0} : {1} {2}", this.stepDatas.id, booster.type, booster.GetValue()));
        }
    }
}
