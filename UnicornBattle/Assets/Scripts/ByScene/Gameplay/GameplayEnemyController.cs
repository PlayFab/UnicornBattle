using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using System.Collections;

public class GameplayEnemyController : MonoBehaviour {
	
	public RectTransform myRT;
	
	// private int enemyHealth = 100;
	private float animationStart = 0;
	private int pendingValue = 0; 
	
	public TweenPos shaker;
	public float defaultShakeTime = .25f;
	public bool isShaking = false;
	
	public CalloutController enemyAction;
	public GameplayController gameplayController;
	
	public CurrentEncounterController currentEncounter;
	public NextEncounterController nextController;
	
	// ELEMENTS 
	public Image CreepEncountersImage;
//	public Image HeroEncountersImage;
//	public Image GoldCollected;
//	public Image ItemsCollected;
	
	public FillBarController bar;
	
	

	void OnEnable()
	{
        GameplayController.OnGameplayEvent += OnGameplayEventReceived;
	}
	
	void OnDisable()
	{
        GameplayController.OnGameplayEvent -= OnGameplayEventReceived;
	}
	
    void OnGameplayEventReceived(string message,  PF_GamePlay.GameplayEventTypes type )
	{
		
//		if(type == PF_GamePlay.GameplayEventTypes.StartQuest)
//		{
//			// load enemy data
//			TweenCGAlpha.Tween(this.gameObject, 1.0f, 0, 1, TweenMain.Style.Once, TweenMain.Method.EaseIn, null);
//			
//			// FX placement
//			Vector3 pos = myRT.TransformPoint(myRT.position.x, myRT.position.y,0);
//			GameObject go = FX_Placement.fx_lookup.ContainsKey("CFXM_SmokePuffsAltLarge") ? FX_Placement.fx_lookup["CFXM_SmokePuffsAltLarge"] : null;
//			StartCoroutine(FX_Placement.FxHereAndFire(new Vector3(pos.x, pos.y, -2), go));
//			
//			//GameplayController.RaiseGameplayEvent("Initial Encounter", PF_GamePlay.GameplayEventTypes.IntroEncounter); 
//
//		}
		
		
		
		
	}
	
	// Use this for initialization
	void Start () {
		this.myRT = (RectTransform)GetComponent<RectTransform>();
		
	}

	public void Callout(Sprite sprite, string message, UnityAction callback)
	{
		this.enemyAction.actionIcon.overrideSprite = sprite;
		this.enemyAction.actionText.text = message;
		this.enemyAction.CastSpell(callback);
	}
	
	
	public void TakeDamage(int dmg)
	{
		int current = bar.currentValue;
		pendingValue = current - dmg;
		
		Debug.Log(string.Format("Enemy takes {0}", dmg));
		// if afterAttack > 0
		
		gameplayController.turnController.currentEncounter.Data.Vitals.Health -= dmg;
		RequestShake(defaultShakeTime, PF_GamePlay.ShakeEffects.DecreaseHealth);
		
		
		//else
		//StartCoroutine(bar.RefillHealth());
	}	
	
	public void TransitionCurrentEncounterOut(UnityAction callback = null)
	{
		Vector3 pos = myRT.TransformPoint(myRT.position.x, myRT.position.y,0);
        GameObject go = FX_Placement.fx_lookup.ContainsKey(GlobalStrings.SMOKE_EFFECT_1) ? FX_Placement.fx_lookup[GlobalStrings.SMOKE_EFFECT_1] : null;
		StartCoroutine(FX_Placement.FxHereAndFire(new Vector3(pos.x, pos.y, -2), go));
		
		// load enemy data
		TweenCGAlpha.Tween(this.currentEncounter.gameObject, 1.0f, 1, 0, TweenMain.Style.Once, TweenMain.Method.EaseOut, callback);
		
		//GameplayController.RaiseGameplayEvent("Initial Encounter", PF_GamePlay.GameplayEventTypes.IntroEncounter); 
	}
	
	public void TransitionCurrentEncounterIn(UnityAction callback = null)
	{
		// FX placement
		Vector3 pos = myRT.TransformPoint(myRT.position.x, myRT.position.y,0);
        GameObject go = FX_Placement.fx_lookup.ContainsKey(GlobalStrings.SMOKE_EFFECT_2) ? FX_Placement.fx_lookup[GlobalStrings.SMOKE_EFFECT_1] : null;
		StartCoroutine(FX_Placement.FxHereAndFire(new Vector3(pos.x, pos.y, -2), go));
		
		// load enemy data
		TweenCGAlpha.Tween(this.currentEncounter.gameObject, 1.0f, 0, 1, TweenMain.Style.Once, TweenMain.Method.EaseIn, callback);
		
		//GameplayController.RaiseGameplayEvent("Initial Encounter", PF_GamePlay.GameplayEventTypes.IntroEncounter); 
	}
	
	
	public void RequestShake(float seconds, PF_GamePlay.ShakeEffects effect)
	{
		if(this.isShaking == false)
		{
			shaker.enabled = true;
			this.animationStart = Time.time;
			StartCoroutine(Shake(seconds, effect));
		}
		else
		{
			this.animationStart -= seconds;
		}
	}
	
	IEnumerator Shake(float seconds, PF_GamePlay.ShakeEffects effect = PF_GamePlay.ShakeEffects.None)
	{
		yield return new WaitForSeconds(seconds);

		this.shaker.value = new Vector3(this.shaker.from.x + (this.shaker.to.x - this.shaker.from.x)/2, this.shaker.from.y, this.shaker.from.z );
		this.isShaking = false;
		this.shaker.enabled = false;
		
		if( effect == PF_GamePlay.ShakeEffects.DecreaseHealth)
		{
			yield return StartCoroutine(bar.UpdateBar(this.pendingValue));
			
			if(this.pendingValue > 0)
			{
                StartCoroutine(PF_GamePlay.Wait( .5f,() => { GameplayController.RaiseGameplayEvent(GlobalStrings.PLAYER_TURN_END_EVENT, PF_GamePlay.GameplayEventTypes.PlayerTurnEnds); }));
			}
			else
			{
                GameplayController.RaiseGameplayEvent(GlobalStrings.OUTRO_ENEMY_DIED_EVENT, PF_GamePlay.GameplayEventTypes.OutroEncounter);
			}
		}
		else if (effect == PF_GamePlay.ShakeEffects.IncreaseHealth)
		{
			StartCoroutine(bar.UpdateBar(this.pendingValue));
		}
		else if(effect == PF_GamePlay.ShakeEffects.DecreaseMana)
		{
			StartCoroutine(bar.UpdateBar(this.pendingValue));
		}
		else if(effect == PF_GamePlay.ShakeEffects.IncreaseMana)
		{
			StartCoroutine(bar.UpdateBar(this.pendingValue));
		}
		
		this.pendingValue = 0;

		
		
		yield break;
	}
}
