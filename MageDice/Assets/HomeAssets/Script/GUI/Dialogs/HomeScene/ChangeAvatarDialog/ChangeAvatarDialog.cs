using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class ChangeAvatarDialog : BaseSortingDialog
{
    public ChangeAvatarItem changeAvatarPrefab;
    public Transform tranItems;

    public Button btSave;
    public Transform tranBtGrey;

    private Dictionary<string,ChangeAvatarItem> dicItems;
    private string idAvatar;
    public override void OnShow(object data = null, UnityAction callback = null)
    {
        base.OnShow(data, callback);
        ParseConfig();
    }

    public void ParseConfig()
    {
        //load parse all avatar
        if (this.dicItems == null)
        {
            this.dicItems = new Dictionary<string, ChangeAvatarItem>();
            foreach (AvatarItem avatar in CommonAvatar.Instance.avatars)
            {
                ChangeAvatarItem item = Instantiate<ChangeAvatarItem>(this.changeAvatarPrefab, this.tranItems);
                item.gameObject.SetActive(true);
                item.ParseAvatar(avatar, OnChangeAvatar);
                this.dicItems.Add(avatar.avatarID, item);
            }
        }

        this.idAvatar = UserDatas.Instance.info.Avatar;
        if (this.dicItems.ContainsKey(idAvatar))
        {
            this.dicItems[idAvatar].OnChooseAvatar();
        }
    }

    private void OnChangeAvatar(string id)
    {
        this.idAvatar = id;
        this.btSave.interactable = this.idAvatar != UserDatas.Instance.info.Avatar;
        this.tranBtGrey.gameObject.SetActive(this.idAvatar == UserDatas.Instance.info.Avatar);
    }

    public void OnClickSave()
    {
        UserDatas.Instance.info.ChangeAvatar(this.idAvatar);
        OnCloseDialog();
        SoundManager.Instance.PlayButtonClick();
    }
}
