using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EventObject<T0> : UnityEngine.Events.UnityEvent
{

}
[System.Serializable]
public class StandardPlayer : PlayerModel
{
    public int trophy;//trophy
    public bool isBot;
    public StandardPlayer()
    {}

    public StandardPlayer(StandardPlayer p) : base(p)
    {
        this.trophy = p.trophy;

    }
    public void AddTrophy(int trophy)
    {
        this.trophy += trophy;
        if (this.trophy <= 0) this.trophy = 0;
    }
    
}

public class JoinGameHelper : MonoSingleton<JoinGameHelper>
{
    private JoinGameDatas datas;

    public T GetJoinGameData<T>() where T : JoinGameDatas
    {
        return (T) this.datas;
    }

    public GameType GameType
    {
        get
        {
            if (this.datas != null)
            {
                return this.datas.gameType;
            }

            return GameType.AI;
        }
    }

    public long Prize
    {
        get
        {
            if (this.datas != null)
            {
                return this.datas.GetPrizeMoney();
            }

            return 0;
        }
    }

    public List<StandardPlayer> Players
    {
        get
        {
            if (this.datas != null)
            {
                return this.datas.GetPlayers();
            }

            return null;
        }
    }

    public void JoinTutorial()
    {
        this.datas = new JoinGameDatas(GetStandardMainUser());
        this.datas.gameType = GameType.Tutorial;
        this.LoadSceneGame();
    }

    public void JoinTutorialFristGameAI(int roomId = 0)
    {
        RoomConfig config = RoomConfigs.Instance.GetRoom(roomId);
        if (config != null)
        { 
            StandardPlayer p1 = GetStandardMainUser();
            StandardPlayer p2 = RandomStandardPlayerByRoom(config);
            this.datas = new JoinGameStandardDatas(p1, p2, config);
            this.datas.gameType = GameType.AI;
            LoadSceneGame();
        }
        else
        {
            Debug.LogError("CAN NOT GET ROOM vs FRIST AI CONFIG ID = 0" );
        }
    }

    public void JoinRoom(StandardPlayer player,StandardPlayer opponent, RoomConfig config, GameType gameType)
    {
        JoinRoom(false, player, opponent, config, gameType);
    }

    private void BaseJoinRoom(bool MainFirstOpenApp, StandardPlayer player, StandardPlayer opponent, RoomConfig config, GameType gameType)
    {
        //TODO JOIN ROOM
        this.datas = new JoinGameStandardDatas(player, opponent, config);
        this.datas.gameType = gameType;
    }

    public void JoinRoom(bool MainFirstOpenApp ,StandardPlayer player, StandardPlayer opponent, RoomConfig config, GameType gameType)
    {
        BaseJoinRoom(MainFirstOpenApp, player, opponent, config, gameType);
        this.LoadSceneGame();
    }


    private void LoadSceneGame()
    {
        //TODO OPEN GAME
        Debug.Log("<color=blue>Let's open game</color>");
        GameManager.Instance.OnLoadScene(SceneName.GAME);
    }

    public void EndGameAI(bool win)
    {
        JoinGameStandardDatas joinGame = GetJoinGameData<JoinGameStandardDatas>();
        if (joinGame != null)
        {
            if (win)
            {
                RoomDatas.Instance.AddPoint(joinGame.config.id, joinGame.config.pointWin);
                UserProfile.Instance.AddBooster(BoosterType.CUP, joinGame.config.pointWin, "Win_Game",string.Format("Tour_{0}",joinGame.config.id));
                UserProfile.Instance.AddBooster(joinGame.config.prize, string.Format("Tour_{0}", joinGame.config.id),
                    LogSourceWhere.COIN_WIN_GAME);
                joinGame.player.AddTrophy(joinGame.config.pointWin);
                joinGame.opponent.AddTrophy(joinGame.config.pointLose); 
            }
            else
            {
                RoomDatas.Instance.AddPoint(joinGame.config.id, joinGame.config.pointLose);
                //TODO: LOSE TROPHY
                UserProfile.Instance.UseBooster(BoosterType.CUP, Mathf.Abs(joinGame.config.pointLose),"Lose_Game", string.Format("Tour_{0}",joinGame.config.id));
                joinGame.player.AddTrophy(joinGame.config.pointLose);
                joinGame.opponent.AddTrophy(joinGame.config.pointWin);
            }

            //TODO show rate      
        }
    }

    #region Random player
    public static StandardPlayer CreateUserByRoom()
    {
        return CreateRandomStandardPlayer();
    }

    public static StandardPlayer CreateRandomStandardPlayer()
    {
        StandardPlayer p = new StandardPlayer();
        p.info.id = "BotId";
        p.info.nickname = CommonNickname.Instance.GetRandomNickname();
        p.info.Avatar = CommonAvatar.Instance.GetRandomAvatar();
        
        p.isBot = true;

        return p;
    }
    /// <summary>
    /// Tạo một model player để chơi với Opp
    /// </summary>
    /// <param name="room">Room config đang chơi, tour cao sẽ có độ khó cao</param>
    /// <param name="careerOpp">Stat của người đối đầu với model này, thường sẽ là Main Player</param>
    /// <returns></returns>
    public static StandardPlayer RandomStandardPlayerByRoom(RoomConfig room)
    {
        StandardPlayer p = CreateRandomStandardPlayer();

        p = MatchMakingManager.Instance.CalTrophy(room: room, info: p);

        return p;
    }

    /// <summary>
    /// Tạo một model player để chơi với Opp
    /// </summary>
    /// <param name="room">Room config đang chơi, tour cao sẽ có độ khó cao</param>
    /// <param name="careerOpp">Stat của người đối đầu với model này, thường sẽ là Main Player</param>
    /// <returns></returns>
    public static StandardPlayer RandomBotForTournament(RoomConfig room)
    {
        StandardPlayer p = CreateRandomStandardPlayer();

        p = MatchMakingManager.Instance.CalTrophy(room: room, info: p);

        return p;
    }

    public static StandardPlayer GetStandardMainUser()
    {
        StandardPlayer p = new StandardPlayer();
        p.info.id = UserDatas.Instance.info.id;
        p.info.nickname = UserDatas.Instance.info.nickname;
        p.info.Avatar = UserDatas.Instance.info.Avatar;
        p.trophy = (int)UserBoosters.Instance.GetBoosterCommodity(BoosterType.CUP).GetValue();
        p.isBot = false;
        return p;
    }

    public static StandardPlayer GetStandardFriend(string nickname, string id)
    {
        StandardPlayer p = new StandardPlayer();
        p.info.id = id;
        p.info.nickname = nickname;
        p.info.Avatar = UserDatas.Instance.info.Avatar;
        p.trophy = (int)UserBoosters.Instance.GetBoosterCommodity(BoosterType.CUP).GetValue();
        return p;
    }
    #endregion

    /// <summary>
    /// Lấy ra tỉ lệ đánh fail
    /// true: đánh ra ngoài
    /// false: đánh vào trong
    /// </summary>
    /// <returns></returns>
    public bool IsBotShootFail()
    {
        JoinGameStandardDatas gData = GetJoinGameData<JoinGameStandardDatas>();
        if (gData == null)
            return false;

        float userWinRate = UserCareers.MainUserInstance.GetWinRate_LastNGame(10);

        int completeGame = UserCareers.MainUserInstance.CompletedGame;

        float currentRoomWinRate = gData.config.rateBotWin;

        ///Trong 7 game đầu, độ winrate của room được boost lên tối đa 170% = 250 điểm 
        ///| Mỗi game giảm 10% => Khi unlock tour 2 chỉ số sau boost = 102 => game 2 đầu tiên bot không quá khó
        if (completeGame <= GameDefine.AMOUNT_GAMES_BOOST_MAIN)
        {
            currentRoomWinRate *= (1 + (float)(GameDefine.AMOUNT_GAMES_BOOST_MAIN - completeGame) / GameDefine.DIVIDER_IN_GAMES_BOOST_MAIN);
        }
        float random = Random.Range(0, currentRoomWinRate) / 100;

        Debug.Log($"roomrate {currentRoomWinRate} random {random} user winrate {userWinRate}");
        //currentRoomWinRate càng lớn => random càng dễ lớn => random càng dễ lớn hơn userCurrentWinrate => bot càng dễ shoot out
        return random >= userWinRate;
    }
}

public enum GameType
{
    None = 0,
    Standard = 1,
    AI = 2,
    Tutorial = 3,
}

[System.Serializable]
public class JoinGameDatas
{
    public StandardPlayer player;
    public GameType gameType;
    public JoinGameDatas()
    {}

    public JoinGameDatas(StandardPlayer p)
    {
        this.player = p;
    }
    /// <summary>
    /// Số tiền bet
    /// </summary>
    /// <returns></returns>
    public virtual long GetBetMoney()
    {
        

        return 0;
    }
    /// <summary>
    /// Số tiền reward
    /// </summary>
    /// <returns></returns>
    public virtual long GetPrizeMoney()
    {
        

        return 0;
    }

    public virtual List<StandardPlayer> GetPlayers()
    {
        return new List<StandardPlayer> {this.player};
    }
}

public class JoinGameStandardDatas : JoinGameDatas
{   
    public StandardPlayer opponent;
    public RoomConfig config;
    //public BagAmount bagReward;

    public bool IsMainUserWin;
    public bool IsNewUser;

    /// <summary>
    /// only use this for join pratice
    /// </summary>
    /// <param name="p"></param>
    public JoinGameStandardDatas(StandardPlayer p) : base(p)
    {
    }

    public JoinGameStandardDatas(StandardPlayer p, StandardPlayer o, RoomConfig config) : base(p)
    {
        this.opponent = o;
        this.config = config;
        //this.bagReward = config.GetBagReward(UserDatas.Instance.careers.matchWin);
    }

    public override long GetBetMoney()
    {
        if (this.config != null)
        {
            return this.config.fee.GetValue();
        }
        return base.GetBetMoney();
    }

    public override long GetPrizeMoney()
    {
        if (this.config != null)
        {
            return this.config.prize.GetValue();
        }
        return base.GetPrizeMoney();
    }

    public override List<StandardPlayer> GetPlayers()
    {
        return new List<StandardPlayer> {this.player, this.opponent};
    }
}
