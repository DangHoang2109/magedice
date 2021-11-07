using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Events;

[CreateAssetMenu(menuName = "Games/Avatars")]
public class CommonAvatar : ScriptableObject
{
    private static CommonAvatar _isntance;
    public static CommonAvatar Instance
    {
        get
        {
            if (_isntance == null)
            {

                _isntance = LoaderUtility.Instance.GetAsset<CommonAvatar>("Home/Configs/Users/Avatars");

            }
            return _isntance;
        }

    }

    public List<AvatarItem> avatars;

#if UNITY_EDITOR
    private void OnValidate()
    {
        string path = string.Format("Assets/LocalDatas/Avatars/");

        List<Sprite> sprs = GameUtils.LoadAllAssetsInFolder<Sprite>(path, new List<string> { "*.png", "*.jpg" });
        this.avatars = new List<AvatarItem>();
        this.avatars.Add(new AvatarItem(sprs[0].name, "", sprs[0], 0, AvatarType.GIFT));
        for (int i = 1; i < sprs.Count; i++)
        {
            this.avatars.Add(new AvatarItem(sprs[i].name, "", sprs[i], 500, AvatarType.BUY));
        }
    }
#endif

    public string GetRandomAvatar()
    {
        //return Random.Range(1, 25);
        if (avatars!=null)
        {
            if (avatars.Count > 0)
            {
                return avatars[Random.Range(0, avatars.Count)].avatarID;
            }
        }
        return string.Empty;
    }

    /// <summary>
    /// nên đổi tên thành get by index
    /// </summary>
    /// <param name="id"> nên đôi từ id -> index trong list</param>
    /// <returns></returns>
    public Sprite GetAvatarById(int id)
    {
        if (id >= 0 && id < this.avatars.Count)
        {
            return this.avatars[id].img;
        }
        return this.avatars[0].img;
    }
    public Sprite GetAvatarById(string avatar)
    {   
        AvatarItem item = this.avatars.Find(x => x.avatarID.Equals(avatar));
        if (item != null) return item.img;
        return this.avatars[0].img;
    }
    public Sprite GetAvatarByName(string avatar)
    {
        AvatarItem item = this.avatars.Find(x => x.img.name.Equals(avatar));
        if (item != null) return item.img;
        return this.avatars[0].img;
    }
    public bool isAvatarValid(string id)
    {
        if (string.IsNullOrEmpty(id))
        {
            return false;
        }
        AvatarItem result = this.avatars.Find(x => x.avatarID.Equals(id));
        if (result == null)
        {
            return false;
        }
        return true;
    }
    public AvatarItem GetAvatarItemById(string avatar)
    {
        AvatarItem item = this.avatars.Find(x => x.avatarID.Equals(avatar));
        if (item != null) return item;
        return this.avatars[0];
    }

    //public List<AvatarItem> GetAvatars(int count=-1)
    //{
    //    if (avatars != null)
    //    {
    //        //đẩy đã sở hữu xuống
    //        avatars = avatars.OrderBy(x => x.type).OrderBy(x=>UserAvatarData.Instance.CheckIsOwn(x)).ToList();
            
    //        if (count == -1 || count > avatars.Count)
    //        {
    //            return avatars;
    //        }
    //        else
    //        {
    //            return avatars.GetRange(0, count);
    //        }
    //    }
    //    return null;
    //}
}

//enum nguồn mua của avatar item
public enum AvatarType
{
    NONE = -1,
    GIFT = 0,
    WATCHVIDEO = 1,
    BUY = 2,
}
[System.Serializable]
public class AvatarItem
{
    public string avatarID;
    public string avatarName;
    public Sprite img;
    public long price; //giá mua, nếu hàng watch video và tặng bởi các booster thì = 0
    public AvatarType type;

    public AvatarItem()
    {
        avatarName = "0";
        price = 0;
    }
    public AvatarItem(string id, string name, Sprite img, long price, AvatarType type, bool iwOwn = false)
    {
        this.avatarID = id;
        this.avatarName = name;
        this.img = img;

        this.type = type;
        switch (this.type)
        {
            case AvatarType.BUY:
                this.price = price;
                break;
            case AvatarType.GIFT:
                this.price = 0;
                break;
            case AvatarType.WATCHVIDEO:
                this.price = 0;
                break;
        }
    }
}
