using UnityEngine;
using UnityEngine.SceneManagement;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

public class SceneController : Singleton<SceneController> {
	protected SceneController () {} // guarantee this will be always a singleton only - can't use the constructor!
	
	public enum GameScenes { Null = 0, Authenticate = 1, CharacterSelect = 2, Profile = 3, Gameplay = 4, Splash = 5 } 
	//public GameScenes activeScene;
	public GameScenes previousScene = GameScenes.Null;
	
	// handle the spinner //convenient location to raise start spinner events
	public enum SpinnerType {off = 0, standard = 1, blocking = 2}
	public SpinnerType activeSpinner {get; private set;}
	
	public delegate void StartSpinner();
	public static event StartSpinner StartSpinnerRequest;
	
	private List<string> scenes = new List<string>(); // for itterations
	private Coroutine sceneChangeInProgress = null;
	private bool cancelSceneChange = false;
	

	// Use this for initialization
	void Start () {
		// load level enums into a list for easier use
		this.activeSpinner = SpinnerType.off;
		scenes = Enum.GetNames(typeof(GameScenes)).ToList();
		//DontDestroyOnLoad(this);
		//var result = RequestSceneChange(GameScenes.Authenticate);
		//Debug.Log("result: " + result + " : " + scenes.Count);
	}

	public bool ReturnToCharacterSelect()
	{
		return RequestSceneChange(GameScenes.CharacterSelect, .333f);
	}
	
	
	// other options to alter to change scenes
	public bool RequestSceneChange(GameScenes nextScene, float delay = 0)
	{
		if(this.sceneChangeInProgress == null)
		{
			if(nextScene.ToString() == SceneManager.GetActiveScene().name)
			{
				//Debug.Log("Already on the scene.");
				return false;
			}
		
			string match = nextScene.ToString();
			string scene = scenes.FirstOrDefault(z => { return z == match; });
			
			if( String.IsNullOrEmpty(scene))
			{
				//ERROR: SceneNotFound 
				return false;
			}
			
			// TODO:
			// check if already loading another scene
			// check if scene is already loaded
			// check if any delay parameters
			// check logic for adding a between scene
			
			sceneChangeInProgress = StartCoroutine(ChangeScene(match, delay));
			//this.activeScene = nextScene;
			return true;
		}
		else
		{
			// already loading a scene...
			return false;
		}
	}
	
	IEnumerator ChangeScene(string nextScene, float delay)
	{
		this.cancelSceneChange = false;
		Debug.Log(string.Format("Changing scene to {0} after {1} seconds...", nextScene, delay));
		float gameTime = Time.time;
		//this.stagingScene = Application.LoadLevelAsync(nextScene);
		//this.stagingScene.allowSceneActivation = false;
		
		while(gameTime + delay > Time.time)
		{
			//Debug.Log(Time.time - (gameTime + delay));
			if(this.cancelSceneChange)
			{
				//Debug.Log("Change scene canceled.");
				yield break;
			}
			yield return 0;
		} 
		
		this.previousScene =  (GameScenes) Enum.Parse(typeof(GameScenes), SceneManager.GetActiveScene().name);
		
		//Debug.Log("HELLO ISSUES -- " + nextScene);
		SceneManager.LoadSceneAsync(nextScene);
		sceneChangeInProgress = null;
	}
	
	public void CancelLevelLoad()
	{
		if(this.sceneChangeInProgress != null)
		{
			sceneChangeInProgress = null;
			this.cancelSceneChange = true;
		}
		//StopCoroutine("DoSomething");
	}
	
	
	/// <summary>
	/// Raises an even for requesting a spinner animation
	/// </summary>
	public bool RequestSpinner( SpinnerType type)
	{
		if(StartSpinnerRequest != null && this.activeSpinner == SpinnerType.off && type != SpinnerType.off)
		{
			this.activeSpinner = type;
			StartSpinnerRequest();
			return true;
		}
		else if(type == SpinnerType.off)
		{
			// may need to actually stop something here eventually.
			this.activeSpinner = type;
			return true;
		}
		else
		{
			// was busy or no event listeners or was a state mismatch
			return false;
		}
	}
}
