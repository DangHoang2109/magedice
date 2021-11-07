using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CustomGroupToogle : MonoBehaviour
{
    [SerializeField] private bool m_AllowSwitchOff = false;

    public bool allowSwitchOff { get { return m_AllowSwitchOff; } set { m_AllowSwitchOff = value; } }

    public List<CustomToogle> m_Toggles = new List<CustomToogle>();
    #if UNITY_EDITOR
    private void OnValidate()
    {
        this.EnsureValidState();
        CustomToogle[] temps = this.GetComponentsInChildren<CustomToogle>();
        this.m_Toggles = new List<CustomToogle>(temps);
    }
#endif
    
    // Start is called before the first frame update
    protected void Start()
    {
        this.EnsureValidState();
    }
    public void EnsureValidState()
    {
        if (!allowSwitchOff && !AnyTogglesOn() && m_Toggles.Count != 0)
        {
            m_Toggles[0].IsOn = true;
        }
    }
    public bool AnyTogglesOn()
    {
        return m_Toggles.Find(x => x.IsOn) != null;
    }

}
