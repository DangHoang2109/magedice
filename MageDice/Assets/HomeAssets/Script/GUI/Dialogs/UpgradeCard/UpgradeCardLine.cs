using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UpgradeCardLine : MonoBehaviour
{
    [Header("Linekrs")]
    public Image imgIcon;
    public TextMeshProUGUI txtName;
    public TextMeshProUGUI txtOld;
    public Image imgArrow;
    public TextMeshProUGUI txtAddition;

    public RectTransform rectFillNew;
    public RectTransform rectFill;
    public Transform transLine;

    [Header("Stats")]
    public float interval;
    public float durationEach;
    
    public void Parse(float before, float after, float maxWidth, float maxStat)
    {
        this.Reset();
        this.Show(true);
        
        this.txtOld.text = before.ToString();
        this.txtAddition.text = $"+{(after - before)}";

        Vector2 s = this.rectFill.sizeDelta;
        s.x = (after / maxStat) * maxWidth;
        this.rectFillNew.sizeDelta = s;

        s.x = (before / maxStat) * maxWidth;
        this.rectFill.sizeDelta = s;
    }

    public void StartAnimate(float delay)
    {
        this.Clear(false);

        this.imgIcon.transform.DOScale(1f, durationEach)
            .SetDelay(this.interval + delay).SetId(this);
        this.txtName.DOColor(Color.white, durationEach)
            .SetDelay(this.interval * 2f + delay).SetId(this);
        this.txtName.rectTransform.DOAnchorPosY(5f, durationEach)
            .SetDelay(this.interval * 2f + delay).SetId(this);
        this.txtOld.DOColor(Color.white, durationEach)
            .SetDelay(this.interval * 3f + delay).SetId(this);
        this.txtOld.rectTransform.DOAnchorPosY(5f, durationEach)
            .SetDelay(this.interval * 3f + delay).SetId(this);
        this.imgArrow.DOColor(Color.green, durationEach)
            .SetDelay(this.interval * 4f + delay).SetId(this);
        this.imgArrow.rectTransform.DOAnchorPosY(-25f, durationEach)
            .SetDelay(this.interval * 4f + delay).SetId(this);
        this.txtAddition.DOColor(Color.green, durationEach)
            .SetDelay(this.interval * 5f + delay).SetId(this);
        this.txtAddition.rectTransform.DOAnchorPosY(5f, durationEach)
            .SetDelay(this.interval * 5f + delay).SetId(this);
        this.transLine.DOScale(Vector3.one, durationEach)
            .SetDelay(this.interval * 7f + delay).SetId(this);

    }

    public void SkipAnimation()
    {
        this.Clear(true);
    }

    public void Clear(bool isComplete = false)
    {
        DOTween.Kill(this, isComplete);
    }

    private void Reset()
    {
        this.txtName.color = this.txtOld.color
            = this.txtAddition.color = new Color(1f, 1f, 1f, 0f);
        this.imgArrow.color = new Color(0f, 1f, 0f, 0f);
        var ap = this.txtName.rectTransform.anchoredPosition;
        ap.y = 20f;
        this.txtName.rectTransform.anchoredPosition = ap;
        ap = this.txtOld.rectTransform.anchoredPosition;
        ap.y = 20f;
        this.txtOld.rectTransform.anchoredPosition = ap;
        ap = this.txtAddition.rectTransform.anchoredPosition;
        ap.y = 20f;
        this.txtAddition.rectTransform.anchoredPosition = ap;

        var rA = this.imgArrow.GetComponent<RectTransform>();
        ap = rA.anchoredPosition;
        ap.y = 0f;
        rA.anchoredPosition = ap;
        
        this.imgIcon.transform.localScale = Vector3.zero;
        this.transLine.localScale = Vector3.up;
    }

    public void Show(bool isShow)
    {
        this.gameObject.SetActive(isShow);
    }
    
}
