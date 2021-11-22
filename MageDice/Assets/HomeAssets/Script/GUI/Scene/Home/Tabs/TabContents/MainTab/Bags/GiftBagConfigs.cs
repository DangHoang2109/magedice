using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[CreateAssetMenu(menuName = "Configs/GiftBagConfigs", fileName = "GiftBagConfigs")]
public class GiftBagConfigs : ScriptableObject
{
    int REACH_CUES_AMOUNT_BONUS_SLOT = 4; //Nếu user có ít hơn lượng cue này thì bonus thêm 1 slot standard card nữa
    public const int START_HAVE_RARE = 4;
    public const int START_HAVE_LEGEND = 5;
    public const int START_HAVE_COUNTRY = 2;

    ///Sponsor Bag(Free Bag): You receive 2 Sponsor Bags every 4 hours.
    ///Points Bag: You get one daily after you achieve 40 points.
    ///Blue Bag: These take 3 hours to unlock and you maybe rewarded this if you are victorious in a match., 17-22 card
    ///Grand Bag: Takes 8 hours to unlock and you can get one of these randomly from winning a match. 25-30 card
    ///You can only gain Epic card in Elite bag or upon
    ///The String and Character Card will appear randomly in the Grand bag
    ///Elite Bag: Takes 12 hours to unlock and contains 55-65 cards.You can also get these randomly after winning a match.
    ///King Bag: Takes 14 hours to open and contains 90 cards.Similar to the other bags, you can also get this bag after winning a match.

    public static GiftBagConfigs Instance
    {
        get
        {
            return LoaderUtility.Instance.GetAsset<GiftBagConfigs>("Home/Configs/GiftBagConfigs");
        }
    }


    public List<GiftBagToursConfig> giftBagTours;

   
    public List<GiftBagToursConfig> backupgiftBagTours;


    public GiftBagPerTourConfig TutorialGiftBag;
    public GiftBagToursConfig PointbagConfig;

    private StatManager.Tier callbackUnlockCue;

    public void CallbackUnlockRoom(int roomID)
    {
        switch (roomID)
        {
            case 2:
                callbackUnlockCue = StatManager.Tier.Standard;
                break;
            case START_HAVE_RARE:
                callbackUnlockCue = StatManager.Tier.Rare;
                break;
            case START_HAVE_LEGEND:
                callbackUnlockCue = StatManager.Tier.Legendary;
                break;
            default:
                callbackUnlockCue = StatManager.Tier.None;
                break;
        }
        Debug.Log($"register unlocking cue {roomID} {callbackUnlockCue}");
    }
    private void DoneCallbackUnlockRoom()
    {
        Debug.Log($"done unlocking cue");
        callbackUnlockCue = StatManager.Tier.None;
    }


    public int GetMinRoomForBag(BagType type)
    {
        switch (type)
        {
            case BagType.KING_BAG:
                return 3;
            case BagType.PLATINUM_BAG:
                return 2;
            default:
                return 1;
        }
    }
    public int GetCurrentRoomForBag(BagType type)
    {
        RoomDatas RoomDatas = RoomDatas.Instance;
        int currentRoom = RoomDatas.GetRoomUnlockedMax();
        int maxRoom = RoomDatas.GetMaxRoomID();
        int minRoom = GetMinRoomForBag(type);

        switch (type)
        {
            case BagType.KING_BAG:
                return Mathf.Clamp(currentRoom + 1, minRoom, maxRoom);
            case BagType.PLATINUM_BAG:
                return Mathf.Clamp(currentRoom + 1, minRoom, maxRoom);
            default:
                return Mathf.Clamp(currentRoom, minRoom, maxRoom);
        }
    }

    public GiftBagPerTourConfig GetGiftBag(BagType type, int tour)
    {
        GiftBagToursConfig giftBagTours = this.giftBagTours.Find(x => x.bag == type);

        if (giftBagTours != null)
        {
            RoomDatas RoomDatas = RoomDatas.Instance;
            if(RoomDatas.datas.Count > 0)
            {
                if (tour > RoomDatas.datas[RoomDatas.datas.Count - 1].id)
                {
                    tour = RoomDatas.datas[RoomDatas.datas.Count - 1].id;
                }
            }

            return giftBagTours.GetGiftBagByTour(tour);
        }
        return null;
    }
    public GiftBagPerTourConfig GetCurrentTourGiftBag(BagType type)
    {
        return this.GetGiftBag(type, this.GetCurrentRoomForBag(type));
    }
    public OpenBagDialog.BagModel GetTutorialGiftBagModel()
    {

        //check the tour for sure
        GiftBagPerTourConfig giftBagConfig = this.TutorialGiftBag;
        if (giftBagConfig != null)
        {
            OpenBagDialog.BagModel giftBagModel = new OpenBagDialog.BagModel();
            giftBagModel.typeBag = BagType.SILVER_BAG;

            //get value coin
            long valueCoin = giftBagConfig.valueCoin.GetRandomValue();
            if (valueCoin > 0)
            {
                giftBagModel.items.Add(new OpenBagDialog.BagCardModel()
                {
                    earnType = OpenBagDialog.BagCardModel.EarnType.Booster,
                    booster = BoosterType.COIN,
                    value = valueCoin,
                });
            }

            //get value gem
            long valueGem = giftBagConfig.valueGem.GetRandomValue();
            if (valueGem > 0)
            {
                giftBagModel.items.Add(new OpenBagDialog.BagCardModel()
                {
                    earnType = OpenBagDialog.BagCardModel.EarnType.Booster,
                    booster = BoosterType.CASH,
                    value = valueGem,
                });
            }

            //bonus cue
            OpenBagDialog.BagCardModel cueModel = GetStartingCue();
            if (cueModel != null && cueModel.equipmentConfig != null && cueModel.equipmentConfig.kind == StatManager.Kind.NotUnlocked)
            {
                cueModel.value = 1;
                giftBagModel.items.Add(cueModel);
            }

            foreach (CardAmount cardAmount in giftBagConfig.cardAmounts)
            {
                long valueCard = cardAmount.amount;

                OpenBagDialog.BagCardModel bagCardModel = GetStartingCue();
                bagCardModel.earnType = OpenBagDialog.BagCardModel.EarnType.CueCard;
                bagCardModel.value = valueCard;

                if (bagCardModel != null && bagCardModel.equipmentConfig != null)
                {
                    bagCardModel.value = valueCard;
                    giftBagModel.items.Add(bagCardModel);
                }
            }

            return giftBagModel;
        }
        return null;
    }

    /// <summary>
    /// Open bag with the ui of bag type, but the content config before
    /// </summary>
    /// <param name="type">which UI of bag type should we display? Silver Bag, Red bag????</param>
    /// <param name="config">The config u made before, if null, then wtf?</param>
    /// <param name="source"></param>
    /// <returns></returns>
    public OpenBagDialog.BagModel GetGiftBagModel(BagType type, GiftBagPerTourConfig config, string source)
    {
        if (!TutorialDatas.IsTutorialCompleted())
            return this.GetTutorialGiftBagModel();

        int tour = RoomDatas.Instance.GetRoomUnlockedMax();
        tour = tour >= GetMinRoomForBag(type) ? tour : GetMinRoomForBag(type);

        if (config != null)
        {
            OpenBagDialog.BagModel giftBagModel = new OpenBagDialog.BagModel();
            giftBagModel.typeBag = type;

            //get value coin
            long valueCoin = config.valueCoin.GetRandomValue();
            if (valueCoin > 0)
            {
                giftBagModel.items.Add(new OpenBagDialog.BagCardModel()
                {
                    earnType = OpenBagDialog.BagCardModel.EarnType.Booster,
                    booster = BoosterType.COIN,
                    value = valueCoin,
                });
            }

            //get value gem
            long valueGem = config.valueGem.GetRandomValue();
            if (valueGem > 0)
            {
                giftBagModel.items.Add(new OpenBagDialog.BagCardModel()
                {
                    earnType = OpenBagDialog.BagCardModel.EarnType.Booster,
                    booster = BoosterType.CASH,
                    value = valueGem,
                });
            }

            //get value card

            List<DiceID> showedItem = new List<DiceID>();
            //float appearRate = Mathf.Clamp(min_rateAppearOwned + UserDatas.Instance.careers.totalMatch * 0.02f, min_rateAppearOwned, max_rateAppearOwned);
            foreach (CardAmount cardAmount in config.cardAmounts)
            {
                long valueCard = cardAmount.amount;
                StatManager.Tier rariry = cardAmount.tier;//ConvertCardTypeToCueTier(cardAmount.cardType);

                //Lấy ra type có rarity để tặng
                OpenBagDialog.BagCardModel bagCardModel = RandomCueCard(rariry, 1, tour, showedItem);

                if (bagCardModel != null && bagCardModel.equipmentConfig != null)
                {
                    showedItem.Add(bagCardModel.equipmentConfig.id);
                    bagCardModel.value = valueCard;
                    giftBagModel.items.Add(bagCardModel);
                }
            }

            //get bonus card
            if (Random.value <= config.rateBonus)
                AddBonusGift(ref giftBagModel, config, tour, showedItem, !source.Equals("Shop"));

            return giftBagModel;
        }
        return null;
    }

    /// <summary>
    /// Open bag with the ui of bag type, but the content card list configed before
    /// Coin, Cash, Rate for the bonus will use the basic bag
    /// </summary>
    /// <param name="type">which UI of bag type should we display? Silver Bag, Red bag????</param>
    /// <param name="card">The config of list card u made before, if null, then wtf?</param>
    /// <param name="source"></param>
    /// <returns></returns>
    public OpenBagDialog.BagModel GetGiftBagModel(BagType type, List<CardAmount> card, string source)
    {
        if (!TutorialDatas.IsTutorialCompleted())
            return this.GetTutorialGiftBagModel();

        int tour = RoomDatas.Instance.GetRoomUnlockedMax();
        tour = tour >= GetMinRoomForBag(type) ? tour : GetMinRoomForBag(type);

        GiftBagPerTourConfig config = this.GetGiftBag(type, tour);

        if (config != null)
        {
            OpenBagDialog.BagModel giftBagModel = new OpenBagDialog.BagModel();
            giftBagModel.typeBag = type;

            //get value coin
            long valueCoin = config.valueCoin.GetRandomValue();
            if (valueCoin > 0)
            {
                giftBagModel.items.Add(new OpenBagDialog.BagCardModel()
                {
                    earnType = OpenBagDialog.BagCardModel.EarnType.Booster,
                    booster = BoosterType.COIN,
                    value = valueCoin,
                });
            }

            //get value gem
            long valueGem = config.valueGem.GetRandomValue();
            if (valueGem > 0)
            {
                giftBagModel.items.Add(new OpenBagDialog.BagCardModel()
                {
                    earnType = OpenBagDialog.BagCardModel.EarnType.Booster,
                    booster = BoosterType.CASH,
                    value = valueGem,
                });
            }

            //get value card

            List<DiceID> showedItem = new List<DiceID>();
            //float appearRate = Mathf.Clamp(min_rateAppearOwned + UserDatas.Instance.careers.totalMatch * 0.02f, min_rateAppearOwned, max_rateAppearOwned);
            foreach (CardAmount cardAmount in card)
            {

                long valueCard = cardAmount.amount;
                StatManager.Tier rariry = cardAmount.tier;//ConvertCardTypeToCueTier(cardAmount.cardType);

                //Lấy ra type có rarity để tặng
                OpenBagDialog.BagCardModel bagCardModel = RandomCueCard(rariry, 1, tour, showedItem);

                if (bagCardModel != null && bagCardModel.equipmentConfig != null)
                {
                    showedItem.Add(bagCardModel.equipmentConfig.id);
                    bagCardModel.value = valueCard;
                    giftBagModel.items.Add(bagCardModel);
                }
            }

            //get bonus card
            if (Random.value <= config.rateBonus)
                AddBonusGift(ref giftBagModel, config, tour, showedItem, !source.Equals("Shop"));

            return giftBagModel;
        }
        return null;
    }
    public OpenBagDialog.BagModel GetGiftBagModel(BagType type, int tour, string source)
    {
        if (!TutorialDatas.IsTutorialCompleted())
            return this.GetTutorialGiftBagModel();


        //check the tour for sure
        tour = tour >= GetMinRoomForBag(type) ? tour : GetMinRoomForBag(type);

        GiftBagPerTourConfig giftBagConfig = this.GetGiftBag(type, tour);
        if (giftBagConfig != null)
        {
            OpenBagDialog.BagModel giftBagModel = new OpenBagDialog.BagModel();
            giftBagModel.typeBag = type;

            //get value coin
            long valueCoin = giftBagConfig.valueCoin.GetRandomValue();
            if (valueCoin > 0)
            {
                giftBagModel.items.Add(new OpenBagDialog.BagCardModel()
                {
                    earnType = OpenBagDialog.BagCardModel.EarnType.Booster,
                    booster = BoosterType.COIN,
                    value = valueCoin,
                });
            }

            //get value gem
            long valueGem = giftBagConfig.valueGem.GetRandomValue();
            if (valueGem > 0)
            {
                giftBagModel.items.Add(new OpenBagDialog.BagCardModel()
                {
                    earnType = OpenBagDialog.BagCardModel.EarnType.Booster,
                    booster = BoosterType.CASH,
                    value = valueGem,
                });
            }

            //get value card

            List<DiceID> showedItem = new List<DiceID>();
            //float appearRate = Mathf.Clamp(min_rateAppearOwned + UserDatas.Instance.careers.totalMatch * 0.02f, min_rateAppearOwned, max_rateAppearOwned);
            foreach(CardAmount cardAmount in giftBagConfig.cardAmounts)
            {
                long valueCard = cardAmount.amount;
                StatManager.Tier rariry = cardAmount.tier;//ConvertCardTypeToCueTier(cardAmount.cardType);

                //Lấy ra type có rarity để tặng
                OpenBagDialog.BagCardModel bagCardModel = RandomCueCard(rariry, 1, tour, showedItem);

                if(bagCardModel != null && bagCardModel.equipmentConfig != null)
                {
                    showedItem.Add(bagCardModel.equipmentConfig.id);
                    bagCardModel.value = valueCard;
                    giftBagModel.items.Add(bagCardModel);
                }
            }

            //get bonus card
            if (Random.value <= giftBagConfig.rateBonus)
                AddBonusGift(ref giftBagModel, giftBagConfig, tour, showedItem, !source.Equals("Shop"));

            return giftBagModel;
        }
        return null;
    }

    public OpenBagDialog.BagModel GetPointbagModel(int tour)
    {
        //check the tour for sure
        tour = tour >= GetMinRoomForBag(BagType.GOLD_BAG) ? tour : GetMinRoomForBag(BagType.GOLD_BAG);

        GiftBagPerTourConfig giftBagConfig = this.PointbagConfig.GetGiftBagByTour(tour);
        if (giftBagConfig != null)
        {
            OpenBagDialog.BagModel giftBagModel = new OpenBagDialog.BagModel();
            giftBagModel.typeBag = BagType.GOLD_BAG;

            //get value coin
            long valueCoin = giftBagConfig.valueCoin.GetRandomValue();
            if (valueCoin > 0)
            {
                giftBagModel.items.Add(new OpenBagDialog.BagCardModel()
                {
                    earnType = OpenBagDialog.BagCardModel.EarnType.Booster,
                    booster = BoosterType.COIN,
                    value = valueCoin,
                });
            }

            //get value gem
            long valueGem = giftBagConfig.valueGem.GetRandomValue();
            if (valueGem > 0)
            {
                giftBagModel.items.Add(new OpenBagDialog.BagCardModel()
                {
                    earnType = OpenBagDialog.BagCardModel.EarnType.Booster,
                    booster = BoosterType.CASH,
                    value = valueGem,
                });
            }

            //get value card

            List<DiceID> showedItem = new List<DiceID>();
            
            foreach (CardAmount cardAmount in giftBagConfig.cardAmounts)
            {
                long valueCard = cardAmount.amount;
                StatManager.Tier rariry = cardAmount.tier;//ConvertCardTypeToCueTier(cardAmount.cardType);

                //Lấy ra type có rarity để tặng
                OpenBagDialog.BagCardModel bagCardModel = RandomCueCard(rariry, 1, tour, showedItem);

                if (bagCardModel != null && bagCardModel.equipmentConfig != null)
                {
                    showedItem.Add(bagCardModel.equipmentConfig.id);
                    bagCardModel.value = valueCard;
                    giftBagModel.items.Add(bagCardModel);
                }
            }

            //get bonus card
            if (Random.value <= giftBagConfig.rateBonus)
                AddBonusGift(ref giftBagModel, giftBagConfig, tour, showedItem, true);

            return giftBagModel;
        }
        return null;
    }
    private void AddBonusGift(ref OpenBagDialog.BagModel giftBagModel, GiftBagPerTourConfig bagConfig, int tour, List<DiceID> showedItem, bool isWatch = true)
    {
        if ((bagConfig.bonusUnlockCards == null || bagConfig.bonusUnlockCards.Count == 0) && (bagConfig.cueLists == null || bagConfig.cueLists.Count == 0))
            return;

        giftBagModel.isShowInterAtEnd = isWatch;

        if (this.callbackUnlockCue != StatManager.Tier.None)
        {
            //bonus stable level cue
            OpenBagDialog.BagCardModel cueModel = RandomCue(callbackUnlockCue);
            if (cueModel != null && cueModel.equipmentConfig != null)
            {
                showedItem.Add(cueModel.equipmentConfig.id);
                cueModel.value = 1;
                giftBagModel.items.Add(cueModel);
                cueModel.isWatch = isWatch;

                DoneCallbackUnlockRoom();
                return;
            }
        }


        //Check xem bonus là cue ko
        float randCue = UnityEngine.Random.Range(0f, 1f);
#if TEST_BAG_CUE
        randCue = 0;
#endif
        if (randCue <= bagConfig.rateCue)
        {
            //bonus cue
            OpenBagDialog.BagCardModel cueModel = RandomCue(bagConfig.GetCueTier());
            if (cueModel != null && cueModel.equipmentConfig != null)
            {
                showedItem.Add(cueModel.equipmentConfig.id);
                cueModel.value = 1;
                giftBagModel.items.Add(cueModel);
                cueModel.isWatch = isWatch;

                return;
            }
        }

        if (UserDatas.Instance.careers.CompletedGame < 3)
        {
            giftBagModel.isShowInterAtEnd = false;
            isWatch = false;
        }
            
        //Nếu ko phải cue => random ra cue card
        CueRatePair bonusCard = bagConfig.GetRandomBonusCard();
        if(bonusCard != null)
        {
            OpenBagDialog.BagCardModel bagCardModel = RandomUnlockCard(bonusCard.tierID);

            if(bagCardModel == null)
            {
                int index = 0;

                while (bagCardModel == null && index < bagConfig.bonusUnlockCards.Count)
                {
                    bonusCard = bagConfig.bonusUnlockCards[index];
                    bagCardModel = RandomUnlockCard(bagConfig.bonusUnlockCards[index].tierID);
                }
            }

            if (bagCardModel != null && bagCardModel.equipmentConfig != null)
            {
                showedItem.Add(bagCardModel.equipmentConfig.id);
                bagCardModel.value = bonusCard.amount;
                giftBagModel.items.Add(bagCardModel);
                bagCardModel.isWatch = isWatch;


                //Bonus cho user thêm 1 đầu card nữa nếu user hiện đang sở hữu ít hơn 3 cue (4 nếu tính cả cue default)
                if(StatDatas.Instance.CountCueKind(StatManager.Kind.Unlocked) < REACH_CUES_AMOUNT_BONUS_SLOT)
                {
                    bagCardModel.isWatch = false;

                    OpenBagDialog.BagCardModel lastBagCardModel = RandomUnlockCard( StatManager.Tier.Standard, bagCardModel.equipmentConfig.id);
                    showedItem.Add(bagCardModel.equipmentConfig.id);
                    lastBagCardModel.value = Random.Range(1,4);
                    giftBagModel.items.Add(lastBagCardModel);
                    lastBagCardModel.isWatch = isWatch;
                }
                return;
            }
        }

        //Cue card somehow bị null
        //gen gem ra thay thế
        OpenBagDialog.BagCardModel gem = giftBagModel.items.Find(x => x.booster == BoosterType.CASH);
        if (gem != null && gem.value > 0)
        {
            giftBagModel.items.Add(new OpenBagDialog.BagCardModel()
            {
                earnType = OpenBagDialog.BagCardModel.EarnType.Booster,
                booster = BoosterType.CASH,
                value = (long)(gem.value * 1.2f),
                isWatch = isWatch
            });
        }
        else
        {
            //Gen gold ra thay thế
            OpenBagDialog.BagCardModel gold = giftBagModel.items.Find(x => x.booster == BoosterType.COIN);
            if (gold != null && gold.value > 0)
            {
                giftBagModel.items.Add(new OpenBagDialog.BagCardModel()
                {
                    earnType = OpenBagDialog.BagCardModel.EarnType.Booster,
                    booster = BoosterType.COIN,
                    value = (long)(gold.value * Random.Range(4, 7)),
                    isWatch = isWatch
                });
            }
            else
            {
                Debug.LogError("INVALID CONFIG HAS NO CARD, COIN, GEM. Distrute 1k coin alternatively");
                giftBagModel.items.Add(new OpenBagDialog.BagCardModel()
                {
                    earnType = OpenBagDialog.BagCardModel.EarnType.Booster,
                    booster = BoosterType.COIN,
                    value = 1000,
                });
            }
        }
    }

    StatManager.Tier ConvertCardTypeToCueTier(CardType cardType)
    {
        switch (cardType)
        {
            case CardType.EPIC:
                return StatManager.Tier.Legendary;
            case CardType.STANDARD:
                return StatManager.Tier.Standard;
            case CardType.RARE:
                return StatManager.Tier.Rare;
            default:
                Debug.LogError($"INVALID CARD TYPE {cardType}");
                return StatManager.Tier.Standard;
        }
    }

    /// <summary>
    /// Random BagCardModel
    /// </summary>
    /// <param name="rarity">độ hiếm của card</param>
    /// <param name="rateOwn">tỷ lệ lấy card đã sở hữu, nếu không lấy được card đã sở hữu thì random card mới</param>
    /// <returns></returns>
    private OpenBagDialog.BagCardModel RandomCueCard(StatManager.Tier rarity, float rateOwn, int tourID, List<DiceID> showedID)
    {
        OpenBagDialog.BagCardModel bagCardModel = new OpenBagDialog.BagCardModel();

        bagCardModel.earnType = OpenBagDialog.BagCardModel.EarnType.CueCard;
        //kiểm tra data có card này chưa

        float randOwn = Random.value;
        //kiểm tra get card đã có hay tạo mới
        if (randOwn < rateOwn && StatManager.Instance.GetCountKind_Simple(StatManager.Kind.UnlockedNonMaxed) > 0)
        {
            List<StatData> StatDatas = StatManager.Instance.GetDatasByTierKind_Simple(
                                                        rarity,
                                                        StatManager.Kind.UnlockedNonMaxed,
                                                        StatManager.FilterHaveRequireCards)
                                    .Where(x => !showedID.Contains(x.id)).ToList();


            if (StatDatas != null)
            {
                StatData statsData = StatDatas.Shuffle().FirstOrDefault();

                if (statsData != null)
                {
                    bagCardModel.equipmentConfig = statsData;
                }
                else //if statsDatas has 0 element or some other error occur
                {
                    Debug.Log(string.Format("Get random {0} as there is no same rarity in data ", rarity));
                    //random in equipment
                    bagCardModel.equipmentConfig = StatManager.Instance.GetDatasByKind_Complex( StatManager.Kind.Unlocked, StatManager.FilterHaveRequireCards)
                            .Where(x => !showedID.Contains(x.id))
                            .ToList().Shuffle()
                            .FirstOrDefault();
                }
            }
        }
        else
        {
            //random new equipment
            bagCardModel.equipmentConfig = StatManager.Instance.GetDatasByTierKind_Simple(rarity, StatManager.Kind.NotUnlocked, StatManager.FilterHaveRequireCards)
                    .Where(x => !showedID.Contains(x.id) && !x.config.isHide)
                    .ToList().Shuffle()
                    .FirstOrDefault();
        }

        return bagCardModel;
    }

    /// <summary>
    /// Random BagCardModel
    /// </summary>
    /// <param name="rarity">độ hiếm của card</param>
    /// <param name="rateOwn">tỷ lệ lấy card đã sở hữu, nếu không lấy được card đã sở hữu thì random card mới</param>
    /// <returns></returns>
    public OpenBagDialog.BagCardModel RandomCue(StatManager.Tier rarity)
    {
        OpenBagDialog.BagCardModel bagCardModel = new OpenBagDialog.BagCardModel();

        bagCardModel.earnType = OpenBagDialog.BagCardModel.EarnType.Cue;
        //kiểm tra data có card này chưa

        StatData cue = StatManager.Instance.GetDatasByTierKind_Complex(rarity, StatManager.Kind.NotUnlocked)
                .Shuffle()
                .FirstOrDefault();

        if(cue == null)
            cue = StatManager.Instance.GetDatasByKind_Complex(StatManager.Kind.NotUnlocked)
                .Shuffle()
                .FirstOrDefault();
        //random new equipment
        bagCardModel.equipmentConfig = cue;

        return bagCardModel;
    }

    public OpenBagDialog.BagCardModel RandomUnlockCard(StatManager.Tier rarity, DiceID showedIds =  DiceID.NONE)
    {
        OpenBagDialog.BagCardModel bagCardModel = new OpenBagDialog.BagCardModel();

        bagCardModel.earnType = OpenBagDialog.BagCardModel.EarnType.CueCard;
        bagCardModel.isWatch = true;

        StatData c = StatManager.Instance.QueryCueForUnlockBonusCard(rarity, showedIds);
        if(c == null)
        {
            return null;
        }
        else
        {
            bagCardModel.equipmentConfig = c;

        }

        return bagCardModel;
    }

    public OpenBagDialog.BagCardModel GetStartingCue()
    {
        OpenBagDialog.BagCardModel bagCardModel = new OpenBagDialog.BagCardModel();

        bagCardModel.earnType = OpenBagDialog.BagCardModel.EarnType.Cue;
        //kiểm tra data có card này chưa

        bagCardModel.equipmentConfig = StatDatas.Instance.GetStat( DiceID.FIRE);

        bagCardModel.value = 1;

        return bagCardModel;
    }
}

[System.Serializable]
public class GiftBagToursConfig
{
    public BagType bag;
    public List<GiftBagPerTourConfig> gifts;

    public GiftBagPerTourConfig GetGiftBagByTour(int tour)
    {
        return this.gifts.Find(x => x.tour == tour);
    }
}

/// <summary>
/// Lưu config theo tour
/// </summary>
[System.Serializable]
public class GiftBagPerTourConfig: GiftBagConfig
{
    [Header("Tour Origin bag by H")]
    public int tour;
    public List<CardAmount> cardAmounts;

    [Header("Bonus card, watch ads to claim")]
    public float rateBonus;

     public List<CueRatePair> bonusUnlockCards;
    [Header("Rate getting Cue")]
    public float rateCue;

    public List<CueRatePair> cueLists;

    public CardAmount GetCardAmount(CardType cardType)
    {
        return this.cardAmounts.Find(x => x.cardType == cardType);
    }

    public int GetCountCard(CardType cardType)
    {
        CardAmount cardAmount = this.GetCardAmount(cardType);
        if (cardAmount != null)
            return cardAmount.amount;
        return 0;
    }
    public int GetTotalCard()
    {
        int res = 0;
        for (int i = 0; i < this.cardAmounts.Count; i++)
        {
            res += this.cardAmounts[i].amount;
        }

        return res;
    }
    public int GetTotalCard(StatManager.Tier tier)
    {
        CardAmount c = this.cardAmounts.Find(x => x.tier == tier);
        if (c == null)
            return 0;
        else
            return c.amount;
    }
    public int GetTotalCard(bool excludeCountry)
    {
        int res = 0;
        for (int i = 0; i < this.cardAmounts.Count; i++)
        {
            res += this.cardAmounts[i].amount;
        }

        return res;
    }

    public CueRatePair GetRandomBonusCard()
    {
        if (this.bonusUnlockCards.Count > 0)
        {
            float rand = UnityEngine.Random.Range(0f, 1f);
            int reward = 0;

            float totalValue = bonusUnlockCards[reward].rate;
            float rateValue = rand - totalValue;
            while (rateValue > 0)
            {
                reward += 1;
                totalValue += bonusUnlockCards[reward].rate;
                rateValue = rand - totalValue;
            }

            if (reward >= 0 && reward < bonusUnlockCards.Count)
            {
                return bonusUnlockCards[reward];
            }

            return bonusUnlockCards[0];
        }

        return null;
    }

    public StatManager.Tier GetCueTier()
    {
        float rand = UnityEngine.Random.Range(0f, 1f);
        int reward = 0;

        float totalValue = cueLists[reward].rate;
        float rateValue = rand - totalValue;
        while (rateValue > 0)
        {
            reward += 1;
            totalValue += cueLists[reward].rate;
            rateValue = rand - totalValue;
        }

        if (reward >= 0 && reward < cueLists.Count)
        {
            return cueLists[reward].tierID;
        }

        return StatManager.Tier.Standard;
    }

    public bool IsRewardCue()
    {
        return UnityEngine.Random.Range(0f, 1f) <= this.rateCue;
    }
}

/// <summary>
/// Lưu giá trị dùng để random
/// </summary>
[System.Serializable]
public class GiftBagConfig
{
    public ValueRandom valueCoin;
    public ValueRandom valueGem;
}

[System.Serializable]
public class CardRandom
{
    [Header("Tỷ lệ lấy card đã sở hữu")]
    public float rateOwn = 0.8f;
    public ValueRandom value;
}


[System.Serializable]
public class ValueRandom
{
    [Header("Giá trị max, min")]
    public long min;
    public long max;

    public long rangeValue => (max - min);
    public float countStep => ((float)rangeValue) / step;
   
    [Header("Giá trị 1 step khi random")]
    public long step = 1;

    [Header("Tỷ lệ phân phối vào giá trị max (0-1f)")]
    public float rateOfReceiptMax;

    public float ratePerStep => ((1f - rateOfReceiptMax) / ((float)(max - min)))* step; //0.9/900 * 100 = 
    public long GetRandomValue()
    {
        float rateRand = Random.value;
        long value = max;
        float rateValue = rateOfReceiptMax;

        //Debug.LogError("Rate rand " + rateRand);
        //Debug.LogError("Rate per step " + ratePerStep);
        while (rateRand > rateValue)
        {
            value -= step;
            if (value < min)
            {
                return min;
            }

            float rateAdd = ((ratePerStep * rangeValue * rangeValue) / (float)(value * value));// * rateOfReceiptMax;
            rateValue += rateAdd; //value càng nhỏ => rate cộng vào càng lớn
            //Debug.LogError("Rate step "+ rateValue);   
        }
        return value;
    }
}

[System.Serializable]
public class CueRatePair
{
    public StatManager.Tier tierID;
    public float rate;

    public int amount;
}