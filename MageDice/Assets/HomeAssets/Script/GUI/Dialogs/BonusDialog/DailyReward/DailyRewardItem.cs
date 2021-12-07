using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
public class DailyRewardItem : MonoBehaviour
{
    private DailyRewardDayData data;
    private DailyDayClaimState state;
    private int index;

    public TextMeshProUGUI tmpDay;
    public Image imgBG;

    [Header("Prizes")]
    public IBooster bstBooster;
    public Image imageChest;

    public GameObject gChest;
    public GameObject gBooster;

    [Header("Button")]
    public GameObject gClaimed;
    public GameObject gClaim;
    public GameObject gUnreach;
    public GameObject gReClaim;

    public void ParseData(DailyRewardDayData data, int index, Sprite sprite)
    {
        this.data = data;
        state = data.state;
        this.index = index;

        this.tmpDay.SetText($"Day {index + 1}");

        this.imgBG.sprite = sprite;

        ParsePrize();
        ParseState();
    }

    private void ParsePrize() 
    {
        bool isBag = data.IsRewardBag;
        gChest.gameObject.SetActive(isBag);
        gBooster.gameObject.SetActive(!isBag);

        if (isBag)
        {
            imageChest.sprite = BagAssetConfigs.Instance.GetBagAsset(data.bag.bagType).sprBag;
        }
        else
        {
            bstBooster.ParseBooster(data.booster);
        }
    }
    private void ParseState()
    {
        gClaimed.SetActive(state == DailyDayClaimState.CLAIMED);
        gClaim.SetActive(state == DailyDayClaimState.READYCLAIM);
        gUnreach.SetActive(state == DailyDayClaimState.UNREACHED);
        gReClaim.SetActive(state == DailyDayClaimState.RECLAIM);
    }

    public void ClickClaim()
    {
        UserDailyRewardData.Instance.SetDayState(index, DailyDayClaimState.CLAIMED);
        state = DailyDayClaimState.CLAIMED;
        ParseState();

        if (data.IsRewardBag)
            MainBagSlots.OpenBagNow(data.bag, "Dailyreward");
        else
            FxHelper.Instance.ShowFxCollectBooster(data.booster, this.gClaim.transform);
    }
    public void ClickReClaim()
    {
        AdsManager.Instance.ShowVideoReward("Dailyreward", (r) =>
        {
            if (r)
                ClickClaim();
            else
                SoundManager.Instance.PlayButtonClick();
        });
    }
}
