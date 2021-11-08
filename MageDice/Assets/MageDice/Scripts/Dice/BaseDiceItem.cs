using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BaseDiceItem : MonoBehaviour
{
    private BaseDiceData data;
    public T GetData<T>() where T : BaseDiceData
    {
        return this.data as T;
    }

    [Header("UI")]
    public Image imgFront;

    public virtual void SetData<T>(T data) where T :BaseDiceData
    {
        this.data = data;
        Display();
    }
    public virtual void Display()
    {
        this.imgFront.sprite = this.data.Front;
    }
}
