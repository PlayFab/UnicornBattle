using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;
using UnityEngine.UI;

public class SplashScreenController : MonoBehaviour {
	public enum SplashScreenMode {useDarkLogo, useLightLogo};
	public SplashScreenMode activeMode = SplashScreenMode.useLightLogo;
	public string sceneToLoadNext = string.Empty;
	public Image darkImg;
	public Image lightImg;
	public Image hideFire;
	public float waitTime = 3.0f;
	public float fadeInTime = .5f;
	public float fadeOutTime = .5f;
	public Text countDown;
	
	private float startTime = 0;
	
	
	// Use this for initialization
	IEnumerator Start () {
		Debug.Log("Starting...");
		this.hideFire.CrossFadeAlpha(1, .01f * .5f, true);
		if(this.activeMode == SplashScreenMode.useDarkLogo)
		{
			Camera.main.backgroundColor = Color.white;
			this.darkImg.CrossFadeAlpha(0,.01f, true);
			this.darkImg.enabled = true;
			this.lightImg.enabled = false;
			
			this.hideFire.color = Color.white;
		}
		else
		{
			Camera.main.backgroundColor = Color.black;
			this.lightImg.CrossFadeAlpha(0,.01f, true);
			this.darkImg.enabled = false;
			this.lightImg.enabled = true;
			this.hideFire.color = Color.black;
		}
		
		yield return new WaitForSeconds(.333f);
		Debug.Log("Starting Fade Co-routine.");
		StartCoroutine(Fade());
	}
	
	// Update is called once per frame
	void Update () {
		this.countDown.text = string.Format("{0:0.00}", (this.startTime + this.waitTime) - Time.time);
	}
	
	public IEnumerator Fade () {
		this.startTime = Time.time;
		
		if(this.activeMode == SplashScreenMode.useDarkLogo)
		{
			this.darkImg.CrossFadeAlpha(1, fadeInTime, true);
			
			yield return new WaitForSeconds(fadeInTime*2);
			this.hideFire.CrossFadeAlpha(0, fadeInTime * .5f, true);
			
			while(this.startTime + this.waitTime > Time.time)
			{
				yield return new WaitForEndOfFrame();
			}
			
			this.darkImg.CrossFadeAlpha(0, fadeOutTime, true);
			yield return new WaitForSeconds(fadeOutTime);
		}
		else
		{
			this.lightImg.CrossFadeAlpha(1, fadeInTime, true);
			
			yield return new WaitForSeconds(fadeInTime*2);
			this.hideFire.CrossFadeAlpha(0, fadeInTime * .5f, true);
			
			Debug.Log("Starting wait loop");
			while(this.startTime + this.waitTime > Time.time)
			{
				
				yield return new WaitForEndOfFrame();
			}
			
			this.lightImg.CrossFadeAlpha(0, fadeOutTime, true);
			yield return new WaitForSeconds(fadeOutTime+.5f);
		}
		
		
		if(string.IsNullOrEmpty(this.sceneToLoadNext))
		{
			Debug.LogError(string.Format("Tried to load the next scene, but no scene was found. Enter your \"sceneToLoadNext'\" via the inspector."));
		}
		else
		{
			SceneManager.LoadSceneAsync(this.sceneToLoadNext);
		}
		yield break;
	}

}
