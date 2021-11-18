using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Configs/GameAssetsConfigs", fileName = "GameAssetsConfigs")]
public class GameAssetsConfigs : ScriptableObject
{
    [Header("Animation button")]
    public RuntimeAnimatorController animBtController;

    [Header("Booster")]
    public BoosterConfigs boosters;

    [Header("Room assets")]
    public RoomAssetConfigs roomAssets;

    [Header("Bag asset")]
    public BagAssetConfigs bagAsset;

    [Header("Card asset")]
    public CardAssetConfigs cardAsset;

    [Header("Coin cash with value")]
    public SpriteIconValueConfigs valueIconAsset;

    [Header("Spire point battle pass")]
    public Sprite sprPointBattePass;

    [Header("Battle pass")]
    public BattlePassAssetConfigs battlePassAssets;

    [Header("Glove collection")]
    [HideInInspector]
    public GloveAssets gloveAssets;

    public static GameAssetsConfigs Instance
    {
        get
        {
            return LoaderUtility.Instance.GetAsset<GameAssetsConfigs>("Home/Configs/GameAssetsConfigs");
        }
    }
}

#region BOOSTER
[System.Serializable]
public class BoosterConfigs
{
    public List<BoosterConfig> boosters;
    public static BoosterConfigs Instance
    {
        get
        {
            return GameAssetsConfigs.Instance.boosters;
        }
    }

    public BoosterConfig GetBooster(BoosterType type)
    {
        return this.boosters.Find(x => x.type == type);
    }
}


[System.Serializable]
public class BoosterConfig
{
    public BoosterType type;
    public string name;
    public string description;
    public Sprite spr;
    public Sprite sprOff;
}
#endregion

#region ROOM ASSET
[System.Serializable]
public class RoomAssetConfigs
{
    public List<RoomAssetConfig> roomAssets;

    public static RoomAssetConfigs Instance
    {
        get
        {
            return GameAssetsConfigs.Instance.roomAssets;
        }
    }

    public List<RoomAssetConfig> GetRoomAssets()
    {
        return this.roomAssets;
    }

    public RoomAssetConfig GetRoomAsset(int id)
    {
        return this.roomAssets.Find(x => x.id == id);
    }
}

[System.Serializable]
public class RoomAssetConfig
{
    public int id;
    public Sprite[] sprIcons;

    //Rooom bg (không dùng)
    //public Sprite sprBg;
    //public Sprite sprBgHome;

    public Sprite sprMask;
    public Color colorRoom;
}
#endregion

#region Bag asset
[System.Serializable]
public class BagAssetConfigs
{
    public List<BagAssetConfig> bagAssets;

    public static BagAssetConfigs Instance
    {
        get
        {
            return GameAssetsConfigs.Instance.bagAsset;
        }
    }

    public List<BagAssetConfig> GetBagAssets()
    {
        return this.bagAssets;
    }

    public BagAssetConfig GetBagAsset(BagType type)
    {
        return this.bagAssets.Find(x => x.type == type);
    }
}

[System.Serializable]
public class BagAssetConfig
{
    public BagType type;
    public string name; //name of bag type
    public Sprite sprBag;
}

#endregion

#region Card asset
[System.Serializable]
public class CardAssetConfigs
{
    public static CardAssetConfigs Instance
    {
        get
        {
            return GameAssetsConfigs.Instance.cardAsset;
        }
    }

    public List<CardAssetConfig> cardAssets;

    //public CardAssetConfig GetCardAsset(CardType cardType)
    //{
    //    return this.cardAssets.Find(x => x.cardType == cardType);
    //}

    //public CardAssetConfig GetCardAsset(CueSystem.CueManager.Tier cardType)
    //{
    //    return this.cardAssets.Find(x => x.tier == cardType);
    //}
}

[System.Serializable]
public class CardAssetConfig
{
    //public CueSystem.CueManager.Tier tier;
    //public CardType cardType;
    public string name;
    public Sprite sprCard;
    public Color color;
}

#endregion

#region Coin sprite
[System.Serializable]
public class SpriteIconValueConfigs
{
    public static SpriteIconValueConfigs Instance
    {
        get
        {
            return GameAssetsConfigs.Instance.valueIconAsset;
        }
    }

    [Header("Coins")]
    public List<SpriteValueIconConfig> coinIcons;

    [Header("Cashs")]
    public List<SpriteValueIconConfig> cashIcons;

    public Sprite GetSpriteCoin(long value)
    {
        for (int i = this.coinIcons.Count - 1; i >= 0; i--)
        {
            if (this.coinIcons[i].value <= value) return this.coinIcons[i].spr;
        }
        return this.coinIcons[0].spr;
    }

    public Sprite GetSpriteCash(long value)
    {
        for (int i = this.cashIcons.Count - 1; i >= 0; i--)
        {
            if (this.cashIcons[i].value <= value) return this.cashIcons[i].spr;
        }
        return this.cashIcons[0].spr;
    }

    public Sprite GetSprite(BoosterType type, long value)
    {
        switch (type)
        {
            case BoosterType.COIN:
                return GetSpriteCoin(value);
            case BoosterType.CASH:
                return GetSpriteCash(value);
        }
        return null;
    }
}

[System.Serializable]
public class SpriteValueIconConfig
{
    public long value;
    public Sprite spr;
}
#endregion

#region card border
[System.Serializable]
public class CardBorderConfig
{
    public Sprite cardStats;
    public Sprite cardMaterial;
    public Sprite portraitGeneric;
}

#endregion

#region Battle pass asset

[System.Serializable]
public class BattlePassAssetConfigs
{
    public static BattlePassAssetConfigs Instance
    {
        get
        {
            return GameAssetsConfigs.Instance.battlePassAssets;
        }
    }

    public List<BattlePassAssetConfig> battlePasses;

    public List<VipTypeConfig> vipTypes;

    #region Layout
    public BattlePassAssetConfig GetBPAsset(int id)
    {
        if (this.battlePasses != null)
        {
            if (id < this.battlePasses.Count)
            {
                return this.battlePasses[id];
            }
        }
        return null;
    }

    public BattlePassAssetConfig GetRandomBPAssetWithoutId(int index)
    {
         List<BattlePassAssetConfig> battlePassAssets = new List<BattlePassAssetConfig>(this.battlePasses);
        if (battlePassAssets != null)
        {
            if (battlePassAssets.Count > index && index >= 0)
                battlePassAssets.RemoveAt(index);

            if (battlePassAssets.Count > 0)
            {

                return battlePassAssets[Random.Range(0, battlePassAssets.Count)];
            }
        }
        return null;
    }

    public BattlePassAssetConfig GetRandomBPAsset()
    {
        return GetRandomBPAssetWithoutId(-1);
    }

    /// <summary>
    /// Lấy battle pass asset theo thư tự
    /// </summary>
    /// <param name="id"></param>
    /// <returns></returns>
    public BattlePassAssetConfig GetNextBPAsset(int id)
    {
        int nextId = GetNextLayoutId(id);
        return GetBPAsset(nextId);
    }

    public int GetRandomIdBPAssetWithoutID(int id)
    {
        BattlePassAssetConfig battlePass = GetRandomBPAssetWithoutId(id);
        if (battlePass != null) return battlePass.id;
        return -1;
    }

    public int GetNextLayoutId(int id)
    {
        int nextId = id + 1;
        if (nextId >= this.battlePasses.Count) nextId = 0;
        return nextId;
    }

    public int GetRandomIdBPAsset()
    {
        BattlePassAssetConfig battlePass = GetRandomBPAsset();
        if (battlePass != null) return battlePass.id;
        return -1;
    }

    #endregion

    #region Vip type
    public List<VipTypeConfig> GetVipTypeConfigs()
    {
        List<VipTypeConfig> vipConfigs = this.vipTypes.FindAll(x => x.isVip==true);
        return vipConfigs;
    }

    //public VipTypeConfig GetVipTypeConfig(BattlepassDatas.VipType vipType)
    //{
    //    return this.vipTypes.Find(x => x.type == vipType);
    //}
    #endregion
}

[System.Serializable]
public class BattlePassAssetConfig
{
    /// <summary>
    /// Id == index in list
    /// </summary>
    public int id;

    //Một set UI sẽ gồm 3 color, được lấy từ https://flatuicolors.com/
    //Mức độ màu đậm dần
    public Color color1;
    public Color color2;
    public Color color3;
}

[System.Serializable]
public class VipTypeConfig
{
    //public BattlepassDatas.VipType type;
    public bool isVip; //có phải là gói vip không?
    public string strName; //tên
    public string strBonus; //giới thiệu bonus
    public BoosterCommodity price;

    public string GetNameContent()
    {
        return this.strName;
    }

    public string GetBonusContent()
    {
        return this.strBonus;
    }
}

#endregion

#region Glove collection

[System.Serializable]
public class GloveAssets
{
    public List<GloveAsset> gloves;

    public static GloveAssets Instance
    {
        get
        {
            return GameAssetsConfigs.Instance.gloveAssets;
        }
    }

    public List<GloveAsset> GetGloveAssets()
    {
        return this.gloves;
    }

    public GloveAsset GetGloveAsset(int id)
    {
        return this.gloves.Find(x => x.id == id);
    }

    public List<GloveAsset> GetGlovesByType(GloveType gloveType)
    {
        return this.gloves.FindAll(x => x.gloveType == gloveType);
    }

    public int GetRandomIdGloveByType(GloveType gloveType)
    {
        List<GloveAsset> gloves = GetGlovesByType(gloveType);
        if (gloves != null)
        {
            if (gloves.Count > 0)
                return gloves.GetRandom().id;
        }
        return -1;
    }

    public int GetRandomIdGlove()
    {
        if (this.gloves != null)
        {
            if (this.gloves.Count > 0)
                return this.gloves.GetRandom().id;
        }
        return -1;
    }
}


[System.Serializable]
public class GloveAsset
{
    public int id;
    public GloveType gloveType;
    public Sprite sprite;
    public string name;
    public string content;
}

public enum GloveType
{
    NONE,
    Tour, //nhận ở các tour
    WinSteak, //nhận ở win steak
    Event, //nhận ở event
}


#endregion

