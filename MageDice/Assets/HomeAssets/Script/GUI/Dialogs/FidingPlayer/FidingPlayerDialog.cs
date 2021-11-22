using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using TMPro;
using UnityEngine;
using Random = System.Random;
using UnityEngine.UI;
using UnityEngine.Events;

public class FidingPlayerDialog : BaseSortingDialog
{
    [Header("Player")]
    public FindingMainPlayer player;
    public FindingOppPlayer opponent;
    public CanvasGroup cgVs;
    public Transform tranPlayers;

    //[Header("Chart compare")]
    //public StatisticsCompareUI statisticCompare;

    [Header("UI")]
    //public Image bg;
    public Image progressFill;
    public TextMeshProUGUI textNotifi;
    public CanvasGroup cgMoneyTotal;
    public TextCurrency txtMoneyTotal;
    public Transform tranCounting;
    public GameObject btCancle;

    private string Notifi = "The match will start in ";
    private RoomConfig roomConfig;
    private Coroutine coroutineJoinGame;

    private void OnEnable()
    {
        this.cgMoneyTotal.alpha = 0;
        this.tranCounting.gameObject.SetActive(false);
        this.textNotifi.text = "";

        this.Notifi = LanguageManager.GetString("FINDPLAYER_MATCHSTARTIN", LanguageCategory.Games);
    }

    public void ShowFidingPlayer(StandardPlayer main, StandardPlayer opponent, RoomConfig config, Action callback = null)
    {
        this.roomConfig = config;

        //Không đổi bg theo room
        //RoomAssetConfig roomAsset = RoomAssetConfigs.Instance.GetRoomAsset(config.id);
        //if (roomAsset != null) this.bg.sprite = roomAsset.sprBg;

        //_mainPlayer.Setup(main, config);
        //_opponentPlayer.Setup(opponent,config);
        this.player.ParseData(main);
        this.opponent.ParseData(opponent);

        StartFinding(main, opponent,config, callback);
    }
    private void StartFinding(StandardPlayer main, StandardPlayer opponent, RoomConfig config, Action callback = null)
    {
        bool isLess3Matchs = UserDatas.Instance.careers.totalMatch <= 1; //có dưới 1 ván không, show từ ván thứ 2

        this.cgVs.alpha = 0f;
        this.player.panel.localPosition = new Vector3(-500f, 0);
        this.opponent.panel.localPosition = new Vector3(500f, 0);
        this.tranPlayers.localPosition = Vector3.zero;
       
        float duration = 3f;
        #if UNITY_EDITOR
        duration = 0.5f;
        #endif
        DOTween.Kill(this.GetInstanceID());
        Sequence seq = DOTween.Sequence();
        seq.Append(this.cgVs.DOFade(1f, duration / 12f));
        seq.Append(this.player.panel.DOLocalMoveX(0, duration / 12f).SetEase(Ease.OutBack));
        seq.Join(this.opponent.panel.DOLocalMoveX(0, duration / 12f).SetEase(Ease.OutBack));
        seq.AppendCallback(() => {
            this.btCancle.SetActive(false);
            SoundManager.Instance.PlayLoop("snd_matching");
            });
        seq.Append(this.opponent.ShowRollingV2(duration).SetId(this.GetInstanceID()));
        seq.AppendCallback(() => {
            SoundManager.Instance.StopSound("snd_matching");
            SoundManager.Instance.Play("snd_match_found");
            this.opponent.ShowInfo();
        });
        //if (config.fee.GetValue() > 0)
        //{
        //    seq.Append(this.player.ShowMoney(config.fee.GetValue()).SetEase(Ease.Linear).SetId(this.GetInstanceID()));
        //    seq.Join(this.opponent.ShowMoney(config.fee.GetValue()).SetEase(Ease.Linear).SetId(this.GetInstanceID()));
        //}
        if (config.prizePerWave.GetValue() > 0)
        {
            seq.Append(AddTotalMoney(config).SetId(this.GetInstanceID()));
        }       
        //seq.Append(this.tranPlayers.DOLocalMoveY(255f, duration / 6f).SetEase(Ease.Linear)); //đẩy player lên
        seq.OnComplete(() => this.coroutineJoinGame = StartCoroutine(WaitJumpToGame(callback)));

        /*if (!isLess3Matchs)
        {
            seq.Append(this.tranPlayers.DOLocalMoveY(255f, duration / 6f).SetEase(Ease.Linear)); //đẩy player lên

            //statstic compare (không dùng)
            //seq.Join(this.statisticCompare.Show(main, opponent, (this.roomConfig != null) ? this.roomConfig.id : 0, duration / 4f).SetDelay(duration / 8f).SetId(this.GetInstanceID()));

            seq.OnComplete(() => this.coroutineJoinGame = StartCoroutine(WaitJumpToGame(callback)));
        }
        else
        {
            seq.AppendCallback(()=>
            {
                Debug.Log("<color=yellow>Call back</color>");
                callback?.Invoke();
            });
        }*/
        seq.SetId(this.GetInstanceID());
    }

    private Sequence AddTotalMoney(RoomConfig roomConfig, float duration = 0.5f)
    {
        //ADD TO TOTAL MONEY PHARSE
        //this.txtMoneyTotal.tmpValue.text = "0";
        //long gameMoney = roomConfig.prizePerWave.GetValue();
        //long fee = roomConfig.fee.GetValue();

        Sequence seq = DOTween.Sequence();
        //seq.AppendCallback(() => SoundManager.Instance.Play("snd_collect_coin2"));
        //seq.Join(this.player.TakeAllMoney(fee, 1f).SetEase(Ease.Linear));
        //seq.Join(this.opponent.TakeAllMoney(fee, 1f).SetEase(Ease.Linear));
        //seq.Join(this.txtMoneyTotal.AddValueAnimtion(0, gameMoney, 1f).SetEase(Ease.Linear));
        //seq.Join(this.cgMoneyTotal.DOFade(1f, 0.5f).SetEase(Ease.Linear));
        //seq.AppendInterval(0.5f);
        //seq.Append(this.player.HidePrize().SetEase(Ease.Linear));
        //seq.Join(this.opponent.HidePrize().SetEase(Ease.Linear));
        //seq.SetId(this.GetInstanceID());
        return seq;
    }


    private IEnumerator WaitJumpToGame(Action callback)
    {
        /*#if UNITY_EDITOR
        yield return new WaitForEndOfFrame();
        callback?.Invoke();
        yield break;
        #endif*/
        this.tranCounting.gameObject.SetActive(true);
        float timeWaiting = 5f;
        float timeCounting = 0;
        int timeOld = 5;
        while (timeCounting < timeWaiting)
        {
            int time = Mathf.RoundToInt(timeWaiting - timeCounting);
            if (timeOld> time)
            {
                timeOld = time;
                SoundManager.Instance.Play("snd_countdown2");
            }
            this.textNotifi.text = Notifi + time.ToString() + "s...";
            this.progressFill.fillAmount = (1 - timeCounting / timeWaiting);
            timeCounting += Time.deltaTime;
            yield return null;
        }

        Debug.Log("<color=yellow>Call back</color>");
        callback?.Invoke();
    }


    public void ClickCancelFinding()
    {
        SoundManager.Instance.StopSound("snd_matching");
        DOTween.Kill(this.GetInstanceID());
        if (this.coroutineJoinGame != null) StopCoroutine(this.coroutineJoinGame);
        this.ClickCloseDialog();
    }
}
