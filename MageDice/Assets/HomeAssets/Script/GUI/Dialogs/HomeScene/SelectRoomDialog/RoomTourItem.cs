using Coffee.UIEffects;
using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class RoomTourItem : RoomItem
{
    // added by mr K // tutorial first match
    public System.Action onClickPlay;
    [System.NonSerialized]
    public bool isBlockPlayByTutorial = false;
    
    [Header("Lock")]
    public Transform tranLock;
    public Transform tranIconLock;
    public IBooster bstUnlock;

    [Header("Unlock")]
    public Transform tranUnlock;

    [Header("Progress")]
    public Image imgProgress;
    public Sprite[] sprProgress;
    public TextMeshProUGUI tmpProgress;
    public TextMeshProUGUI tmpWinPoint;
    public TextMeshProUGUI tmpLosePoint;

    [Space(2f)]
    [Header("Prize and fee")]
    public IBooster bstPrize;
    public IBooster bstFee;

    [Header("Button")]
    public StateImageUI btEntry;
    public UIShiny shinyEntry;
    public HorizontalLayoutGroup horizontalLayout;

    [Header("Fx unlock")]
    public FxUnlockRoom fxUnlock;

    [Header("Pro entry")]
    public GameObject gBtnFreeEntry;
    public IBooster bstFreeEntry;

    protected override void ParseData(RoomData roomData)
    {
        base.ParseData(roomData);
        if (roomData != null && this.config!= null)
        {
            this.tmpName.SetText(string.Format("Tour {0}",this.config.id));
            this.tranLock.gameObject.SetActive(!roomData.unlocked);
            this.tranIconLock.gameObject.SetActive(!roomData.unlocked);
            this.tranUnlock.gameObject.SetActive(roomData.unlocked);
            this.bstFee.ParseBooster(this.config.fee);
            this.bstPrize.ParseBooster(this.config.prize);

            if (roomData.unlocked)
            {
                this.tmpWinPoint.SetText(this.config.pointWin.ToString());
                this.tmpLosePoint.SetText(this.config.pointLose.ToString());
                this.btEntry.ShowState(1);
                this.shinyEntry.enabled = true;

                ParseProgress();

                //Nếu là max room unlock và user chưa claim free entry
                //if (RoomDatas.Instance.GetRoomUnlockedMax() == roomData.id && BattlepassDatas.Instance.IsCanClaimFreeEntry)
                //{
                //    bstFreeEntry.ParseBooster(new BoosterCommodity(this.config.fee.type, this.config.fee.GetValue() * 3));
                //    this.gBtnFreeEntry.SetActive(true);
                //}
            }
            else
            {
                this.bstUnlock.ParseBooster(this.config.unlock);
                this.bstPrize.ShowSpriteOff();
                this.bstFee.ShowSpriteOff();
                this.btEntry.ShowState(0);
                this.shinyEntry.enabled = false;
            }
        }
        StartCoroutine(IeReOnHorizontalLayout());
    }

    private void ParseProgress()
    {
        if (this.data!=null && this.config != null)
        {
            this.tmpProgress.SetText(string.Format("{0}/{1}", this.data.point, this.config.pointMax));

            float ratioPoint = (float)this.data.point / (float)this.config.pointMax;
            this.imgProgress.fillAmount = ratioPoint;
            this.imgProgress.sprite = (ratioPoint < 1f) ? this.sprProgress[0] : this.sprProgress[1];
        }  
    }

    private IEnumerator IeReOnHorizontalLayout()
    {
        yield return new WaitForEndOfFrame();
        this.horizontalLayout.enabled = false;
        yield return new WaitForEndOfFrame();
        this.horizontalLayout.enabled = true;
    }

    public override void OnClickPlay()
    {
        // added by mr K // tutorial first match
        this.onClickPlay?.Invoke();
        if (this.isBlockPlayByTutorial)
            return;
        
        base.OnClickPlay();
        if (this.data != null && this.config!= null)
        {
            if (!data.unlocked)
            {
                Notification.Instance.ShowNotificationIcon(LanguageManager.GetString("TITLE_ROOMLOCKED", LanguageCategory.Games));
                //MessageBox.Instance.ShowMessageBox("Noice", "Please unlock room");
            }
            else
            {
               // if (UserProfile.Instance.UseBooster(this.config.fee, string.Format("Room_{0}", this.config.id), LogSinkWhere.JOIN_ROOM))
               if(UserProfile.Instance.IsCanUseBooster(this.config.fee.type, this.config.fee.GetValue())) 
               {
                    //TEMP
                   this.FindPlayer();
                }
                else
                {
                    BoosterCommodity coin = UserBoosters.Instance.GetBoosterCommodity(this.config.fee.type);
                    long coinNeed = this.config.fee.GetValue() - coin.GetValue();
                    //NeedMoreCoinDialogs dialog =
                    //    GameManager.Instance.OnShowDialogWithSorting<NeedMoreCoinDialogs>("Home/GUI/Dialogs/NeedMoreCoin/NeedMoreCoinDialog",
                    //        PopupSortingType.CenterBottomAndTopBar);
                    //dialog?.ParseData(coinNeed, "Upgrade_Card", () =>
                    //{
                    //    this.FindPlayer();
                        
                    //});
                    //MessageBox.Instance.ShowMessageBox("Noice", "Not enough fee");
                }
            }
        }
        else
        {
            if (this.data == null) Debug.LogError("Room data is NULL");
            if (this.config == null) Debug.LogError("Room config is NULL");
        }
        
    }

    private void FindPlayer()
    {
        StandardPlayer player = JoinGameHelper.GetStandardMainUser();
        StandardPlayer opponent = JoinGameHelper.RandomStandardPlayerByRoom(this.config);

        //TODO trận đánh đầu tiên từ khi mở game
        bool MainFirstMatchOpenApp = false;

        FidingPlayerDialog dialog = GameManager.Instance.OnShowDialogWithSorting<FidingPlayerDialog>("Home/GUI/Dialogs/FindingPlayer/FindingPlayerDialog", PopupSortingType.OnTopBar);
        dialog?.ShowFidingPlayer(player, opponent, this.config, () =>
        {
            UserProfile.Instance.UseBooster(this.config.fee, string.Format("Room_{0}", this.config.id),
                LogSinkWhere.JOIN_ROOM);
            JoinGameHelper.Instance.JoinRoom(MainFirstMatchOpenApp , player, opponent, this.config, GameType.AI);
            //Move to game scene                       
        }); 
    }

    public override void ShowEffectUnlock(UnityAction callback = null)
    {
        base.ShowEffectUnlock(callback);

        if (this.config != null)
        {
            Transform tranPopup = GameManager.Instance.GetScene().dialog;
            FxUnlockRoom fxUnlock = Instantiate<FxUnlockRoom>(this.fxUnlock, tranPopup);
            fxUnlock.ShowEffect(this.config.id, tranPopup.TransformPoint(this.tranIconLock.localPosition), () =>
            { 
                this.transform.DOPunchScale(new Vector3(-0.1f, -0.1f), 0.2f).SetId(this);
                ReloadRoom();
                callback?.Invoke();
            });

            this.tranIconLock.gameObject.SetActive(false);
        } 
    }

    public override void OnClickInfo()
    {
        base.OnClickInfo();
        TourInfoDialog tourInfoDialog = GameManager.Instance.OnShowDialogWithSorting<TourInfoDialog>("Home/GUI/Dialogs/HomeScene/TourInfo/TourInfoDialog", PopupSortingType.OnTopBar);
        tourInfoDialog.ParseConfig(this.config);
    }

    public override void OnClickPreview()
    {
        base.OnClickPreview();
        Notification.Instance.ShowNotificationIcon(LanguageManager.GetString("TITLE_COOMINGSOON"));
    }

    public void OnClickClaimFreeEntry()
    {
        BoosterCommodity b = new BoosterCommodity(this.config.fee.type, this.config.fee.GetValue() * 3);
        this.gBtnFreeEntry.SetActive(false);
        //BattlepassDatas.Instance.ClaimFreeEntry(b);
        FxHelper.Instance.ShowFxCollectBooster(b, this.gBtnFreeEntry.transform);

    }
}
