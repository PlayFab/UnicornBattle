using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using System;
using System.Linq;
using System.Collections;
using System.Collections.Generic;
using PlayFab;
using PlayFab.ClientModels;

public class InterstitialController : MonoBehaviour {
	public Text message;
	public Text timer;
	public Image spinner;
	public Image mainImage;
	public Button next;
	public Button prev;
	
	public Text ThanksForPlaying;
	
	public float startTime;
	public float waitTime = 5;
	
	private UB_SaleData saleOnDisplay; 
	
    public delegate void InterstitalComplete();
    public static event InterstitalComplete OnInterstitialFinish;
	
	private List<UB_PromotionalItem> Tips = new List<UB_PromotionalItem>();
	private UB_PromotionalItem CurrentTip;
	
	void OnEnable()
	{
		Tips = PF_GameData.promoItems.FindAll((i) => { return i.PromoType == PromotionalItemTypes.Tip; }).ToList();
		if(this.Tips.Count >0)
		{
			int rng = UnityEngine.Random.Range(0, this.Tips.Count-1);
			this.CurrentTip = Tips[rng];
			message.text = this.CurrentTip.PromoBody;
		}
		
		if(PF_GameData.MinimumInterstitialWait > 0)
		{
			waitTime = PF_GameData.MinimumInterstitialWait;
		}
		
		if(PF_GameData.PromoAssets.Count > 0)
		{
			ThanksForPlaying.gameObject.SetActive(false);
			
			int rng = UnityEngine.Random.Range(0, PF_GameData.PromoAssets.Count);
			
			UB_UnpackedAssetBundle assets = GameController.Instance.cdnController.GetAssetsByID(PF_GameData.PromoAssets[rng].PromoId);
			this.mainImage.overrideSprite = Sprite.Create(assets.Splash, new Rect(0,0,assets.Splash.width, assets.Splash.height), new Vector2(0.5f, 0.5f));
	
		}
		else
		{
			Debug.Log("No Assets found in PromoAssets");
		}

		StartCoroutine(Spin());
	}
	
	void OnDisable()
	{
        if (OnInterstitialFinish != null)
            OnInterstitialFinish();
	}

	
	public void Next()
	{
        if (Tips.Count == 0) { message.text = GlobalStrings.LOADING_MSG; return; }

        int index = Tips.IndexOf(this.CurrentTip);
        index = (index + 1) % Tips.Count;
        this.CurrentTip = Tips[index];
        message.text = this.CurrentTip.PromoBody;
    }

    public void Prev()
    {
        if (Tips.Count == 0) { message.text = GlobalStrings.LOADING_MSG; return; }

        int index = Tips.IndexOf(this.CurrentTip);
        index = (index + Tips.Count - 1) % Tips.Count;
        this.CurrentTip = Tips[index];
        message.text = this.CurrentTip.PromoBody;
	}
	
	public void Skip()
	{
		this.gameObject.SetActive(false);
	}
	
	public void ShowInterstitial()
	{
		this.gameObject.SetActive(true);
	}
	
	
	IEnumerator Spin()
	{
		this.startTime = Time.time;
		while(this.startTime + this.waitTime > Time.time)
		{
			
			yield return new WaitForEndOfFrame();
		}
		this.gameObject.SetActive(false);
	}

	// Update is called once per frame
	void Update () {
		this.timer.text = string.Format("{0:0.00}", (this.startTime + this.waitTime) - Time.time);
	}
	
}
