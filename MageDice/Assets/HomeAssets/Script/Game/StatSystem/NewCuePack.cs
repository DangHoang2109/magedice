using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class NewCuePack
{
    public List<DiceID> newIds;
    public NewCuePack()
    {
        newIds = new List<DiceID>();
    }
}
