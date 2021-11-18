using Coffee.UIEffects;
using DG.Tweening;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class CueUnlock : MonoBehaviour
{
    public CanvasGroup cv;
    public Image imgFrame;
    public Image imgCue;
    
    private int lastLockCount = 0;
    public UIDissolve[] dissolves1;
    public UIDissolve[] dissolves2;

    private int index;

    

    public UIEffect fxLight;
    
    public void ParseCue(StatData c)
    {
        DOTween.Kill(this);
        this.SetLightCue(0f);

        this.imgCue.sprite = c.config?.sprStatItem;
        
        if (this.imgFrame != null)
            this.imgFrame.color = ShopCueRef.Instance.GetColorTier(c.config?.tier ?? StatManager.Tier.Standard)
                ?.colorLight?? Color.white;

        if (c.level != 0)
        {
            this.ShowCover(0);
        }
        else
        {
            long req = c.RequirementCard;
            if (req == 0)
            {
                this.ShowCover(0);
            }
            else
            {
                this.ShowCover((int) (req - c.cards));
            }
        }
    }

    private void ShowCover(int count)
    {
        if (this.dissolves1 == null || this.dissolves1.Length == 0)
            return;

        if (count < 0)
            count = 0;
        else if (count > this.dissolves1.Length)
            count = this.dissolves1.Length;
        
        this.lastLockCount = count;

        for (int i1 = 0; i1 < count; ++i1)
        {
            this.dissolves1[i1].effectFactor = 0f;
            this.dissolves2[i1].effectFactor = 0f;
        }

        if (count == this.dissolves1.Length)
            return;
        
        for (int i2 = count; i2 < this.dissolves1.Length; ++i2)
        {
            this.dissolves1[i2].effectFactor = 1f;
            this.dissolves2[i2].effectFactor = 1f;
        }
    }

    public void AnimateRemoveCoverInShop(int count, float delay,
        TweenCallback callWhenUnlockStart, TweenCallback callCompleteAnimation)
    {
        DOTween.Kill(this);

        Sequence seq;
        if (!this.gameObject.activeSelf)
        {
            this.cv.alpha = 0f;
            this.gameObject.SetActive(true);
            
            seq = DOTween.Sequence().AppendInterval(delay);
            seq.Append(this.cv.DOFade(1f, 0.25f));
        }
        else
        {
            seq = DOTween.Sequence().AppendInterval(delay);
        }
        
        if (this.lastLockCount > 0)
        {
            this.index = lastLockCount - 1;
            int min = lastLockCount - count;
            if (min < 0)
                min = 0;
            for (int i = this.lastLockCount - 1; i >= min; --i)
            {
                seq.AppendCallback(this.StartRevealingACover);
                seq.AppendInterval(0.25f);
            }

            if (min == 0)
            {
                if (callWhenUnlockStart != null)
                {
                    seq.AppendCallback(callWhenUnlockStart);
                }
                seq.AppendCallback(this.AnimateCueFastUnlocked);
                seq.AppendInterval(1f);
            }
        }
        else
        {
            if (callWhenUnlockStart != null)
            {
                seq.AppendCallback(callWhenUnlockStart);
            }
            seq.AppendInterval(0.65f);
        }
        
        if (callCompleteAnimation != null)
        {
            seq.AppendCallback(callCompleteAnimation);
        }
        seq.SetId(this);
    }
    public void AnimateRemoveCoverInBag(int count, float delay,
        TweenCallback callWhenUnlockStart, TweenCallback callCompleteAnimation)
    {
        DOTween.Kill(this);

        Sequence seq;
        if (!this.gameObject.activeSelf)
        {
            this.cv.alpha = 0f;
            this.gameObject.SetActive(true);
            
            seq = DOTween.Sequence().AppendInterval(delay);
            seq.Append(this.cv.DOFade(1f, 0.25f));
        }
        else
        {
            seq = DOTween.Sequence().AppendInterval(delay);
        }
        
        if (this.lastLockCount > 0)
        {
            this.index = lastLockCount - 1;
            int min = lastLockCount - count;
            if (min < 0)
                min = 0;
            for (int i = this.lastLockCount - 1; i >= min; --i)
            {
                seq.AppendCallback(this.StartRevealingACover);
                seq.AppendInterval(0.25f);
            }

            if (min == 0)
            {
                if (callWhenUnlockStart != null)
                {
                    seq.AppendCallback(callWhenUnlockStart);
                }
                
                seq.AppendInterval(1f);
            }
        }
        else
        {
            if (callWhenUnlockStart != null)
            {
                seq.AppendCallback(callWhenUnlockStart);
            }
            seq.AppendInterval(0.65f);
        }
        
        if (callCompleteAnimation != null)
        {
            seq.AppendCallback(callCompleteAnimation);
        }
        seq.SetId(this);
    }

    private void StartRevealingACover()
    {
        SoundManager.Instance.Play("snd_panel");
        int ind = this.index--;
        DOTween.To(this.Return0, (x) =>
        {
            this.dissolves1[ind].effectFactor = x;
            this.dissolves2[ind].effectFactor = x;
        }, 1f, 1.5f).SetId(this);
    }

    private void AnimateCueFastUnlocked()
    {
        DOTween.Sequence()
            .Append(DOTween.To(this.Return0, this.SetLightCue, 1f, 0.5f).SetEase(Ease.OutSine))
            .Append(DOTween.To(this.Return1, this.SetLightCue, 0f, 0.5f).SetEase(Ease.InSine))
            .SetId(this);
    }

    private float Return0()
    {
        return 0;
    }
    private float Return1()
    {
        return 1;
    }

    private void SetLightCue(float l)
    {
        this.fxLight.colorFactor = l;
    }

    public void Hide()
    {
        this.OnHideComplete();
    }
    
    // public void StartHide()
    // {
    //     this.cv.DOFade(0f, 0.5f)
    //         .OnComplete(this.OnHideComplete).SetId(this);
    // }

    private void OnHideComplete()
    {
        DOTween.Kill(this);
        this.gameObject.SetActive(false);
    }

    // private Tween MakeDissolve(UIDissolve dissolve, float duration)
    // {
    //     return DOTween.To(() => 0f,
    //         (x) => dissolve.effectFactor = x,
    //         1f, duration);
    // }

    // private void PlaySfxSwift()
    // {
    //     SoundManager.Instance.Play("snd_panel");
    // }
    // private void PlaySfxPalletStart()
    // {
    //     SoundManager.Instance.Play("sfx_pellet_start");
    // }
}
