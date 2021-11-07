using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BGTexScroller : MonoBehaviour
{
    public Vector2 speed;

    private Material mat;
    
    private void Awake()
    {
        Image img = this.GetComponent<Image>();
        this.mat = img.material = Instantiate(img.material);
    }

    private void Update()
    {
        this.mat.mainTextureOffset += this.speed * Time.deltaTime;
    }

    private void OnDestroy()
    {
        Destroy(this.mat);
    }
}
