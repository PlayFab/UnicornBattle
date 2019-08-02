using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class CalloutController : MonoBehaviour
{
	public Image callout;
	public Image actionIcon;
	public Text actionText;
	public TweenCGAlpha tweener;
	public CanvasGroup cGroup;

	public void CastSpell(UnityAction callback = null)
	{
		//fade in
		UBAnimator.IntroPane(this.gameObject, .25f, () =>
		{
			// fade out after wait period
			UnityAction fadeOut = () =>
			{
				UBAnimator.OutroPane(this.gameObject, .5f, callback);
			};

			// wait period
			StartCoroutine(UBAnimator.Wait(1.0f, fadeOut));
		});
	}
}