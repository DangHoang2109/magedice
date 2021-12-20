using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public enum GameState
{
    WAITING = 0,
    PLAYING = 1,
    PAUSING = 2,
    ENDING = 3,
}
public class MageDiceGameManager : MonoSingleton<MageDiceGameManager>
{
    [SerializeField] private Transform _tfTower;
    public Transform TfTower => this._tfTower;

    [SerializeField] private Vector3 _tfMaxSafeRight;
    public Vector3 TfMaxSafeRight => this._tfMaxSafeRight;

    [SerializeField] private Vector3 _tfMaxSafeLeft;
    public Vector3 TfMaxSafeLeft => this._tfMaxSafeLeft;

    public GamWaveController WaveController;
    public GameCoinController CoinController;

    private GameBoardManager _GameBoardManager;
    [Header("UI")]
    public MageBehavior Mage;
    [SerializeField] private MageGameUI UI;


    //data
    JoinGameStandardDatas _gameData;
    GameState _state;

    private void Start()
    {
        StartCoroutine(ieStartGame());
    }

    IEnumerator ieStartGame()
    {
        yield return new WaitForEndOfFrame();
        _state = GameState.WAITING;

        this._gameData = JoinGameHelper.Instance.GetJoinGameData<JoinGameStandardDatas>();
        if (_gameData == null || this._gameData.mapConfig == null)
        {
            _gameData = new JoinGameStandardDatas(null, RoomConfigs.Instance.GetRoom(1));
        }
        OnStartGame();
    }

    public void Replay()
    {
        GameManager.Instance.OnLoadScene(SceneName.GAME);
    }
    protected virtual void OnStartGame()
    {
        PerkDatas PerkDatas = PerkDatas.Instance;

        this.WaveController = new GamWaveController();
        this.WaveController.StartGame(this._gameData.mapConfig);
        this.WaveController.OnWaveChange += OnWaveChange;

        MageGameData mageData = new MageGameData(MageGameConfigs.Instance.Mage);
        mageData.SetPerk(PerkDatas.GetCurrentStat(PerkID.RECOVER_HP), PerkDatas.GetCurrentStat(PerkID.TOTAL_HP));
        this.Mage.Spawned(mageData);

        this.CoinController = new GameCoinController();
        this.CoinController.OnCostNextDiceChange += OnPriceSpawnDiceChange;
        this.CoinController.OnCurrentCoinChange += OnCurrentCoinChange;
        this.CoinController.StarGame(MageGameConfigs.Instance.CoinIncrement);
        this.CoinController.SetWallet(this.Mage.InitCoin + (long)PerkDatas.GetCurrentStat(PerkID.STARTING_COIN));

        _GameBoardManager = GameBoardManager.Instance;
        _GameBoardManager.JoinGame(_gameData.userStatDeck);
        _GameBoardManager.SetPerkData(PerkDatas);
        _GameBoardManager.StartPlay();

        MonsterManager.Instance.StartGame(this._gameData.mapConfig);
        MonsterManager.Instance.SetPerk(PerkDatas.GetCurrentStat(PerkID.KILL_MONS_BONUS));
        this._state = GameState.PLAYING;
    }

    private void OnWaveChange(int wave)
    {
        //show popup wave
        this.UI.waveNotify.Show(wave);
        //update text wave top
        this.UI.tmpCurrentWave.SetText($"{wave} WAVE");

        this.Mage.NextWave();
    }
    private void OnPriceSpawnDiceChange(long price)
    {
        this.UI.tmpPriceNextDice.SetText($"{price}");
    }
    private void OnCurrentCoinChange(long current)
    {
        this.UI.tmpCurrentCoin.SetText($"{current}");
    }
    public bool OnBuyUpgrade(long cost)
    {
        return this.CoinController.UseCoin(cost);
    }
    public bool OnSpawnDice()
    {
        return this.CoinController.BuyDice();
    }
    public void OnAddHP(float hpAdd)
    {
        this.Mage.AddHP(hpAdd);
    }
    public void OnAddCoin(long coin)
    {
        this.CoinController.AddCoin(coin);
    }

    public void OnKillMonster(long monsCoin)
    {
        this.CoinController.AddCoin(monsCoin + Mage.AddCoinKillMons);
    }
    public void OnPauseGame(bool isPause)
    {
        this._state = isPause ? GameState.PAUSING : GameState.PLAYING;
        this._GameBoardManager.OnPauseGame(isPause);
        MonsterManager.Instance.OnPauseGame(isPause);
        BulletManager.Instance.OnPauseGame(isPause);
    }
    public void OnEndGame(bool isWIn)
    {
        if (this._state == GameState.ENDING)
            return;

        this.OnPauseGame(true);

        this._state = GameState.ENDING;
        this._gameData.SetPrize(new List<BoosterCommodity>()
        {
            GetCoinReward(WaveController.CompletedtWave)
        }, WaveController.CompletedtWave);

        JoinGameHelper.Instance.EndGameAI(isWIn);
        //show dialog end game
        EndGameStandardDialog d = GameManager.Instance.OnShowDialog<EndGameStandardDialog>("Games/GUI/EndGame/EndStandardGameDialog");
            d.ParseData(this._gameData, isWIn, WaveController.CompletedtWave);
    }
    private BoosterCommodity GetCoinReward(int waveConplete)
    {
        long prizePerWave = this._gameData.roomConfig.prizePerWave.GetValue()+ (long)PerkDatas.Instance.GetCurrentStat(PerkID.WAVE_REWARD);
        long value = prizePerWave * waveConplete;
        for (int i = 1; i < waveConplete / 10; i++)
        {
            value += (long)(this._gameData.roomConfig.multiplierBossWave * i * prizePerWave);
        }

        return new BoosterCommodity(key: this._gameData.roomConfig.prizePerWave.type, value: value);
    }

    public void OnFinalBossDead()
    {
        this.OnEndGame(true);
    }

    public void OnClickMenu()
    {
        GameManager.Instance.OnShowDialog<GameMenuDialog>("Games/GUI/MenuGameDialog");
    }
}

[System.Serializable]
public class MageGameUI
{
    public TextMeshProUGUI tmpCurrentCoin;
    public TextMeshProUGUI tmpPriceNextDice;
    public TextMeshProUGUI tmpCurrentWave;

    public WaveNotify waveNotify;
}