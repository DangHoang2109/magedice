using System;
using System.Collections;
using System.Collections.Generic;
using Coffee.UIEffects;
using DG.Tweening;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class CueUpgrade : MonoBehaviour
{
    public TextMeshProUGUI txtLevel;
    
    public TextMeshProUGUI txtCountCard;
    //public Transform transProgressHolder;
    public Image imgProgress;
    public Image imgArrow;


    public GameObject goButtonUpgrade;

    public IBooster uiBooster;

    [Tooltip("Must the same count as the unlock requirement cards count")]
    public GameObject[] goPartsCover;
    
    public void ParseCueToBuy(StatData c)
    {
        if(this.goButtonUpgrade != null)
            this.goButtonUpgrade.SetActive(false);
        this.imgArrow.color = Color.clear;
        this.txtLevel.text = string.Empty;
        
        
        this.imgProgress.gameObject.SetActive(false);
        
        long req = c.RequirementCard;
         if (req == 0)
         {
             this.ShowCover(0);
         }
         else
         {
             this.ShowCover((int)(req - c.cards));
         }
    }

    public void ParseCueBought(StatData c)
    {
        this.ShowCover(0);

        if (c.config.tier == StatManager.Tier.None)
        {
            this.imgProgress.gameObject.SetActive(false);
            if(this.goButtonUpgrade != null)
                this.goButtonUpgrade.SetActive(false);
            this.txtLevel.text = "";
        }
        else
        {
            this.imgProgress.gameObject.SetActive(true);
            if (c.IsMaxLevel)
            {
                this.txtLevel.text = "Level Max";

                if (this.goButtonUpgrade != null)
                    this.goButtonUpgrade.SetActive(false);

                this.imgProgress.fillAmount = 1f;
                this.imgProgress.color = Color.yellow;
                this.txtCountCard.text = c.cards.ToString();

                this.imgArrow.color = Color.clear;

            }
            else
            {
                this.txtLevel.text = $"Level {c.level}";

                if (this.goButtonUpgrade != null)
                {
                    this.goButtonUpgrade.SetActive(true);
                    BoosterType boosterType;

                    switch (c.config.upgradeType)
                    {
                        case StatManager.UnlockType.Coin:
                            boosterType = BoosterType.COIN;
                            break;
                        case StatManager.UnlockType.Cash:
                            boosterType = BoosterType.CASH;
                            break;
                        default:
                            boosterType = BoosterType.NONE;
                            Debug.LogException(new System.Exception("CueUpgrade ParseCueBought: type not supported: " +
                                                                    c.config.upgradeType.ToString()));
                            return;
                    }

                    this.uiBooster.ParseBooster(new BoosterCommodity(boosterType, c.UpgradePrice));
                }


                long req = c.RequirementCard;
                if (req == 0)
                {
                    this.imgProgress.fillAmount = 1f;
                    this.imgProgress.color = new Color(0.8f, 0.8f, 0.8f);
                    this.txtCountCard.text = c.cards.ToString();
                }
                else
                {
                    this.imgProgress.fillAmount = (float) c.cards / (float) req;
                    this.txtCountCard.text = $"{c.cards}/{req}";
                    
                    this.imgProgress.color = (c.cards >= req)? Color.green : Color.blue;
                }

                this.imgArrow.color = c.cards >= req ? Color.green : Color.blue;
            }
        }
    }

    private void ShowCover(int count)
    {
        if (this.goPartsCover == null || this.goPartsCover.Length == 0)
            return;

        if (count < 0)
            count = 0;
        else if (count > this.goPartsCover.Length)
            count = this.goPartsCover.Length;
        
        
        for (int i1 = 0; i1 < count; ++i1)
        {
            this.goPartsCover[i1].SetActive(true);
        }
        if (count == this.goPartsCover.Length)
            return;
        
        for (int i2 = count; i2 < this.goPartsCover.Length; ++i2)
        {
            this.goPartsCover[i2].SetActive(false);
        }
    }
}
