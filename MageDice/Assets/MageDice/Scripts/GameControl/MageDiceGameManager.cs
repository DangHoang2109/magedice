using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
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

    private void Start()
    {
        OnStartGame();
    }

    protected virtual void OnStartGame()
    {
        this.WaveController = new GamWaveController();
        this.WaveController.StartGame(GameMapConfigs.Instance.GetMap(MapName.GREENLAND));
        this.WaveController.OnWaveChange += OnWaveChange;

        this.Mage.Spawned(new MageGameData(MageGameConfigs.Instance.Mage));

        this.CoinController = new GameCoinController();
        this.CoinController.OnCostNextDiceChange += OnPriceSpawnDiceChange;
        this.CoinController.OnCurrentCoinChange += OnCurrentCoinChange;
        this.CoinController.StarGame(MageGameConfigs.Instance.CoinIncrement);
        this.CoinController.SetWallet(this.Mage.InitCoin);

        _GameBoardManager = GameBoardManager.Instance;
        _GameBoardManager.StartPlay();

        MonsterManager.Instance.StartGame(GameMapConfigs.Instance.GetMap(MapName.GREENLAND));
    }

    private void OnWaveChange(int wave)
    {
        //show popup wave
        //update text wave top
        this.UI.tmpCurrentWave.SetText($"{wave + 1} WAVE");
        Debug.Log($"Wave {wave}");

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
    public bool OnSpawnDice()
    {
        return this.CoinController.BuyDice();
    }
    public void OnKillMonster(long monsCoin)
    {
        this.CoinController.AddCoin(monsCoin + Mage.AddCoinKillMons);
    }
    public void OnPauseGame(bool isPause)
    {
        this._GameBoardManager.OnPauseGame(isPause);
        MonsterManager.Instance.OnPauseGame(isPause);
        BulletManager.Instance.OnPauseGame(isPause);
    }
    public void OnEndGame(bool isWIn)
    {
        this.OnPauseGame(true);
        //show dialog end game
    }
    public void OnFinalBossDead()
    {
        this.OnEndGame(true);
    }
}

[System.Serializable]
public class MageGameUI
{
    public TextMeshProUGUI tmpCurrentCoin;
    public TextMeshProUGUI tmpPriceNextDice;
    public TextMeshProUGUI tmpCurrentWave;

}