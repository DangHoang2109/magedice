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
            this.datas = new JoinGameStandardDatas(p1, config);
            this.datas.gameType = GameType.AI;
            LoadSceneGame();
        }
        else
        {
            Debug.LogError("CAN NOT GET ROOM vs FRIST AI CONFIG ID = 0" );
        }
    }

    private void BaseJoinRoom(StandardPlayer player, RoomConfig config, GameType gameType)
    {
        //TODO JOIN ROOM
        this.datas = new JoinGameStandardDatas(player, config);
        this.datas.gameType = gameType;
    }
    private void BaseJoinRoom(RoomConfig config, GameType gameType)
    {
        //TODO JOIN ROOM
        this.datas = new JoinGameStandardDatas(GetStandardMainUser(), config);
        this.datas.gameType = gameType;
    }
    public void JoinRoom(StandardPlayer player, RoomConfig config, GameType gameType)
    {
        BaseJoinRoom(player, config, gameType);
        this.LoadSceneGame();
    }
    public void JoinRoom(RoomConfig config, GameType gameType)
    {
        BaseJoinRoom(config, gameType);
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
                RoomDatas.Instance.ClearRoom(joinGame.roomConfig.id);
            }
            else
            {
            }

            UserProfile.Instance.AddBoosters(joinGame.GetPrizes(), string.Format("Tour_{0}", joinGame.roomConfig.id),
                LogSourceWhere.COIN_WIN_GAME);
            BagSlotDatas.Instance.CollectBag(joinGame.bagReward, $"Tour {joinGame.roomConfig.id}", "Win_Game",
                string.Format("Tour_{0}", joinGame.roomConfig.id));
        }
    }
    public void BackHomeScene()
    {
        GameManager.Instance.OnLoadScene(SceneName.HOME, null, () =>
        {
            if (UserCareers.MainUserInstance.CompletedGame >= 2)
            {
                AdsManager.Instance.ShowInterstitial(LogAdsInterstitialWhere.END_GAME);
            }
        });
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
    public List<StatItemStats> userStatDeck;

    public JoinGameDatas()
    {}

    public JoinGameDatas(StandardPlayer p)
    {
        this.player = p;
        this.userStatDeck = new List<StatItemStats>(StatManager.Instance.CurrentCueStats);
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
    public RoomConfig roomConfig;
    public MapConfig mapConfig;
    public BagAmount bagReward;

    public bool IsMainUserWin;
    public bool IsNewUser;

    public List<BoosterCommodity> prizes;

    /// <summary>
    /// only use this for join pratice
    /// </summary>
    /// <param name="p"></param>
    public JoinGameStandardDatas(StandardPlayer p) : base(p)
    {
    }

    public JoinGameStandardDatas(StandardPlayer p, RoomConfig config) : base(p)
    {
        this.roomConfig = config;
        this.mapConfig = GameMapConfigs.Instance.GetMap((MapName)config.id);

        int matchWin = (UserDatas.Instance != null && UserDatas.Instance.careers != null) ? UserDatas.Instance.careers.matchWin : 0;
        
    }

    public override long GetBetMoney()
    {
        //if (this.roomConfig != null)
        //{
        //    return this.roomConfig.fee.GetValue();
        //}
        return base.GetBetMoney();
    }

    public void SetPrize(List<BoosterCommodity> p, int waveComplete)
    {
        this.prizes = new List<BoosterCommodity>(p);

        int indexWaveBoss = waveComplete / 10 - 1;
        if (indexWaveBoss >= 0 && RoomDatas.Instance.GetRoom(roomConfig.id).NotClaimThisIndex(indexWaveBoss))
        {
            this.bagReward = roomConfig.GetBagReward(indexWaveBoss);
            RoomDatas.Instance.GetRoom(roomConfig.id).SetIndexBagClaim(indexWaveBoss);
        }
        else
            this.bagReward = null;
    }
    public List<BoosterCommodity> GetPrizes()
    {
        return this.prizes;
    }
    public override long GetPrizeMoney()
    {
        if (this.roomConfig != null)
        {
            return this.roomConfig.prizePerWave.GetValue();
        }
        return base.GetPrizeMoney();
    }
}
