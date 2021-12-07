using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MissionTarget
{
#if LITE
#else
    public virtual void DoMisison(MissionData data, long step)
    {
        
    }
    public virtual void RewardMission(MissionData data)
    {
        data.CompleteMission();
    }
    public virtual string GetName(MissionData data)
    {
        //return string.Format(LanguageManager.Instance.GetString((string.Format("MISSION_ITEM_{0}", data.type.ToString())), LanguageCategory.Feature)
        //    , GameUtils.FormatMoneyDot(data.totalStep));
        return LanguageManager.GetString(string.Format("MISSION_NAME_{0}", data.id), LanguageCategory.MissionPass);
    }
    public virtual string GetDescription(MissionData data)
    {
        return LanguageManager.GetString(string.Format("MISSION_NAME_{0}", data.id), LanguageCategory.MissionPass);
    }
    public virtual string GetPlay(MissionData data)
    {
        return LanguageManager.GetString("DES_PLAY");
    }
    public virtual bool IsComplete()
    {
        return false;
    }
#endif
}
