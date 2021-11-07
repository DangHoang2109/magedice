using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Animator))]
public class BaseAnimButton : Button
{
    private Animator anim;

#if UNITY_EDITOR
    protected override void OnValidate()
    {
        this.transition = Transition.Animation;
        this.anim = this.GetComponent<Animator>();
        if (this.anim != null)
        {
            if (this.anim.runtimeAnimatorController == null)
                this.anim.runtimeAnimatorController = GameAssetsConfigs.Instance.animBtController;
        }
    }
#endif
}
