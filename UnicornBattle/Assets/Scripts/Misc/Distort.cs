using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using System.Collections;

public class Distort : MonoBehaviour {
	public bool AnimatedFx = true;
	public Outline redOutline;
	public Outline blueOutline;

	public void OnTitleAnimationComplete()
	{
		if(this.AnimatedFx)
		{
			StartCoroutine(DistortTitlePositive(redOutline));
			StartCoroutine(DistortTitleNegative(blueOutline));
		}
	}
	
	public IEnumerator DistortTitlePositive(Outline obj)
	{
		float yDistMin = 6;
		float yDistMax = 30;
		float lerpTime = .10f;
		float currentLerpTime = 0;
		float perc = 0;
		
		bool playForwards = true;
		
		while(true)
		{
			currentLerpTime += Time.deltaTime;
			perc = currentLerpTime / lerpTime;
			
			if(perc >= 1f )
			{
				yield return new WaitForSeconds(.1f);
				yDistMin = 30;
				yDistMax = 0;
				currentLerpTime = 0;
				playForwards = !playForwards;
				if(playForwards == true)
				{
					yield break;
				}
			}
			
			
			var lerpedVal = Mathf.Lerp(yDistMin, yDistMax, perc);
			obj.effectDistance = new Vector2(obj.effectDistance.x, lerpedVal);
			yield return new WaitForEndOfFrame();
		}
	}
	
	
	public IEnumerator DistortTitleNegative(Outline obj)
	{
		float yDistMin = 6;
		float yDistMax = -30;
		float lerpTime = .15f;
		float currentLerpTime = 0;
		float perc = 0;
		
		bool playForwards = true;
		
		while(true)
		{
			currentLerpTime += Time.deltaTime;
			perc = currentLerpTime / lerpTime;
			
			if(perc >= 1f )
			{
				yield return new WaitForSeconds(.1f);
				yDistMin = -30;
				yDistMax = 0;
				currentLerpTime = 0;
				playForwards = !playForwards;
				if(playForwards == true)
				{
					yield break;
				}
			}
			
			
			var lerpedVal = Mathf.Lerp(yDistMin, yDistMax, perc);
			obj.effectDistance = new Vector2(lerpedVal, obj.effectDistance.y);
			yield return new WaitForEndOfFrame();
		}
	}
	
}
