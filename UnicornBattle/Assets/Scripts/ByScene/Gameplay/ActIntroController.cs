using System.Collections;
using System.Linq;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

namespace UnicornBattle.Controllers
{

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

#pragma warning disable 0649
        [SerializeField] private Image m_HeroPortrait;
        [SerializeField] private Text m_HeroName;
        [SerializeField] private Image m_BossPortrait;
        [SerializeField] private Text m_BossName;
#pragma warning restore 0649

        private bool m_SkipClicked;

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
            m_SkipClicked = true;
            titleAnimator.PlayReverse();
            if (activeState == DisplayStates.BossIntro)
            {
                UBAnimator.OutroPane(gameObject, .5f, null);
            }
            else if (activeState == DisplayStates.PlayerDied)
            {
                ContinueToGameOver();
            }
            else
            {
                if (UBGamePlay.UseRaidMode)
                {
                    GameplayController.RaiseGameplayEvent(GlobalStrings.QUEST_COMPLETE_EVENT, UBGamePlay.GameplayEventTypes.OutroQuest);
                }
                else
                {
                    StartQuest();
                }
                UBAnimator.OutroPane(gameObject, .5f);
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
                    UBAnimator.IntroPane(gameObject, .75f);
                }
            }
            else if (state == DisplayStates.PlayerDied)
            {
                UpdateFields_Fail();
                UBAnimator.IntroPane(gameObject, .75f);
            }
        }

        public void UpdateFields_Act()
        {
            GameController l_gc = GameController.Instance;
            if (null == l_gc) return;

            if (l_gc.ActiveLevel != null
                && activeState == DisplayStates.ActIntro
                && l_gc.ActiveLevel.levelData.Acts.Count > 0)
            {
                ActName.text = GameController.Instance.QuestProgress.CurrentAct.Key;
                ActIntro.text = GameController.Instance.QuestProgress.CurrentAct.Value.IntroMonolog;

                if (null != m_HeroPortrait)
                    m_HeroPortrait.sprite = GameController.Instance.iconManager.GetIconById(l_gc.ActiveCharacter.baseClass.Icon, IconManager.IconTypes.Class);
                //PlayerLevel.text = "" + PF_PlayerData.activeCharacter.characterData.CharacterLevel;

                if (null != m_HeroName)
                    m_HeroName.text = l_gc.ActiveCharacter.CharacterName;

                var l_gpc = dirCanvasController.gameplayController;
                if (null == l_gpc) return;
                var l_tc = l_gpc.turnController;
                if (null == l_tc) return;
                var l_encounter = l_tc.currentEncounter;
                if (null == l_encounter) return;

                if (null != m_BossPortrait)
                    m_BossPortrait.sprite = GameController.Instance.iconManager.GetIconById(l_encounter.Data.Icon, IconManager.IconTypes.Encounter);
                if (null != m_BossName)
                    m_BossName.text = l_encounter.DisplayName;
            }
        }

        public IEnumerator Spin()
        {
            m_SkipClicked = false;
            titleAnimator.PlayForward();
            descriptionAnimator.PlayForward();

            startTime = Time.time;
            while (startTime + waitTime > Time.time)
            {
                if (m_SkipClicked) yield break;
                yield return new WaitForEndOfFrame();
            }

            descriptionAnimator.PlayReverse();

            StartCoroutine(UBAnimator.Wait(.55f, () =>
            {
                if (m_SkipClicked) return;
                titleAnimator.PlayReverse();
                UBAnimator.OutroPane(gameObject, .5f);

                if (activeState == DisplayStates.PlayerDied)
                    ContinueToGameOver();
                else if (UBGamePlay.UseRaidMode)
                    GameplayController.RaiseGameplayEvent(GlobalStrings.QUEST_COMPLETE_EVENT, UBGamePlay.GameplayEventTypes.OutroQuest);
                else
                    StartQuest();
            }));
        }

        public void UpdateFields_Boss()
        {
            if (GameController.Instance.ActiveLevel != null)
            {
                ActName.text = GameController.Instance.ActiveEncounterList.First().DisplayName;
                ActIntro.text = GameController.Instance.QuestProgress.CurrentAct.Value.IntroBossMonolog;
            }
        }

        public void UpdateFields_Fail()
        {
            if (GameController.Instance.ActiveLevel != null)
            {
                var act = GameController.Instance.QuestProgress.CurrentAct;
                ActName.text = ActName.text = string.Format("{0} Failed!", GameController.Instance.QuestProgress.CurrentAct.Key);
                ActIntro.text = act.Value.FailureMonolog;
            }
        }

        public void ContinueToNextAct()
        {
            dirCanvasController.gameplayController.turnController.AdvanceAct();

            UnityAction afterActAdvances = () => { ShowInfo(DisplayStates.ActIntro); };
            UBAnimator.Wait(1.0f, afterActAdvances);
        }

        public void ContinueToQuestComplete()
        {
            GameplayController.RaiseGameplayEvent(GlobalStrings.QUEST_COMPLETE_EVENT, UBGamePlay.GameplayEventTypes.OutroQuest);
            UBAnimator.OutroPane(gameObject, .75f);
        }

        public void ContinueToGameOver()
        {
            GameplayController.RaiseGameplayEvent(GlobalStrings.PLAYER_DIED_EVENT, UBGamePlay.GameplayEventTypes.EndQuest);
            UBAnimator.OutroPane(gameObject, .75f);
        }

        public void StartQuest()
        {
            UBAnimator.OutroPane(gameObject, .75f);
            GameplayController.RaiseGameplayEvent(GlobalStrings.QUEST_START_EVENT, UBGamePlay.GameplayEventTypes.StartQuest);
        }
    }
}