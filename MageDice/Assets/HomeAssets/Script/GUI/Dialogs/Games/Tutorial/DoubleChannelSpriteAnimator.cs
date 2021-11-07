using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class DoubleChannelSpriteAnimator : MonoBehaviour
{
    public List<SpriteRenderer> renderersChannelA;
    public List<SpriteRenderer> renderersChannelB;

    protected Transform cachedTransform;
    public Transform CachedTransform
    {
        get
        {
            if (this.cachedTransform == null)
            {
                this.cachedTransform = this.transform;
            }
            return this.cachedTransform;
        }
    }


    protected float _timePassed = 0f;
    protected Color _colA;
    protected Color _colB;

    [SerializeField]
    protected int _count;

    [Space(10f)]
    public float fadeDuration = 0.3f;
    protected float _fade = 1f;

    protected virtual float GetFade() { return this._fade; }
    protected virtual void SetFade(float value) { this._fade = value; }

#if UNITY_EDITOR
    [ContextMenu("Fade in")]
#endif
    public virtual void FadeIn()
    {
        DOTween.Kill(this);
        DOTween.To(
            getter: this.GetFade,
            setter: this.SetFade,
            endValue: 1f,
            duration: this.fadeDuration
            ).SetId(this);
    }

#if UNITY_EDITOR
    [ContextMenu("Fade out")]
#endif
    public virtual void FadeOut()
    {
        DOTween.Kill(this);
        DOTween.To(
            getter: this.GetFade,
            setter: this.SetFade,
            endValue: 0f,
            duration: this.fadeDuration
            ).SetId(this);
    }

#if UNITY_EDITOR
    [ContextMenu("get all children 2 layer")]
    private void GetAllChildren2Layer()
    {
        _ = this.CachedTransform;

        this.renderersChannelA = new List<SpriteRenderer>(this.cachedTransform.childCount);
        this.renderersChannelB = new List<SpriteRenderer>(this.renderersChannelA.Capacity);

        SpriteRenderer sprRenderer;
        foreach(Transform child in this.cachedTransform)
        {
            sprRenderer = child.GetComponent<SpriteRenderer>();
            if (sprRenderer == null)
                continue;

            this.renderersChannelA.Add(sprRenderer);
            sprRenderer = child.GetChild(0).GetComponent<SpriteRenderer>();
            this.renderersChannelB.Add(sprRenderer);
        }

        this._count = this.renderersChannelA.Count;
    }
#endif
}
