using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using TMPro;
using UnityEngine.UI;

public class Notification : MonoSingleton<Notification>
{
    public CanvasGroup canNoti;
    public RectTransform panel;
    public TextMeshProUGUI tmpMessage;

    [Header("Hor layout")]
    public ContentSizeFitter contentSize;

    [Header("Icon")]
    public Transform panelIcon;
    public Image imgIcon;
    public Sprite sprIcon;

    public void ShowNotificationIcon(string message)
    {
        this.ShowNotification(message, this.sprIcon);
    }

    private IEnumerator IeShowNotification(string message, Sprite sprIcon = null)
    {
        this.tmpMessage.text = message;

        bool icon = sprIcon != null;
        if (sprIcon != null)
        {
            this.imgIcon.sprite = sprIcon;
        }
        this.panelIcon.gameObject.SetActive(icon);

        this.panel.localPosition = new Vector3(this.panel.localPosition.x, 100, 0);
        this.panel.localScale = new Vector3(0.5f, 0.5f);
        this.canNoti.gameObject.SetActive(true);
        this.canNoti.alpha = 0f;

        yield return new WaitForEndOfFrame();
        this.contentSize.enabled = false;
        yield return new WaitForEndOfFrame();
        this.contentSize.enabled = true;

        DOTween.Kill(this);
        Sequence seq = DOTween.Sequence();
        seq.Join(this.canNoti.DOFade(1, 0.2f));
        seq.Join(this.panel.DOLocalMoveY(0, 0.2f).SetEase(Ease.OutBack));
        seq.Join(this.panel.DOScale(1, 0.2f).SetEase(Ease.OutBack));
        seq.AppendInterval(1.0f);
        seq.Append(this.canNoti.DOFade(0, 0.2f).SetEase(Ease.Linear));
        seq.SetId(this);
        seq.OnComplete(() => this.canNoti.gameObject.SetActive(false));

        SoundManager.Instance.Play("snd_noti");
    }

    public void ShowNotification(string message, Sprite sprIcon = null)
    {
        StartCoroutine(IeShowNotification(message, sprIcon));
    }
}
