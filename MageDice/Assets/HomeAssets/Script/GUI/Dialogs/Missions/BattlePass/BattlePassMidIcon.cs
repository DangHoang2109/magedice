using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class BattlePassMidIcon : MonoBehaviour
{
    [Header("Animation")]
    public Animator animCurrent;

    public GameObject goCurrent;
    public TextMeshProUGUI tmpNumber;

    public void ShowNumber(int number)
    {
        this.tmpNumber.text = number.ToString();
    }

    public void ShowCurrent(bool isCurrent = true)
    {
        this.animCurrent.SetBool("Current", isCurrent);
        this.tmpNumber.gameObject.SetActive(!isCurrent);
        this.goCurrent.gameObject.SetActive(isCurrent);
    }
}
