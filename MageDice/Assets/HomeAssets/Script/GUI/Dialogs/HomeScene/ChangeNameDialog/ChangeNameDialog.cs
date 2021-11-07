using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class ChangeNameDialog : BaseSortingDialog
{
    public TMP_InputField infName;
    public Button btSave;
    public Image imgNoSave;

    private string oldName = "";
    private bool canSave;

    private void OnEnable()
    {
        this.infName.onValueChanged.AddListener(OnChangeName);
    }

    private void OnDisable()
    {
        this.infName.onValueChanged.RemoveListener(OnChangeName);
    }

    private void OnChangeName(string name)
    {
        if (name != null)
        {
            canSave = name.Length >= 3 && name.Length <= 10 && !this.infName.text.Equals(this.oldName);
            this.btSave.interactable = canSave;
            this.imgNoSave.gameObject.SetActive(!canSave);
        }
    }

    public override void OnShow(object data = null, UnityAction callback = null)
    {
        base.OnShow(data, callback);

        //get name player
        this.oldName = UserDatas.Instance.info.nickname;
        this.infName.SetTextWithoutNotify(UserDatas.Instance.info.nickname);
        OnChangeName(oldName);
    }

    public void OnClickSave()
    {
        UserDatas.Instance.info.ChangeName(this.infName.text);
        OnCloseDialog();
        SoundManager.Instance.PlayButtonClick();
    }
}
