using System.Collections;
using System.Collections.Generic;
using UnityEngine;
public class BaseDiceData
{
    public DiceID id;
    private DiceConfig _config;
    public DiceConfig Config
    {
        get
        {
            if (this.id == DiceID.NONE)
            {
                Debug.LogError("ID NULL");
                return null;
            }

            if (this._config == null)
                this._config = DiceConfigs.Instance.GetConfig(this.id);

            return _config;
        }
    }

    public Sprite Front => this.Config.front;
    public Color color => this.Config.dotColor;


    public virtual T SetData<T>(DiceID id, DiceConfig config = null) where T : BaseDiceData
    {
        this.id = id;
        this._config = config;

        return this as T;
    }
    public virtual void ClearData()
    {
        this.id = DiceID.NONE;
        this._config = null;
    }

}
