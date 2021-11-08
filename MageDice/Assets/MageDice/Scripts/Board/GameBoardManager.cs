using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
public class GameBoardManager : MonoSingleton<GameBoardManager>
{
    [SerializeField] private GameBoardCollumn[] collumns;
    public GameBoardCollumn[] Collumns => this.collumns;

    [SerializeField] private GameBoardLine activeLine;
    public GameBoardLine ActiveLine => this.activeLine;

    public List<DiceID> userDicesList;


    ///test
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
    }

    [ContextMenu("StartPlay")]
    public void StartPlay()
    {
        this.ActiveLine.StartMove();
    }

    public void ClickAddDice()
    {
        Debug.Log("cl add dice");
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
            .SetDot<GameDiceData>(nextDot);

        Debug.Log($"add dice {id} {result.id} {nextDot} {result.Dot}");

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
}
