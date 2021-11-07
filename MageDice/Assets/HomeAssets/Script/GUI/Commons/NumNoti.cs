using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class NumNoti : MonoBehaviour
{
    public GameObject goNum;
    public TextMeshProUGUI txtNum;

    public void ShowNumNoti(int num)
    {
        this.txtNum.text = num.ToString();
        this.goNum.gameObject.SetActive(num > 0);
    }
}
