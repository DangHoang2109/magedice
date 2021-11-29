using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using UnityEngine.Events;

public class GameBoardManager : MonoSingleton<GameBoardManager>
{
    public class CallbackEventFloat : UnityEvent<float>
    {

    }

    [SerializeField] private List<GameBoardSlot> slot;
    public List<GameBoardSlot> Slots => this.slot;

    [SerializeField] private GameBoardCollumn[] collumns;
    public GameBoardCollumn[] Collumns => this.collumns;

    [SerializeField] private GameBoardLine activeLine;
    public GameBoardLine ActiveLine => this.activeLine;

    [SerializeField] private BoosterDiceDeck boosterDeck;

    public List<DiceID> userDicesList;
    public Dictionary<DiceID,StatItemStats> dicUserDiceStartingStat;
    public Dictionary<DiceID, CallbackEventFloat> dicCallbackBoosterUpdate;


    private MageDiceGameManager _gameManager;
    public MageDiceGameManager MageGameManager
    {
        get
        {
            if (_gameManager == null)
                _gameManager = MageDiceGameManager.Instance;

            return _gameManager;
        }
    }

    public GameDiceItem prefabDice;

    //user perk
    private float perk2SpotChance;
    private float perkBulletDamage;
    private float perkBulletSpeed;
    private float perkBulletCritical;
    private float perkBarSpeed;

    public GameBoardCollumn GetCollumn(int index)
    {
        if (index >= 0 && index < this.collumns.Length)
            return this.collumns[index];

        Debug.LogError($"Index invalid {index}");
        return null;
    }

    public void ResetData()
    {
        for (int i = 0; i < this.collumns.Length; i++)
        {
            this.collumns[i].ResetData();
        }

    }
    private void OnValidate()
    {
        this.collumns = this.GetComponentsInChildren<GameBoardCollumn>();
        this.slot = new List<GameBoardSlot>(this.GetComponentsInChildren<GameBoardSlot>());
    }

    public void JoinGame(List<StatItemStats> stat)
    {
        this.dicUserDiceStartingStat = new Dictionary<DiceID, StatItemStats>();
        this.dicCallbackBoosterUpdate = new Dictionary<DiceID, CallbackEventFloat>();
        this.userDicesList = new List<DiceID>();
        foreach(StatItemStats s in stat)
        {
            userDicesList.Add(s.id);
            dicUserDiceStartingStat.Add(s.id, s);

            CallbackEventFloat e = new CallbackEventFloat();
            dicCallbackBoosterUpdate.Add(s.id, e);
        }

        this.boosterDeck.ParseData(userDicesList);
    }
    public void SetPerkData(PerkDatas data)
    {
        this.perk2SpotChance = data.GetCurrentStat(PerkID.TWO_SPOT_DICE);
        this.perkBulletCritical = data.GetCurrentStat(PerkID.CRITICAL_CHANCE);
        this.perkBulletDamage = data.GetCurrentStat(PerkID.BULLET_SPEED);
        this.perkBulletSpeed = data.GetCurrentStat(PerkID.BASE_ATTACK);
        this.perkBarSpeed = data.GetCurrentStat(PerkID.BAR_SPEED);
    }
    public void StartPlay()
    {
        this.ActiveLine.AddSpeed(perkBarSpeed);
        this.ActiveLine.StartMove();
    }
    public void OnPauseGame(bool isPause)
    {
        this.ActiveLine.PauseGame(isPause);
    }
    public BoosterDiceDeck.BoosterDiceData UpgradeBooster(BoosterDiceDeck.BoosterDiceData current)
    {
        if (MageGameManager.OnBuyUpgrade(current._priceUpgradeNext))
        {
            DiceBoosterConfigs boosterConfig = DiceConfigs.Instance.boosterConfigs;
            DiceBoosterConfig config = boosterConfig.GetLevel(current._currentLevel + 1);

            current._currentLevel++;
            current._isMax = config.isMax;
            current._currentBoostPercent = config.currentBoostPercent;
            current._priceUpgradeNext = config.costUpgradeNext;

            if (dicCallbackBoosterUpdate.ContainsKey(current._id))
                dicCallbackBoosterUpdate[current._id].Invoke(current._currentBoostPercent);

            Debug.Log($"Upgrade booster {current._id} success to lv {current._currentLevel} current {current._currentBoostPercent}");
        }
        else
        {
            Debug.Log($"Not enough money {current._priceUpgradeNext}");
        }

        return current;
    }
    public void ClickAddDice()
    {
        GameBoardCollumn freeCollumn = this.collumns.Where(x => !x.IsFull).ToList().GetRandomSafe();
        if (freeCollumn == null)
            return;

        if (MageGameManager.OnSpawnDice())
            AddDice(RandomDice(), freeCollumn);
    }
    public GameDiceData RandomDice(GameDiceData previous = null)
    {
        int nextDot = 1;

        if (previous != null && previous.Dot < 6)
        {
            nextDot = previous.Dot + 1;
        }
        else
            nextDot = Random.value <= perk2SpotChance ? 2 : 1;

        DiceID id = userDicesList.GetRandomSafe();

        GameDiceData result = new GameDiceData();
        result
            .SetData<GameDiceData>(id)
            .SetPerk<GameDiceData>(perkBulletDamage: perkBulletDamage, perkBulletSpeed: perkBulletSpeed, perkBulletCritical: perkBulletCritical)
            .SetDot<GameDiceData>(nextDot)
            .SetEffect<GameDiceData>(dicUserDiceStartingStat[id], this.boosterDeck.dicBoosterDiceStat[id]._currentBoostPercent); //

        result.OnSpawned();

        this.dicCallbackBoosterUpdate[id].AddListener(result.onChangeDiceBoosterPercent);


        return result;
    }
    public void AddDice(GameDiceData dice, GameBoardCollumn freeCollumn)
    {
        if (freeCollumn == null)
             freeCollumn = this.collumns.Where(x => !x.IsFull).ToList().GetRandomSafe();

        if (freeCollumn == null)
            return;

        GameDiceItem item = Instantiate(prefabDice);//must get from pool
        item.SetData(dice);
        freeCollumn.PlaceDice(item);

        boosterDeck.OnDotChange(dice.id, TotalDotOfID(dice.id));
    }

    /// <summary>
    /// Merge 2 dice lại
    /// </summary>
    /// <param name="diceReplace">Viên dice nằm, bị thay thế bởi dice khác</param>
    /// <param name="diceReturn">Viên dice drag, bị thu hồi về pool</param>
    public void MergeDice(GameDiceItem diceReplace, GameDiceItem diceReturn)
    {
        DiceID id = diceReplace.Data.id;

        this.dicCallbackBoosterUpdate[id].AddListener(diceReplace.Data.onChangeDiceBoosterPercent);
        this.dicCallbackBoosterUpdate[id].AddListener(diceReturn.Data.onChangeDiceBoosterPercent);

        diceReplace.Data.OnMerged();
        diceReturn.Data.OnMerged();

        GameDiceData newData = RandomDice(diceReplace.Data);
        diceReplace.SetData(newData);

        diceReturn.UnPlace();
        Destroy(diceReturn.gameObject);

        boosterDeck.OnDotChange(newData.id, TotalDotOfID(newData.id));
        boosterDeck.OnDotChange(id, TotalDotOfID(id));
    }

    public int TotalDotOfID(DiceID id)
    {
        return this.Slots.Where(x => x.IsPlacing && x.item.Data.id == id).Sum(x => x.item.Data.Dot);
    }

    public void BlockRow(bool isBlock, int row, float time = -1f)
    {
        Debug.Log($"Block Row {isBlock} {row}");
        for (int i = 0; i < this.Collumns.Length; i++)
        {
            Collumns[i].Block(isBlock, row);
        }

        StartCoroutine(ieWait(time, () =>
        {
            for (int i = 0; i < this.Collumns.Length; i++)
            {
                Collumns[i].Block(!isBlock, row);
            }
        }));
    }
    private IEnumerator ieWait(float time, System.Action callback)
    {
        if (time <= 0)
            callback?.Invoke();

        yield return new WaitForSeconds(time);
        callback?.Invoke();
    }

    #region Test
    public GameDiceData GetDice(DiceID id, GameDiceData previous = null)
    {
        int nextDot = 1;
        if (previous != null && previous.Dot < 6)
        {
            nextDot = previous.Dot + 1;
        }

        GameDiceData result = new GameDiceData();
        result.SetData<GameDiceData>(id)
            .SetDot<GameDiceData>(nextDot)
            .SetEffect<GameDiceData>(dicUserDiceStartingStat[id], this.boosterDeck.dicBoosterDiceStat[id]._currentBoostPercent);

        return result;
    }
    [ContextMenu("PlaceDice/Poision")]
    private void PlaceDicePoision()
    {
        GameBoardCollumn freeCollumn = this.collumns.Where(x => !x.IsFull).ToList().First();
        if (freeCollumn == null)
            return;

        GameDiceData dice = GetDice(DiceID.POISION);
        GameDiceItem item = Instantiate(prefabDice);//must get from pool
        item.SetData(dice);
        freeCollumn.PlaceDice(item);
    }
    [ContextMenu("PlaceDice/Ice")]
    private void PlaceDiceIce()
    {
        GameBoardCollumn freeCollumn = this.collumns.Where(x => !x.IsFull).ToList().First();
        if (freeCollumn == null)
            return;

        GameDiceData dice = GetDice(DiceID.ICE);
        GameDiceItem item = Instantiate(prefabDice);//must get from pool
        item.SetData(dice);
        freeCollumn.PlaceDice(item);
    }
    #endregion
}
