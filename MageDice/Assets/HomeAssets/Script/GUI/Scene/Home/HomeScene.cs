using Cosina.Components;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class HomeScene : BaseScene
{
    //TODO Home scene

    protected override IEnumerator Start()
    {
        SoundManager.Instance.PlayMusic("bgHome");
        //RoomDatas.callbackUnlockRoom += GiftBagConfigs.Instance.CallbackUnlockRoom;

        return base.Start(); 
    }

    public override void OnParseData()
    {
        base.OnParseData();
        if (TutorialDatas.TUTORIAL_PHASE == TutorialDatas.DONE_PHASE_AI)
        {
            this.gameObject.AddComponent<TutorialHomeStep1>();
        }
        else
        {
            //check show rate
            if (TutorialDatas.IsTutorialCompleted())
            {
                if (RateGameDialog.isShowRate)
                {
                    Invoker.Invoke(ShowRateDialog, 1f);
                }
                //check show offer
                //else if (StoreSpecialData.Instance.IsShowSpOfferAtHome())
                //{
                //    Invoker.Invoke(ShowSpecialOffer, 1f);
                //}
            }
        }
    }

    private void ShowSpecialOffer()
    {
        //GameManager.Instance.OnShowDialogWithSorting<SpecialOfferDialog>("Home/GUI/Dialogs/SpecialOffer/SpecialOfferDialog",
        //    PopupSortingType.CenterBottomAndTopBar);
    }

    private void ShowRateDialog()
    {
        GameManager.Instance.OnShowDialogWithSorting<RateGameDialog>("Home/GUI/Dialogs/RateGame/RateGameDialog", PopupSortingType.OnTopBar);
    }


    public override void OnClear()
    {
        base.OnClear();
        SoundManager.Instance.StopMusic("bgHome");
    }

    private void Update()
    {
    //    double secondRemain = 0;
    //    BattlepassDatas.Instance.IsOutDate(ref secondRemain);

    //    if (secondRemain < 0)
    //        Debug.Log("PASS OUTDATE");
    //    else
    //    {
    //        System.TimeSpan time = System.TimeSpan.FromSeconds(secondRemain);

    //        //here backslash is must to tell that colon is
    //        //not the part of format, it just a character that we want in output
    //        string str = time.ToString(@"dd\:hh\:mm\:ss");

    //        Debug.Log(string.Format("Pass outdate in {0}", str));
    //    }
    }
}
