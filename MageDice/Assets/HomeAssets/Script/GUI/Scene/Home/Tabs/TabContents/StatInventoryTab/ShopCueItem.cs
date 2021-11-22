using System;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;
using UnityEngine.Events;
using Cosina.Components;
using System.Collections.Generic;

public class ShopCueItem : MonoBehaviour, IIndexable
{
    [Header("linker")]
    public GameObject goMaster;
    [Space(15f)]

    
    // icon
    public Image imgCue;
    [Space(5f)]
    public Text txtCueName;

    [Space(5f)]
    //public Image imgBgName;
    //public RectTransform rectBgName;
    public Image imgBg;

    
    // stats
    [Header("part"), SerializeField]
    private StatGUIStatLines partStatLines;
    public StatGUIUpgrade partUpgrade;
    //public ShopCueBtnBuy partBuy;
    public GameObject goFrameSelected;
    
    public bool needRemoveAfterBuy = false;


    private StatData _data;
    public StatData Data
    {
        get { return this._data; }
    }

    [SerializeField]
    private int _index;
    public void SetIndex(int index)
    {
        this._index = index;
    }

    private ShopCuesTab _host;

    private CanvasGroup canvasGroup;

    private bool needToRefresh = false;


    private System.Action<ShopCueItem, int> _onDestroy;
    private bool _isFading = false;
    public bool IsFading
    {
        get { return this._isFading; }
    }


    /// <summary>
    /// using Id to check this is using or not
    /// </summary>
    public ShopCueItem ParseData(StatData data, int index, bool needFade)
    {
        // for recycler view
        this._index = index;

        // for animation
        this.goMaster.SetActive(true);
        if (this.canvasGroup == null)
            this.canvasGroup = this.goMaster.GetComponent<CanvasGroup>();
        else
        {
            this.canvasGroup.DOKill();
            this._isFading = false;
        }
        if (needFade)
        {
            this.canvasGroup.alpha = 0f;
            this.canvasGroup.DOFade(1f, 0.75f);
        }
        else
        {
            this.canvasGroup.alpha = 1f;
        }

        // parsing data
        this._data = data;

        this.name = data.config.id.ToString();

        if(txtCueName != null)
            this.txtCueName.text = data.config.statName;

        this.imgCue.sprite = data.config.sprStatItem;
        //var tier = data.config.tier;
        //this.imgBg.color = ShopCueRef.Instance.GetColorTier(tier).colorDark;

        this.Refresh(StatManager.Instance.CurrentStatId);
        
        // bind event
        StatManager.Instance.OnCueChanged += this.OnLineupUpdated;
        StatManager.Instance.OnCardBought += this.OnAStatUpdated;
        StatManager.Instance.OnCueUpgraded += OnAStatUpdated;
        
        StatManager.Instance.OnCueGained += this.OnACueGained;
        
        GameManager.Instance.OnSceneChanging += this.BackStack;

#if CHEAT
        this.ShowCheatButtons();
#endif

        // transform
        this.transform.localScale = Vector3.one;
        Vector3 v = this.transform.localPosition;
        v.z = 0;
        this.transform.localPosition = v;

        this._onDestroy = null;
        
        return this;
    }

#if UNITY_EDITOR
    [ContextMenu("Refresh")]
#endif
    private void Refresh(List<DiceID> usingStat)
    {
        switch (this._data.kind)
        {
            case StatManager.Kind.NotUnlocked:
                if(partStatLines != null)
                    this.partStatLines.ParseStatsNext(this._data);
                this.partUpgrade.ParseCueToBuy(this._data);
                break;
            case StatManager.Kind.UnlockedNonMaxed:
            case StatManager.Kind.Maxed:
                if (partStatLines != null)
                    this.partStatLines.ParseStatsCurrent(this._data);
                this.partUpgrade.ParseCueBought(this._data);
                this.CheckUseState(usingStat);
                break;
            default:
                Debug.LogException(new System.Exception("ShopCueItem Refresh: kind exception: " + this._data.kind.ToString()));
                break;
        }
    }
    
    public ShopCueItem SetRemoveAfterBuy(System.Action<ShopCueItem, int> callbackDestroy)
    {
        if (callbackDestroy != null)
        {
            this.needRemoveAfterBuy = true;
            this._onDestroy = callbackDestroy;
        }
        else
        {
            this.needRemoveAfterBuy = false;
        }
        return this;
    }

    public ShopCueItem SetHost(ShopCuesTab host)
    {
        this._host = host;

        return this;
    }

    private void CheckUseState(List<DiceID> usingStat)
    {
        this.goFrameSelected.SetActive(usingStat.Contains(this._data.id));
    }
    private void CheckUseState(List<StatData> usingStat)
    {
        this.goFrameSelected.SetActive(usingStat.Find(x => x.id == this._data.id) != null);
    }
    public void OnClickItem()
    {
        // show dialog cue detail
        var d = GameManager.Instance.OnShowDialogWithSorting<StatGUIDetailDialog>(
            "Home/GUI/StatDetail/StatDetailDialog",
            PopupSortingType.CenterBottomAndTopBar,
            this._data);
        SoundManager.Instance.PlayButtonClick();
    }

    private void OnACueGained(StatData cue)
    {
        if(cue.id.Equals(this._data.id))
        {
            if (this.needRemoveAfterBuy)
            {
                this._onDestroy?.Invoke(this, this._index);
            }
        }
    }
    
    private void BackStack()
    {
        if (this.gameObject.activeSelf)
            this.ProcReturn();
    }

    public void ProcReturn()
    {
        //if (partStatLines != null)
        //    this.partStatLines.ClearAnim();
        
        this.gameObject.SetActive(false);
        ShopCueRef.Instance.ReturnItem(this);

        StatManager.Instance.OnCueChanged -= this.OnLineupUpdated;
        StatManager.Instance.OnCardBought -= this.OnAStatUpdated;
        StatManager.Instance.OnCueUpgraded -= OnAStatUpdated;
        
        StatManager.Instance.OnCueGained -= this.OnACueGained;
        
        GameManager.Instance.OnSceneChanging -= this.BackStack;
    }
    
    private void OnAStatUpdated(StatData c)
    {
        if (c.id != this._data.id) //nếu trong lineup mới ko contain id này
        {
            //no need this
            //if (this.goFrameSelected.activeSelf)
            //{
            //    this.CheckUseState(cs);
            //}
            //else
            //{
            //    return;
            //}
        }

        if (this.gameObject.activeInHierarchy)
        {
            this.Refresh(StatManager.Instance.CurrentStatId);
        }
        else
        {
            this.needToRefresh = true;
        }
    }
    private void OnLineupUpdated(List<StatData> cs)
    {
        if (cs.Find( c => c.id == this._data.id) == null) //nếu trong lineup mới ko contain id này
        {
            if (this.goFrameSelected.activeSelf)
            {
                this.CheckUseState(cs);
            }
            else
            {
                return;
            }
        }

        if (this.gameObject.activeInHierarchy)
        {
            this.Refresh(StatManager.Instance.CurrentStatId);
        }
        else
        {
            this.needToRefresh = true;
        }
    }

    public void SetAppear(bool isShow)
    {
        this.goMaster.SetActive(isShow);
    }

    private void OnEnable()
    {
        if (this.needToRefresh)
        {
            this.Refresh(StatManager.Instance.CurrentStatId);
            this.needToRefresh = false;
        }
    }


        #region cheat
#if UNITY_EDITOR || CHEAT

    [ContextMenu("Show Cheat Buttons")]
    private void ShowCheatButtons()
    {
        this.CreateButton("cheat cards", "cards", this.CheatAdd100Cards,
            this.transform, new Vector3(-400f, 0f));

        if (this._data.kind == StatManager.Kind.NotUnlocked)
        {
            this.CreateButton("cheat buy", "buy", this.CheatUnlockThisCue,
                this.transform, new Vector3(200f, 0f));
        }
    }

    [ContextMenu("Cheat add 10 cards")]
    private void CheatAdd10Cards()
    {
        if (!Application.isPlaying)
            return;
        StatManager.Instance.AddCard(this._data, 10);
    }
    [ContextMenu("Cheat add 100 cards")]
    private void CheatAdd100Cards()
    {
        if (!Application.isPlaying)
            return;
        StatManager.Instance.AddCard(this._data, 100);
    }

    [ContextMenu("Cheat unlock this")]
    private void CheatUnlockThisCue()
    {
        if (!Application.isPlaying)
            return;

        if (this._data.kind != StatManager.Kind.NotUnlocked)
            return;

        StatManager.Instance.WinCue(this._data);
        this.Refresh(StatManager.Instance.CurrentStatId);
    }

    private GameObject CreateButton(string name, string text
        , UnityAction call, Transform parent, Vector3 localPos)
    {
        GameObject go = new GameObject(name, typeof(RectTransform), typeof(Image), typeof(Button));
        go.transform.SetParent(parent);
        RectTransform r = go.GetComponent<RectTransform>();
        r.localPosition = localPos;
        r.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, 100f);
        r.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, 70f);
        r.localScale = Vector3.one;

        Button b = go.GetComponent<Button>();
        b.image = go.GetComponent<Image>();
        b.onClick.RemoveAllListeners();
        b.onClick.AddListener(call);

        GameObject goTextCheatCard = new GameObject("t", typeof(RectTransform), typeof(Text));
        goTextCheatCard.transform.SetParent(r);
        goTextCheatCard.transform.localScale = Vector3.one;
        goTextCheatCard.transform.localPosition = Vector3.zero;
        Text t = goTextCheatCard.GetComponent<Text>();
        t.text = text;
        t.font = Resources.GetBuiltinResource(typeof(Font), "Arial.ttf") as Font;
        t.horizontalOverflow = HorizontalWrapMode.Overflow;
        t.verticalOverflow = VerticalWrapMode.Overflow;
        t.alignment = TextAnchor.MiddleCenter;
        t.fontSize = 20;
        t.color = Color.black;

        return go;
    }
#endif
        #endregion cheat
}
