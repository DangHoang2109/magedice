using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class AvatarIcon : BaseIcon
{
    public Image imgAvatar;

    protected override void Init()
    {
        base.Init();
        LoadUserAvatar();
    }

    private void LoadUserAvatar()
    {
        this.imgAvatar.sprite = UserDatas.Instance.info.SprAvatar;
        /*Sprite sprAvatar = CommonAvatar.Instance.GetAvatarById(UserDatas.Instance.info.avatar);
        if (sprAvatar != null) this.imgAvatar.sprite = sprAvatar;*/
    }

    public override void OnClickIcon()
    {
        Debug.LogError("Click avatar");
        UserProfileDialog userPorfile = GameManager.Instance.OnShowDialogWithSorting<UserProfileDialog>("Home/GUI/Dialogs/Profiles/UserProfileDialog", PopupSortingType.OnTopBar);
        userPorfile.OnClosed += LoadUserAvatar;
        base.OnClickIcon();
    }
}
