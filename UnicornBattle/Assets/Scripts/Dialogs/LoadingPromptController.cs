using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using System.Collections;

public class LoadingPromptController : MonoBehaviour {
	public Image spinner;
	
	
	
	
	// Use this for initialization
	void Start () {
//		var tween = new TweenRot(){ 
//			GameObject = this.spinner.gameObject,
//			duration = 3.0f,
//			from = Vector3.zero,
//			to = new Vector3(0,0,100),
//			style = TweenMain.Style.Loop,	
//			method =  TweenMain.Method.Linear,
//			delay = 0
//		};		
			
//		var tween = new TweenRot();
//		tween.from = Quaternion.identity
//		tween.duration = 3.0f;
//		tween.style =  
//		
		//var tween = TweenRot.Tween(this.spinner.gameObject, 3.0f, Quaternion.Euler(0,0,0) , Quaternion.Euler(0, 0, 15), TweenMain.Style.Loop, TweenMain.Method.Linear, null);
		//tween.delay = 0;
		//tween.
		//tween.PlayReverse();
	}

	public void RaiseLoadingPrompt()
	{
		this.gameObject.SetActive(true);
	}
	
	public void CloseLoadingPrompt()
	{
		this.gameObject.SetActive(false);
	}
}
