using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class MissionDialogs : BaseSortingDialog
{
    public TabBase tab;

    [Header("Tabs")]
    public BattlePassTab battlePassTab;
    public MissionTab missionTab;

    [Header("Contents")]
    public BattlePassTabContent battlePassTabContent;
    public MissionTabContent missionTabContent;

    [Header("Battle pass")]
    public BattlePassLevelProgress bpLevelProgress;
    public TextMeshProUGUI tmpTimeBp;

    [Header("Layout")]
    public Image imgTop;
    public Image imgContent;

    private double timeRemain;
    private bool isBtpOutDate; //battle pass out date

    private void Start()
    {
        this.tab.Init();
    }
    public override void OnShow(object data = null, UnityAction callback = null)
    {
        base.OnShow(data, callback);
        ParseLayout();
    }

    public void ChangeTab(int tabIndex)
    {
        this.tab.ChangeTab(tabIndex);
    }

    private void ParseLayout()
    {
        int idAsset = BattlepassDatas.Instance.GetIdAssetActiveBattlePass();
        Debug.Log("<color=yellow>Parse bp layout </color>" + idAsset);

        BattlePassAssetConfig battlePassAsset = BattlePassAssetConfigs.Instance.GetBPAsset(idAsset);
        if (battlePassAsset != null)
        {
            this.imgTop.color = battlePassAsset.color2;
            this.imgContent.color = battlePassAsset.color3;

            this.battlePassTab.ParseLayout(battlePassAsset);
            this.missionTab.ParseLayout(battlePassAsset);

            this.battlePassTabContent.ParseLayout(battlePassAsset);

            this.bpLevelProgress.ParseLayout(battlePassAsset);
        }
    }

    private void Update()
    {
        if (this.isBtpOutDate) return;

        if (BattlepassDatas.Instance.IsOutDate(ref this.timeRemain))
        {
            ShowOutDateBattlepass();
        }
        this.tmpTimeBp.text = GameUtils.ConvertFloatToTime(this.timeRemain, "dd'd'hh'h'");
    }


    private void ParseNewBattlePass()
    {
        Debug.Log("<color=yellow>Start new BattlePass</color>");

        //TODO parse new battle pass
        BattlepassDatas.Instance.NextBattlePassData();

        //pares lại data battle pass
        this.battlePassTabContent.ParseData();

        this.isBtpOutDate = false;

        ParseLayout();
    }

    [ContextMenu("Test out date battle pass")]
    private void ShowOutDateBattlepass()
    {
        this.isBtpOutDate = true;

        //TODO show reset battle pass
        MessageBox.Instance.ShowMessageBox("BattlePass is over", "Start new battlePass");
        MessageBox.Instance.SetButtonOk("OK");
        MessageBox.Instance.SetEvent(ParseNewBattlePass);
    }

}
