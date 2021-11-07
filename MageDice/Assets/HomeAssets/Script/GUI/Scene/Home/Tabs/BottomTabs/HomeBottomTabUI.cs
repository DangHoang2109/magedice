using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class HomeBottomTabUI : MonoBehaviour
{
    [Header("Tab")]
    public Tab tab;

    [Header("Rect")]
    public RectTransform rect; //for scale size when selected

    [Header("UI")]
    public Animator anim;
    public CanvasGroup canvasGroup;
    public Image icon;
    public TextMeshProUGUI tmpName;
    public Transform tranHighLight;

    [Header("Noti text")]
    public NotiTextUI notiText;
    public NotiTextUI notiNum;

    public int indexTab => tab.tabIndex;

    public System.Action OnTabAnimateStarted;
    public System.Action OnTabAnimateCompleted;

#if UNITY_EDITOR
    private void OnValidate()
    {
        this.tab = this.GetComponent<Tab>();
        this.tmpName = this.GetComponentInChildren<TextMeshProUGUI>();
    }
#endif

    protected virtual void OnEnable()
    {
        OnClear();
    }

    protected virtual void OnDisable()
    {
        this.tab.onValueChanged.RemoveAllListeners();
    }

    public virtual void ShowSelectTab(int indexTab)
    {
        bool onShow = indexTab == tab.tabIndex;
        //selected => scale center
        if (onShow)
        {
            this.rect.pivot = new Vector2(0.5f, 0.5f);
        }
        else
        {
            //on left tab selected
            if (indexTab < tab.tabIndex) this.rect.pivot = new Vector2(0, 0.5f);
            //on right tab selected
            else this.rect.pivot = new Vector2(1, 0.5f);
        }

        this.anim.SetBool("Selected", onShow);

        DOTween.Kill(this);
        Sequence seq = DOTween.Sequence();
        seq.Join(this.canvasGroup.DOFade(onShow ? 1 : 0.6f, 0.4f));
        seq.Join(this.icon.transform.DOLocalMoveY(onShow ? 16 : 6, 0.4f));
        seq.Join(this.icon.DOColor(onShow ? Color.green : Color.white, 0.4f));
        seq.Join(this.icon.transform.DOScale(onShow ? Vector3.one : new Vector3(0.95f, 0.95f), 0.4f));
        seq.Join(this.tmpName.DOColor(onShow ? Color.green : Color.white, 0.4f));
        seq.Join(this.tmpName.transform.DOScale(onShow ? Vector3.one : new Vector3(1, 0), 0.4f));
        seq.Join(this.rect.DOSizeDelta(onShow ? new Vector2(200, 110) : new Vector2(160, 110), 0.25f));
        seq.AppendCallback(this.OnTabShowCompleted);
        seq.SetId(this);
        
        this.OnTabAnimateStarted?.Invoke();
    }

    private void OnTabShowCompleted()
    {
        this.OnTabAnimateCompleted?.Invoke();
    }
    
    
    public void ShowHighLight(bool onShow)
    {
        this.tranHighLight.gameObject.SetActive(onShow);
        if (onShow) this.canvasGroup.alpha = 1f;
    }

    public void HideAllNoti()
    {
        this.notiText.HideNoti();
        this.notiNum.HideNoti();
    }

    public void ShowNotiText(string content)
    {
        this.notiText.ShowNoti(content);
    }

    public void ShowNotiNum(string content)
    {
        this.notiNum.ShowNoti(content);
    }

    public void OnClear()
    {
        this.HideAllNoti();
        this.ShowHighLight(false);
        ShowSelectTab(-1);
    }
}
