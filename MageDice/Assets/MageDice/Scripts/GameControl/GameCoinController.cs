using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameCoinController
{
    [SerializeField] private DicePriceIncrementConfig _config;
    public DicePriceIncrementConfig Config
    {
        get
        {
            if (_config == null)
                _config = MageGameConfigs.Instance.CoinIncrement;
            return _config;
        }
    }

    #region Purchase Dice
    public int TimeBuy { get => _timeBuy; set { _timeBuy = value; OnCostNextDiceChange?.Invoke(CostNextDice); } }

    [SerializeField] private int _timeBuy;

    public System.Action<long> OnCostNextDiceChange;

    public long CostNextDice
    {
        get
        {
            return Config.basePrice + (Config.increStep + TimeBuy/5) * TimeBuy;
        }
    }
    #endregion

    #region Coin Wallet
    private long _currentCoin;
    public long CurrentCoin { get => _currentCoin; set { _currentCoin = value; OnCurrentCoinChange?.Invoke(_currentCoin); } }

    public System.Action<long> OnCurrentCoinChange;

    #endregion



    public void StarGame(DicePriceIncrementConfig config)
    {
        this._config = config;
        this.TimeBuy = 0;

    }
    public void SetWallet(long coin)
    {
        this.CurrentCoin = coin;
    }
    public bool BuyDice()
    {
        if (!UseCoin(CostNextDice))
            return false;

        this.TimeBuy++;
        return true;
    }
    public bool UseCoin(long coin)
    {
        if (CurrentCoin < coin)
            return false;

        CurrentCoin -= coin;
        return true;
    }
    public void AddCoin(long coin)
    {
        this.CurrentCoin += coin;
    }
}
