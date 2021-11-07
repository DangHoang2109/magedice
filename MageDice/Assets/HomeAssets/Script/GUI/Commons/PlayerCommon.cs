using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class PlayerCommon : MonoBehaviour
{
    public Image imgAvatar;
    public TextMeshProUGUI txtNickname;
    
    protected PlayerModel model;

    public virtual void ParseData(PlayerModel model)
    {
        this.model = model;
        this.SetNickname(this.model.info.nickname);   
        this.SetAvatar(this.model.info.SprAvatar);
    }

    public virtual void SetNickname(string nickname)
    {
        this.txtNickname.text = string.Format("{0}", nickname);
    }

    public virtual void SetAvatar(Sprite spr)
    {
        this.imgAvatar.sprite = spr;
    }
    public virtual void SetAvatar(string avatar)
    {
        Sprite sprAvatar = CommonAvatar.Instance.GetAvatarById(avatar);
        if (sprAvatar != null)
            this.imgAvatar.sprite = sprAvatar;
    }

}
