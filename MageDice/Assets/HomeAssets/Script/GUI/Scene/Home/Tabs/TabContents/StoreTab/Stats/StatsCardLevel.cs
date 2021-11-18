using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class StatsCardLevel : MonoBehaviour
{
    public GameObject panel;
    public TextMeshProUGUI txtLevel;
    public TextMeshProUGUI txtProgress;
    public Image imgProgress;
    public GameObject panelUpgrade;
    public Sprite sprUpgrade;
    public Sprite sprProgress;

    public void Show(bool isShow)
    {
        this.panel.SetActive(isShow);
    }
    /// <summary>
    /// 
    /// </summary>
    /// <param name="level">level hien tai</param>
    /// <param name="current">card hien tai</param>
    /// <param name="total">tong card can upgarde</param>
    public void ParseData(int level, int current, int total)
    {
        this.Show(true);
        float fill = (float) current / (float) total;
        this.txtLevel.text = string.Format("{0}", level + 1);
        this.txtProgress.text = string.Format("{0}/{1}", current, total);

        this.imgProgress.fillAmount = fill;
        if (fill >= 1)
        {
            this.imgProgress.sprite = this.sprUpgrade;
            this.panelUpgrade.SetActive(true);
        }
        else
        {
            this.imgProgress.sprite = this.sprProgress;
            this.panelUpgrade.SetActive(false);
        }
    }

    public void SetLevel(string level)
    {
        this.txtLevel.text = level;
    }
}
