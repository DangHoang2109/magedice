using Coffee.UIEffects;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MainClaimRewardIcon : BaseIcon
{
    [Header("Texts")]
    //public GameObject goCommingSoon;
    public Transform tranText;

    //[Header("Sprite")]
    //public Image imgBt;
    //public Sprite sprBtGray;
    //public Sprite sprBtOrange;

    //[Header("Progress")]
    //public BattlePassLevelProgress progress;

    [Header("Highlight")]
    public UIShiny uiHightlight;


    private void Start()
    {
        //Hiển thị battle pass
        //this.goCommingSoon.SetActive(false);
        //this.tranText.localPosition = new Vector3(this.tranText.localPosition.x, 2f);
        //this.imgBt.sprite = this.sprBtOrange;

        CheckShowHighlight();

        #region OLD
        //Không hiển thị battle pass
        //this.goCommingSoon.SetActive(true);
        //this.tranText.localPosition = new Vector3(this.tranText.localPosition.x, 7f);
        //this.imgBt.sprite = this.sprBtGray;
        #endregion

    }

    protected override void OnEnable()
    {
        base.OnEnable();

        //TODO callback highlight

        //callback battlepass
        //BattlepassDatas.callbackProgress += this.DoProgress;
        //BattlepassDatas.callbackReward += this.OnRewardBattlePass;
        //BattlepassDatas.callbackBuyBattlePass += this.OnBuyBattlePass;
        //if (MissionDatas.Instance != null)
        //    MissionDatas.Instance.callbackCompleteMission += CompleteMission;
    }

    protected override void OnDisable()
    {
        base.OnDisable();

        //TODO remove callback highlight

        //callback battlepass
        //BattlepassDatas.callbackProgress -= this.DoProgress;
        //BattlepassDatas.callbackReward -= this.OnRewardBattlePass;
        //BattlepassDatas.callbackBuyBattlePass -= this.OnBuyBattlePass;
        //if (MissionDatas.Instance!=null)
        //    MissionDatas.Instance.callbackCompleteMission -= CompleteMission;
    }

    //private void OnRewardBattlePass(BattlepassStepData step)
    //{
    //    CheckShowHighlight();
    //}

    //private void DoProgress(BattlepassStepData stepData, int level)
    //{
    //    CheckShowHighlight();
    //}

    private void OnBuyBattlePass(bool isBuyProPass)
    {
        CheckShowHighlight();
    }

    private void CompleteMission()
    {
        CheckShowHighlight();
    }

    private void CheckShowHighlight()
    {
        //bool canReward = BattlepassDatas.Instance.GetCountBattlePassesCanReward() > 0 || MissionDatas.Instance.GetCountMissionCanReward() > 0;
        //OnHighlight(canReward);
        //this.progress.gameObject.SetActive(!canReward);
        //this.tranText.localPosition = new Vector3(this.tranText.localPosition.x, canReward ? 4f : 12f);

        ////parse lại data progress
        //if (!canReward)
        //{
        //    BattlepassData battlePass = BattlepassDatas.Instance.activePass;
        //    this.progress.ParseData(battlePass.CurrentStep, battlePass.CurrentIndex);
        //}
    }


    public void OnHighlight(bool highlight)
    {
        this.uiHightlight.enabled = highlight;
    }

    public override void OnClickIcon()
    {
        base.OnClickIcon();
        //MissionDialogs dialog = GameManager.Instance.OnShowDialogWithSorting<MissionDialogs>("Home/GUI/Dialogs/Missions/MissionDialogs",
        //    PopupSortingType.CenterBottomAndTopBar);
        //dialog?.ChangeTab(0);

        //Coming soon
        //Notification.Instance.ShowNotificationIcon(LanguageManager.GetString("TITLE_COOMINGSOON"));
    }
}
