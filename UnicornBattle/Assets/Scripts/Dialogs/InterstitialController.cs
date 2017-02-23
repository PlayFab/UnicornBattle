using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public class InterstitialController : MonoBehaviour
{
    public Text message;
    public Text timer;
    public Image spinner;
    public Image mainImage;
    public Button next;
    public Button prev;

    public Text ThanksForPlaying;

    public float startTime;
    public float waitTime = 5;
    
    public delegate void InterstitalComplete();
    public static event InterstitalComplete OnInterstitialFinish;

    private List<UB_PromotionalItem> Tips = new List<UB_PromotionalItem>();
    private UB_PromotionalItem CurrentTip;

    void OnEnable()
    {
        Tips = PF_GameData.promoItems.FindAll((i) => { return i.PromoType == PromotionalItemTypes.Tip; }).ToList();
        if (Tips.Count > 0)
        {
            int rng = UnityEngine.Random.Range(0, Tips.Count - 1);
            CurrentTip = Tips[rng];
            message.text = CurrentTip.PromoBody;
        }

        if (PF_GameData.MinimumInterstitialWait > 0)
        {
            waitTime = PF_GameData.MinimumInterstitialWait;
        }

        if (PF_GameData.PromoAssets.Count > 0)
        {
            ThanksForPlaying.gameObject.SetActive(false);

            int rng = UnityEngine.Random.Range(0, PF_GameData.PromoAssets.Count);
            var assets = GameController.Instance.cdnController.GetAssetsByID(PF_GameData.PromoAssets[rng].PromoId);
            mainImage.overrideSprite = Sprite.Create(assets.Splash, new Rect(0, 0, assets.Splash.width, assets.Splash.height), new Vector2(0.5f, 0.5f));
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

        int index = Tips.IndexOf(CurrentTip);
        index = (index + 1) % Tips.Count;
        CurrentTip = Tips[index];
        message.text = CurrentTip.PromoBody;
    }

    public void Prev()
    {
        if (Tips.Count == 0) { message.text = GlobalStrings.LOADING_MSG; return; }

        int index = Tips.IndexOf(CurrentTip);
        index = (index + Tips.Count - 1) % Tips.Count;
        CurrentTip = Tips[index];
        message.text = CurrentTip.PromoBody;
    }

    public void Skip()
    {
        gameObject.SetActive(false);
    }

    public void ShowInterstitial()
    {
        gameObject.SetActive(true);
    }

    IEnumerator Spin()
    {
        startTime = Time.time;
        while (startTime + waitTime > Time.time)
        {
            yield return new WaitForEndOfFrame();
        }
        gameObject.SetActive(false);
    }

    // Update is called once per frame
    void Update()
    {
        timer.text = string.Format("{0:0.00}", (startTime + waitTime) - Time.time);
    }
}
