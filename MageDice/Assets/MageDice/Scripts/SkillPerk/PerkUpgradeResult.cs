using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
using DG.Tweening;
using System.Linq;
using UnityEngine.Events;
public class PerkUpgradeResult : MonoBehaviour
{
    private bool isAnnimating;
    private System.Action callback;

    public float POS_UPPER;
    public float POS_STARTING;

    public SkillPerkIconItem itemDisplayer;
    public UpgradeCardLine statDisplayer;

    public CanvasGroup tfIcon;

    private RectTransform rectTransStat;
    private RectTransform RectTransStat
    {
        get
        {
            if (ReferenceEquals(this.rectTransStat, null))
            {
                this.rectTransStat = this.GetComponent<RectTransform>();
            }
            return this.rectTransStat;
        }
    }

    public void ParseData(PerkData result)
    {
        float maxWidth = this.RectTransStat.rect.width;

        itemDisplayer.ParseData(result);

        statDisplayer.Parse(result.PreviousStat, result.CurrentStat, maxWidth, result.MaxStat);
        statDisplayer.ParseInfo(result.Asset.name, result.Asset.sprIcon);

        this.gameObject.SetActive(true);
    }

    public void StartAnimate(System.Action cb = null)
    {
        callback = cb;
        isAnnimating = true;

           Sequence seq = DOTween.Sequence();
        seq.Join(tfIcon.DOFade(1, 0.75f));
        seq.Join(tfIcon.transform.DOLocalMoveY(POS_UPPER, 0.75f));

        statDisplayer.StartAnimate(0.5f);

        seq.AppendInterval(1.5f);

        seq.OnComplete(() =>
        {
            isAnnimating = false;
            callback?.Invoke();
            Clear();

        });
    }

    public void Clear()
    {
        tfIcon.alpha = 0;
        tfIcon.transform.localPosition = new Vector3(tfIcon.transform.localPosition.x, POS_STARTING);

        if (this.statDisplayer.gameObject.activeSelf)
        {
            this.statDisplayer.Clear(false);
            this.statDisplayer.Show(false);
        }

        this.gameObject.SetActive(false);
    }

    public void ClickStep()
    {
        return;

        if (!isAnnimating)
            Clear();
    }
}
