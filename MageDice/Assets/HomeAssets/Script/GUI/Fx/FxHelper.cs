using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

/// <summary>
/// for effect boosters
/// </summary>
public class FxHelper : MonoSingleton<FxHelper>
{
    [Header("Round transform")]
    public Transform tranBotLeft;
    public Transform tranTopRight;

    [Header("Fx booster")]
    public FxBooster fxBooster;

    [Header("Fx booster par")]
    public FxBoosterParticle fxBoosterPar;

    [Header("Use fx particle")]
    public bool useFxParticle;

    //giá trị scale lại khoản cách cho đúng với các màn hình
    private Vector3 distaceScale;
    private Dictionary<string,Stack> dicStacks;

    private void OnEnable()
    {
        this.dicStacks = new Dictionary<string, Stack>();
    }


    private void Start()
    {
        this.distaceScale.x = this.tranTopRight.position.x - this.tranBotLeft.position.x;
        this.distaceScale.y = this.tranTopRight.position.y - this.tranBotLeft.position.y;
    }

    /// <summary>
    /// Fx collect booster: Nổ ra, bay về
    /// </summary>
    /// <param name="booster"> loại booster</param>
    /// <param name="start">transform nhận</param>
    /// <param name="des">transform bay về</param>
    /// <param name="num">số lượng</param>
    /// <param name="scale0">scale lúc tạo ra</param>
    /// <param name="scale1">scale nổ</param>
    /// <param name="scale1">scale lúc bay về</param>
    /// <param name="rangeX0">kích thước vụ nổ</param>
    /// <param name="rangeY0">kích thước vụ nổ</param>
    /// <param name="time0">time nổ</param>
    /// <param name="time1">time bay về</param>
    /// <param name="delayPer">time bay về của từng cái</param>
    /// <param name="callback"></param>
    public void ShowFxCollectBooster(BoosterType booster, Transform start, Transform des, int num = 10, float scale0 = 0.2f, float scale1=1f, float scale2= 0.4f, float rangeX0 = 100, float rangY0 = 100,   float speed0 = 10f, float speed1 = 22f, UnityAction callback = null)
    {

        BoosterConfig boosterConfig = BoosterConfigs.Instance.GetBooster(booster);
        if (boosterConfig == null) return;
        if (boosterConfig.spr == null) return;
        if (booster == BoosterType.COIN || booster == BoosterType.CASH)
            SoundManager.Instance.Play("snd_collect_coin2");
        ShowFxCollectBySprite(boosterConfig.spr, start, des, callback, num, scale0, scale1, scale2, rangeX0, rangY0, speed0, speed1);
    }

    public void ShowFxCollectBoosterByPos(BoosterType booster, Vector3 posStart, Vector3 posDes, int num = 10, float scale0 = 0.2f, float scale1 = 1f, float scale2 = 0.4f, float rangeX0 = 100, float rangY0 = 100, float speed0 = 10f, float speed1 = 22f, UnityAction callback = null)
    {
        BoosterConfig boosterConfig = BoosterConfigs.Instance.GetBooster(booster);
        if (boosterConfig == null) return;
        if (boosterConfig.spr == null) return;
        if (booster == BoosterType.COIN || booster == BoosterType.CASH)
            SoundManager.Instance.Play("snd_collect_coin2");
        ShowFxCollectBySpriteAndPos(boosterConfig.spr, posStart, posDes, callback, num, scale0, scale1, scale2, rangeX0, rangY0, speed0, speed1);
    }


    public void ShowFxCollectBooster(BoosterCommodity booster, Transform start, UnityAction callback = null)
    {
        long num = booster.GetValue();
        if (num > 10) num = 10;

        //get end value là giá trị hiện tại
        
        long endValue = UserBoosters.Instance.GetBoosterCommodity(booster.type).GetValue();
        long curValue = endValue - booster.GetValue();
        if (curValue < 0) curValue = 0;

        Debug.Log("<color=yellow>Cur value </color>" + curValue);
        Debug.Log("<color=yellow>End value </color>" + endValue);

        if (booster.type == BoosterType.COIN || booster.type == BoosterType.CASH)
            SoundManager.Instance.Play("snd_collect_coin2");

        switch (booster.type)
        {
            case BoosterType.COIN:
                HomeTopUI.Instance.coinBoosterUI.TxtCoin.AddValueAnimtion(curValue, endValue, 1f, 1f, callback);
                if (!useFxParticle) ShowFxCollectBooster(booster.type, start, HomeTopUI.Instance.coinBoosterUI.GetTranIcon(), (int)num, 0.2f, 1, 0.4f, 100f, 100f, 10f, 22f); //, callback
                else ShowFxCollectBoosterPar(booster.type, start, HomeTopUI.Instance.coinBoosterUI.GetTranIcon(), (int)num, 1f, 2f);//callback
                break;
            case BoosterType.CASH:
                HomeTopUI.Instance.cashBoosterUI.TxtCoin.AddValueAnimtion(curValue, endValue, 1f, 1f, callback);
                if (!useFxParticle) ShowFxCollectBooster(booster.type, start, HomeTopUI.Instance.cashBoosterUI.GetTranIcon(), (int)num, 0.2f, 1, 0.4f, 100f, 100f, 10f, 22f); //, callback
                else ShowFxCollectBoosterPar(booster.type, start, HomeTopUI.Instance.cashBoosterUI.GetTranIcon(), (int)num, 1f, 2f); //, callback
                break;
        }
    }

    public void ShowFxCollectBoosters(List<BoosterCommodity> boosters, Transform start, UnityAction callback = null)
    {
        if (boosters != null)
        {
            for (int i = 0; i < boosters.Count; i++)
            {
                ShowFxCollectBooster(boosters[i], start, (i == 0)? callback: null);   
            }
        }       
    }

    /// <summary>
    /// Show booster có particle
    /// </summary>
    /// <param name="booster"></param>
    /// <param name="start"></param>
    /// <param name="des"></param>
    /// <param name="num"></param>
    /// <param name="scale"></param>
    /// <param name="time"></param>
    /// <param name="callback"></param>
    public void ShowFxCollectBoosterPar(BoosterType booster, Transform start, Transform des, int num = 10, float scale = 1f, float time = 2f, UnityAction callback = null)
    {
        fxBoosterPar.ShowFx(start.position, des.position, num, scale, time, callback);
    }

    /// <summary>
    /// Show booster theo sprite
    /// </summary>
    /// <param name="spr"></param>
    /// <param name="start"></param>
    /// <param name="des"></param>
    /// <param name="callback"></param>
    /// <param name="num"></param>
    /// <param name="scale0"></param>
    /// <param name="scale1"></param>
    /// <param name="scale2"></param>
    /// <param name="rangeX0"></param>
    /// <param name="rangY0"></param>
    /// <param name="speed0"></param>
    /// <param name="speed1"></param>
    public void ShowFxCollectBySprite(Sprite spr, Transform start, Transform des, UnityAction callback = null, int num = 10, float scale0 = 0.2f, float scale1 = 1f, float scale2 = 0.4f, float rangeX0 = 100, float rangY0 = 100, float speed0 = 10f, float speed1 = 22f)
    {
        if (spr == null) return;

        if (this.dicStacks != null)
        {
            if (!this.dicStacks.ContainsKey(spr.name)) this.dicStacks.Add(spr.name, new Stack());
            if (this.dicStacks.ContainsKey(spr.name))
            {
                Sequence seq = DOTween.Sequence();
                List<FxBooster> coins = new List<FxBooster>();
                Stack stack = this.dicStacks[spr.name];
                if (stack != null)
                {
                    //check không đủ stack
                    if (stack.Count < num)
                    {
                        int exceed = num - stack.Count;
                        for (int i = 0; i < exceed; i++)
                        {
                            FxBooster fxBooster = Instantiate(this.fxBooster, this.transform);
                            fxBooster.ShowBooster(spr);
                            stack.Push(fxBooster);
                        }
                    }

                    float time0 = (rangeX0 / speed0) * Time.deltaTime;

                    Vector3 posStart = start.position;
                    Vector3 posEnd = des.position;

                    //lấy chiều y (chiều dọc làm chuẩn)
                    posStart.x = (start.position.x - this.tranBotLeft.position.x) / this.distaceScale.y;
                    posStart.y = (start.position.y - this.tranBotLeft.position.y) / this.distaceScale.y;
                    posStart.z = 0f;

                    posEnd.x = (des.position.x - this.tranBotLeft.position.x) / this.distaceScale.y;
                    posEnd.y = (des.position.y - this.tranBotLeft.position.y) / this.distaceScale.y;
                    posEnd.z = 0f;

                    float distance = Vector3.Distance(posStart, posEnd) * 1280f; //1280f là màn hình chuẩn

                    Debug.Log("<color=blue>Distance fx move: </color>" + distance);

                    float time1 = Mathf.Abs((distance / (speed1 * 1.5f)) * Time.deltaTime); //1.5f là hệ số bổ sung thêm 

                    if (time1 < 0.4f) time1 = 0.4f;
                    if (time1 > 1f) time1 = 1f;

                    //tất cả coin bung tròn ra
                    for (int i = 0; i < num; i++)
                    {
                        FxBooster coin = stack.Pop() as FxBooster;
                        coin.transform.position = start.position;
                        coin.transform.localScale = new Vector3(scale0, scale0);
                        coin.gameObject.SetActive(true);

                        float xAxis = Random.Range(-rangeX0, rangeX0);
                        float yAxis = Random.Range(-rangY0 / 2, rangY0);

                        Vector3 axis = new Vector3(xAxis + coin.transform.position.x, yAxis + coin.transform.position.y, 0);

                        coins.Add(coin);
                        seq.Join(coin.transform.DOMove(axis, time0));
                        seq.Join(coin.transform.DOScale(scale1, time0));
                    }
                    //lần lượt từng coin bay vào des
                    des.position = new Vector3(des.position.x, des.position.y, 0);
                    seq.AppendCallback(() =>
                    {
                        float delayPer = time1 / coins.Count;
                        for (int i = 0; i < coins.Count; i++)
                        {
                            int curIndex = i;
                            coins[curIndex].gameObject.transform.DOScale(scale2, time1 - (i * 0.01f));
                            coins[curIndex].gameObject.transform.DOMove(des.position, time1 - (i * 0.01f))
                                    .SetDelay(curIndex * delayPer)
                                    .SetEase(Ease.Linear)
                                    .OnComplete(() =>
                                    {
                                        coins[curIndex].gameObject.SetActive(false);
                                        stack.Push(coins[curIndex]);
                                    });
                        }
                    });

                    seq.OnComplete(() =>
                    {
                        callback?.Invoke();
                    });
                }
            }
        }
    }

    public void ShowFxCollectBySpriteAndPos(Sprite spr, Vector3 posStart, Vector3 posDes, UnityAction callback = null, int num = 10, float scale0 = 0.2f, float scale1 = 1f, float scale2 = 0.4f, float rangeX0 = 100, float rangY0 = 100, float speed0 = 10f, float speed1 = 22f)
    {
        posStart.z = 0f;
        posDes.z = 0f;

        if (spr == null) return;

        if (this.dicStacks != null)
        {
            if (!this.dicStacks.ContainsKey(spr.name)) this.dicStacks.Add(spr.name, new Stack());
            if (this.dicStacks.ContainsKey(spr.name))
            {
                Sequence seq = DOTween.Sequence();
                List<FxBooster> coins = new List<FxBooster>();
                Stack stack = this.dicStacks[spr.name];
                if (stack != null)
                {
                    //check không đủ stack
                    if (stack.Count < num)
                    {
                        int exceed = num - stack.Count;
                        for (int i = 0; i < exceed; i++)
                        {
                            FxBooster fxBooster = Instantiate(this.fxBooster, this.transform);
                            fxBooster.ShowBooster(spr);
                            stack.Push(fxBooster);
                        }
                    }

                    float time0 = (rangeX0 / speed0) * Time.deltaTime;
                    float distance = Vector3.Distance(posStart, posDes) * 1280f; //1280f là màn hình chuẩn

                    Debug.Log("<color=blue>Distance fx move: </color>" + distance);

                    float time1 = Mathf.Abs((distance / (speed1 * 1.5f)) * Time.deltaTime); //1.5f là hệ số bổ sung thêm 

                    if (time1 < 0.4f) time1 = 0.4f;
                    if (time1 > 1f) time1 = 1f;

                    //tất cả coin bung tròn ra
                    for (int i = 0; i < num; i++)
                    {
                        FxBooster coin = stack.Pop() as FxBooster;
                        coin.transform.position = posStart;
                        coin.transform.localScale = new Vector3(scale0, scale0);
                        coin.gameObject.SetActive(true);

                        float xAxis = Random.Range(-rangeX0, rangeX0);
                        float yAxis = Random.Range(-rangY0 / 2, rangY0);

                        Vector3 axis = new Vector3(xAxis + coin.transform.position.x, yAxis + coin.transform.position.y, 0);

                        coins.Add(coin);
                        seq.Join(coin.transform.DOMove(axis, time0));
                        seq.Join(coin.transform.DOScale(scale1, time0));
                    }
                    //lần lượt từng coin bay vào des
                    seq.AppendCallback(() =>
                    {
                        float delayPer = time1 / coins.Count;
                        for (int i = 0; i < coins.Count; i++)
                        {
                            int curIndex = i;
                            coins[curIndex].gameObject.transform.DOScale(scale2, time1 - (i * 0.01f));
                            coins[curIndex].gameObject.transform.DOMove(posDes, time1 - (i * 0.01f))
                                    .SetDelay(curIndex * delayPer)
                                    .SetEase(Ease.Linear)
                                    .OnComplete(() =>
                                    {
                                        coins[curIndex].gameObject.SetActive(false);
                                        stack.Push(coins[curIndex]);
                                    });
                        }
                    });

                    seq.OnComplete(() =>
                    {
                        callback?.Invoke();
                    });
                }
            }
        }
    }

    #region Convert position => ratio in canvas
    public Vector3 GetPositionByRatio(Vector3 ratio)
    {
        return GameUtils.ConvertRatioInCanvasToPos(ratio, this.tranBotLeft.position, this.tranTopRight.position);
    }

    public Vector3 GetRatioInCanvasByPos(Vector3 pos)
    {
        return GameUtils.ConvertPosInCanvasToRatio(pos, this.tranBotLeft.position, this.tranTopRight.position);
    }

    ///// <summary>
    ///// Chuyển đổi tọa độ trên popup (canvas game) về tọa độ canvas fx
    ///// </summary>
    ///// <param name="position">tọa độ trên popup (canvas game)</param>
    ///// <returns></returns>
    //public Vector3 GetPosFxInGameCanvas(Vector3 position)
    //{
    //    GameScene gameScene = GameManager.Instance.GetScene() as GameScene;
    //    if (gameScene == null) return Vector3.zero;
    //    Vector3 rUIPos = gameScene.GetRatioInCanvasByPos(position);
    //    return GetPositionByRatio(rUIPos);
    //}
    #endregion
}
