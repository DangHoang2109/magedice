using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Events;

public class MainBagSlots : MonoSingleton<MainBagSlots>
{
    public MainBagSlot[] bagSlots;

#if UNITY_EDITOR
    private void OnValidate()
    {
        this.bagSlots = this.GetComponentsInChildren<MainBagSlot>();
    }
#endif

    private void OnEnable()
    {
        UserProfile.Instance.AddCallbackBooster(BoosterType.BAG, OnChangeBag);
    }

    private void OnDisable()
    {
        UserProfile.Instance.RemoveCallbackBooster(BoosterType.BAG, OnChangeBag);
    }

    private void OnChangeBag(BoosterCommodity booster)
    {
        if (booster!=null)
            ParseData();
    }

    private void ParseData()
    {
        List<BagSlotData> datas = BagSlotDatas.Instance.GetBagSlots();
        if (datas != null)
        {
            for (int i = 0; i < datas.Count; i++)
            {
                if (i < this.bagSlots.Length)
                {
                    this.bagSlots[i].ParseData(datas[i]);
                }
            }
            ReplayAllAnims();
        }
    }

    public void ReloadSlot(int id)
    {
        if (id < this.bagSlots.Length)
        {
            this.bagSlots[id].ReloadData();
            ReplayAllAnims();
        }
    }

    private void ReplayAllAnims()
    {
        foreach(MainBagSlot slot in this.bagSlots)
        {
            slot.ReplayAnim();
        }
    }

    private void OnChangeValueBag(BoosterCommodity booster)
    {
        ParseData();
    }

    /// <summary>
    /// Mở luôn khi nhận bag
    /// </summary>
    /// <param name="bags"></param>
    public static void OpenBagNow(List<BagAmount> bags, string source)
    {
        List<OpenBagDialog.BagModel> listBags = new List<OpenBagDialog.BagModel>();

        foreach(BagAmount bag in bags)
        {
            for(int i=0; i< bag.amount; i++)
            {
                listBags.Add(GiftBagConfigs.Instance.GetGiftBagModel(bag.bagType, bag.tour, source));
            }          
        }

        if (listBags != null)
        {
            OpenBagDialog openBag = GameManager.Instance.OnShowDialogWithSorting<OpenBagDialog>(
                    "Home/GUI/Dialogs/OpenBag/OpenBag",
                    PopupSortingType.OnTopBar,
                    listBags);
        }      
    }
    public static void OpenBagNow(BagAmount bag, string source)
    {
        List<OpenBagDialog.BagModel> listBags = new List<OpenBagDialog.BagModel>();

        for (int i = 0; i < bag.amount; i++)
        {
            listBags.Add(GiftBagConfigs.Instance.GetGiftBagModel(bag.bagType, bag.tour, source));
        }

        if (listBags != null)
        {
            OpenBagDialog openBag = GameManager.Instance.OnShowDialogWithSorting<OpenBagDialog>(
                    "Home/GUI/Dialogs/OpenBag/OpenBag",
                    PopupSortingType.OnTopBar,
                    listBags);
        }
    }
    public static OpenBagDialog OpenBagNow(BagType bagType, int tour, string source)
    {
        OpenBagDialog openBag = GameManager.Instance.OnShowDialogWithSorting<OpenBagDialog>(
                    "Home/GUI/Dialogs/OpenBag/OpenBag",
                    PopupSortingType.OnTopBar,
                    GiftBagConfigs.Instance.GetGiftBagModel(bagType, tour, source)); //RoomDatas.Instance.GetRoomUnlockedMax()
        return openBag;
    }
    public static OpenBagDialog OpenBagNow(BagType type, List<CardAmount> card, string source)
    {
        OpenBagDialog openBag = GameManager.Instance.OnShowDialogWithSorting<OpenBagDialog>(
                    "Home/GUI/Dialogs/OpenBag/OpenBag",
                    PopupSortingType.OnTopBar,
                    GiftBagConfigs.Instance.GetGiftBagModel(type, card, source)); //RoomDatas.Instance.GetRoomUnlockedMax()
        return openBag;
    }
    public void OpenPointBag(int tour)
    {
        OpenBagDialog openBag = GameManager.Instance.OnShowDialogWithSorting<OpenBagDialog>(
                    "Home/GUI/Dialogs/OpenBag/OpenBag",
                    PopupSortingType.OnTopBar,
                    GiftBagConfigs.Instance.GetPointbagModel(tour)); //RoomDatas.Instance.GetRoomUnlockedMax()
    }
    public void OpenTutorialBag()
    {
        OpenBagDialog openBag = GameManager.Instance.OnShowDialogWithSorting<OpenBagDialog>(
                    "Home/GUI/Dialogs/OpenBag/OpenBag",
                    PopupSortingType.OnTopBar,
                    GiftBagConfigs.Instance.GetTutorialGiftBagModel());
    }
    
}
