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
    //public IBooster bstUnlock;

    [Header("Unlock")]
    public Transform tranUnlock;
    public HorizontalLayoutGroup horizontalLayout;
    public IBooster bstPrizePerWave;

    //[Header("Progress")]
    //public RoomScoreProgress scoreProgress;

    //[Space(2f)]
    //[Header("Prize and fee")]
    //public IBooster bstPrize;
    //public IBooster bstFee;

    [Header("Button")]
    public StateImageUI btEntry;
    public UIShiny shinyEntry;

    [Header("Fx unlock")]
    public FxUnlockRoom fxUnlock;

    [Header("Pro entry")]
    public GameObject gBtnFreeEntry;
    public IBooster bstFreeEntry;

    //private bool isFullPoint; //full thanh trophy

    protected override void ParseData(RoomData roomData)
    {
        base.ParseData(roomData);
        if (roomData != null && this.config != null)
        {
            this.tmpName.SetText(this.config.name);
            this.tranLock.gameObject.SetActive(!roomData.unlocked);
            this.tranIconLock.gameObject.SetActive(!roomData.unlocked);
            this.tranUnlock.gameObject.SetActive(roomData.unlocked);
            //this.bstFee.ParseBooster(this.config.fee);
            //this.bstPrize.ParseBooster(this.config.prizePerWave);

            if (roomData.unlocked)
            {
                this.bstPrizePerWave.ParseBooster(this.config.prizePerWave);
                //this.scoreProgress.ParseWinLoseScore(this.config.pointWin, this.config.pointLose);
                this.btEntry.ShowState(1);
                this.shinyEntry.enabled = true;

                ParseProgress();

                //Nếu là max room unlock và user chưa claim free entry
                //if (RoomDatas.Instance.GetRoomUnlockedMax() == roomData.id && BattlepassDatas.Instance.IsCanClaimFreeEntry)
                //{
                //    bstFreeEntry.ParseBooster(new BoosterCommodity(this.config.fee.type, this.config.fee.GetValue() * 3));
                //    this.gBtnFreeEntry.SetActive(true);
                //}

                //this.isFullPoint = roomData.point >= config.pointMax;
            }
            else
            {
                //this.bstUnlock.ParseBooster(this.config.unlock);
                //this.bstPrize.ShowSpriteOff();
                //this.bstFee.ShowSpriteOff();
                this.btEntry.ShowState(0);
                this.shinyEntry.enabled = false;
            }

            ////parse glove collection
            //this.scoreProgress.ParseGlove(this.config.idGloveCollection, this.isFullPoint, ClickGlove);
        }
        StartCoroutine(IeReOnHorizontalLayout());
    }

    private void ParseProgress()
    {
        //if (this.data != null && this.config != null)
        //{
        //    this.scoreProgress.ParseProgreess(this.data.point, this.config.pointMax);
        //}
    }

    /// <summary>
    /// callback khi click glove item
    /// </summary>
    /// <param name="id"></param>
    private void ClickGlove(int idGlove)
    {
        //if (this.isFullPoint)
        //{
        //    if (this.config != null)
        //    {
        //        GloveAsset gloveAsset = GloveAssets.Instance.GetGloveAsset(this.config.idGloveCollection);
        //        if (gloveAsset != null)
        //        {
        //            UserCollectionData.Instance.CollectGlove(gloveAsset.id, 1, $"Tour {this.config.id}");

        //            ////Reset point
        //            RoomDatas.Instance.SetPoint(this.config.id, 0);

        //            this.scoreProgress.AnimProgress(0, 0.4f, () => {
        //                ParseProgress();
        //                this.scoreProgress.gloveItem.ShowHighLight(false);

        //                //TODO effect
        //                Debug.Log("<color=blue>Effect glove</color>");
        //                CollectGloveDialog dialog = GameManager.Instance.OnShowDialogWithSorting<CollectGloveDialog>("Home/GUI/Dialogs/CollectGlove/CollectGloveDialog",
        //    PopupSortingType.OnTopBar);

        //                dialog.ParseGlove(this.config.idGloveCollection);
        //                dialog.ShowAnimCollect(this.scoreProgress.gloveItem.transform.position, HomeTopUI.Instance.avatarIcon.transform.position);
        //            });
        //        }
        //    }
        //}
        //else
        //{
        //    Notification.Instance.ShowNotificationIcon(LanguageManager.GetString("GLOVE_COLLECT", LanguageCategory.Games));
        //}
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
        if (this.data != null && this.config != null)
        {
            if (!data.unlocked)
            {
                Notification.Instance.ShowNotificationIcon(LanguageManager.GetString("TITLE_ROOMLOCKED", LanguageCategory.Games));
                //MessageBox.Instance.ShowMessageBox("Noice", "Please unlock room");
            }
            else
            {
                //FindPlayerCommon.UseCoinAndFindPlayer(this.config);
                FindPlayerCommon.JoinRoomAI(this.config);
            }
        }
        else
        {
            if (this.data == null) Debug.LogError("Room data is NULL");
            if (this.config == null) Debug.LogError("Room config is NULL");
        }

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
        //BoosterCommodity b = new BoosterCommodity(this.config.fee.type, this.config.fee.GetValue() * 3);
        //this.gBtnFreeEntry.SetActive(false);
        //BattlepassDatas.Instance.ClaimFreeEntry(b);
        //FxHelper.Instance.ShowFxCollectBooster(b, this.gBtnFreeEntry.transform);

    }
}
