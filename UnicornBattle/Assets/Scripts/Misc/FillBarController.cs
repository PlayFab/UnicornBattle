using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using System.Collections;

public class FillBarController : MonoBehaviour {
	public int maxValue;
	public int currentValue;
	
	public Image topBar;
	public Image btmBar;
	
	public TweenColor colorFader;
	public float fadeDuration = 2;
	
	public bool useRGBColors = false;
	public bool useFadeAnimation = false;
	
	//TODO add this color array to switch up the fade colors after the animation completes
	public Color[] fadeColors;

	// Update is called once per frame
	void Update () 
	{
		Rect current = topBar.rectTransform.rect;
		float percentFilled =  current.width / btmBar.rectTransform.rect.width;
		
		if(!Mathf.Approximately(percentFilled, this.currentValue))
		{
			topBar.rectTransform.sizeDelta = new Vector2(btmBar.rectTransform.rect.width * currentValue/maxValue, btmBar.rectTransform.rect.height);
			if(this.useRGBColors)
			{
				if(percentFilled >= .75f)
				{
					this.topBar.color = Color.green;
				}
				else if(percentFilled > .25f && percentFilled < .75f)
				{
					this.topBar.color = Color.yellow;
				}
				else if(percentFilled <= .25f)
				{
					this.topBar.color = Color.red;
				}
			}
		} 
		
		if(this.useFadeAnimation && this.colorFader != null && this.colorFader.enabled == false)
		{
			this.colorFader.enabled = true;
			this.colorFader.duration = this.fadeDuration;
		}
		else if(this.useFadeAnimation == false && this.colorFader != null && this.colorFader.enabled)
		{
			this.colorFader.enabled = false;
		}
		
	}
	
	// should only be used in battle (player life or enemy life)
	public IEnumerator UpdateBar(int target, bool immediate = false)
	{
		if(currentValue > target && !immediate)
		{
			while(currentValue > target && currentValue > 0)
			{
				currentValue -= Mathf.RoundToInt(this.maxValue * .01f);
				yield return new WaitForEndOfFrame();
			}
			if(target <= 0)
			{
				// fire the event, and let the turn manager decide how to proceed.
				//GameplayController.RaiseGameplayEvent("Target Died", PF_GamePlay.GameplayEventTypes.OutroEncounter);
			}
		}
		else if (currentValue < target && !immediate)
		{
			while(currentValue < target && currentValue < this.maxValue)
			{
				currentValue += Mathf.RoundToInt(this.maxValue * .01f);
				yield return new WaitForEndOfFrame();
			}
		}
		else if(immediate && currentValue != target)
		{
			currentValue = target;
		}
		yield break;
	}
	
	public IEnumerator UpdateBarWithCallback(int target, bool immediate = false, UnityAction callback = null)
	{
		if(currentValue > target && !immediate)
		{
			while(currentValue > target && currentValue > 0)
			{
				currentValue -= Mathf.RoundToInt(this.maxValue * .01f);
				yield return new WaitForEndOfFrame();
			}
		}
		else if (currentValue < target && !immediate)
		{
			while(currentValue < target && currentValue < this.maxValue)
			{
				currentValue += Mathf.RoundToInt(this.maxValue * .01f);
				yield return new WaitForEndOfFrame();
			}
		}
		else
		{
			currentValue = target;
		}
		
		if(callback != null)
		{
			callback();
		
		}
		yield break;
	}
}
