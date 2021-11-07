using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class PlayerModel
{
    public UserInfo info;

    public PlayerModel()
    {
        this.info = new UserInfo();
    }

    public PlayerModel(PlayerModel p)
    {
        this.info = p.info;
    }
}
