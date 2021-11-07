using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class UserProfileDialog : BaseSortingDialog
{
    public ProfileTabBase tabBase;
    public override void OnShow(object data = null, UnityAction callback = null)
    {
        base.OnShow(data, callback);
        this.tabBase.Init();
    }
    
}
