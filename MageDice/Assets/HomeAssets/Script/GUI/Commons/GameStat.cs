using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class GameStat : MonoBehaviour
{
    public TextMeshProUGUI txtStats;
    public Image imgColorSpike;
    public RectTransform rectSpike;

    private readonly Color[] COLOR_SPIKE_GROUP = new[]
    {
        new Color(1f, 0f, 0f, 0.5f),
        new Color(0.8f, 0.5f, 0.5f, 0.5f),
        new Color(0.85f, 0.8f, 0f, 0.5f),
        new Color(0f, 0.8f, 1f, 0.5f),
        new Color(0f, 1f, 0f, 0.5f)
    };
    
    private float[] timeFrames;
    private int index = 0;

    private void Awake()
    {
        this.timeFrames = new float[30];
    }

    private void Update()
    {
        this.timeFrames[index] = Time.deltaTime;
        if ((++index) == this.timeFrames.Length)
        {
            float avgTime = timeFrames.Average();
            this.UpdateStats(fps: Mathf.Round((1f/avgTime) * 10f) / 10f, timeLongest: timeFrames.Max());
            this.index = 0;
        }
    }

    private void UpdateStats(float fps, float timeLongest)
    {
        this.txtStats.text = $"FPS: {fps}";

        this.rectSpike.sizeDelta = new Vector2(50f, timeLongest * 5000f);
        if (timeLongest < 0.017f)
        {
            this.imgColorSpike.color = COLOR_SPIKE_GROUP[4];
        }
        else if (timeLongest < 0.03f)
        {
            this.imgColorSpike.color = COLOR_SPIKE_GROUP[3];
        }
        else if (timeLongest < 0.05f)
        {
            this.imgColorSpike.color = COLOR_SPIKE_GROUP[2];
        }
        else if (timeLongest < 0.1f)
        {
            this.imgColorSpike.color = COLOR_SPIKE_GROUP[1];
        }
        else
        {
            this.imgColorSpike.color = COLOR_SPIKE_GROUP[0];
        }
    }
}
