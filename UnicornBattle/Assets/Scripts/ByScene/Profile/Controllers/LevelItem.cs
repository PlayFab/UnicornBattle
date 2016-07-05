using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using System.Collections;

//IPointerClickHandler
public class LevelItem : MonoBehaviour{
	public LevelPicker lPicker;
	public Image levelIcon;
	public Image levelMastery;
	public string levelName;
	public int difficulty = 0; // 0 = easy, 1 = medium 2 = hard 
	public UB_LevelData levelData;
	public Button myButton;
	
	// Use this for initialization
	void Start () 
	{
        this.lPicker = (LevelPicker)this.transform.GetComponentInParent<LevelPicker>();
        myButton.onClick.AddListener(() => { lPicker.LevelItemClicked(this); });
    }

//	public void OnPointerClick(PointerEventData pData)
//	{
//		//Debug.Log("!!!!");
//		lPicker.LevelItemClicked(this);
//	}

	public LevelItemHelper GetLevelItemData()
	{
		return new LevelItemHelper()
		{ 
			levelIcon = this.levelIcon.overrideSprite, 
			levelName = this.levelName, 
			difficulty = this.difficulty, 
			levelData = this.levelData 
		};
	}
}


// temp fix for storing monobehavior refs in a static class, doh!
public class LevelItemHelper
{
	public Sprite levelIcon;
	public string levelName;
	public int difficulty = 0; // 0 = easy, 1 = medium 2 = hard 
	public UB_LevelData levelData;
}
