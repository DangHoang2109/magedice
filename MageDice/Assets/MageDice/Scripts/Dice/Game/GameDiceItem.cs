using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GameDiceItem : BaseDiceItem
{
    public GameDiceData Data => this.GetData<GameDiceData>();

    [Header("Game UI")]
    public GameObject gDot;
    public Image[] dots;

    public override void SetData<T>(T data)
    {
        base.SetData(data);
    }
    public override void Display()
    {
        base.Display();

        DisplayDot();
    }
    public virtual void DisplayDot()
    {
        int dot = this.Data.Dot;
        if (dot > 0 && dot <= 6)
        {
            this.gDot.SetActive(true);
            DiceDotConfig config = DiceConfigs.Instance.GetDotConfig(dot);

            if (config != null)
            {
                for (int i = 0; i < this.dots.Length; i++)
                {
                    if (i < config.positions.Count)
                    {
                        dots[i].color = this.Data.color;
                        dots[i].gameObject.SetActive(true);
                        dots[i].transform.localPosition = config.positions[i];
                    }
                    else
                        dots[i].gameObject.SetActive(false);
                }
            }
        }
        else
            this.gDot.SetActive(false);
    }

    public void Active()
    {
        if (this.Data != null)
            this.Data.ActiveEffect();
    }
}
