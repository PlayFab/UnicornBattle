using System.Collections;
using UnityEngine;
using UnityEngine.UI;

namespace UnicornBattle.Controllers
{
	public class ExpBarController : MonoBehaviour
	{
		[Range(0.0f, 1.0f)]
		public float expAmt = .85f;
		public Image topBar;
		public Image btmBar;

		// Update is called once per frame
		void Update()
		{
			//int health = currentHealth * 100 / maxHealth; 
			Rect current = topBar.rectTransform.rect;
			float percentFilled = current.width / btmBar.rectTransform.rect.width;

			if (!Mathf.Approximately(percentFilled, this.expAmt))
			{
				topBar.rectTransform.sizeDelta = new Vector2(btmBar.rectTransform.rect.width * expAmt, btmBar.rectTransform.rect.height);

				if (percentFilled >= .75f)
				{
					this.topBar.color = Color.green;
				}
				else if (percentFilled >.25f && percentFilled < .75f)
				{
					this.topBar.color = Color.yellow;
				}
				else if (percentFilled <= .25f)
				{
					this.topBar.color = Color.red;
				}
				//Debug.Log (string.Format("Exp Changed: {0:P1} filled.", percentFilled));
			}

		}

		public IEnumerator TakeDamage(float target)
		{
			while (expAmt > target && expAmt > 0)
			{
				expAmt -= .01f;
				//topBar.rectTransform.sizeDelta = new Vector2(btmBar.rectTransform.rect.width * expAmt, btmBar.rectTransform.rect.height);
				yield return new WaitForEndOfFrame();
			}
			if (target <= 0)
			{
				Debug.Log("Enemy Died...");
				StartCoroutine(RefillHealth());
			}
			yield break;
		}

		public IEnumerator RefillHealth()
		{
			yield return new WaitForSeconds(1.75f);
			expAmt = 0;
			while (expAmt < 1)
			{
				if (expAmt >.99)
				{
					expAmt = 1.0f;
				}
				else
				{
					expAmt += .01f;
				}
				yield return new WaitForEndOfFrame();
			}
			yield break;
		}

	}
}