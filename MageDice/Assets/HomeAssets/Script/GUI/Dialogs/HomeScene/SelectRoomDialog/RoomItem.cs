using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class RoomItem : MonoBehaviour
{
    [Header("Room images")]
    public Image imgIcon;
    public Image imgBg;
    public Image imgGradient;
    public Image imgMask;
    public TextMeshProUGUI tmpName;

    [Header("Button")]
    public Transform tranBtPlay;

    protected UnityAction<int> callback;
    protected RoomConfig config;
    protected RoomData data;

    public int RoomId;

    //parse asset config
    public void ParseConfig(RoomConfig config, RoomData data, UnityAction<int> callback = null)
    {
        if (config != null)
        {
            this.config = config;
            this.callback = callback;
            this.RoomId = config.id;
            this.data = data;
            ParseData(data);
        }
    }

    public void ReloadRoom()
    {
        Debug.Log("reload room");
        ParseConfig(this.config, this.data);
    }

    protected virtual void ParseData(RoomData roomData)
    {
        if (roomData != null)
        {
            RoomAssetConfig roomAsset = RoomAssetConfigs.Instance.GetRoomAsset(roomData.id);

            if (roomAsset == null)
                return;

            this.imgIcon.sprite = roomAsset.sprIcons[roomData.unlocked ? 1 : 0]; //kiểm tra đã unlock chưa
            this.imgBg.color = roomData.unlocked ? roomAsset.colorRoom : Color.gray;
            this.imgGradient.color = roomData.unlocked ? roomAsset.colorRoom : Color.gray;
            this.imgMask.sprite = roomAsset.sprMask;
        }
    }

    public virtual void ShowEffectUnlock(UnityAction callback = null)
    {
        
    }

    #region Action
    public virtual void OnClickPreview()
    {
        //Debug.LogError("Click preview");
        SoundManager.Instance.PlayButtonClick();
    }

    public virtual void OnClickInfo()
    {
        //Debug.LogError("Click info");
        SoundManager.Instance.PlayButtonClick();
    }

    public virtual void OnClickPlay()
    {
        //Debug.LogError("Click play");
        this.callback?.Invoke(this.config.id);
        SoundManager.Instance.PlayButtonClick();
    }
    #endregion
}
