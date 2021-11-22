using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
using DG.Tweening;
public class WaveNotify : MonoBehaviour
{
    public CanvasGroup canvas;
    public TextMeshProUGUI tmpWave;

    public void Show(int currentWave)
    {
        this.tmpWave.SetText($"Wave {currentWave}");

        Sequence seq = DOTween.Sequence();
        seq.Join(this.canvas.DOFade(1f, 0.25f));
        seq.AppendInterval(2f);
        seq.Append(this.canvas.DOFade(0f, 0.25f));
    }
}
