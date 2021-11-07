using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
public class MatchMakingManager : MonoSingleton<MatchMakingManager>
{
    public int NumMatchPlay { get => numMatchPlay; set{
            numMatchPlay = value;
        } }
    private int numMatchPlay;

    public override void Init()
    {
        base.Init();

        //Không cần lưu data cũ, chỉ cần lưu trong session hiện tại vì khi user đã out game họ không còn nhiều ấn tượng với stat cũ
        NumMatchPlay = 0;
    }


    /// <summary>
    /// Tính randome trophy theo room
    /// </summary>
    /// <param name="room"></param>
    /// <param name="info"></param>
    /// <returns></returns>
    public StandardPlayer CalTrophy(RoomConfig room,
        StandardPlayer info)
    {
        if (info == null)
            info = JoinGameHelper.CreateRandomStandardPlayer();

        //tính trophy của bot
        int trophyDiffer = room.GetTrophyDifferent(); //+ upgrade bluffing
        int UserTrophy = (int)UserBoosters.Instance.GetBoosterCommodity(BoosterType.CUP).GetValue();

        info.trophy = Mathf.Clamp(UserTrophy + GameUtils.SafeRandom(-trophyDiffer, trophyDiffer),
            (int)room.unlock.GetValue(), UserTrophy + (room.id + 1) * 3);

        return info;
    }

}
