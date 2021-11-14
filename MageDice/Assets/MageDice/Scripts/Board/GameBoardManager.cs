using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
public class GameBoardManager : MonoSingleton<GameBoardManager>
{
    [SerializeField] private List<GameBoardSlot> slot;
    public List<GameBoardSlot> Slots => this.slot;

    [SerializeField] private GameBoardCollumn[] collumns;
    public GameBoardCollumn[] Collumns => this.collumns;

    [SerializeField] private GameBoardLine activeLine;
    public GameBoardLine ActiveLine => this.activeLine;

    public List<DiceID> userDicesList;

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

    public void StartPlay()
    {
        this.ActiveLine.StartMove();
    }
    public void OnPauseGame(bool isPause)
    {
        this.ActiveLine.PauseGame(isPause);
    }
    public void ClickAddDice()
    {
        if(MageGameManager.OnSpawnDice())
            AddDice(RandomDice());
    }
    public GameDiceData RandomDice(GameDiceData previous = null)
    {
        int nextDot = 1;
        if(previous != null && previous.Dot < 6)
        {
            nextDot = previous.Dot + 1;
        }

        DiceID id = userDicesList.GetRandomSafe();

        GameDiceData result = new GameDiceData();
        result.SetData<GameDiceData>(id)
            .SetDot<GameDiceData>(nextDot)
            .SetEffect<GameDiceData>();


        return result;
    }
    public void AddDice(GameDiceData dice)
    {
        GameBoardCollumn freeCollumn = this.collumns.Where(x => !x.IsFull).ToList().GetRandomSafe();
        if (freeCollumn == null)
            return;

        GameDiceItem item = Instantiate(prefabDice);//must get from pool
        item.SetData(dice);
        freeCollumn.PlaceDice(item);
    }

    /// <summary>
    /// Merge 2 dice lại
    /// </summary>
    /// <param name="diceReplace">Viên dice nằm, bị thay thế bởi dice khác</param>
    /// <param name="diceReturn">Viên dice drag, bị thu hồi về pool</param>
    public void MergeDice(GameDiceItem diceReplace, GameDiceItem diceReturn)
    {
        GameDiceData newData = RandomDice(diceReplace.Data);
        diceReplace.SetData(newData);

        diceReturn.UnPlace();
        Destroy(diceReturn.gameObject);
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
            .SetEffect<GameDiceData>();

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
