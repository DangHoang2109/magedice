using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.ParticleSystemJobs;

public class ParticleMover : MonoBehaviour
{
    public ParticleSystem ps;
    public float duration;

    public float distanceX;
    public float offsetX;

    private ParticleSystem.ShapeModule shapeModule;

    private Coroutine actionRunning;
    
    #if UNITY_EDITOR
    private void OnValidate()
    {
        if (ps == null)
        {
            this.ps = this.GetComponent<ParticleSystem>();
            this.duration = this.ps.main.duration;
        }

    }
#endif
    private void Awake()
    {
        this.shapeModule = this.ps.shape;
    }

    public void Play()
    {
        if(this.ps.isPlaying)
            this.Stop();
        
        this.ps.Play();
        this.actionRunning = this.StartCoroutine(this.OnPlayingParticle());
    }

    public void Stop()
    {
        this.ps.Stop();
        if(this.actionRunning != null)
            this.StopCoroutine(this.actionRunning);
    }
    private IEnumerator OnPlayingParticle()
    {
        YieldInstruction endFrame = new WaitForEndOfFrame();
        
        while (this.ps.time < this.duration)
        {
            float x = (this.ps.time / this.duration) * this.distanceX + offsetX;
            shapeModule.position = new Vector3(x, 0f, 0f);
            yield return endFrame;
        }

        this.actionRunning = null;
    }
}
