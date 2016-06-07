using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using System.Collections;

public class CanvasController : Singleton<CanvasController> {
	protected CanvasController () {} // guarantee this will be always a singleton only - can't use the constructor!
	
	public Transform settingsMenu;
	public Transform modalAlert;
}
