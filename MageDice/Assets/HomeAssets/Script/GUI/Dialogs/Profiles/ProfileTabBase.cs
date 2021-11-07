using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ProfileTabBase : TabBase
{
    protected override void Start()
    {
        
    }

    private void OnEnable()
    {
        this.tabCurrentIndex = -1;
        //this.Init();
    }
}
