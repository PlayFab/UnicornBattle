using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;
using System.Collections;

public class AchievementItem : MonoBehaviour {
	public Image icon;
	public FillBarController progressBar;
	public Text display;
	public Text Name;
	public Image coloredBar;
	public Image overlay;

	public void CompleteAchievement()
	{
		this.overlay.gameObject.SetActive(false);
	}
	
	public void ResetImage()
	{
		this.overlay.gameObject.SetActive(true);
	}
}
