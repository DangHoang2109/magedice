using System;
using System.Collections;
using System.Collections.Generic;
using Coffee.UIEffects;
using DG.Tweening;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class StatGUIUpgrade : MonoBehaviour
{
    public TextMeshProUGUI txtLevel;
    
    public TextMeshProUGUI txtCountCard;
    public Image imgProgress;
    public Image imgEnough;
    //public Image imgArrow;


    public GameObject goButtonUpgrade;

    public IBooster uiBooster;

    //[Tooltip("Collect enough to unlock free ,Must the same count as the unlock requirement cards count")]
    //public GameObject[] goPartsCover;
    private int cur;
    private int newCur;
    private int tot;
    private System.Action callback;

    public void ParseCueToBuy(StatData c)
    {
        if(this.goButtonUpgrade != null)
            this.goButtonUpgrade.SetActive(false);
        //this.imgArrow.color = Color.clear;
        if (this.txtLevel != null)
            this.txtLevel.text = string.Empty;
        
        
        this.imgProgress.gameObject.SetActive(false);
        
        tot = (int)c.RequirementCard;
        this.cur = (int)c.cards;

        if (tot == 0)
        {
            this.imgProgress.fillAmount = 1f;
            //this.imgProgress.color = new Color(0.8f, 0.8f, 0.8f);
            this.txtCountCard.text = c.cards.ToString();
        }
        else
        {
            this.imgProgress.fillAmount = (float)c.cards / (float)tot;
            this.txtCountCard.text = $"{c.cards}/{tot}";

            //this.imgProgress.color = (c.cards >= req)? Color.green : Color.blue;
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


            if (this.txtLevel != null)
                this.txtLevel.text = "";
        }
        else
        {
            this.cur = (int)c.cards;
            if (c.IsMaxLevel)
            {
                if (this.txtLevel != null)
                    this.txtLevel.text = "Level Max";

                if (this.goButtonUpgrade != null)
                    this.goButtonUpgrade.SetActive(false);

                this.imgProgress.gameObject.SetActive(false);
                imgEnough.gameObject.SetActive(true);
                this.txtCountCard.text = c.cards.ToString();

                //this.imgArrow.color = Color.clear;

            }
            else
            {
                if (this.txtLevel != null)
                    this.txtLevel.text = $"Level {c.level}";

                this.imgProgress.gameObject.SetActive(true);
                imgEnough.gameObject.SetActive(false);

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


                tot = (int)c.RequirementCard;
                if (tot == 0)
                {
                    this.imgProgress.fillAmount = 1f;
                    //this.imgProgress.color = new Color(0.8f, 0.8f, 0.8f);
                    this.txtCountCard.text = c.cards.ToString();
                }
                else
                {
                    this.imgProgress.fillAmount = (float) c.cards / (float)tot;
                    this.txtCountCard.text = $"{c.cards}/{tot}";
                    
                    //this.imgProgress.color = (c.cards >= req)? Color.green : Color.blue;
                }

                //this.imgArrow.color = c.cards >= req ? Color.green : Color.blue;
            }
        }
    }

    private void ShowCover(int count)
    {
        //if (this.goPartsCover == null || this.goPartsCover.Length == 0)
        //    return;

        //if (count < 0)
        //    count = 0;
        //else if (count > this.goPartsCover.Length)
        //    count = this.goPartsCover.Length;
        
        
        //for (int i1 = 0; i1 < count; ++i1)
        //{
        //    this.goPartsCover[i1].SetActive(true);
        //}
        //if (count == this.goPartsCover.Length)
        //    return;
        
        //for (int i2 = count; i2 < this.goPartsCover.Length; ++i2)
        //{
        //    this.goPartsCover[i2].SetActive(false);
        //}
    }

    public StatGUIUpgrade DoFillTo(in long newCurrent, in float duration)
    {
        return DoFillTo((int)newCurrent, duration);
    }

    public StatGUIUpgrade DoFillTo(in int newCurrent, in float duration)
    {
        this.newCur = newCurrent;

        //this.imgProgress.DOFillAmount((float)newCurrent/this.tot, duration)
        //   .OnComplete(this.OnFillCompleted).SetId(this);

        DOTween.To(this.GetCurrent, this.OnUpdateFill, newCurrent, duration)
            .OnComplete(this.OnFillCompleted).SetId(this);

        return this;
    }


    private float GetCurrent()
    {
        return (float)this.cur;
    }
    private void OnUpdateFill(float current)
    {
        this.imgProgress.fillAmount = current / (float)this.tot;
        this.txtCountCard.text = string.Format("{0}/{1}", Mathf.RoundToInt(current), this.tot);
    }

    private void OnFillCompleted()
    {
        this.OnUpdateFill(this.cur = this.newCur);
        // ensure no miscalculation from rounding float
        this.txtCountCard.text = string.Format("{0}/{1}", this.cur, this.tot);

        //if (this.cur >= this.tot)
        //{
        //    if (this.panelUpgrade != null)
        //    {
        //        this.panelUpgrade.SetActive(true);
        //    }
        //}
        //else
        //{
        //    this.imgProgress.color = TennisColor.ColorProgressLightBlue;
        //    if (this.panelUpgrade != null)
        //    {
        //        this.panelUpgrade.SetActive(false);
        //    }
        //}

        if (this.callback == null)
            return;

        var c2 = callback;
        this.callback = null;
        c2.Invoke();
    }
}
