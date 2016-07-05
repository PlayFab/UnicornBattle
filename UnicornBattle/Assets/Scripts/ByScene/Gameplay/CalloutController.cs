using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

public class CalloutController : MonoBehaviour {
	public Image callout;
	public Image actionIcon;
	public Text actionText;
	public TweenCGAlpha tweener;
	public CanvasGroup cGroup;

	public void CastSpell(UnityAction callback = null)
	{
		//fade in
		PF_GamePlay.IntroPane(this.gameObject, .25f, ()=>
		{
			// fade out after wait period
			UnityAction fadeOut = () => 
			{  
				PF_GamePlay.OutroPane(this.gameObject, .5f,	callback);
			};
			
			// wait period
			StartCoroutine (PF_GamePlay.Wait(1.0f, fadeOut));
		});
	}
}
