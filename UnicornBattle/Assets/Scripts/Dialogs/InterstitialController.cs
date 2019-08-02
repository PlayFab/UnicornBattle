using System.Collections;
using System.Collections.Generic;
using UnicornBattle.Managers;
using UnicornBattle.Models;
using UnityEngine;
using UnityEngine.UI;

namespace UnicornBattle.Controllers
{

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

        private List<UBPromotionalItem> Tips = new List<UBPromotionalItem>();
        private UBPromotionalItem CurrentTip;

        private PromotionsManager m_PromotionsManager;

        void OnEnable()
        {
            if (null == m_PromotionsManager)
                m_PromotionsManager = MainManager.Instance.getPromotionsManager();

            Tips = m_PromotionsManager.GetUBPromotionalItems(PromotionalItemTypes.Tip);
            if (Tips.Count > 0)
            {
                int rng = UnityEngine.Random.Range(0, Tips.Count - 1);
                CurrentTip = Tips[rng];
                message.text = CurrentTip.PromoBody;
            }

            GameDataManager l_gameDataMgr = MainManager.Instance.getGameDataManager();
            if (null == l_gameDataMgr) return;
            waitTime = (l_gameDataMgr.MinimumInterstitialWait > 0) ? l_gameDataMgr.MinimumInterstitialWait : waitTime;

            if (m_PromotionsManager.HasAnyPromotions())
            {
                ThanksForPlaying.gameObject.SetActive(false);
                UBUnpackedAssetBundle[] l_assets = m_PromotionsManager.GetAllPromoAssets();

                int rng = UnityEngine.Random.Range(0, l_assets.Length);
                var assets = GameController.Instance.cdnController.GetAssetsByID(l_assets[rng].PromoId);
                mainImage.overrideSprite = Sprite.Create(assets.Splash, new Rect(0, 0, assets.Splash.width, assets.Splash.height), new Vector2(0.5f, 0.5f));
            }
            else
            {
                Debug.LogWarning("No Assets found in PromoAssets");
            }

            StartCoroutine(Spin());
            TelemetryManager.RecordScreenViewed(TelemetryScreenId.LoadingScreen);
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
}