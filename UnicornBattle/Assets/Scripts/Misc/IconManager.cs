using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using System.Collections;
using System.Collections.Generic;

public class IconManager : MonoBehaviour {

	public List<Icon> iconRepository;
	public Sprite defaultIcon; // use this if we get a request for a non-existent icon;
	
	public enum IconTypes { Spell, Class, Item, Status, Encounter, Level, Misc }

	public Sprite GetIconById(string id)
	{
		Icon icon = iconRepository.Find((i) => { return i.id == id; });
		if(icon != null)
		{
			return icon.sprite;
		}
		else
		{
			return defaultIcon;
		}
	}
	
	
}

[System.Serializable]
public class Icon
{
	public string id;
	public Sprite sprite;
	public IconManager.IconTypes type;
}
