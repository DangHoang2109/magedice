using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Image))]
public class StateImageUI : MonoBehaviour
{
    [SerializeField]
    private Image img;

    [SerializeField]
    private Sprite[] sprStates;

#if UNITY_EDITOR
    private void OnValidate()
    {
        if (this.img== null)
        {
            this.img = this.GetComponent<Image>();
        }
    }
#endif

    public void ShowState(int index)
    {
        if (index < sprStates.Length)
        {
            this.img.sprite = sprStates[index];
        }
    }
}


