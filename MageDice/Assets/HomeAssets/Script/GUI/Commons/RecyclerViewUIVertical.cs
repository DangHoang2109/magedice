using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using System.Security.Cryptography;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

namespace Cosina.Components
{
    public interface IIndexable
    {
        void SetIndex(int index);
    }


    /// <summary>
    /// WARNING! this work for top to bottom list
    /// <para>
    /// KEY features: Init(), Refresh(), RemoveItem(), InstantScrollTo(int),
    /// SmoothlyScrollTo(int, float, ease), OnCollectionChanged( isLessItem )
    /// </para>
    /// <para>
    /// Events: OnScrolledToTop, OnScrolledToBottom, OnScrolledToNearlyBottom
    /// </para>
    /// </summary>
    public class RecyclerViewUIVertical : MonoBehaviour
    {
        private const float NORMALIZE_SNAP_EQUAL = 0.0001f;

        // events
        public event System.Action OnScrolledToTop;
        public event System.Action OnScrolledToBottom;
        public event System.Action OnScrolledToNearlyBottom;

        [Header("linker")]
        public ScrollRect scrollRect;
        public RectTransform rectView;
        public RectTransform rectContent;

        [Header("stat")]
        public float paddingTop;
        public float paddingBottom;
        public float spacing;

        [Space(10f), Tooltip("How many items left, to invoke event nearly bottom?")]
        public int countNearlyEdge = 2;

        [Tooltip("for calculate the view, not adjust the items")]
        public float heightItem;

        //private List<RectTransform> _trans;
        private Dictionary<int, RectTransform> _dictTrans;
        private int _countItemAppearanceFix;
        private int _lastCountItems = 0;

        private Vector2 _lastScrollPos;

        [Header("debug")]
        [SerializeField, Tooltip("The smallest index appear on the screen")]
        private int _minAppear;
        [SerializeField, Tooltip("The largest index appear on the screen")]
        private int _maxAppear;


        private System.Action<RectTransform, int> _callParseItem;
        private System.Func<int> _funcGetCount;
        private System.Func<RectTransform> _funcTakeItem;
        private System.Action<RectTransform> _callReturnItem;

        private bool _isInitialized = false;
        
        /// <summary>
        /// 
        /// </summary>
        /// <param name="callParseItem"> when this function be called, you need to GetComponent you wanted in the item and parse that, the int is index of the item </param>
        /// <param name="funcGetCountItems"> when this function be called, you return the count of items </param>
        /// <param name="funcTakeItemFromStock"> you fetch an item from items POOL and give me </param>
        /// <param name="callReturnItemToStock"> I return you the item, you return it into items POOL </param>
        public void Init(System.Action<RectTransform, int> callParseItem,
            System.Func<int> funcGetCountItems,
            System.Func<RectTransform> funcTakeItemFromStock,
            System.Action<RectTransform> callReturnItemToStock)
        {
            this._callParseItem = callParseItem;
            this._funcGetCount = funcGetCountItems;
            this._funcTakeItem = funcTakeItemFromStock;
            this._callReturnItem = callReturnItemToStock;

            this.Initialize();
            this.FirstPopulate();
            this.InitScroll();

            this._isInitialized = true;
        }

        private void Initialize()
        {
            this._countItemAppearanceFix = Mathf.CeilToInt(this.rectView.rect.height / (this.heightItem + this.spacing))
                + 3;
        }

        private void FirstPopulate()
        {
            int count = this._funcGetCount();
            int cap = this._countItemAppearanceFix < count ? this._countItemAppearanceFix : count;
            Vector2 s = this.rectContent.sizeDelta;
            this.UpdateSize(count, s);

            //this._trans = new List<RectTransform>(cap);
            this._dictTrans = new Dictionary<int, RectTransform>(cap);
            RectTransform tmp;

            for (int i = 0; i < cap; ++i)
            {
                tmp = this._funcTakeItem();
                //this._trans.Add(tmp);
                tmp.SetParent(this.rectContent);
                tmp.anchorMin = new Vector2(0f, 1f);
                tmp.anchorMax = Vector2.one;
                s = tmp.sizeDelta;
                s.x = 0f;
                tmp.sizeDelta = s;
                tmp.gameObject.SetActive(true);
                tmp.localPosition = new Vector3(0.5f,
                    -this.paddingTop - (i * (this.heightItem + this.spacing)),
                    0f);
                this._callParseItem(tmp, i);
                this._dictTrans.Add(i, tmp);
            }

            this._minAppear = 0;
            this._maxAppear = cap - 1;// cap -1 moi dung
            this._lastCountItems = count;

            this._lastScrollPos = this.scrollRect.normalizedPosition = new Vector2(0f, 1f);
        }



        private void InitScroll()
        {
            this._lastScrollPos = this.scrollRect.normalizedPosition;
            this.scrollRect.onValueChanged.RemoveListener(this.OnScrollChanged);
            this.scrollRect.onValueChanged.AddListener(this.OnScrollChanged);
        }

        /// <param name="count">count of items</param>
        private void UpdateSize(int count, Vector2 sizeOriginal)
        {
            sizeOriginal.y = count * (heightItem + spacing) - this.spacing
                + this.paddingTop + this.paddingBottom;
            if (sizeOriginal.y < this.rectView.rect.height)
                sizeOriginal.y = this.rectView.rect.height;
            this.rectContent.sizeDelta = sizeOriginal;
        }

        // [System.Obsolete("This is not worked yet, please give the developer some food to have this work!")]
        // public void AddItem(int index)
        // {
        //     if(this._lastCountItems == 0)
        //     {
        //         this.FirstPopulate();
        //     }
        //     else
        //     {
        //         // TODO! handle item insertion
        //     }
        // }

        public void RemoveItem(int index, bool smoothDrop)
        {
            //Debug.LogError("RemoveItem func is not usable!");
            smoothDrop &= this.gameObject.activeInHierarchy;

            this.UpdateSize(this._funcGetCount(), this.rectContent.sizeDelta);

            RectTransform tmp, justRemoved;

            if (this._minAppear > index)
            {
                for(int i = this._minAppear; i <= this._maxAppear; ++i)
                {
                    tmp = this._dictTrans[i];
                    this._dictTrans.Remove(i);
                    this._dictTrans.Add(i - 1, tmp);
#if UNITY_EDITOR
                    if (tmp.GetComponent<IIndexable>() == null)
                        Debug.LogError("RecyclerView note: you MUST have your item has IIndexable!");
                    else
                        tmp.GetComponent<IIndexable>().SetIndex(i - 1);
#else
                    tmp.GetComponent<IIndexable>().SetIndex(i - 1);
#endif

                    this.MoveItem(i - 1, smoothDrop);
                }
                --this._minAppear;
                --this._maxAppear;
            }
            else if (this._maxAppear >= index)
            {
                justRemoved = this._dictTrans[index];
                this._dictTrans.Remove(index);
                for(int j = index + 1; j <= this._maxAppear; ++j)
                {
                    tmp = this._dictTrans[j];
                    this._dictTrans.Remove(j);
                    this._dictTrans.Add(j - 1, tmp);
#if UNITY_EDITOR
                    if (tmp.GetComponent<IIndexable>() == null)
                        Debug.LogError("RecyclerView note: you MUST have your item has IIndexable!");
                    else
                        tmp.GetComponent<IIndexable>().SetIndex(j - 1);
#else
                    tmp.GetComponent<IIndexable>().SetIndex(j - 1);
#endif

                    this.MoveItem(j - 1, smoothDrop);
                }

                if(this._maxAppear > this._funcGetCount())
                {
                    Debug.LogError("RecyclerView RemoveItem max appear > count !!!");
                }

                if(this._dictTrans.Count > this._funcGetCount()
                    || this._maxAppear == this._funcGetCount())
                {
                    if(this._minAppear > 0)
                    {
                        this._dictTrans.Add(--this._minAppear, justRemoved);
                        this._callParseItem(justRemoved, this._minAppear);
                        this.MoveItem(this._minAppear, smooth: false);
                    }
                    else
                    {
                        this._callReturnItem(justRemoved);
                    }
                    --this._maxAppear;
                }
                else
                {
                    this._dictTrans.Add(this._maxAppear, justRemoved);
                    this._callParseItem(justRemoved, this._maxAppear);


                    justRemoved.localPosition = new Vector3(0.5f,
                            -this.paddingTop - ((this._maxAppear + 1) *
                            (this.heightItem + this.spacing)),
                            0f);

                    this.MoveItem(this._maxAppear, smoothDrop);
                }
            }
        }

        public void MoveItem(int index, bool smooth)
        {
            float newY = -this.paddingTop - (index * (this.heightItem + this.spacing));
            if(smooth)
            {
                this._dictTrans[index].DOLocalMoveY(newY, 0.25f).SetEase(Ease.OutBack);
            }
            else
            {
                this._dictTrans[index].localPosition = new Vector3(0.5f, newY, 0f);
            }
        }

        /// <summary>
        /// WARNING: not effect in the case you increase/decrease number of items
        /// </summary>
        public void Refresh()
        {
            if (this._funcGetCount() == 0)
                return;

            for(int i = this._minAppear; i <= this._maxAppear; ++i)
            {
                this._callParseItem(this._dictTrans[i], i);
            }
        }

        private bool _collectionChanged = false;
        /// <summary>
        /// isLessItem: your new collection has less Items?
        /// </summary>
        /// <param name="isLessItem">The new collection has less items?</param>
        public void OnCollectionChanged(bool isLessItem)
        {
            this._collectionChanged = true;
            
            int count = this._funcGetCount();
            if(!isLessItem)// more items
            {
                this.UpdateSize(count, this.rectContent.sizeDelta);
                this._lastScrollPos = this.scrollRect.normalizedPosition;

                int cap = this._countItemAppearanceFix < count ? this._countItemAppearanceFix : count;
                if (this._maxAppear < cap)
                {
                    RectTransform tmp;
                    Vector2 s;
                    for (int i = this._maxAppear + 1; i < cap; ++i)
                    {
                        tmp = this._funcTakeItem();
#if UNITY_EDITOR
                        tmp.name = i.ToString();
#endif
                        //this._trans.Add(tmp);
                        tmp.SetParent(this.rectContent);
                        tmp.anchorMin = new Vector2(0f, 1f);
                        tmp.anchorMax = Vector2.one;
                        s = tmp.sizeDelta;
                        s.x = 0f;
                        tmp.sizeDelta = s;
                        tmp.gameObject.SetActive(true);
                        tmp.localPosition = new Vector3(0.5f,
                            -this.paddingTop - (i * (this.heightItem + this.spacing)),
                            0f);
                        this._dictTrans.Add(i, tmp);
                    }
                    this._maxAppear = cap - 1;
                }
            }
            else// less items
            {
                this.UpdateSize(count, this.rectContent.sizeDelta);


                if (this._minAppear >= count)
                {
                    int maxNewAppear = count - 1;
                    int minNewAppear = count - this._countItemAppearanceFix;

                    if(minNewAppear < 0)
                    {
                        int countRemove = Mathf.Abs(minNewAppear);
                        for(int i = this._maxAppear - countRemove + 1; i <= this._maxAppear; ++i)
                        {
                            this._callReturnItem(this._dictTrans[i]);
                            this._dictTrans.Remove(i);
                        }
                        this._maxAppear -= countRemove;
                        minNewAppear = 0;
                    }
                    int countDifferent = this._minAppear - minNewAppear;
                    RectTransform tmp;
                    for(int i1 = minNewAppear; i1 <= maxNewAppear; ++i1)
                    {
                        tmp = this._dictTrans[i1 + countDifferent];
                        this._dictTrans.Add(i1, tmp);
                        this._dictTrans.Remove(i1 + countDifferent);
                        tmp.localPosition = new Vector3(0.5f,
                            -this.paddingTop - (i1 * (this.heightItem + this.spacing)),
                            0f);
                    }
                    this._minAppear = minNewAppear;
                    this._maxAppear = maxNewAppear;
                    this._lastScrollPos = this.scrollRect.normalizedPosition
                        = new Vector2(1f, 0f);
                }
                else
                {
                    while (this._maxAppear >= count)
                    {
                        this._callReturnItem(this._dictTrans[this._maxAppear]);
                        this._dictTrans.Remove(this._maxAppear--);
                    }
                }
            }
            this.Refresh();
        }

        private void OnScrollChanged(Vector2 v)
        {
            if (this._dictTrans == null || this._dictTrans.Count == 0)
            {
                return;
            }

            if (!_isInitialized)
            {
                Debug.LogException(new System.Exception("RecyclerView OnScrollChanged exception! Hasn't _isInitialized!"));
                return;
            }

            int count = this._funcGetCount();
            RectTransform tmp;

            if (v.y < this._lastScrollPos.y) // scroll down
            {
                int maxNewAppear = (int)System.Math.Round((1f - v.y) *
                    (count - this._countItemAppearanceFix + 4)
                    + this._countItemAppearanceFix - 2,
                    System.MidpointRounding.AwayFromZero);

                if (maxNewAppear >= count)
                    maxNewAppear = count - 1;

                if(maxNewAppear != this._maxAppear)// new > old
                {
                    for(int i = this._maxAppear + 1; i <= maxNewAppear; ++i)
                    {
                        if (_dictTrans.ContainsKey(i))
                            continue;

                        if (!_dictTrans.ContainsKey(this._minAppear))
                        {
                            Debug.LogException(new System.Exception($"RecyclerView {this.name} Exception! OnScrollChanged scroll DOWN: _dictTrans NOT contain _minAppear!\n"
                                                                    + $"-dicCount {_dictTrans.Count} -i {i} -minAppear {_minAppear} -maxAppear {_maxAppear} -collectionChanged {_collectionChanged}"));
                            return;
                        }
                        
                        tmp = this._dictTrans[this._minAppear];
                        this._dictTrans.Remove(this._minAppear++);
                        
                        if (_dictTrans.ContainsKey(i))
                        {
                            Debug.LogException(new System.Exception($"RecyclerView {this.name} Exception! OnScrollChanged scroll DOWN: _dictTrans contained index i already! Trying to add will result in a duplicate!\n"
                                                                    + $"-dicCount {_dictTrans.Count} -i {i} -minAppear {_minAppear} -maxAppear {_maxAppear} -collectionChanged {_collectionChanged}"));
                            return;
                        }
                        this._dictTrans.Add(i, tmp);
                        this._callParseItem(tmp, i);

                        tmp.localPosition = new Vector3(0.5f,
                            - this.paddingTop - (i * (this.heightItem + this.spacing)),
                            0f);

                        this._maxAppear = i;
                    }
                    //this._maxAppear = maxNewAppear;
                    if (maxNewAppear > count - 1 - countNearlyEdge)
                        this.OnScrolledToNearlyBottom?.Invoke();
                }

                if (v.y < NORMALIZE_SNAP_EQUAL)
                    this.OnScrolledToBottom?.Invoke();
            }
            else if(v.y > this._lastScrollPos.y) // scroll up
            {
                int minNewAppear = (int)System.Math.Round((1f - v.y) *
                    (count - this._countItemAppearanceFix + 4) - 2,
                    System.MidpointRounding.AwayFromZero);

                if (minNewAppear < 0)
                    minNewAppear = 0;

                if(minNewAppear != this._minAppear)// new < old
                {
                    for(int i = this._minAppear - 1; i >= minNewAppear; --i)
                    {
                        if (_dictTrans.ContainsKey(i))
                            continue;

                        if (!_dictTrans.ContainsKey(this._maxAppear))
                        {
                            Debug.LogException(new System.Exception($"RecyclerView {this.name} Exception! OnScrollChanged scroll UP: _dictTrans not contain _maxAppear!\n"
                                                                    + $"-dicCount {_dictTrans.Count} -i {i} -minAppear {_minAppear} -maxAppear {_maxAppear} -collectionChanged {_collectionChanged}"));
                            return;
                        }
                        tmp = this._dictTrans[this._maxAppear];
                        this._dictTrans.Remove(this._maxAppear--);
                        
                        if (_dictTrans.ContainsKey(i))
                        {
                            Debug.LogException(new System.Exception($"RecyclerView {this.name} Exception! OnScrollChanged scroll UP: _dictTrans contained index i already! Trying to add will result in a duplicate!\n"
                                                                    + $"-dicCount {_dictTrans.Count} -i {i} -minAppear {_minAppear} -maxAppear {_maxAppear} -collectionChanged {_collectionChanged}"));
                            return;
                        }
                        this._dictTrans.Add(i, tmp);
                        this._callParseItem(tmp, i);

                        tmp.localPosition = new Vector3(0.5f,
                            - this.paddingTop - (i * (this.heightItem + this.spacing)),
                            0f);
                        this._minAppear = i;
                    }
                    //this._minAppear = minNewAppear;
                }
                if (v.y > (1f - NORMALIZE_SNAP_EQUAL))
                    this.OnScrolledToTop?.Invoke();
            }
            this._lastScrollPos = v;
        }


#if UNITY_EDITOR
        [ContextMenu("Force Update Scroll State")]
#endif
        private void ForceUpdateScrollState()
        {
            this.OnScrollChanged(this.scrollRect.normalizedPosition);
        }

        /// <summary>
        /// instantly scroll to an index in total count of items
        /// </summary>
        public void InstantScrollTo(int index)
        {
            float position = this.GetIndexNormalizedPosition(index);
#if UNITY_EDITOR
            Debug.Log("Instant scroll to pos: " + position);
#endif
            this.SetNewScrollPos(position);
        }

        /// <summary>
        /// smoothly scroll to an index in total count of items
        /// </summary>
        public void SmoothlyScrollTo(int index, float duration, DG.Tweening.Ease ease = Ease.Unset)
        {
            float position = this.GetIndexNormalizedPosition(index);
#if UNITY_EDITOR
            Debug.Log("Smoothly scroll to pos: " + position);
#endif

            var t = DOTween.To(
                getter: this.GetLastScrollPos,
                setter: this.SetNewScrollPos,
                endValue: position,
                duration:  duration
                );
            if (ease != Ease.Unset)
                t.SetEase(ease);
        }

        private float GetLastScrollPos()
        {
            return this._lastScrollPos.y;
        }

        private void SetNewScrollPos(float x)
        {
            this.scrollRect.normalizedPosition = new Vector2(0f, x);
        }

        private float GetIndexNormalizedPosition(int index)
        {
            float position = 1f;

            int count = this._funcGetCount();
            float countMinus = (float)count - Mathf.Floor(this.CountItemInViews());

            if (countMinus > 0f)
            {
                position -= ((float)index / countMinus);
                if (position > 1f)
                    position = 1f;
                else if (position < 0f)
                    position = 0f;
            }
            return position;
        }

        private float CountItemInViews()
        {
            return (this.rectView.rect.height) / (heightItem + spacing);
        }

#if UNITY_EDITOR
        [Header("editor")]
        public int testIndex;
        public float testDuration = 0.5f;
        public Ease testEase = Ease.Unset;
        [ContextMenu("scroll instantly to test index")]
        public void ScrollInstantlyToIndex()
        {
            this.InstantScrollTo(testIndex);
        }
        [ContextMenu("scroll smoothly to test index")]
        public void ScrollSmoothlyToIndex()
        {
            Invoker.Invoke(() =>
            this.SmoothlyScrollTo(testIndex, testDuration, testEase),
            2f);
        }

        [ContextMenu("ExpandHeight500")]
        public void ExpandHeight500()
        {
            Invoker.Invoke(this.TrulyProcExpand500, 1f);
        }
        private void TrulyProcExpand500()
        {
            var s = this.rectContent.sizeDelta;
            s.y += 500f;
            this.rectContent.sizeDelta = s;

        }
        [ContextMenu("ShrinkHeight500")]
        public void ShrinkHeight500()
        {
            Invoker.Invoke(this.TrulyProcShrink300, 1f);
        }
        private void TrulyProcShrink300()
        {
            var s = this.rectContent.sizeDelta;
            s.y -= 300f;
            this.rectContent.sizeDelta = s;

        }
#endif
    }
}