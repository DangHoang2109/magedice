using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;
public class DotOnlineItem : MonoBehaviour
{
    public Image imgFill;
    public Transform tfRewardItem;

    [SerializeField]
    private RewardItemUI reward;

    public RewardItemUI Reward => reward;

    
    public Tween DoDot(bool isLastItem ,float time = 0.25f, float delay = 0.1f)
    {
        return this.imgFill.DOFillAmount(1f, time)
            .SetDelay(delay);
            //.OnComplete(()=> {
            //    DoActiveIndex(isLastItem);
            //});
    }

    public void ActiveIndex(bool isCurrent)
    {
        Vector3 scale = isCurrent ?  new Vector3(1.25f, 1.25f) : new Vector3(0.75f, 0.75f);
        this.tfRewardItem.localScale = scale;

        this.imgFill.fillAmount = 0f;
    }
    public Tween DoActiveIndex(bool isCurrent)
    {

        Vector3 scale = isCurrent ? new Vector3(1.25f, 1.25f) : new Vector3(0.75f, 0.75f);

        Debug.Log("scale to " + scale);
        return this.tfRewardItem.DOScale(scale, 0.2f);
    }
}
