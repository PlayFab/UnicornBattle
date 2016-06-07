using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using System;
using System.Collections;
using System.Collections.Generic;

public class FX_Placement : MonoBehaviour {
	public Camera FX_Cam; 
	public ParticleSystem explosion;
	public Vector2 clickPos = Vector2.zero;
	public Text txtPos;
	public AudioClip boomFx;
	public List<GameObject> effects = new List<GameObject>();
	
	public static Dictionary<string, GameObject> fx_lookup = new Dictionary<string, GameObject>();
	
	// Use this for initialization
	void Start () {
	
		foreach(var fx in effects)
		{
			string key = fx.name;
			if(!fx_lookup.ContainsKey(key))
			{
				fx_lookup.Add(key, fx); 
			}
		}
	
	}

	public static IEnumerator FxHereAndFire(Vector3 here, GameObject prefab, float delay = 0)
	{
		if(prefab == null || here == Vector3.zero)
		{
			yield break;
		}
		
		if (delay > 0)
		{
			yield return new WaitForSeconds(delay);
		}
		
		var fx = CFX_SpawnSystem.GetNextObject(prefab, false);
		
		fx.transform.position = here;
		fx.gameObject.SetActive(true);
	}
	
	
//	//based on world pos
//	public void OnPointerClick(BaseEventData eData)
//	{
//		
//		if(!explosion.isPlaying)
//		{
//			PointerEventData pData = (PointerEventData)eData;
//			int rng = Mathf.FloorToInt(Random.Range(5,50));
//			Vector3 worldPos = FX_Cam.ScreenToWorldPoint(new Vector3(pData.position.x, pData.position.y, rng));
//			
//			explosion.transform.position = worldPos; //new Vector3(worldPos.x, worldPos.y, rng); 
//			//Debug.Log (pData.pointerPressRaycast.gameObject.name );
//			
//			//txtPos.text = string.Format("ClickPos: [x]:{0}, [y]:{1}, [z]:{2}", pData.position.x, pData.position.y, rng);
//		
//			if(this.boomFx != null)
//			{
//				FX_Cam.GetComponent<AudioSource>().clip = this.boomFx;
//				FX_Cam.GetComponent<AudioSource>().Play();
//			}
//			explosion.Play();
//			
//		}
//	}

//	public void AttackFx(GameplayEnemyController enemy)
//	{
//		Vector3 center = enemy.myRT.rect.center;
//		
//		int rng = Mathf.FloorToInt(Random.Range(5,25));
//		//Vector3 worldPos = FX_Cam.ScreenToWorldPoint(new Vector3(center.x, center.y, rng));
//	
//		Vector3 worldPos = enemy.myRT.TransformPoint(new Vector3(center.x, center.y + (enemy.myRT.offsetMin.y *.40f), rng));
//		
//		//Debug.Log(worldPos);
//		
//		explosion.transform.position = new Vector3(worldPos.x, worldPos.y, .01f); 
//		//txtPos.text = string.Format("ClickPos: [x]:{0}, [y]:{1}, [z]:{2}", center.x, center.y, rng);
//		
//		if(this.boomFx != null)
//		{
//			FX_Cam.GetComponent<AudioSource>().clip = this.boomFx;
//			FX_Cam.GetComponent<AudioSource>().Play();
//		}
//		explosion.Play();
//	}
	
	public void TestFx(GameplayEnemyController enemy, string fxName = null)
	{
		Vector3 center = enemy.myRT.rect.center;
		
		int rng = Mathf.FloorToInt(UnityEngine.Random.Range(5,25));
		//Vector3 worldPos = FX_Cam.ScreenToWorldPoint(new Vector3(center.x, center.y, rng));
		
		Vector3 worldPos = enemy.myRT.TransformPoint(new Vector3(center.x, center.y + (enemy.myRT.offsetMin.y *.40f), rng));
		
		Func<GameObject> GetRadomFX = () => 
		{
			int rnd = UnityEngine.Random.Range(0,effects.Count);
			Debug.Log(""+rnd);
			return CFX_SpawnSystem.GetNextObject(effects[rnd], false);
		};
		
		GameObject fx;
		if(fxName == null)
		{
			fx = GetRadomFX();
		}
		else
		{
			if(FX_Placement.fx_lookup.ContainsKey(fxName))
			{
				fx = CFX_SpawnSystem.GetNextObject(FX_Placement.fx_lookup[fxName], false);
			}
			else
			{
				fx = GetRadomFX();
			}
		}
		fx.transform.position = FX_Cam.transform.position + new Vector3(worldPos.x, worldPos.y, 5f);
		
		//txtPos.text = string.Format("ClickPos: [x]:{0}, [y]:{1}, [z]:{2}", center.x, center.y, rng);
		fx.SetActive(true);
		if(this.boomFx != null)
		{
//			FX_Cam.GetComponent<AudioSource>().clip = this.boomFx;
//			FX_Cam.GetComponent<AudioSource>().Play();
		}
		
	}
	
	// will eventually need spell sounds?
	// will need combo or not effects.
	public void PlayerTakesDamage(PlayerUIEffectsController player, string fxName = null)
	{
		RectTransform lifeRect =  player.LifeBar.btmBar.gameObject.GetComponent<RectTransform>();
		Vector3 center = lifeRect.rect.center;
		
		
		Vector3 worldPos = lifeRect.TransformPoint(new Vector3(lifeRect.rect.xMax, center.y, 0));
		GameObject fx;
		
		
		Func<GameObject> GetRadomFX = () => 
		{
			int rnd = UnityEngine.Random.Range(0,effects.Count);
			//Debug.Log(""+rnd);
			return CFX_SpawnSystem.GetNextObject(effects[rnd], false);
		};
		
		
		if(fxName == null)
		{
			fx = GetRadomFX();
		}
		else
		{
			if(FX_Placement.fx_lookup.ContainsKey(fxName))
			{
			   fx = CFX_SpawnSystem.GetNextObject(FX_Placement.fx_lookup[fxName], false);
			}
			else
			{
				fx = GetRadomFX();
			}
		}
		
		fx.transform.position = new Vector3(FX_Cam.transform.position.x + worldPos.x, FX_Cam.transform.position.y + worldPos.y, -23f);
		
		fx.SetActive(true);
		if(this.boomFx != null)
		{
			//FX_Cam.GetComponent<AudioSource>().clip = this.boomFx;
			//FX_Cam.GetComponent<AudioSource>().Play();
		}
		
	}
	
}
