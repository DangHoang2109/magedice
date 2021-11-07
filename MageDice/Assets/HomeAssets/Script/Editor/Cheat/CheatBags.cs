#if UNITY_EDITOR

using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class CheatBags : ScriptableWizard
{
    //public BagType bagToCheat = BagType.KING_BAG;
    //public int countCheat = 4;

    //[MenuItem("Cheats/Bags/Add...")]
    //static void CreateWizard()
    //{
    //    var w = ScriptableWizard.DisplayWizard<CheatBags>("Cheat Add Bags",
    //        "Cheat"/*, "Apply"*/);
    //    //If you don't want to use the secondary button simply leave it out:
    //    //ScriptableWizard.DisplayWizard<WizardCreateLight>("Create Light", "Create");
    //    w.helpString = "NEED TO RUN IN GAME";
    //}

    //void OnWizardCreate()
    //{
    //    for (int i = 0; i < countCheat; ++i)
    //    {
    //        if (!BagSlotDatas.Instance.CollectBag(this.bagToCheat, "Cheat","cheat", "Cheat", 1))
    //            Debug.LogError("Full bags");    
    //    }
    //}

    // void OnWizardUpdate()
    // {
    //     //helpString = "-0 start_over\n-1 done_swipe\n-2 done_bag\n-3 done_equip\n-4 done_upgrade";
    // }

    // // When the user presses the "Apply" button OnWizardOtherButton is called.
    // void OnWizardOtherButton()
    // {
    //     if (Selection.activeTransform != null)
    //     {
    //         Light lt = Selection.activeTransform.GetComponent<Light>();
    //
    //         if (lt != null)
    //         {
    //             lt.color = Color.red;
    //         }
    //     }
    // }
    //
}
#endif