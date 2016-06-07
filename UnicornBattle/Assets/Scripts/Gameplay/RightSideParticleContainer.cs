using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using System.Collections;
using System.Collections.Generic;


public class RightSideParticleContainer : MonoBehaviour {
	public Transform particlePrefab;
	public List<RectTransform> spawnPoints = new List<RectTransform>();
	
	
	
	// Use this for initialization
	void Start () 
	{
		foreach(var point in spawnPoints)
		{
			Vector3 center = point.rect.center;
			
			Vector3 wp = point.TransformPoint(new Vector3(center.x, center.y, 0));
			wp+= Vector3.back;
			wp+= Vector3.up*.5f;
			//Debug.Log(wp);
			
			//explosion.transform.position = new Vector3(worldPos.x, worldPos.y, 10); 
			
			Instantiate(particlePrefab, wp, particlePrefab.rotation);
		}	
	}
}
