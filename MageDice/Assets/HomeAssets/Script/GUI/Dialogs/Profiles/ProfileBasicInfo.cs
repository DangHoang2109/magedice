using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class ProfileBasicInfo : PlayerCommon
{
    public TextMeshProUGUI txtPlayerId;
    public TextCurrencyResize txtTrophies;
    public TextCurrencyResize txtCoinEarned;
    public TextCurrencyResize txtCoin;


    public override void ParseData(PlayerModel model)
    {
        base.ParseData(model);
    }

    public void ParseData(PlayerModel model, long trophies, long coinEarned, long coin)
    {
        this.ParseData(model);
        this.txtPlayerId.text = string.Format(LanguageManager.GetString("PROFILE_PLAYERID", LanguageCategory.Feature), this.model.info.id);
        this.txtCoin.ParseData(coin);
        this.txtTrophies.ParseData(trophies);
        this.txtCoinEarned.ParseData(coinEarned);
    }

    private void ReloadName()
    {
        this.SetNickname(UserDatas.Instance.info.nickname);
    }

    private void ReloadAvatar()
    {
        this.SetAvatar(UserDatas.Instance.info.SprAvatar);
    }

    public void OnClickChangeAvatar()
    {
        ChangeAvatarDialog changeAvatar = GameManager.Instance.OnShowDialogWithSorting<ChangeAvatarDialog>("Home/GUI/Dialogs/HomeScene/ChangeAvatar/ChangeAvatarDialog", PopupSortingType.OnTopBar);
        changeAvatar.OnClosed += ReloadAvatar;
        SoundManager.Instance.PlayButtonClick();
    }

    public void OnClickChangeName()
    {
        ChangeNameDialog changeName = GameManager.Instance.OnShowDialogWithSorting<ChangeNameDialog>("Home/GUI/Dialogs/HomeScene/ChangeName/ChangeNameDialog", PopupSortingType.OnTopBar);
        changeName.OnClosed += ReloadName;
        SoundManager.Instance.PlayButtonClick();
    }
}
