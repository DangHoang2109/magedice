using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
using DG.Tweening;
using System.Linq;
using UnityEngine.Events;

public class SkillPerkDialog : BaseSortingDialog
{
    public int CountDummyStep;
    public float timeFlashy;

    [Header("Items")]
    public SkillPerkIconItem[] standardItems;
    public SkillPerkIconItem[] rareItems;
    public SkillPerkIconItem[] epicItems;
    [Header("UI")]
    public TextMeshProUGUI tmpUpgradeTimes;
    public IBooster bstUpgradeCost;

    [Header("Result")]
    public PerkUpgradeResult resultPart;
    public GameObject gButtonClose;

    private Dictionary<SkillPerkAssets.PerkRank, List<SkillPerkIconItem>> dicItems;
    private List<SkillPerkIconItem> listItems;
    private bool _isAnimating;
    private PerkDatas PerkData;

    public override void OnShow(object data = null, UnityAction callback = null)
    {
        PerkData = PerkDatas.Instance;

        if (dicItems == null)
            PrepareData();

        this.resultPart.Clear();
        base.OnShow(data, callback);

        ParseData();
    }
    public void PrepareData()
    {
        dicItems = new Dictionary<SkillPerkAssets.PerkRank, List<SkillPerkIconItem>>();
        dicItems.Add(SkillPerkAssets.PerkRank.STANDARD, new List<SkillPerkIconItem>(standardItems));
        dicItems.Add(SkillPerkAssets.PerkRank.RARE, new List<SkillPerkIconItem>(rareItems));
        dicItems.Add(SkillPerkAssets.PerkRank.EPIC, new List<SkillPerkIconItem>(epicItems));

        listItems = new List<SkillPerkIconItem>();
        listItems.AddRange(standardItems);
        listItems.AddRange(rareItems);
        listItems.AddRange(epicItems);
    }

    public void ParseData()
    {
        _isAnimating = false;

        OnUpdateInfo();

        List<PerkData> datas = PerkData.data;

        for (int i = 0; i < this.listItems.Count; i++)
        {
            if (i >= datas.Count)
                break;

            this.listItems[i].ParseData(datas[i]);
        }
    }

    public void OnUpdateInfo()
    {
        tmpUpgradeTimes.SetText($"Upgrade {PerkData.TotalUpgradeStep} times");
        bstUpgradeCost.ParseBooster(PerkData.CostUpgradeNext);
    }

    public void OnClickUpgrade()
    {
        if (!_isAnimating)
        {
            if (PerkData.OnClickUpgrade())
            {
                StartAnnimateUpgrade(PerkData.GetUpgradeResult());
            }
            else
            {
                GameUtils.ShowNeedMoreBooster(PerkData.CostUpgradeNext, ()=>StartAnnimateUpgrade(PerkData.GetUpgradeResult()));
            }
        }
    }

    private void StartAnnimateUpgrade(int id)
    {
        _isAnimating = true;
        gButtonClose.SetActive(false);

        List<SkillPerkIconItem> indexDummyRandom = this.listItems.Where(x => x.ID != id).QueryRandom(CountDummyStep).ToList();
        if (CountDummyStep > indexDummyRandom.Count)
            indexDummyRandom.AddRange(this.listItems.Where(x => x.ID != id).QueryRandom(CountDummyStep).ToList());

        indexDummyRandom.Add(this.listItems.Find(x => x.ID == id));

        Sequence seq = DOTween.Sequence();
        for (int i = 0; i < indexDummyRandom.Count; i++)
        {
            seq.Append(indexDummyRandom[i].DoAnimateUpgrade(timeFlashy));
        }

        SkillPerkIconItem lastItem = indexDummyRandom[indexDummyRandom.Count - 1];
        seq.Append(lastItem.DoAnimateRandomComplete(timeFlashy));
        seq.OnComplete(()=> {
            CompleteAnimatedUpgrade(id);
        });
    }
    private void CompleteAnimatedUpgrade(int id)
    {
        PerkData.OnCompleteUpgrade(id);

        PerkData data = PerkData.data.Find(x => x.id == id);

        this.listItems.Find(x => x.ID == id).ParseData(data);

        resultPart.ParseData(data);
        resultPart.StartAnimate(() =>
        {
            gButtonClose.SetActive(true);
            OnUpdateInfo();
        });

        _isAnimating = false;
    }
}
