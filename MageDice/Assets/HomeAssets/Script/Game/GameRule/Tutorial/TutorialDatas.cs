using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// newer than TutorialData
public class TutorialDatas 
{

#if UNITY_EDITOR
    /*[UnityEditor.MenuItem("Test/Set tut phase 0")]
    public static void SetTutPhase0()
    {
        TUTORIAL_PHASE = 0;
    }
    [UnityEditor.MenuItem("Test/Set tut phase 1")]
    public static void SetTutPhase1()
    {
        TUTORIAL_PHASE = 1;
    }
    [UnityEditor.MenuItem("Test/Set tut phase 2")]
    public static void SetTutPhase2()
    {
        TUTORIAL_PHASE = 2;
    }
    [UnityEditor.MenuItem("Test/Set tut phase 3")]
    public static void SetTutPhase3()
    {
        TUTORIAL_PHASE = 3;
    }
    [UnityEditor.MenuItem("Test/Set tut phase 4")]
    public static void SetTutPhase4()
    {
        TUTORIAL_PHASE = 4;
    }
    [UnityEditor.MenuItem("Test/Set tut phase 5")]
    public static void SetTutPhase5()
    {
        TUTORIAL_PHASE = 5;
    }*/
#endif
    

    
    private const string KEY_TUTORIAL_PHASE = "TUTORIAL_PHASE";

    public const int NEVER_START_TUTORIAL = 0;//new user, chưa start tutorial nào
    public const int DONE_PHASE_FIRST = 1;//player drag and fire blue ball in potted
    public const int DONE_PHASE_AI = 2; // nó kết thúc ván đầu đầu tiên vs AI
    public const int TUT_PHASE_FINAL = 100;//Complete toàn bộ tutorial
    public static int TUTORIAL_PHASE
    {
        get
        {
            return PlayerPrefs.GetInt(KEY_TUTORIAL_PHASE, 0);
        }
        set
        {
            PlayerPrefs.SetInt(KEY_TUTORIAL_PHASE, value);
        }
    }

    public static bool IsTutorialCompleted()
    {
        return TUTORIAL_PHASE >= TUT_PHASE_FINAL;
    }

}
