using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ProPassButtonIcon : BaseIcon
{
    [Header("Button")]
    public Button button;

    public TextMeshProUGUI tmpProPass;

    protected override void OnEnable()
    {
        base.OnEnable();
        //BattlepassDatas.callbackBuyBattlePass += this.OnBuyBattlePass;
    }

    protected override void OnDisable()
    {
        base.OnDisable();
        //BattlepassDatas.callbackBuyBattlePass -= this.OnBuyBattlePass;
    }

    private void Start()
    {
        //OnBuyBattlePass(BattlepassDatas.Instance.IsProPass());
    }

    private void OnBuyBattlePass(bool isBuyProPass)
    {
        //switch (BattlepassDatas.Instance.GetVipType())
        //{
        //    case BattlepassDatas.VipType.None:
        //        this.button.interactable = true;
        //        this.tmpProPass.text = LanguageManager.GetString("PASS_UNLOCKPROBTN", LanguageCategory.MissionPass);
        //        break;
        //    case BattlepassDatas.VipType.Vip:
        //        this.button.interactable = true;
        //        this.tmpProPass.text = LanguageManager.GetString("PASS_UNLOCKFULLPROBTN", LanguageCategory.MissionPass);
        //        break;
        //    case BattlepassDatas.VipType.VipFull:
        //        this.button.interactable = false;
        //        this.tmpProPass.text = LanguageManager.GetString("PASS_UNLOCKEDFULLBTN", LanguageCategory.MissionPass);
        //        break;
        //}
    }

    public override void OnClickIcon()
    {
        base.OnClickIcon();
        //Hiện battle pass
        //BuyBattlePassDialogV2 dialog = GameManager.Instance.OnShowDialogWithSorting<BuyBattlePassDialogV2>("Home/GUI/Dialogs/BattlePass/BuyBattlePassDialogV2",
        //    PopupSortingType.OnTopBar);

        //Không hiện battle pass
        //Notification.Instance.ShowNotificationIcon(LanguageManager.GetString("TITLE_COOMINGSOON"));
    }
}
