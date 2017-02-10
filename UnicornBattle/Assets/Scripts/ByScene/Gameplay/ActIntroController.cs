using System.Collections;
using System.Linq;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class ActIntroController : MonoBehaviour
{
    public Text ActName;
    public Text ActIntro;
    public Button StartButton;
    public DirectionCanvasController dirCanvasController;

    public TweenColor colorFader;
    public CanvasGroup cg;

    public TweenRot titleAnimator;
    public TweenAlpha descriptionAnimator;

    public float startTime = 0;
    public float waitTime = 5;

    public DisplayStates activeState = DisplayStates.ActIntro;
    public enum DisplayStates { ActIntro, ActOutro, BossIntro, PlayerDied }

    void OnEnable()
    {
        InterstitialController.OnInterstitialFinish += OnInterstitialFinish;
    }

    void OnDisable()
    {
        InterstitialController.OnInterstitialFinish -= OnInterstitialFinish;
    }

    public void OnSkipClicked()
    {
        titleAnimator.PlayReverse();
        if (activeState == DisplayStates.BossIntro)
        {
            PF_GamePlay.OutroPane(gameObject, .5f, null);
        }
        else if (activeState == DisplayStates.PlayerDied)
        {
            ContinueToGameOver();
        }
        else
        {
            if (PF_GamePlay.UseRaidMode)
            {
                GameplayController.RaiseGameplayEvent(GlobalStrings.QUEST_COMPLETE_EVENT, PF_GamePlay.GameplayEventTypes.OutroQuest);
            }
            else
            {
                StartQuest();
            }
            PF_GamePlay.OutroPane(gameObject, .5f);
        }
    }

    void OnInterstitialFinish()
    {
        titleAnimator.ResetToBeginning();
        descriptionAnimator.ResetToBeginning();

        StartCoroutine(Spin());
    }

    public void ShowInfo(DisplayStates state)
    {
        activeState = state;

        if (state == DisplayStates.ActIntro)
        {
            UpdateFields_Act();
        }
        else if (state == DisplayStates.ActOutro)
        {
            UpdateFields_Act();
        }
        else if (state == DisplayStates.BossIntro)
        {
            // hack to make sure that we do not stomp on already existing info slides
            if (cg.alpha == 0)
            {
                UpdateFields_Boss();

                StartCoroutine(Spin());
                PF_GamePlay.IntroPane(gameObject, .75f);
            }
        }
        else if (state == DisplayStates.PlayerDied)
        {
            UpdateFields_Fail();
            PF_GamePlay.IntroPane(gameObject, .75f);
        }
    }


    public void UpdateFields_Act()
    {
        if (PF_GamePlay.ActiveQuest != null
          && activeState == DisplayStates.ActIntro
          && PF_GamePlay.ActiveQuest.levelData.Acts.Count > 0)
        {
            ActName.text = PF_GamePlay.QuestProgress.CurrentAct.Key;
            ActIntro.text = PF_GamePlay.QuestProgress.CurrentAct.Value.IntroMonolog;
        }
    }

    public IEnumerator Spin()
    {
        titleAnimator.PlayForward();
        descriptionAnimator.PlayForward();

        startTime = Time.time;
        while (startTime + waitTime > Time.time)
            yield return new WaitForEndOfFrame();

        descriptionAnimator.PlayReverse();

        StartCoroutine(PF_GamePlay.Wait(.55f, () =>
        {
            titleAnimator.PlayReverse();
            PF_GamePlay.OutroPane(gameObject, .5f);

            if (activeState == DisplayStates.PlayerDied)
                ContinueToGameOver();
            else if (PF_GamePlay.UseRaidMode)
                GameplayController.RaiseGameplayEvent(GlobalStrings.QUEST_COMPLETE_EVENT, PF_GamePlay.GameplayEventTypes.OutroQuest);
            else
                StartQuest();
        }));
    }


    public void UpdateFields_Boss()
    {
        if (PF_GamePlay.ActiveQuest != null)
        {
            ActName.text = PF_GamePlay.encounters.First().DisplayName;
            ActIntro.text = PF_GamePlay.QuestProgress.CurrentAct.Value.IntroBossMonolog;
        }
    }

    public void UpdateFields_Fail()
    {
        if (PF_GamePlay.ActiveQuest != null)
        {
            var act = PF_GamePlay.QuestProgress.CurrentAct;
            ActName.text = ActName.text = string.Format("{0} Failed!", PF_GamePlay.QuestProgress.CurrentAct.Key);
            ActIntro.text = act.Value.FailureMonolog;
        }
    }

    public void ContinueToNextAct()
    {
        dirCanvasController.gameplayController.turnController.AdvanceAct();

        UnityAction afterActAdvances = () => { ShowInfo(DisplayStates.ActIntro); };
        PF_GamePlay.Wait(1.0f, afterActAdvances);
    }

    public void ContinueToQuestComplete()
    {
        GameplayController.RaiseGameplayEvent(GlobalStrings.QUEST_COMPLETE_EVENT, PF_GamePlay.GameplayEventTypes.OutroQuest);
        PF_GamePlay.OutroPane(gameObject, .75f);
    }

    public void ContinueToGameOver()
    {
        GameplayController.RaiseGameplayEvent(GlobalStrings.PLAYER_DIED_EVENT, PF_GamePlay.GameplayEventTypes.EndQuest);
        PF_GamePlay.OutroPane(gameObject, .75f);
    }

    public void StartQuest()
    {
        PF_GamePlay.OutroPane(gameObject, .75f);
        GameplayController.RaiseGameplayEvent(GlobalStrings.QUEST_START_EVENT, PF_GamePlay.GameplayEventTypes.StartQuest);
    }
}
