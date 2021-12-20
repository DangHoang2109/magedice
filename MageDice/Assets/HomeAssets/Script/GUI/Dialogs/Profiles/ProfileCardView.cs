using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ProfileCardView : MonoBehaviour
{
   public Transform panelCard;

   //public void ParseData(LineupDatas lineup)
   //{
   //   foreach (var card in lineup.cards)
   //   {
   //      ProfileCardItem item = Instantiate(this.prefabCard, this.panelCard);
   //      StatsConfig config = StatsConfigs.GetStatsConfig(card.card, card.id);
   //      if (config != null)
   //      {
   //         item?.ParseData(config);

   //      }
   //   }
   //}

   public void Clear()
   {
      for (int i = 0; i < this.panelCard.childCount; i++)
      {
         Destroy(this.panelCard.GetChild(i).gameObject);
      }
   }
   private void OnDisable()
   {
      this.Clear();
   }
}
