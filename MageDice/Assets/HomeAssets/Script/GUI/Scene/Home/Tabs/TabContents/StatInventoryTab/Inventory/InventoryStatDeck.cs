using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InventoryStatDeck : MonoBehaviour
{
    [SerializeField] protected List<InventoryStatSlot> slots;


#if UNITY_EDITOR
    private void OnValidate()
    {
        this.slots = new List<InventoryStatSlot>(this.GetComponentsInChildren<InventoryStatSlot>());
    }
#endif

    public void ParseData(List<DiceID> ids)
    {
        List<StatData> stats = StatDatas.Instance.GetStats(ids);
        for (int i = 0; i < slots.Count; i++)
        {
            slots[i].ParseData(stats[i]);
        }
            
    }
    public void ParseData(List<StatData> stats)
    {
        for (int i = 0; i < slots.Count; i++)
        {
            slots[i].ParseData(stats[i]);
        }

    }
}
