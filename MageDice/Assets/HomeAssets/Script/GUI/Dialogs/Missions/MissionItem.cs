using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class MissionItem : MonoBehaviour
{
    public RectTransform panelDoing;
    public RectTransform panelDone;
    public List<TextMeshProUGUI> txtNames;
    public TextMeshProUGUI txtDescription;
    public Image imgProgress;
    public TextMeshProUGUI txtProgress;

    [Header("Reward")]
    public MissionRewardItem rewardBooster;
    public MissionRewardItem rewardPointBag;

    [Header("GameObject")]
    public GameObject goProMission;
    public GameObject panelLock;
    public Button btCollect;

    [Header("Super pro")]
    public Button btBuySuperPro;

    private bool isUnlockSuperPro; //cần unlock super pro
    private MissionData data;
    public MissionData Data => this.data;
    private UnityAction<MissionItem> onClickCallback;
    private RectTransform _rectTransform => this.transform as RectTransform;

    private void OnDisable()
    {
        if (this.isUnlockSuperPro)
            BattlepassDatas.callbackBuyBattlePass -= this.OnBuyBattlePass;
    }

    private void OnBuyBattlePass(bool isBuyProPass)
    {
        bool isUptoSuperPro = data.isProMission && BattlepassDatas.Instance.GetVipType() != BattlepassDatas.VipType.VipFull;
        this.btBuySuperPro.gameObject.SetActive(isUptoSuperPro);

        this.panelDoing.gameObject.SetActive(this.data.status != MissionData.MissionStatus.DONE);
        this.panelDone.gameObject.SetActive(this.data.status == MissionData.MissionStatus.DONE);
    }

    public void ParseData(MissionData data, UnityAction<MissionItem> onClick)
    {
        this.data = data;
        this.onClickCallback = onClick;
        if (this.data != null)
        {
            this.goProMission.gameObject.SetActive(this.data.isProMission);
            
            foreach (var t in this.txtNames)
            {
                t.text = string.Format("{0}", this.data.GetName());
            }
            this.panelDoing.gameObject.SetActive(this.data.status != MissionData.MissionStatus.DONE);
            this.panelDone.gameObject.SetActive(this.data.status == MissionData.MissionStatus.DONE);
            if (this.data.status == MissionData.MissionStatus.DONE)
            {
                this._rectTransform.sizeDelta = new Vector2(this._rectTransform.sizeDelta.x, 50);
            }
            else
            {
                this._rectTransform.sizeDelta = new Vector2(this._rectTransform.sizeDelta.x, 150);
                this.txtDescription.text = string.Format("{0}", this.data.GetDescription());
                this.imgProgress.fillAmount = this.data.GetProgressFill();

                this.isUnlockSuperPro = data.isProMission && BattlepassDatas.Instance.GetVipType() != BattlepassDatas.VipType.VipFull;
                this.btBuySuperPro.gameObject.SetActive(this.isUnlockSuperPro);
                if (!this.isUnlockSuperPro)
                {
                    this.btCollect.gameObject.SetActive(this.data.IsComplete());
                }
                else
                {
                    this.btCollect.gameObject.SetActive(false);
                    BattlepassDatas.callbackBuyBattlePass += this.OnBuyBattlePass;
                }

                this.rewardBooster.ParseData(this.data.reward);
                this.rewardPointBag.ParseValueText(this.data.stepAddpass);

                this.panelLock.SetActive(false);
                
                if (this.data.IsComplete())
                {
                    this.txtProgress.text = LanguageManager.GetString("MISSION_PROGRESS_COLLECT", LanguageCategory.MissionPass);
                    this.imgProgress.color = Color.green;
                    this.imgProgress.fillAmount = 1f;
                }
                else
                {
                    this.txtProgress.text = string.Format("{0}", this.data.GetProgress());
                    this.imgProgress.color = Color.blue;
                }
            }
        }
    }

    public void ClickReward()
    {
        Debug.Log("Click reward mission");
        if (this.data != null)
        {
            if (this.data.IsComplete())
            {
                MissionData temp =MissionDatas.Instance.RewardMission(this.data.id);
                if (temp != null)
                {
                    this.btCollect.interactable = false;
                    this.onClickCallback?.Invoke(this);
                    FxHelper.Instance.ShowFxCollectBooster(this.Data.reward, this.btCollect.transform);
                    this.StartCoroutine(this.OnWaitingReward(temp));
                }
            }
        }
    }

    private IEnumerator OnWaitingReward(MissionData temp)
    {
        yield return new WaitForSeconds(1.0f);
        this.ParseData(temp, null);
    }
}
