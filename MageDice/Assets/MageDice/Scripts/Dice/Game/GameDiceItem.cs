using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using System.Linq;
public class GameDiceItem : BaseDiceItem
{
    public GameDiceData Data => this.GetData<GameDiceData>();
    private GameBoardManager _BoardManager;
    private GameBoardManager BoardManager
    {
        get
        {
            if (this._BoardManager == null)
                this._BoardManager = GameBoardManager.Instance;

            return _BoardManager;
        }
    }

    [Header("Game UI")]
    public GameObject gDot;
    public Image[] dots;
    public GameObject gBlock;

    public override void SetData<T>(T data)
    {
        base.SetData(data);
    }
    public override void Display()
    {
        base.Display();

        DisplayDot();
    }
    public virtual void DisplayDot()
    {
        int dot = this.Data.Dot;
        if (dot > 0 && dot <= 6)
        {
            this.gDot.SetActive(true);
            DiceDotConfig config = DiceConfigs.Instance.GetDotConfig(dot);

            if (config != null)
            {
                for (int i = 0; i < this.dots.Length; i++)
                {
                    if (i < config.positions.Count)
                    {
                        dots[i].color = this.Data.color;
                        dots[i].gameObject.SetActive(true);
                        dots[i].transform.localPosition = config.positions[i];
                    }
                    else
                        dots[i].gameObject.SetActive(false);
                }
            }
        }
        else
            this.gDot.SetActive(false);
    }

    public virtual void Place(GameBoardSlot slot)
    {
        this.currentSlot = slot;
    }
    public virtual void UnPlace()
    {
        this.currentSlot.Clear();
    }
    public void Active()
    {
        if (this.Data != null && this.interactState != STATE.BLOCKING)
            this.Data.ActiveEffect();
    }
    
    public void Block(bool isBlock)
    {
        this.interactState = isBlock ? STATE.BLOCKING : STATE.IDDLE;
        gBlock.SetActive(isBlock);
    }

    #region Interact handler
    protected GameBoardSlot currentSlot;
    protected override void OnCustomBeginDrag(PointerEventData eventData)
    {
        base.OnCustomBeginDrag(eventData);

        this.transform.SetParent(BoardManager.transform);
        this.transform.SetAsLastSibling();
    }
    protected override void OnCustomDrag(PointerEventData eventData)
    {
        base.OnCustomDrag(eventData);
    }
    protected override void OnCustomEndDrag(PointerEventData eventData)
    {
        base.OnCustomEndDrag(eventData);

        //get nearest slot
        List<GameBoardSlot> slots = BoardManager.Slots;
        GameBoardSlot nearestSlot = slots.OrderBy(x => GameUtils.DistanceBetween(x.transform.position, this.transform.position)).FirstOrDefault();
        if(nearestSlot != null)
        {
            //check if slot is not empty and contain same dot + id to this
            if (nearestSlot.IsPlacing && nearestSlot != this.currentSlot)
            {
                GameDiceItem dice = nearestSlot.item;
                if (dice.interactState != STATE.BLOCKING &&
                    this.interactState != STATE.BLOCKING) 
                {
                    if(dice.Data.id == this.Data.id || dice.Data.diceEffect.IsCanMergeWithAny || this.Data.diceEffect.IsCanMergeWithAny)
                    {
                        if(dice.Data.Dot == this.Data.Dot)
                        {
                            //merge if true
                            BoardManager.MergeDice(dice, this);
                            return;
                        }
                    }
                }
            }
        }
        //return back if false
        ReturnDiceBack();
    }
    protected void ReturnDiceBack()
    {
        this.currentSlot.SetTransformDice();
    }
    protected override void OnCustomPointerClick(PointerEventData eventData)
    {
        base.OnCustomPointerClick(eventData);
    }
    #endregion Interact handler
}
