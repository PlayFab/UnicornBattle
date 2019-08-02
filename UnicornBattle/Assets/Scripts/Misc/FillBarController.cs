using System.Collections;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace UnicornBattle.Controllers
{
	public class FillBarController : MonoBehaviour
	{
		public int maxValue;
		public int currentValue;

		public Gradient gradient;

		public Image frame;
		public Image fill;
		public Image fillBG;

		public TweenColor colorFader;
		public float fadeDuration = 2;

		public bool useRGBColor = false;
		public bool useFadeAnimation = false;
		public bool useGradientColor = true;

		//TODO add this color array to switch up the fade colors after the animation completes
		public Color[] fadeColors;

		// Update is called once per frame
		void Update()
		{
			if (fillBG != null)
			{
				Rect current = fillBG.rectTransform.rect;
				float percent = (float) currentValue / (float) maxValue;

				fill.rectTransform.sizeDelta = new Vector2((fillBG.rectTransform.rect.width * percent), fill.rectTransform.sizeDelta.y);

				if (this.useRGBColor)
				{
					if (percent >= .75f) this.fill.color = Color.green;
					else if (percent >.25f && percent < .75f) this.fill.color = Color.yellow;
					else if (percent <= .25f) this.fill.color = Color.red;

				}
				// Gradient is prioritzed 
				if (this.useGradientColor)
				{
					this.fill.color = gradient.Evaluate(percent);
				}

			}
		}

		// should only be used in battle (player life or enemy life)
		public IEnumerator UpdateBar(int target, bool immediate = false)
		{
			if (currentValue > target && !immediate)
			{
				while (currentValue > target && currentValue > 0)
				{
					currentValue -= Mathf.RoundToInt(this.maxValue * .01f);
					yield return new WaitForEndOfFrame();
				}
				if (target <= 0)
				{
					// fire the event, and let the turn manager decide how to proceed.
					//GameplayController.RaiseGameplayEvent("Target Died", PF_GamePlay.GameplayEventTypes.OutroEncounter);
				}
			}
			else if (currentValue < target && !immediate)
			{
				while (currentValue < target && currentValue < this.maxValue)
				{
					currentValue += Mathf.RoundToInt(this.maxValue * .01f);
					yield return new WaitForEndOfFrame();
				}
			}
			else if (immediate && currentValue != target)
			{
				currentValue = target;
			}
			yield break;
		}

		public IEnumerator UpdateBarWithCallback(int target, bool immediate = false, UnityAction callback = null)
		{
			if (currentValue > target && !immediate)
			{
				while (currentValue > target && currentValue > 0)
				{
					currentValue -= Mathf.RoundToInt(this.maxValue * .001f);
					yield return new WaitForEndOfFrame();
				}
			}
			else if (currentValue < target && !immediate)
			{
				while (currentValue < target && currentValue < this.maxValue)
				{
					currentValue += Mathf.RoundToInt(this.maxValue * 0.001f);
					yield return new WaitForEndOfFrame();
				}
			}
			else
			{
				currentValue = target;
			}

			if (callback != null)
			{
				callback();

			}
			yield break;
		}
	}
}