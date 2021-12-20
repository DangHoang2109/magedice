using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using UnityEngine.UI;
using TMPro;
public class EndGameStandardDialog : EndGameDialog
{
    [Header("reward")]
    public Transform rewardPanel;
    public IBooster bstCoinWin;
    public BagBoosterUI bstBag;

    public GameObject gButtonX2Reward;

    [Header("Result")]
    public GameObject gWin;
    public GameObject gLose;

    public TextMeshProUGUI tmpMapName;

    [Header("Progress")]
    public Image fillProgress;
    public TextMeshProUGUI tmpProgress;
    public GameObject gProgress;

    protected JoinGameStandardDatas _gameData;
    protected bool _iswin;


    public void ParseData(JoinGameStandardDatas gameData, bool isWin, int roundCompleted)
    {
        this._iswin = isWin;
        this._gameData = gameData;

        ParseReward();
        ShowResult();
        ShowFillProgress(roundCompleted, gameData.mapConfig.waves.Count);
        ShowButton();
    }

    public override void ParseReward()
    {
        base.ParseReward();

        bstCoinWin.ParseBooster(this._gameData.GetPrizes().Find(x => x.type == BoosterType.COIN));
        this.bstBag.ParseBag(this._gameData.bagReward);


    }
    public override void ShowResult()
    {
        base.ShowResult();

        this.gWin.SetActive(_iswin);
        this.gLose.SetActive(!_iswin);

        this.tmpMapName.SetText(this._gameData.mapConfig.GetName());
    }

    public void ShowFillProgress(int roundComplete, int waveAmout)
    {
        if (!_iswin)
        {
            this.fillProgress.fillAmount = 0;
            this.gProgress.SetActive(true);
            tmpProgress.SetText($"{roundComplete}/{waveAmout}");
            this.fillProgress.DOFillAmount((float)roundComplete / waveAmout, 0.25f);
        }
    }
    public void ShowButton()
    {
        this.gButtonX2Reward.SetActive(this._gameData.GetPrizes().Find(x => x.type == BoosterType.COIN) != null);

        this.canvasRewardButton.DOFade(1f, 0.25f);
        this.canvasInterractButton.DOFade(1f, 0.25f).SetDelay(3f);
    }
    protected override void Clear()
    {
        base.Clear();

        this.canvasInterractButton.alpha = 0;
        this.canvasRewardButton.alpha = 0;
        this.gProgress.SetActive(false);
    }

    protected override void OnAdsSuccess()
    {
        base.OnAdsSuccess();

        this.canvasRewardButton.alpha = 0;

        UserProfile.Instance.AddBooster(BoosterType.COIN, this._gameData.GetPrizes().Find(x => x.type == BoosterType.COIN).GetValue(), "Win_Game", LogSourceWhere.COIN_DOUBLE_WIN_GAME);
        this.bstCoinWin.ParseBooster(new BoosterCommodity(BoosterType.COIN, this._gameData.GetPrizes().Find(x => x.type == BoosterType.COIN).GetValue() * 2));
        FxHelper.Instance.ShowFxCollectBooster(BoosterType.COIN, this.gButtonX2Reward.transform, this.rewardPanel);
    }

    public void OnClickReplay()
    {
        MageDiceGameManager.Instance.Replay();
    }
}
