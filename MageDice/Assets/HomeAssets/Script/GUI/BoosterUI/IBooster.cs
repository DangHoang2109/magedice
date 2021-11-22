using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// UI display ONLY
/// </summary>
public class IBooster : MonoBehaviour
{
    public Image imgBooster;
    public TextMeshProUGUI tmpValue;
    public BoosterCommodity booster;

    [SerializeField]
    protected bool onCallbackBooster; //true: load gia tri theo type booster, false: parse gia tri cua booster

#if UNITY_EDITOR
    protected virtual void OnValidate()
    {
        if (this.tmpValue== null)
            this.tmpValue = this.transform.GetComponentInChildren<TextMeshProUGUI>();
        if (this.imgBooster == null)
            this.imgBooster = this.transform.GetComponentInChildren<Image>();
    }
#endif

    protected virtual void OnEnable()
    {        
        if (this.booster != null)
        {
                
            if (this.booster.type != BoosterType.NONE)
            {
                if (this.onCallbackBooster)
                {
                    //Load value booster
                    UserProfile.Instance.AddCallbackBooster(this.booster.type, this.OnChangeValue);
                }
                else this.ParseBooster(this.booster);
            }
        }  
    }

    protected virtual void OnDisable()
    {
#if UNITY_EDITOR
        if (GameManager.isApplicationQuit)
            return;
#endif
        if (this.onCallbackBooster)
        {
            UserProfile.Instance.RemoveCallbackBooster(this.booster.type, this.OnChangeValue);
        }
    }

    protected void OnChangeValue(BoosterCommodity booster)
    {
        if (booster!=null)
            this.ParseBoostValue(booster.GetValue());
    }

    public virtual void ParseBoostValue(long value)
    {
        SetValueText(GameUtils.FormatMoneyDot(value));
    }
    public virtual void ParseBoosterValue(long value, string booster)
    {
        SetValueText(string.Format("{0}{1}", booster, GameUtils.FormatMoneyDot(value)));
    }

    public virtual void ParseBoosterImage(BoosterType boosterType)
    {
        this.imgBooster.sprite = BoosterConfigs.Instance.GetBooster(boosterType).spr;
    }

    public virtual void ParseBooster(BoosterCommodity booster)
    {
        this.booster = booster;
        if(this.booster != null)
        {
            if (this.booster.type != BoosterType.NONE)
            {
                this.imgBooster.sprite = BoosterConfigs.Instance.GetBooster(this.booster.type).spr;
                SetValueText(GameUtils.FormatMoneyDot(this.booster.GetValue()));
            }         
        }
        this.gameObject.SetActive(this.booster != null);
    }

    public void ShowSpriteOff(bool isSprOff = true)
    {
        if (this.booster != null)
        {
            BoosterConfig config = BoosterConfigs.Instance.GetBooster(this.booster.type);
            this.imgBooster.sprite = isSprOff? config.sprOff : config.spr;
        }
    }

    protected virtual void SetValueText(string text)
    {
        this.tmpValue.SetText(text);
    }

}
