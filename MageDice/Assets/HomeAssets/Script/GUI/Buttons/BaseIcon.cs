using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BaseIcon : MonoBehaviour
{
    protected virtual void OnEnable()
    {
        this.Init();  
    }
    protected virtual void OnDisable()
    {
        this.Clear();
    }

    public virtual void OnShow()
    { }
    
    protected virtual void Init()
    { }
    protected virtual void Clear()
    { }
    public virtual void OnClickIcon()
    {
        SoundManager.Instance.PlayButtonClick();
    }

}
