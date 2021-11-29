using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class InventoryDiceItem : BaseDiceItem
{
    private DiceID id;

    public System.Action<DiceID> OnClicked;

    public override void SetData<T>(T data)
    {
        this.id = data.id;
        //ko thể drag item này được
        this.interactState = STATE.BLOCKING;
    }
    public void SetCallback(System.Action<DiceID> onClick)
    {
        this.OnClicked = onClick;
    }

    protected override void OnCustomPointerClick(PointerEventData eventData)
    {
        Debug.Log("click");

        base.OnCustomPointerClick(eventData);
        this.OnClicked?.Invoke(this.id);
    }
    public override void Display()
    {
        base.Display();
    }
}
