using System.Collections;
using System.Collections.Generic;
using UnityEngine;


/// <summary>
/// Chứa các hàm chung để find player
/// </summary>
public class FindPlayerCommon : MonoBehaviour
{
    /// <summary>
    /// Kiểm tra coin và find player
    /// </summary>
    /// <param name="config"></param>
    public static void UseCoinAndFindPlayer(RoomConfig config)
    {
        //if (config != null)
        //{
        //    StandardPlayer player = JoinGameHelper.GetStandardMainUser();
        //    StandardPlayer opponent = JoinGameHelper.RandomStandardPlayerByRoom(config);

        //    if (UserProfile.Instance.IsCanUseBooster(config.fee.type, config.fee.GetValue()))
        //    {
        //        ShowFindPlayer(config, player, opponent);
        //    }
        //    else
        //    {
        //        BoosterCommodity coin = UserBoosters.Instance.GetBoosterCommodity(config.fee.type);
        //        long coinNeed = config.fee.GetValue() - coin.GetValue();
        //        NeedMoreCoinDialogs dialog =
        //            GameManager.Instance.OnShowDialogWithSorting<NeedMoreCoinDialogs>("Home/GUI/Dialogs/NeedMoreCoin/NeedMoreCoinDialog",
        //                PopupSortingType.CenterBottomAndTopBar);
        //        dialog?.ParseData(coinNeed, "Play_game", () =>
        //        {
        //            ShowFindPlayer(config, player, opponent);
        //        });
        //        //MessageBox.Instance.ShowMessageBox("Noice", "Not enough fee");
        //    }
        //}
    }

    public static void UseCoinAndReplay(RoomConfig config, StandardPlayer opponent)
    {
        //if (config != null && opponent != null)
        //{
        //    StandardPlayer player = JoinGameHelper.GetStandardMainUser();
        //    if (UserProfile.Instance.IsCanUseBooster(config.fee.type, config.fee.GetValue()))
        //    {
        //        //TODO replay game
        //        JoinRoomAI(config, player);
        //    }
        //    else
        //    {
        //        BoosterCommodity coin = UserBoosters.Instance.GetBoosterCommodity(config.fee.type);
        //        long coinNeed = config.fee.GetValue() - coin.GetValue();
        //        NeedMoreCoinDialogs dialog =
        //            GameManager.Instance.OnShowDialogWithSorting<NeedMoreCoinDialogs>("Home/GUI/Dialogs/NeedMoreCoin/NeedMoreCoinDialog",
        //                PopupSortingType.CenterBottomAndTopBar);
        //        dialog?.ParseData(coinNeed, "Play_game", () =>
        //        {
        //            //TODO replay game
        //            JoinRoomAI(config, player);
        //        });
        //        //MessageBox.Instance.ShowMessageBox("Noice", "Not enough fee");
        //    }
        //}
    }

    /// <summary>
    /// Sử dụng finding, find new game at endGame
    /// </summary>
    /// <param name="config"></param>
    private static void ShowFindPlayer(RoomConfig config, StandardPlayer player, StandardPlayer opponent)
    {
        if (config != null)
        {
            JoinRoomPlayer(config, player);

            //FidingPlayerDialog dialog = GameManager.Instance.OnShowDialogWithSorting<FidingPlayerDialog>("Home/GUI/Dialogs/FindingPlayer/FindingPlayerDialog", PopupSortingType.OnTopBar);
            //dialog?.ShowFidingPlayer(player, opponent, config, () =>
            //{
            //    JoinRoomAI(config, player, opponent);
            //});
        }
    }

    public static void JoinRoomPlayer(RoomConfig config, StandardPlayer player)
    {
        //UserProfile.Instance.UseBooster(config.fee, string.Format("Room_{0}", config.id),
        //            LogSinkWhere.JOIN_ROOM);
        JoinGameHelper.Instance.JoinRoom( player, config, GameType.AI);
    }
    public static void JoinRoomAI(RoomConfig config)
    {
        //UserProfile.Instance.UseBooster(config.fee, string.Format("Room_{0}", config.id),
        //            LogSinkWhere.JOIN_ROOM);
        JoinGameHelper.Instance.JoinRoom(config, GameType.AI);
    }
}
