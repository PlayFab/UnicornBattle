using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;
using System.Collections;

public class PlayerEncounterInputController : MonoBehaviour {
	public Button AttackButton;
	public Button UseItemButton;
	public Button EvadeButton;
	public Button ViewStoreButton;
	public Button RescueButton;
	public Text encounterDescription;
	
	public TweenPos tweener;
	public PlayerUIEffectsController pUIController;
	
	
	void Awake()
	{
		//initialPos = this.GetComponent<RectTransform>().localPosition;	
	}
	
	// Use this for initialization
	void Start () {
		AttackButton.onClick.AddListener(() => { pUIController.StartEncounterInput(PF_GamePlay.PlayerEncounterInputs.Attack); });
		UseItemButton.onClick.AddListener(() => { pUIController.StartEncounterInput(PF_GamePlay.PlayerEncounterInputs.UseItem); });
		EvadeButton.onClick.AddListener(() => { pUIController.StartEncounterInput(PF_GamePlay.PlayerEncounterInputs.Evade); });
		ViewStoreButton.onClick.AddListener(() => { pUIController.StartEncounterInput(PF_GamePlay.PlayerEncounterInputs.ViewStore); });
		RescueButton.onClick.AddListener(() => { pUIController.StartEncounterInput(PF_GamePlay.PlayerEncounterInputs.Rescue); });
	}
	
	
	public void EnableEncounterOptions( EncounterTypes type, string encounterName, bool wasAmbushed = false)
	{
		
        if(type.ToString().Contains(GlobalStrings.ENCOUNTER_CREEP))
		{
			SetupCreepEncounter();
            this.encounterDescription.text = wasAmbushed ? string.Format(GlobalStrings.ENCOUNTER_AMBUSH_MSG, encounterName) : string.Format(GlobalStrings.ENCOUNTER_ENEMY_MSG, encounterName);
			
		}
		else if(type == EncounterTypes.Hero)
		{
			SetupHeroEncounter();
            this.encounterDescription.text = string.Format(GlobalStrings.ENCOUNTER_HERO_MSG, encounterName);
		}
		else if(type == EncounterTypes.Store)
		{
			SetupStoreEncounter();
            this.encounterDescription.text = string.Format(GlobalStrings.ENCOUNTER_VENDOR_MSG, encounterName);
		}
	}
	
	
	
	void SetupCreepEncounter()
	{
		AttackButton.gameObject.SetActive(true);
		UseItemButton.gameObject.SetActive(true);
		EvadeButton.gameObject.SetActive(true);
		ViewStoreButton.gameObject.SetActive(false);
		RescueButton.gameObject.SetActive(false);
	}
	
	void SetupHeroEncounter()
	{
		AttackButton.gameObject.SetActive(false);
		UseItemButton.gameObject.SetActive(false);
		EvadeButton.gameObject.SetActive(false);
		ViewStoreButton.gameObject.SetActive(false);
		RescueButton.gameObject.SetActive(true);
	}
	
	void SetupStoreEncounter()
	{
		AttackButton.gameObject.SetActive(false);
		UseItemButton.gameObject.SetActive(false);
		EvadeButton.gameObject.SetActive(true);
		ViewStoreButton.gameObject.SetActive(true);
		RescueButton.gameObject.SetActive(false);
	}
}
