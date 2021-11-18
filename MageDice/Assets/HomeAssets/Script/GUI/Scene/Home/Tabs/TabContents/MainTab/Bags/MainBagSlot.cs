using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class MainBagSlot : MonoBehaviour
{
    [Header("Slot")]
    public int idSlot;
   

    [Header("Not or have")]
    public Transform tranNoBag;
    public Transform tranBag;
    public Animator[] animTops;

    [Header("Bag")]
    public Image imgBag;
    public TextMeshProUGUI tmpName;

    [Header("Waitting")]
    public Transform tranWaitting;
    public TextMeshProUGUI tmpWaitting;
    public IBooster bstOpen;

    [Header("Exist")]
    public Transform tranExist;
    public TextMeshProUGUI tmpTime;

    [Header("Ready open")]
    public Transform tranReadyOpen;

    private BagSlotData data;

    private bool isWaitting;
    private double timeRemain;

    public void ParseData(BagSlotData data)
    {
        this.data = data;
        if (this.data!= null)
        {
            BagConfig bagConfig = BagConfigs.Instance.GetBag(this.data.type);
            if (bagConfig != null)
            {
                bool haveBag = data.IsExistBag();
                this.tranBag.gameObject.SetActive(haveBag);
                this.tranNoBag.gameObject.SetActive(!haveBag);
                this.isWaitting = false;

                if (haveBag)
                {
                    BagAssetConfig bagAsset = BagAssetConfigs.Instance.GetBagAsset(this.data.type);
                    if (bagAsset != null) this.imgBag.sprite = bagAsset.sprBag;
                    this.tmpName.SetText(this.data.name);
                    

                    this.tranExist.gameObject.SetActive(false);
                    this.tranWaitting.gameObject.SetActive(false);
                    this.tranReadyOpen.gameObject.SetActive(false);

                    //Debug.LogError("slot state " + this.data.state);
                    switch (this.data.state)
                    {
                        case BagSlotState.EXIST:
                            this.tranExist.gameObject.SetActive(true);
                            this.tmpName.transform.localPosition = Vector3.zero;

                            if (data.name.Equals("TUTORIAL"))
                                this.tmpTime.SetText(GameUtils.ConvertFloatToTime(data.TOTAL_TIME_WAIT, "mm'm'ss's'"));
                            else
                                this.tmpTime.SetText(GameUtils.ConvertFloatToTime(bagConfig.totalTimeWait, "hh'h'mm'm'"));

                            break;
                        case BagSlotState.WAITTING:
                            this.tranWaitting.gameObject.SetActive(true);
                            this.tmpName.transform.localPosition = new Vector3(0, 15);         
                            this.bstOpen.ParseBooster(this.data.bstUnlock);
                            this.isWaitting = true;  
                            break;
                        case BagSlotState.READY_OPEN:
                            this.tranReadyOpen.gameObject.SetActive(true);
                            this.tmpName.transform.localPosition = Vector3.zero;
                            break;
                        default:
                            break;
                    }

                }
            }
        }
    }

    public bool IsHaveBag()
    {
        return this.data != null;
    }
    public void ReloadData()
    {
        if (this.data != null)
        {
            ParseData(this.data);
        }
    }

    public void ReplayAnim()
    {
        foreach (Animator anim in this.animTops)
        {
            if (anim.gameObject.activeInHierarchy)
                anim.Play(0);
        }
    }

    private string GetTimeFormat(double time)
    {
        if (time <= 60)
            return "mm'm'ss's'";
        else
            return "hh'h'mm'm'";
    }
    private void Update()
    {
        if (this.data != null)
        {
            if (this.isWaitting)
            {
                if (!this.data.IsReadyToOpen(ref timeRemain))
                {
                    //Debug.LogError(this.data.TOTAL_TIME_WAIT - this.timeRemain);
                    double timeRemain = this.data.TOTAL_TIME_WAIT - this.timeRemain;
                    this.tmpWaitting.SetText(GameUtils.ConvertFloatToTime(timeRemain, GetTimeFormat(timeRemain)));
                    this.bstOpen.ParseBooster(this.data.bstUnlock);
                }
                else
                {
                    this.isWaitting = false;
                    ParseData(this.data);
                }
            }
        }    
    }

    public void OnClick()
    {
        if (this.data != null)
        {
            if (this.data.IsExistBag())
            {
                if (this.data.state == BagSlotState.READY_OPEN)
                {
                    Debug.LogError("Open bag");
                    MainBagSlots.OpenBagNow(this.data.type, this.data.tourID, "MainBagSlot");
                    ParseData(this.data.OpenBag());
                }
                else
                {
                    BagInfoDialog bagInfoDialog = GameManager.Instance.OnShowDialogWithSorting<BagInfoDialog>(
                        "Home/GUI/Dialogs/HomeScene/BagInfo/BagInfoDialog", PopupSortingType.CenterBottomAndTopBar);
                    bagInfoDialog.ParseData(this.data);
                }        
            }
            else
            {
                Notification.Instance.ShowNotificationIcon(LanguageManager.GetString("BAGSLOT_WINTOGET", LanguageCategory.Feature));
            }
        }
        else
        {
            Debug.LogError("Data slot is null");
        }
        //SoundManager.Instance.PlayButtonClick();
    }
}

public enum BagSlotState
{
    NONE,       //no bag
    EXIST,      //đã nhận
    WAITTING,   //đang đợi mở
    READY_OPEN        //đợi xong, có thể mở
}
