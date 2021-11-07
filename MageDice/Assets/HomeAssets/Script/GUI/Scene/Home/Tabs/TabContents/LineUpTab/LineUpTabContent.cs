using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class LineUpTabContent : TabContent
{
    //TODO line up content
    public override void OnHide(int index, object data = null, UnityAction callback = null)
    {
        base.OnHide(index, data, callback);
    }

    public void OnClickChangeOutfit()
    {
        HideLayout();

        Debug.Log("Click change outfit");
        SoundManager.Instance.PlayButtonClick();
    }

    private void HideLayout()
    {
        HomeTabs.Instance.ShowLayout(HomeTabs.LayoutHome.LayoutMid, false);
        HomeTabs.Instance.ShowLayout(HomeTabs.LayoutHome.LayoutBottom, false);
        
    }

    private void ShowLayout()
    {
        HomeTabs.Instance.ShowLayout(HomeTabs.LayoutHome.LayoutMid, true);
        HomeTabs.Instance.ShowLayout(HomeTabs.LayoutHome.LayoutBottom, true);

    }
}
