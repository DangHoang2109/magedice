using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;


/// <summary>
/// Render a dash line (of dot), using Sprites
/// <para>
/// Main function: MakeLine(vec3, vec3)
/// </para>
/// </summary>
public class LineDashDotSprs : DoubleChannelSpriteAnimator
{
#if UNITY_EDITOR
    [Header("EDITOR")]
    public Vector3 start;
    public Vector3 end;

    [ContextMenu("Test")]
    public void TestEditorStat()
    {
        this.MakeLine(start, end);
    }

    [Header("truly stats")]
#endif


    [Tooltip("This distance is relative")]
    public float distanceDot;

    [Space(10f)]
    public float colorByTimeIntensity = -10f;
    public float colorByOrderIntensity = 0.5f;


    private void Awake()
    {
        this._colA = Color.clear;
        this._colB = Color.white;
        this._timePassed = 0f;
    }

    public void MakeLine(Vector3 start, Vector3 end)
    {
        this.transform.position = start;

        start.y = end.y = -2.2f;

        Vector3 direct = end - start;
        float distance = direct.magnitude / distanceDot;
        
        // calculate the space-count
        this._count = Mathf.RoundToInt(Mathf.Abs(distance));// count of space between dots
        direct = direct / this._count;
        
        //distanceEachDots = distance / count;// distance between each dot
        this.SpawnDot(++this._count);// count of dots


        for(int i = 1; i < this._count; ++i)
        {
            this.renderersChannelA[i].transform.localPosition = direct * i;
        }

#if UNITY_EDITOR
        this.start = start;
        this.end = end;
#endif
    }

    

    private void SpawnDot(int count)
    {
        _ = this.CachedTransform;

        SpriteRenderer tmp;
        if(this.renderersChannelA.Count < count)
        {
            for(int i = this.renderersChannelA.Count; i < count; ++i)
            {
                tmp = GameObject.Instantiate(this.renderersChannelA[0], this.cachedTransform);
                this.renderersChannelA.Add(tmp);
                this.renderersChannelB.Add(tmp.transform.GetChild(0).GetComponent<SpriteRenderer>());
            }

        }
        else if(this.renderersChannelA.Count > count)
        {
            for(int i = this.renderersChannelA.Count - 1; i >= count; --i)
            {
                this.renderersChannelA[i].gameObject.SetActive(false);
            }
        }
    }

    private void Update()
    {
        if (this._fade == 0 && this._colA.a == 0)
            return;

        this._colA.a = this._colB.a = this._fade;
        if(this._fade == 0)// fade
        {
            for (int i = this._count - 1; i >= 0; --i)
            {
                this.renderersChannelA[i].color = this._colA;
                this.renderersChannelB[i].color = this._colB;
            }
            return;
        }

        // no fade
        this._timePassed += Time.deltaTime;
        for(int i = this._count - 1; i >= 0; --i)
        {
            this._colA.r = Mathf.Sin(this._timePassed * this.colorByTimeIntensity
                + i * this.colorByOrderIntensity) + 0.4f;
            this._colA.g = Mathf.Sin(this._timePassed * this.colorByTimeIntensity
                + i * this.colorByOrderIntensity + 2 * Mathf.PI / 3) + 0.4f;
            this._colA.b = Mathf.Sin(this._timePassed * this.colorByTimeIntensity
                + i * this.colorByOrderIntensity + 4 * Mathf.PI / 3) + 0.4f;

            this.renderersChannelA[i].color = this._colA;
            this.renderersChannelB[i].color = this._colB;
        }
    }
}
