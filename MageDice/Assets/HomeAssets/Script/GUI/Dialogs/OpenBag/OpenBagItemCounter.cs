using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class OpenBagItemCounter : MonoBehaviour
{
    // [System.Serializable]
    // public class BagKindCard
    // {
    //     public BagType type;
    //     public Color col;
    //     public Sprite sprCard;
    //
    //     public static BagType ExtractType(BagKindCard b)
    //     {
    //         return b.type;
    //     }
    //
    //     public static BagKindCard ExtractSelf(BagKindCard b)
    //     {
    //         return b;
    //     }
    // }

    [Header("linker")]
    public Image imgCardBG;
    public Image imgTextBG;
    public TextMeshProUGUI txtCounter;

    // [Header("data")]
    // public BagKindCard[] bagKindCards;
    // private Dictionary<BagType, BagKindCard> dicBagTypeCard;
    public void Init()
    {
        // if (this.bagKindCards is null || this.bagKindCards.Length == 0)
        // {
        //     this.dicBagTypeCard = new Dictionary<BagType, BagKindCard>(0);    
        // }
        // else
        // {
        //     this.dicBagTypeCard = this.bagKindCards.ToDictionary(BagKindCard.ExtractType, BagKindCard.ExtractSelf);
        // }
    }
    
    public void Parse(BagType type, int countStart)
    {
        // if (this.dicBagTypeCard.ContainsKey(type))
        // {
        //     this.imgCardBG.sprite = this.dicBagTypeCard[type].sprCard;
        //     this.imgTextBG.color = this.dicBagTypeCard[type].col;
        // }
        this.txtCounter.text = countStart.ToString();
    }

    public void UpdateCount(int count)
    {
        this.txtCounter.text = count.ToString();
    }
}
