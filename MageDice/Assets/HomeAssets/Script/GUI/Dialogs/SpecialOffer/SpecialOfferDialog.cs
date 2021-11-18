using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class SpecialOfferDialog : BaseSortingDialog
{
    [Header("Item")]
    public SpecialOfferItem specialOfferItem;


    public override void OnShow(object data = null, UnityAction callback = null)
    {
        base.OnShow(data, callback);
        ParseData();
    }

    private void ParseData()
    {
        StoreSpecialPackageConfig special = StoreSpecialData.Instance.GetSpecial();
        if (special != null)
        {
            this.specialOfferItem.ParseConfig(special);
            this.specialOfferItem.cbCloseDialog = this.ClickCloseDialog;
        }
    }

#if UNITY_EDITOR
    [UnityEditor.MenuItem("Test/Special offer/Show")]
    private static void TestShowSpecialOffer()
    {
        SpecialOfferDialog dialog = GameManager.Instance.OnShowDialogWithSorting<SpecialOfferDialog>("Home/GUI/Dialogs/SpecialOffer/SpecialOfferDialog",
            PopupSortingType.CenterBottomAndTopBar);
    }

    [UnityEditor.MenuItem("Test/Special offer/Create special")]
    private static void TestOutOfTime()
    {
        //Xóa special
        StoreSpecialData.Instance.CreateSpecial();
    }
#endif
}
