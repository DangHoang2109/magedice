using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class StorePackageTab : StoreChildTab
{
    [Header("Special")]
    //public TextMeshProUGUI tmpTimeOffer;
    public GameObject panelSpecialTile;
    public GameObject panelSpecial;
    public SpecialOfferItem specialOfferItem;

    [Header("Packages")]
    public List<StorePackageItem> packageItems;
    private Dictionary<StorePackageLayoutType, List<StorePackageItem>> dicPackageList;

    public UnityAction callbackParseSpecial;

    private List<StorePackageConfig> configs;

    //private double timeRemain;

#if UNITY_EDITOR
    protected override void OnValidate()
    {
        base.OnValidate();

        this.packageItems = new List<StorePackageItem>();
        StorePackageItem[] temps = this.GetComponentsInChildren<StorePackageItem>();
        if (temps!= null)
        {
            foreach(StorePackageItem item in temps)
            {
                if (!(item is SpecialOfferItem))
                    this.packageItems.Add(item);
            }
        }       
    }
#endif


    private void Start()
    {
        ParseData();
    }

    private void ParseData()
    {
        ParseDataPackages();
        ParseDataSpecial();
    }

    private void ParseDicPackages()
    {
        this.dicPackageList = new Dictionary<StorePackageLayoutType, List<StorePackageItem>>();

        for (int iLayout = 0; iLayout < System.Enum.GetValues(typeof(StorePackageLayoutType)).Length; iLayout++)
        {
            StorePackageLayoutType layoutType = (StorePackageLayoutType)iLayout;
            List<StorePackageItem> packs = this.packageItems.FindAll(x => x.layoutType == layoutType);
            if (packs != null) this.dicPackageList.Add(layoutType, packs);
        }
    }

    private void ParseDataPackages()
    {
        #region packages
        if (this.dicPackageList == null)
        {
            ParseDicPackages();
        }

        //lấy config từ datas
        this.configs = StorePackagesData.Instance.GetPackages();
        if (this.configs != null)
        {
            for (int iLayout = 0; iLayout < System.Enum.GetValues(typeof(StorePackageLayoutType)).Length; iLayout++)
            {
                StorePackageLayoutType layoutType = (StorePackageLayoutType)iLayout;
                List<StorePackageConfig> packs = this.configs.FindAll(x => x.layoutType == layoutType);
                if (packs != null)
                {
                    if (this.dicPackageList.ContainsKey(layoutType))
                    {
                        List<StorePackageItem> items = this.dicPackageList[layoutType];
                        if (items != null)
                        {
                            for (int i = 0; i < items.Count; i++)
                            {
                                if (i < packs.Count)
                                {
                                    items[i].gameObject.SetActive(true);
                                    items[i].ParseConfig(packs[i]);
                                }
                                else items[i].gameObject.SetActive(false);
                            }
                        }
                    }
                }
            }
        }
        #endregion
    }

    private void ParseDataSpecial()
    {
        #region special

        bool onSpecial = StoreSpecialData.Instance.IsAvailable();
        this.panelSpecialTile.gameObject.SetActive(onSpecial);
        this.panelSpecial.gameObject.SetActive(onSpecial);

        if (onSpecial)
        {
            StoreSpecialPackageConfig specialConfig = StoreSpecialData.Instance.GetSpecial();
            if (specialConfig != null)
            {
                this.specialOfferItem.ParseConfig(specialConfig);
                this.specialOfferItem.cbCloseDialog += ParseDataSpecial; //call back nếu hết h hoặc mua package
            }
               
        }
        this.callbackParseSpecial?.Invoke();
        #endregion
    }

    //private void Update()
    //{
    //    if (StorePackagesData.Instance.IsStillTimeOfPackage(ref timeRemain))
    //    {
    //        this.tmpTimeOffer.SetText(GameUtils.ConvertFloatToTime(StorePackagesData.Instance.TOTAL_TIME_WAIT - this.timeRemain, "hh'h'mm'm'"));
    //    }
    //    else
    //    {
    //        ParseData();
    //    }
    //}
}
