using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
public class ScreenLogger : MonoSingleton<ScreenLogger>
{
    public TextMeshProUGUI txtLog;

    public void Log(string mss)
    {
        if(string.IsNullOrEmpty(txtLog.text) || string.IsNullOrWhiteSpace(txtLog.text))
            txtLog.SetText(string.Format("<color=red>{0}</color>", mss));
        else
            txtLog.SetText(string.Format("<color=blue>{0}</color> \n <color=red>{1}</color>", txtLog.text.Replace("<color=red>", "").Replace("<color=blue>", ""), mss));
    }
}
