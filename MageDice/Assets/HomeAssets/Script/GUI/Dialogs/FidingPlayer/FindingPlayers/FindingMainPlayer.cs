using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FindingMainPlayer : FindingPlayer
{
    public override void ParseData(PlayerModel model)
    {
        base.ParseData(model);
        ShowInfo();
    }
}
