/*******************************************************************************
* Copyright 2012-2014 One Platform Foundation
*
* Licensed under the Apache License, Version 2.0 (the "License");
* you may not use this file except in compliance with the License.
* You may obtain a copy of the License at
*
* http://www.apache.org/licenses/LICENSE-2.0
*
* Unless required by applicable law or agreed to in writing, software
* distributed under the License is distributed on an "AS IS" BASIS,
* WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
* See the License for the specific language governing permissions and
* limitations under the License.
******************************************************************************/

using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using OnePF;

/// <summary>
/// Sample of the in-game shop
/// </summary>
public class ShopDemo : MonoBehaviour
{
//    #region UI constants
//
//    private const int OFFSET = 5;
//    private const int BUTTON_WIDTH = 200;
//    private const int BUTTON_HEIGHT = 80;
//
//    private const int SIDE_BUTTON_WIDTH = 140;
//    private const int SHOP_BUTTON_HEIGHT = 60;
//
//    private const int WINDOW_WIDTH = 400;
//    private const int WINDOW_HEIGHT = 320;
//
//    private const int FONT_SIZE = 24;
//
//    #endregion
//
//    #region Product IDs
//
//	public const string SKU_SMALL_GEM = "small_gem_bundle";
//	public const string SKU_MED_GEM = "medium_gem_bundle";
//	public const string SKU_LARGE_GEM = "large_gem_bundle";
//
//    #endregion
//
//    const string SLIDE_ME = "SlideME";
//
//    /// <summary>
//    /// Shop is active right now
//    /// </summary>
//    private bool _processingPayment = false;
//    /// <summary>
//    /// If shop window is on screen
//    /// </summary>
//    private bool _showShopWindow = false;
//    /// <summary>
//    /// Text in the popup window
//    /// </summary>
//    private string _popupText = "";
//
//    [SerializeField]
//    GameObject _gui = null;
//
//    [SerializeField]
//    ShipController _ship = null;
//
//    #region Billing
//
//    private void Awake()
//    {
//        // Subscribe to all billing events
//        OpenIABEventManager.billingSupportedEvent += OnBillingSupported;
//        OpenIABEventManager.billingNotSupportedEvent += OnBillingNotSupported;
//        OpenIABEventManager.queryInventorySucceededEvent += OnQueryInventorySucceeded;
//        OpenIABEventManager.queryInventoryFailedEvent += OnQueryInventoryFailed;
//        OpenIABEventManager.purchaseSucceededEvent += OnPurchaseSucceded;
//        OpenIABEventManager.purchaseFailedEvent += OnPurchaseFailed;
//        OpenIABEventManager.consumePurchaseSucceededEvent += OnConsumePurchaseSucceeded;
//        OpenIABEventManager.consumePurchaseFailedEvent += OnConsumePurchaseFailed;
//        OpenIABEventManager.transactionRestoredEvent += OnTransactionRestored;
//        OpenIABEventManager.restoreSucceededEvent += OnRestoreSucceeded;
//        OpenIABEventManager.restoreFailedEvent += OnRestoreFailed;
//    }
//
//    private void Start()
//    {
//        // Map SKUs for iOS
//        OpenIAB.mapSku(SKU_SMALL_GEM, OpenIAB_iOS.STORE, "30_real");
//        OpenIAB.mapSku(SKU_MED_GEM, OpenIAB_iOS.STORE, "noncons_2");
//        OpenIAB.mapSku(SKU_LARGE_GEM, OpenIAB_iOS.STORE, "noncons_1");
//
//        // Map SKUs for Google Play
//        OpenIAB.mapSku(SKU_SMALL_GEM, OpenIAB_Android.STORE_GOOGLE, "small_gem_bundle");
//        OpenIAB.mapSku(SKU_LARGE_GEM, OpenIAB_Android.STORE_GOOGLE, "medium_gem_bundle");
//        OpenIAB.mapSku(SKU_MED_GEM, OpenIAB_Android.STORE_GOOGLE, "large_gem_bundle");
//
//        // Map SKUs for Amazon
//        OpenIAB.mapSku(SKU_SMALL_GEM, OpenIAB_Android.STORE_AMAZON, "amazon.sku_repair_kit");
//        OpenIAB.mapSku(SKU_LARGE_GEM, OpenIAB_Android.STORE_AMAZON, "amazon.sku_premium_skin");
//        OpenIAB.mapSku(SKU_MED_GEM, OpenIAB_Android.STORE_AMAZON, "amazon.sku_god_mode");
//
//        // Map SKUs for SlideME
//        OpenIAB.mapSku(SKU_SMALL_GEM, SLIDE_ME, "sm.sku_repair_kit");
//        OpenIAB.mapSku(SKU_LARGE_GEM, SLIDE_ME, "sm.sku_premium_skin");
//        OpenIAB.mapSku(SKU_MED_GEM, SLIDE_ME, "sm.sku_god_mode");
//
//        // Map SKUs for Yandex.Store
//        OpenIAB.mapSku(SKU_SMALL_GEM, OpenIAB_Android.STORE_YANDEX, "yandex.sku_repair_kit");
//        OpenIAB.mapSku(SKU_LARGE_GEM, OpenIAB_Android.STORE_YANDEX, "yandex.sku_premium_skin");
//        OpenIAB.mapSku(SKU_MED_GEM, OpenIAB_Android.STORE_YANDEX, "yandex.sku_god_mode");
//
//        // Map SKUs for Windows Phone 8
//        OpenIAB.mapSku(SKU_SMALL_GEM, OpenIAB_WP8.STORE, "wp8.sku_repair_kit");
//        OpenIAB.mapSku(SKU_LARGE_GEM, OpenIAB_WP8.STORE, "wp8.sku_premium_skin");
//        OpenIAB.mapSku(SKU_MED_GEM, OpenIAB_WP8.STORE, "wp8.sku_god_mode");
//
//        // Set some library options
//        var options = new OnePF.Options();
//        
//        // Add Google Play public key
//		options.storeKeys.Add(OpenIAB_Android.STORE_GOOGLE, "MIIBIjANBgkqhkiG9w0BAQEFAAOCAQ8AMIIBCgKCAQEAqAgfUKWZ1AFrZa8ot6plf2XCambRZE7EvNpeuENdmMSmN+sloTelHoceCkqkdM//J+caBUbuj1DayZkcmUbFnShPZpI8/9/71fkwY33SvsSjMtoeK5l7vK122WqvLprbaAVF8yvQDLyBJpaVUfpsHH2kOjszGa9XORHMSUqc6qDjSWjkf0FaA+qBc/ffrnCWOu2adlhd3grwEsS6sFEb0XyaNsHN8lq38S7GokHOlI1ehMjf9WtpZaafLjNHkEEu+FdVEIRf6PSUUcwEqdkhy8yucYlLEAvhVEX/oXF9U9Mk9St3FHOxRvQwqZzjoGz+cxmSlQvUM0vAIQhKIgSnYQIDAQAB");
//
//        // Add SlideME public key
//        options.storeKeys.Add(SLIDE_ME, "MIIBIjANBgkqhkiG9w0BAQEFAAOCAQ8AMIIBCgKCAQEA5p5XkwapZsXrpHvrML6Oac4OuDwGPBfC8j1GMiPka0v1MXGN6rcC37qIOOsEMN9v9csS3mLPGINMHmcDJTTrIuLDbSB0QmB7iC3EzfUBAitHghEgDOba0Jn06tfcMrXalNQ8lpZJh4W1QgwWKra0CUTEHWKGwOdTS6YLQanvsC6B/16iGGFGymkKjGi0ptouplgvwZHe+4gqo6SoR5tRK7fkcSS+qSzHYdvAcmhzAYGKaV1Ihjy3dd9n2Jz5XeoNag4MSbKQ0YmHyjmyvyKliKOMDps3V5X9DJzTSSVOSYDVbrFPtdKzr2mJD7T7mtoTnaXYUQLCWOCQs2Oi7djW+QIDAQAB");
//
//        // Add Yandex.Store public key
//        options.storeKeys.Add(OpenIAB_Android.STORE_YANDEX, "MIIBIjANBgkqhkiG9w0BAQEFAAOCAQ8AMIIBCgKCAQEArvbmWP5xYYSKpdnjvLIRWlFzkGT1xoZIekhWPkGonE+KRd5EWNejLPvOcayY98+NCkLHKSENQSzH+T8IUIEQOhNFzviIblfy3UYG0cFcIehKOV8IiIxvPRAWHJdzrohTjCCU1A/Lz1NtUn/yxyezrbC0l2ebAdrZSTONMNxMSKAB1+YVbzXI7u4ls9/FIVnEXOyMnCHADEOgoklFnXEdajJHN/1o+cRz61ow8Qxr+IwG4SSQD1vlv7qkxwEVqBXdhkJBPprZRLq1+VWu+E3f+nYY0I/mHqsn+pY6z/XYK21gpo4ZbCQzsEDc9ySMeW9mYXZgcIdCTuUic9S26tYfGQIDAQAB");
//
//        OpenIAB.init(options);
//    }
//
//    private void OnDestroy()
//    {
//        // Unsubscribe to avoid nasty leaks
//        OpenIABEventManager.billingSupportedEvent -= OnBillingSupported;
//        OpenIABEventManager.billingNotSupportedEvent -= OnBillingNotSupported;
//        OpenIABEventManager.queryInventorySucceededEvent -= OnQueryInventorySucceeded;
//        OpenIABEventManager.queryInventoryFailedEvent -= OnQueryInventoryFailed;
//        OpenIABEventManager.purchaseSucceededEvent -= OnPurchaseSucceded;
//        OpenIABEventManager.purchaseFailedEvent -= OnPurchaseFailed;
//        OpenIABEventManager.consumePurchaseSucceededEvent -= OnConsumePurchaseSucceeded;
//        OpenIABEventManager.consumePurchaseFailedEvent -= OnConsumePurchaseFailed;
//        OpenIABEventManager.transactionRestoredEvent -= OnTransactionRestored;
//        OpenIABEventManager.restoreSucceededEvent -= OnRestoreSucceeded;
//        OpenIABEventManager.restoreFailedEvent -= OnRestoreFailed;
//    }
//
//    // Verifies the developer payload of a purchase.
//    bool VerifyDeveloperPayload(string developerPayload)
//    {
//        /*
//         * TODO: verify that the developer payload of the purchase is correct. It will be
//         * the same one that you sent when initiating the purchase.
//         * 
//         * WARNING: Locally generating a random string when starting a purchase and 
//         * verifying it here might seem like a good approach, but this will fail in the 
//         * case where the user purchases an item on one device and then uses your app on 
//         * a different device, because on the other device you will not have access to the
//         * random string you originally generated.
//         *
//         * So a good developer payload has these characteristics:
//         * 
//         * 1. If two different users purchase an item, the payload is different between them,
//         *    so that one user's purchase can't be replayed to another user.
//         * 
//         * 2. The payload must be such that you can verify it even when the app wasn't the
//         *    one who initiated the purchase flow (so that items purchased by the user on 
//         *    one device work on other devices owned by the user).
//         * 
//         * Using your own server to store and verify developer payloads across app
//         * installations is recommended.
//         */
//        return true;
//    }
//
//    private void OnBillingSupported()
//    {
//        Debug.Log("Billing is supported");
//        OpenIAB.queryInventory(new string[] { SKU_LARGE_GEM, SKU_MED_GEM, SKU_SMALL_GEM });
//    }
//
//    private void OnBillingNotSupported(string error)
//    {
//        Debug.Log("Billing not supported: " + error);
//    }
//
//    private void OnQueryInventorySucceeded(Inventory inventory)
//    {
//        Debug.Log("Query inventory succeeded: " + inventory);
//
//        // Do we have the infinite ammo subscription?
//        Purchase godModePurchase = inventory.GetPurchase(SKU_MED_GEM);
//        bool godModeSubscription = (godModePurchase != null && VerifyDeveloperPayload(godModePurchase.DeveloperPayload));
//        Debug.Log("User " + (godModeSubscription ? "HAS" : "DOES NOT HAVE") + " god mode subscription");
//        _ship.IsGodMode = godModeSubscription;
//
//        // Check premium skin purchase
//        Purchase cowboyHatPurchase = inventory.GetPurchase(SKU_LARGE_GEM);
//        bool isPremiumSkin = (cowboyHatPurchase != null && VerifyDeveloperPayload(cowboyHatPurchase.DeveloperPayload));
//        Debug.Log("User " + (isPremiumSkin ? "HAS" : "HAS NO") + " premium skin");
//        _ship.IsPremiumSkin = isPremiumSkin;
//
//        // Check for delivery of expandable items. If we own some, we should consume everything immediately
//        Purchase repairKitPurchase = inventory.GetPurchase(SKU_SMALL_GEM);
//        if (repairKitPurchase != null && VerifyDeveloperPayload(repairKitPurchase.DeveloperPayload))
//            OpenIAB.consumeProduct(inventory.GetPurchase(SKU_SMALL_GEM));
//    }
//
//    private void OnQueryInventoryFailed(string error)
//    {
//        Debug.Log("Query inventory failed: " + error);
//    }
//
//    private void OnPurchaseSucceded(Purchase purchase)
//    {
//        Debug.Log("Purchase succeded: " + purchase.Sku + "; Payload: " + purchase.DeveloperPayload);
//        if (!VerifyDeveloperPayload(purchase.DeveloperPayload))
//            return;
//        
//        Debug.Log("Purchase JSON: " + purchase.OriginalJson);
//        Debug.Log("Purchase Signature: " + purchase.Signature);
//        
//        // Check what was purchased and update game
//        switch (purchase.Sku)
//        {
//            case SKU_SMALL_GEM:
//                _ship.AddRepairKit();
//                // Consume repair kit
//                OpenIAB.consumeProduct(purchase);
//                break;
//            case SKU_MED_GEM:
//                _ship.IsGodMode = true;
//                break;;
//            case SKU_LARGE_GEM:
//                _ship.IsPremiumSkin = true;
//                break;
//            default:
//                Debug.LogWarning("Unknown SKU: " + purchase.Sku);
//                break;
//        }
//        _processingPayment = false;
//    }
//
//    private void OnPurchaseFailed(int errorCode, string error)
//    {
//        Debug.Log("Purchase failed: " + error);
//        _processingPayment = false;
//    }
//
//    private void OnConsumePurchaseSucceeded(Purchase purchase)
//    {
//        Debug.Log("Consume purchase succeded: " + purchase.ToString());
//        _processingPayment = false;
//    }
//
//    private void OnConsumePurchaseFailed(string error)
//    {
//        Debug.Log("Consume purchase failed: " + error);
//        _processingPayment = false;
//    }
//
//    private void OnTransactionRestored(string sku)
//    {
//        Debug.Log("Transaction restored: " + sku);
//    }
//
//    private void OnRestoreSucceeded()
//    {
//        Debug.Log("Transactions restored successfully");
//    }
//
//    private void OnRestoreFailed(string error)
//    {
//        Debug.Log("Transaction restore failed: " + error);
//    }
//
//    #endregion // Billing
//
//    #region GUI
//
//    void DrawPopup(int windowID)
//    {
//        // Close button
//        if (GUI.Button(new Rect(WINDOW_WIDTH - 50, 0, 50, 50), "X"))
//        {
//            _popupText = "";
//            PauseGame(false);
//        }
//        // Text
//        GUI.Label(new Rect(10, WINDOW_HEIGHT * 0.3f, WINDOW_WIDTH - 20, WINDOW_HEIGHT), _popupText);
//    }
//
//    /// <summary>
//    /// Show shop window
//    /// </summary>
//    /// <param name="windowID">window id</param>
//    void DrawShopWindow(int windowID)
//    {
//        // Close button
//        if (GUI.Button(new Rect(WINDOW_WIDTH - 50, 0, 50, 50), "X"))
//        {
//            ShowShopWindow(false);
//        }
//
//        // Disable everything while processing anything
//        if (_processingPayment)
//        {
//            GUI.Box(new Rect(10, 40, WINDOW_WIDTH - 20, SHOP_BUTTON_HEIGHT), "Processing payment...");
//            return;
//        }
//
//        GUI.skin.box.alignment = TextAnchor.MiddleCenter;
//
//        int topOffset = 80;
//
//        // Buy god mode subscription
//        Rect rect = new Rect(10, topOffset, WINDOW_WIDTH - 20, SHOP_BUTTON_HEIGHT);
//        if (_ship.IsGodMode)
//        {
//			GUI.Box(rect, "Small Gem Bundle");
//        }
//        else if (GUI.Button(rect, "Medium Gem Bundle"))
//        {
//            _processingPayment = true;
//            OpenIAB.purchaseSubscription(SKU_MED_GEM);
//        }
//
//        topOffset += SHOP_BUTTON_HEIGHT + OFFSET * 2;
//
//        // Buy Repair Kit
//        rect = new Rect(10, topOffset, WINDOW_WIDTH - 20, SHOP_BUTTON_HEIGHT);
//        if (GUI.Button(rect, "Small Gem Bundle"))
//        {
//            _processingPayment = true;
//            OpenIAB.purchaseProduct(SKU_SMALL_GEM);
//        }
//
//        topOffset += SHOP_BUTTON_HEIGHT + OFFSET * 2;
//
//        // Buy Premium Skin
//        rect = new Rect(10, topOffset, WINDOW_WIDTH - 20, SHOP_BUTTON_HEIGHT);
//        if (_ship.IsPremiumSkin)
//        {
//            GUI.Box(rect, "Premium Skin purchased");
//        }
//        else if (GUI.Button(rect, "Large Gem Bundle"))
//        {
//            _processingPayment = true;
//            OpenIAB.purchaseProduct(SKU_LARGE_GEM);
//        }
//    }
//
//    void ShowShopWindow(bool show)
//    {
//        _showShopWindow = show;
//        PauseGame(show);
//    }
//
//    void OnGUI()
//    {
//        // Set UI font size
//        GUI.skin.window.fontSize = GUI.skin.label.fontSize = GUI.skin.box.fontSize = GUI.skin.button.fontSize = FONT_SIZE;
//        
//        if (!_showShopWindow)
//        {
//            if (string.IsNullOrEmpty(_popupText) && GUI.Button(new Rect(Screen.width - BUTTON_WIDTH - OFFSET, OFFSET, BUTTON_WIDTH, BUTTON_HEIGHT), "Shop", GUI.skin.button))
//            {
//                ShowShopWindow(true);
//            }
//            if (GUI.Button(new Rect(Screen.width - BUTTON_WIDTH - OFFSET, OFFSET * 2 + BUTTON_HEIGHT, BUTTON_WIDTH, BUTTON_HEIGHT / 2), "Restore", GUI.skin.button))
//            {
//                OpenIAB.restoreTransactions();
//            }
//            if (GUI.Button(new Rect(Screen.width - BUTTON_WIDTH - OFFSET, OFFSET * 3 + BUTTON_HEIGHT * 1.5f , BUTTON_WIDTH, BUTTON_HEIGHT), "Repair", GUI.skin.button))
//            {
//                _ship.UseRepairKit();
//            }
//        }
//        else
//        {
//            GUI.Window(0, new Rect(Screen.width / 2 - WINDOW_WIDTH / 2, Screen.height / 2 - WINDOW_HEIGHT / 2, WINDOW_WIDTH, WINDOW_HEIGHT), DrawShopWindow, "Game Shop");
//        }
//
//        if (!string.IsNullOrEmpty(_popupText))
//        {
//            GUI.Window(0, new Rect(Screen.width / 2 - WINDOW_WIDTH / 2, Screen.height / 2 - WINDOW_HEIGHT / 2, WINDOW_WIDTH, WINDOW_HEIGHT), DrawPopup, "");
//        }
//    }
//    #endregion // GUI
//
//    /// <summary>
//    /// Simply pause game
//    /// </summary>
//    /// <param name="pause">game is paused</param>
//    void PauseGame(bool pause)
//    {
//        if (pause)
//        {
//            Time.timeScale = 0;
//            _gui.SetActive(false);
//        }
//        else
//        {
//            Time.timeScale = 1;
//            _gui.SetActive(true);
//        }
//    }
}
