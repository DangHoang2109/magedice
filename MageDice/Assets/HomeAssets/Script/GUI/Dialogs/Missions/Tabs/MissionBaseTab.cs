using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MissionBaseTab : Tab
{
    public NumNoti numNoti;

    [Header("Layout")]
    public Image imgBg;
    public Image imgSelected;


    public void ParseLayout(BattlePassAssetConfig battlePassAsset)
    {
        this.imgBg.color = battlePassAsset.color3;
        this.imgSelected.color = battlePassAsset.color1;
    }
}
