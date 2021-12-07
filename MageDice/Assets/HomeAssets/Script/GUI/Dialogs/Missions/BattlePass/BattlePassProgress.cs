using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

public class BattlePassProgress : MonoBehaviour
{
    private const float TopSize = 100;//0.03333f;
    private const float MaxHeight = 5060;
    private const float TopLocalPosition = 375;
    private const float ItemSize = 230;
    public ScrollRect scrollRect;
    public RectTransform bgTop;

    public RectTransform bgBot;

    public RectTransform panelLine;

    public void SetLastLevel(int lv, float delay = 0.0f)
    {
        int lastLv = lv;
        this.MoveScroll(lastLv, null, 0.1f, delay);
    }

    public void SetBackGround(int lv)
    {
        float timeMoving = 0.1f;
        float yTop = lv * ItemSize + TopSize - 2f;
        Vector2 topPos = new Vector2(this.bgTop.sizeDelta.x, yTop);
        float yBot = MaxHeight - yTop;
        Vector2 botPos = new Vector2(this.bgBot.sizeDelta.x, yBot);
        this.bgTop.DOSizeDelta(topPos, timeMoving);
        this.bgBot.DOSizeDelta(botPos, timeMoving);
        this.panelLine.DOLocalMoveY(-yTop, timeMoving);

    }

    public void SetScroll(int lv)
    {
        float scroll = ((lv - 1) > 0 ? (lv - 1) : 0) * ItemSize + TopSize +  TopLocalPosition - 100;
        this.scrollRect.content.DOLocalMoveY(scroll, 0.1f);
    }
    public void LevelUp(int lv, TweenCallback callback = null)
    {
        this.MoveScroll(lv, () =>
        {
            
            this.MoveBg(lv, callback);
        }, 0.5f, 0.5f);
    }
    
    private void MoveBg(int lv, TweenCallback callback)
    {
        float timeMoving = 0.5f;
        float yTop = lv * ItemSize + TopSize;
        Vector2 topPos = new Vector2(this.bgTop.sizeDelta.x, yTop);
        float yBot = MaxHeight - yTop;
        Vector2 botPos = new Vector2(this.bgBot.sizeDelta.x, yBot);
        Sequence seq = DOTween.Sequence();
        seq.Join(this.bgTop.DOSizeDelta(topPos, timeMoving).SetEase(Ease.Linear));
        seq.Join(this.bgBot.DOSizeDelta(botPos, timeMoving).SetEase(Ease.Linear));
        seq.Join(this.panelLine.DOLocalMoveY(-yTop, timeMoving).SetEase(Ease.Linear));
        seq.OnComplete(callback);
    }
    [ContextMenu("level up")]
    void test()
    {
        this.MoveScroll(1, () =>
        {
            this.MoveBg(1, null);
        });
    }
    private void MoveScroll(int lv, TweenCallback callback = null, float time = 0.5f, float delay = 0f)
    {
        float scroll = (lv - 1) * ItemSize + TopSize +  TopLocalPosition - 100;
        this.scrollRect.content.DOLocalMoveY(scroll, time).SetDelay(delay).SetEase(Ease.Linear).OnComplete(callback);
    }
    
    

    public void OnChangeScroll(Vector2 vec)
    {
        
    }
    
}
