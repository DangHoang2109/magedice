using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
public class MageGameData : PersonGameData
{
    public MageConfig config;

    private float currentStamina;
    private float maxStamina;

    [Header("Booster Inital stat")]
    private long initCoin;

    private long addCoinKillMons;
    private long addCoinSkipWave;
    private float recoverHPEachWave;
    #region Callback
    public UnityAction<float> onChangeStamina;
    #endregion Callback

    #region Getter stter
    public float CurrentStamina { get => currentStamina; set { currentStamina = value; onChangeStamina?.Invoke(currentStamina); } }
    public float MaxStamina { get => maxStamina; set => maxStamina = value; }
    public long InitCoin { get => initCoin; set => initCoin = value; }
    public long AddCoinKillMons { get => addCoinKillMons; set => addCoinKillMons = value; }
    public long AddCoinSkipWave { get => addCoinSkipWave; set => addCoinSkipWave = value; }
    public float RecoverHPEachWave { get => recoverHPEachWave; set => recoverHPEachWave = value; }
    #endregion Getter stter

    public MageGameData(MageConfig c) : base(c)
    {
        this.config = c;

        this.currentStamina = 0;
        this.maxStamina = c.stamina;

        ///add user save data;
        this.initCoin = c.initCoin;
        this.addCoinKillMons = c.addCoinKillMons;
        this.addCoinSkipWave = c.addCoinSkipWave;
        this.recoverHPEachWave = c.recoverHPEachWave;
    }
    public void SetPerk(float recoverHP, float addTotalHP)
    {
        this.recoverHPEachWave += recoverHP;
        this.currentHP += addTotalHP;
    }
}
public class MageBehavior : BasePersonBehavior
{
    public GameStatBar StaminaBar;
    private MageGameData Data;

    #region Getter stter
    public long InitCoin { get => this.Data.InitCoin; set => this.Data.InitCoin = value; }
    public long AddCoinKillMons { get => this.Data.AddCoinKillMons; set => this.Data.AddCoinKillMons = value; }
    public long AddCoinSkipWave { get => this.Data.AddCoinSkipWave; set => this.Data.AddCoinSkipWave = value; }
    public float RecoverHPEachWave { get => this.Data.RecoverHPEachWave; set => this.Data.RecoverHPEachWave = value; }

    #endregion Getter stter


    public override void Spawned(PersonGameData config)
    {
        this.Data = config as MageGameData;

        base.Spawned(Data);

        //set stamina
        StaminaBar.ParseData(max: this.Data.MaxStamina, current: this.Data.CurrentStamina);
        this.Data.onChangeStamina += StaminaBar.OnChangeValue;
    }

    public void NextWave()
    {
        this.AddHP(this.RecoverHPEachWave);
    }
    protected override void Dead()
    {
        base.Dead();
        MageDiceGameManager.Instance.OnEndGame(false);
    }
}
