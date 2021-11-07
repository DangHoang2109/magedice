using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class UserCareerOverview : MonoBehaviour
{
    public TextMeshProUGUI txtTotalMatch;
    public TextMeshProUGUI txtMatchWon;
    public TextMeshProUGUI txtWinRate;
    public TextMeshProUGUI txtMaxTrophies;
    public TextMeshProUGUI txtWinStreak;
    public TextMeshProUGUI txtMaxWinStreak;

    public void ParseData(UserCareers careers)
    {
        this.txtTotalMatch.text = careers.totalMatch.ToString();
        this.txtMatchWon.text = careers.matchWin.ToString();
        this.txtWinRate.text = string.Format("{0}", careers.GetWinRate().ToString("P"));
        this.txtMaxTrophies.text = careers.trophyMax.ToString();
        this.txtWinStreak.text = careers.winStreak.ToString();
        this.txtMaxWinStreak.text = careers.maxWinStreak.ToString();
    }
}
