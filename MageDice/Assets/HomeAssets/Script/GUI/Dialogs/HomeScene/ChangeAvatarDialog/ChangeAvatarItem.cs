using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class ChangeAvatarItem : MonoBehaviour
{
    private string id;
    public Image imgAvatar;
    public Toggle tgAvatar;

    private UnityAction<string> callback;

    public void ParseAvatar(AvatarItem avatar, UnityAction<string> callback)
    {
        if (avatar != null)
        {
            this.id = avatar.avatarID;
            if (avatar.img != null) this.imgAvatar.sprite = avatar.img;
        }
        this.callback = callback;
    }

    public void OnChooseAvatar()
    {
        this.tgAvatar.isOn = true;
    }

    public void OnChangeAvatar()
    {
        if (this.tgAvatar.isOn)
            this.callback?.Invoke(id);
        SoundManager.Instance.PlayButtonClick();
    }
}
