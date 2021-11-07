using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameScene : BaseScene
{
    //public Transform tranBotLeft, tranTopRight;

   public override void OnParseData()
   {
      base.OnParseData();
   }

    //#region Convert position => ratio in canvas
    //public Vector3 GetPositionByRatio(Vector3 ratio)
    //{
    //    return GameUtils.ConvertRatioInCanvasToPos(ratio, this.tranBotLeft.position, this.tranTopRight.position);
    //}

    //public Vector3 GetRatioInCanvasByPos(Vector3 pos)
    //{
    //    return GameUtils.ConvertPosInCanvasToRatio(pos, this.tranBotLeft.position, this.tranTopRight.position);
    //}
    //#endregion
}
