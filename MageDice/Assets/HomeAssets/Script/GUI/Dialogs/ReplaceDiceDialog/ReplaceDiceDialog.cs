using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class ReplaceDiceDialog : BaseSortingDialog
{
    [SerializeField]
    InventoryStatDeck decks;

    [SerializeField]
    StatItemGUI itemDisplayer;
    private StatData _data;
    public StatData Data
    {
        get { return this._data; }
    }

    private List<DiceID> cacheUsing;
    private System.Action<string> onUseCueSucces;
    public override void OnShow(object data = null, UnityAction callback = null)
    {
        base.OnShow(data, callback);

        this._data = this.data as StatData;
        if (this._data != null)
            this.ParseData();
    }
    public void SetCallback(System.Action<string> onUseCueSucces)
    {
        this.onUseCueSucces = onUseCueSucces;
    }
    private void ParseData()
    {
        cacheUsing = new List<DiceID>(StatDatas.Instance.CurrentStatId);
        decks.ParseData(cacheUsing);
        decks.SetClick(OnClickReplace);
        itemDisplayer.ParseData(this._data);
    }

    private void OnClickReplace(DiceID idReplace)
    {
        StatDatas datas = StatDatas.Instance;
        StatManager manager = StatManager.Instance;

        if(manager.IsUsing(idReplace) && !manager.IsUsing(this.Data.id))
        {
            int index = cacheUsing.IndexOf(idReplace);
            cacheUsing.RemoveAt(index);
            cacheUsing.Insert(index, this.Data.id);

            StatManager.Instance.ChangeCue(cacheUsing, onUseCueSucces);
        }

        this.ClickCloseDialog();
    }
}
