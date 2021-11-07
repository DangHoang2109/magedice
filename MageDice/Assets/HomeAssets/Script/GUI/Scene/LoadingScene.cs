using System;
using System.Collections;
using System.Collections.Generic;
using Cosinas.Firebase;
using UnityEngine;

public class LoadingScene : BaseScene
{

   public override void OnParseData()
   {
      base.OnParseData();
      this.StartCoroutine(this.OnLoadData());

   }
   private IEnumerator OnLoadData()
   {
      yield return new WaitForEndOfFrame();
      yield return GameDataManager.Instance.OnLoadData();
      yield return new WaitForEndOfFrame();
      Vibration.Init();
      yield return new WaitForEndOfFrame();
      GameManager.Instance.OpenGame();
      yield return new WaitForEndOfFrame();
      this.OpenGame();
   }

   private void OpenGame()
   {
      if (TutorialDatas.TUTORIAL_PHASE <= TutorialDatas.NEVER_START_TUTORIAL)
      {
         //JOIN FRIST TUTORIAL FIRE BLUE BALL
         JoinGameHelper.Instance.JoinTutorial();
      }
      else
      {
         if (TutorialDatas.TUTORIAL_PHASE == TutorialDatas.DONE_PHASE_FIRST)
         {
            JoinGameHelper.Instance.JoinTutorialFristGameAI();
         }
         else
         {
            GameManager.Instance.OnLoadScene(SceneName.HOME);
         }
      }
    }
}
