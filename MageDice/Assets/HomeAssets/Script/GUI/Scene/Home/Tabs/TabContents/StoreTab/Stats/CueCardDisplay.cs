using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class CueCardDisplay : MonoBehaviour
{
    public TextMeshProUGUI tmpName;
    public Image[] imgCues;
    public StatGUIStatLines statLines;
    public StatsCardLevelV2 lvl;


    private void BaseParseCue(StatData StatData)
    {
        if (StatData != null)
        {
            if (StatData.config != null)
            {
                //parse name cue
                this.tmpName.text = StatData.config.statName;

                //parse image cue
                foreach (Image imgCue in this.imgCues)
                {
                    imgCue.sprite = StatData.config.sprStatItem;
                }

                long countCard = StatData.cards;
                long requirement = StatData.RequirementCard;
                this.lvl.ParseData(StatData.level, countCard, requirement, Color.white);
            }
        }
    }

    public void ParseCue(DiceID cueId)
    {
        StatData StatData = StatDatas.Instance.GetStat(cueId);
        ParseCue(StatData);
    }

    public void ParseCue(StatData StatData)
    {
        BaseParseCue(StatData);
        if (StatData != null)
        {
            if (StatData.level <= 0)
            {
                this.statLines.ParseStats(StatData.NextStats);
            }
            else this.statLines.ParseStats(StatData.CurrentStats);
        }
    }

    public void ParseCueFullStats(DiceID cueId)
    {
        StatData StatData = StatDatas.Instance.GetStat(cueId);
        ParseCueFullStats(StatData);
    }

    public void ParseCueFullStats(StatData StatData)
    {
        BaseParseCue(StatData);
        if (StatData != null)
            this.statLines.ParseStats(StatData.FullStats);
    }
}
