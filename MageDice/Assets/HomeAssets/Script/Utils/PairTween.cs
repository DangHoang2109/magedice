using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;

namespace Cosina.Components
{
    [System.Serializable]
    public class PairTweenVector
    {
        public Vector3 value;
        public float duration;
        public Ease ease;
    }
    
    
    [System.Serializable]
    public class PairTweenFloat
    {
        public float value;
        public float duration;
        public Ease ease;
    }

    public static class PairTweenExtension
    {
        public static Sequence ToScaleSequence(this PairTweenVector[] me, Transform t)
        {
            if (me is null || me.Length == 0)
                return null;
            
            Sequence s = DOTween.Sequence();
            for (int i = 0; i < me.Length; ++i)
            {
                if (me[i].ease == Ease.Unset)
                {
                    s.Append(t.DOScale(me[i].value, me[i].duration)); 
                }
                else
                {
                    s.Append(t.DOScale(me[i].value, me[i].duration).SetEase(me[i].ease));
                }
            }
            return s;
        }
        public static Sequence ToScaleSequence(this PairTweenVector[] me, Transform t, params TweenCallback[] callbackAppend)
        {
            if (me is null || me.Length == 0)
                return null;
            
            Sequence s = DOTween.Sequence();
            if (callbackAppend is null || callbackAppend.Length == 0)
            {
                for (int i = 0; i < me.Length; ++i)
                {
                    if (me[i].ease == Ease.Unset)
                    {
                        s.Append(t.DOScale(me[i].value, me[i].duration));
                    }
                    else
                    {
                        s.Append(t.DOScale(me[i].value, me[i].duration).SetEase(me[i].ease));
                    }
                }
            }
            else
            {
                for (int i = 0; i < me.Length; ++i)
                {
                    if (me[i].ease == Ease.Unset)
                    {
                        s.Append(t.DOScale(me[i].value, me[i].duration));
                    }
                    else
                    {
                        s.Append(t.DOScale(me[i].value, me[i].duration).SetEase(me[i].ease));
                    }

                    if (callbackAppend.Length > i && callbackAppend[i] != null)
                        s.AppendCallback(callbackAppend[i]);
                }
            }

            return s;
        }
        
        public static Sequence ToScaleSequence(this PairTweenFloat[] me, Transform t)
        {
            if (me is null || me.Length == 0)
                return null;
            
            Sequence s = DOTween.Sequence();
            for (int i = 0; i < me.Length; ++i)
            {
                if (me[i].ease == Ease.Unset)
                {
                    s.Append(t.DOScale(me[i].value, me[i].duration)); 
                }
                else
                {
                    s.Append(t.DOScale(me[i].value, me[i].duration).SetEase(me[i].ease));
                }
            }
            return s;
        }
        
        public static Sequence ToMoveSequence(this PairTweenVector[] me, Transform t, params TweenCallback[] callbackAppend)
        {
            if (me is null || me.Length == 0)
                return null;
            
            Sequence s = DOTween.Sequence();
            if (callbackAppend is null || callbackAppend.Length == 0)
            {
                for (int i = 0; i < me.Length; ++i)
                {
                    if (me[i].ease == Ease.Unset)
                    {
                        s.Append(t.DOMove(me[i].value, me[i].duration));
                    }
                    else
                    {
                        s.Append(t.DOMove(me[i].value, me[i].duration).SetEase(me[i].ease));
                    }
                }
            }
            else
            {
                for (int i = 0; i < me.Length; ++i)
                {
                    if (me[i].ease == Ease.Unset)
                    {
                        s.Append(t.DOMove(me[i].value, me[i].duration));
                    }
                    else
                    {
                        s.Append(t.DOMove(me[i].value, me[i].duration).SetEase(me[i].ease));
                    }

                    if (callbackAppend.Length > i && callbackAppend[i] != null)
                        s.AppendCallback(callbackAppend[i]);
                }
            }

            return s;
        }
    }
}
