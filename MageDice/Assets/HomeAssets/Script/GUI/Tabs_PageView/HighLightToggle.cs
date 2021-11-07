using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HighLightToggle : MonoBehaviour
{
    public Toggle toggle;

    public GameObject goHightLight;

    private void OnEnable()
    {
        this.toggle.onValueChanged.AddListener(ChangeToggle);
    }

    private void OnDisable()
    {
        this.toggle.onValueChanged.RemoveListener(ChangeToggle);
    }

    private void ChangeToggle(bool isOn)
    {
        if (isOn) this.transform.SetAsLastSibling(); //hiển thị ở phía trên
        this.goHightLight.gameObject.SetActive(isOn);
    }
}
