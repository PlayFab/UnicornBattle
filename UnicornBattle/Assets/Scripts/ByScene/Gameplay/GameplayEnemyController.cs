using System.Collections;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

namespace UnicornBattle.Controllers
{
    public class GameplayEnemyController : MonoBehaviour
    {
        // private int enemyHealth = 100;
        private float animationStart = 0;
        private int pendingValue = 0;
        private float defaultShakeTime = .25f;
        private bool isShaking = false;

        public RectTransform myRT;
        public TweenPos shaker;
        public Image CreepEncountersImage;
        public FillBarController bar;
        public CalloutController enemyAction;
        public GameplayController gameplayController;
        public CurrentEncounterController currentEncounter;
        public NextEncounterController nextController;

        void OnEnable()
        {
            GameplayController.OnGameplayEvent += OnGameplayEventReceived;
        }

        void OnDisable()
        {
            GameplayController.OnGameplayEvent -= OnGameplayEventReceived;
        }

        void OnGameplayEventReceived(string message, UBGamePlay.GameplayEventTypes type) { }

        void Start()
        {
            myRT = GetComponent<RectTransform>();
        }

        public void Callout(Sprite sprite, string message, UnityAction callback)
        {
            enemyAction.actionIcon.overrideSprite = sprite;
            enemyAction.actionText.text = message;
            enemyAction.CastSpell(callback);
        }

        public void TakeDamage(int dmg)
        {
            var current = bar.currentValue;
            pendingValue = current - dmg;

            gameplayController.turnController.currentEncounter.Data.Vitals.Health -= dmg;
            RequestShake(defaultShakeTime, UBGamePlay.ShakeEffects.DecreaseHealth);
        }

        public void TransitionCurrentEncounterOut(UnityAction callback = null)
        {
            var pos = myRT.TransformPoint(myRT.position.x, myRT.position.y, 0);
            var go = FX_Placement.fx_lookup.ContainsKey(GlobalStrings.SMOKE_EFFECT_1) ? FX_Placement.fx_lookup[GlobalStrings.SMOKE_EFFECT_1] : null;
            StartCoroutine(FX_Placement.FxHereAndFire(new Vector3(pos.x, pos.y, -2), go));

            // load enemy data
            TweenCGAlpha.Tween(currentEncounter.gameObject, 1.0f, 1, 0, TweenMain.Style.Once, TweenMain.Method.EaseOut, callback);

            //GameplayController.RaiseGameplayEvent("Initial Encounter", PF_GamePlay.GameplayEventTypes.IntroEncounter); 
        }

        public void TransitionCurrentEncounterIn(UnityAction callback = null)
        {
            // FX placement
            var pos = myRT.TransformPoint(myRT.position.x, myRT.position.y, 0);
            var go = FX_Placement.fx_lookup.ContainsKey(GlobalStrings.SMOKE_EFFECT_2) ? FX_Placement.fx_lookup[GlobalStrings.SMOKE_EFFECT_1] : null;
            StartCoroutine(FX_Placement.FxHereAndFire(new Vector3(pos.x, pos.y, -2), go));

            // load enemy data
            TweenCGAlpha.Tween(currentEncounter.gameObject, 1.0f, 0, 1, TweenMain.Style.Once, TweenMain.Method.EaseIn, callback);
        }

        public void RequestShake(float seconds, UBGamePlay.ShakeEffects effect)
        {
            if (isShaking == false)
            {
                shaker.enabled = true;
                animationStart = Time.time;
                StartCoroutine(Shake(seconds, effect));
            }
            else
            {
                animationStart -= seconds;
            }
        }

        IEnumerator Shake(float seconds, UBGamePlay.ShakeEffects effect = UBGamePlay.ShakeEffects.None)
        {
            yield return new WaitForSeconds(seconds);

            shaker.value = new Vector3(shaker.from.x + (shaker.to.x - shaker.from.x) / 2, shaker.from.y, shaker.from.z);
            isShaking = false;
            shaker.enabled = false;

            if (effect == UBGamePlay.ShakeEffects.DecreaseHealth)
            {
                yield return StartCoroutine(bar.UpdateBar(pendingValue));

                if (pendingValue > 0)
                    StartCoroutine(UBAnimator.Wait(.5f, () => { GameplayController.RaiseGameplayEvent(GlobalStrings.PLAYER_TURN_END_EVENT, UBGamePlay.GameplayEventTypes.PlayerTurnEnds); }));
                else
                    GameplayController.RaiseGameplayEvent(GlobalStrings.OUTRO_ENEMY_DIED_EVENT, UBGamePlay.GameplayEventTypes.OutroEncounter);
            }
            else if (effect == UBGamePlay.ShakeEffects.IncreaseHealth)
            {
                StartCoroutine(bar.UpdateBar(pendingValue));
            }
            else if (effect == UBGamePlay.ShakeEffects.DecreaseMana)
            {
                StartCoroutine(bar.UpdateBar(pendingValue));
            }
            else if (effect == UBGamePlay.ShakeEffects.IncreaseMana)
            {
                StartCoroutine(bar.UpdateBar(pendingValue));
            }

            pendingValue = 0;
        }
    }
}