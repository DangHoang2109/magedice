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
}
