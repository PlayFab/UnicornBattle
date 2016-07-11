using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using OnePF;


public class IAB_Controller : MonoBehaviour 
{
    #region Product IDs

	public const string SKU_SMALL_GEM = "small_gem_bundle";
	public const string SKU_MED_GEM = "medium_gem_bundle";
	public const string SKU_LARGE_GEM = "large_gem_bundle";
	public const string SKU_XLARGE_GEM = "extralarge_gem_bundle";
	public const string SKU_MEGA_GEM = "mega_gem_bundle";
	public const string SKU_EPIC_GEM = "epic_gem_bundle";

    #endregion
	//private bool _processingPayment = false;
		

	#region Billing
	//
	    private void Awake()
	    {
	        // Subscribe to all billing events
	        OpenIABEventManager.billingSupportedEvent += OnBillingSupported;
	        OpenIABEventManager.billingNotSupportedEvent += OnBillingNotSupported;
	        OpenIABEventManager.queryInventorySucceededEvent += OnQueryInventorySucceeded;
	        OpenIABEventManager.queryInventoryFailedEvent += OnQueryInventoryFailed;
	        OpenIABEventManager.purchaseSucceededEvent += OnPurchaseSucceded;
	        OpenIABEventManager.purchaseFailedEvent += OnPurchaseFailed;
	        OpenIABEventManager.consumePurchaseSucceededEvent += OnConsumePurchaseSucceeded;
	        OpenIABEventManager.consumePurchaseFailedEvent += OnConsumePurchaseFailed;
	        OpenIABEventManager.transactionRestoredEvent += OnTransactionRestored;
	        OpenIABEventManager.restoreSucceededEvent += OnRestoreSucceeded;
	        OpenIABEventManager.restoreFailedEvent += OnRestoreFailed;
	    }
	
	    private void Start()
	    {
			if (Application.platform == RuntimePlatform.Android || Application.platform == RuntimePlatform.IPhonePlayer) //OpenIAB.IsCurrentPlatformSupported () deprecated?
			{
				// Map SKUs for iOS
				OpenIAB.mapSku(SKU_SMALL_GEM, OpenIAB_iOS.STORE, "small_gem_bundle");
				OpenIAB.mapSku(SKU_MED_GEM, OpenIAB_iOS.STORE, "medium_gem_bundle");
				OpenIAB.mapSku(SKU_LARGE_GEM, OpenIAB_iOS.STORE, "large_gem_bundle");
				OpenIAB.mapSku(SKU_XLARGE_GEM, OpenIAB_iOS.STORE, "extralarge_gem_bundle");
				OpenIAB.mapSku(SKU_MEGA_GEM, OpenIAB_iOS.STORE, "mega_gem_bundle");
				OpenIAB.mapSku(SKU_EPIC_GEM, OpenIAB_iOS.STORE, "epic_gem_bundle");

				// Map SKUs for Google Play
				OpenIAB.mapSku(SKU_SMALL_GEM, OpenIAB_Android.STORE_GOOGLE, "small_gem_bundle");
				OpenIAB.mapSku(SKU_MED_GEM, OpenIAB_Android.STORE_GOOGLE, "medium_gem_bundle");
				OpenIAB.mapSku(SKU_LARGE_GEM, OpenIAB_Android.STORE_GOOGLE, "large_gem_bundle");
				OpenIAB.mapSku(SKU_XLARGE_GEM, OpenIAB_Android.STORE_GOOGLE, "extralarge_gem_bundle");
				OpenIAB.mapSku(SKU_MEGA_GEM, OpenIAB_Android.STORE_GOOGLE, "mega_gem_bundle");
				OpenIAB.mapSku(SKU_EPIC_GEM, OpenIAB_Android.STORE_GOOGLE, "epic_gem_bundle");



				 //Set some library options
				 var options = new OnePF.Options();
				
				//Add Google Play public key
				options.storeKeys.Add(OpenIAB_Android.STORE_GOOGLE, "MIIBIjANBgkqhkiG9w0BAQEFAAOCAQ8AMIIBCgKCAQEAqAgfUKWZ1AFrZa8ot6plf2XCambRZE7EvNpeuENdmMSmN+sloTelHoceCkqkdM//J+caBUbuj1DayZkcmUbFnShPZpI8/9/71fkwY33SvsSjMtoeK5l7vK122WqvLprbaAVF8yvQDLyBJpaVUfpsHH2kOjszGa9XORHMSUqc6qDjSWjkf0FaA+qBc/ffrnCWOu2adlhd3grwEsS6sFEb0XyaNsHN8lq38S7GokHOlI1ehMjf9WtpZaafLjNHkEEu+FdVEIRf6PSUUcwEqdkhy8yucYlLEAvhVEX/oXF9U9Mk9St3FHOxRvQwqZzjoGz+cxmSlQvUM0vAIQhKIgSnYQIDAQAB");
				OpenIAB.init(options);

			} else 
			{
				Debug.Log ("-- IAP -- Not Supported on this platform --");
			}

	 	   }
	
	    private void OnDestroy()
	    {
	        // Unsubscribe to avoid nasty leaks
	        OpenIABEventManager.billingSupportedEvent -= OnBillingSupported;
	        OpenIABEventManager.billingNotSupportedEvent -= OnBillingNotSupported;
	        OpenIABEventManager.queryInventorySucceededEvent -= OnQueryInventorySucceeded;
	        OpenIABEventManager.queryInventoryFailedEvent -= OnQueryInventoryFailed;
	        OpenIABEventManager.purchaseSucceededEvent -= OnPurchaseSucceded;
	        OpenIABEventManager.purchaseFailedEvent -= OnPurchaseFailed;
	        OpenIABEventManager.consumePurchaseSucceededEvent -= OnConsumePurchaseSucceeded;
	        OpenIABEventManager.consumePurchaseFailedEvent -= OnConsumePurchaseFailed;
	        OpenIABEventManager.transactionRestoredEvent -= OnTransactionRestored;
	        OpenIABEventManager.restoreSucceededEvent -= OnRestoreSucceeded;
	        OpenIABEventManager.restoreFailedEvent -= OnRestoreFailed;
	    }
	
	    // Verifies the developer payload of a purchase.
	    bool VerifyDeveloperPayload(string developerPayload)
	    {
	        /*
	         * TODO: verify that the developer payload of the purchase is correct. It will be
	         * the same one that you sent when initiating the purchase.
	         * 
	         * WARNING: Locally generating a random string when starting a purchase and 
	         * verifying it here might seem like a good approach, but this will fail in the 
	         * case where the user purchases an item on one device and then uses your app on 
	         * a different device, because on the other device you will not have access to the
	         * random string you originally generated.
	         *
	         * So a good developer payload has these characteristics:
	         * 
	         * 1. If two different users purchase an item, the payload is different between them,
	         *    so that one user's purchase can't be replayed to another user.
	         * 
	         * 2. The payload must be such that you can verify it even when the app wasn't the
	         *    one who initiated the purchase flow (so that items purchased by the user on 
	         *    one device work on other devices owned by the user).
	         * 
	         * Using your own server to store and verify developer payloads across app
	         * installations is recommended.
	         */
	        return true;
	    }
	
	    private void OnBillingSupported()
	    {
	        Debug.Log("Billing is supported");
		OpenIAB.queryInventory(new string[] { SKU_EPIC_GEM, SKU_MEGA_GEM, SKU_XLARGE_GEM, SKU_LARGE_GEM, SKU_MED_GEM, SKU_SMALL_GEM });
	    }
	
	    private void OnBillingNotSupported(string error)
	    {
	        Debug.Log("Billing not supported: " + error);
	    }
	
	    private void OnQueryInventorySucceeded(Inventory inventory)
	    {
	        Debug.Log("Query inventory succeeded: " + inventory);
	
//	        // Do we have the infinite ammo subscription?
//	        Purchase godModePurchase = inventory.GetPurchase(SKU_MED_GEM);
//	        bool godModeSubscription = (godModePurchase != null && VerifyDeveloperPayload(godModePurchase.DeveloperPayload));
//	        Debug.Log("User " + (godModeSubscription ? "HAS" : "DOES NOT HAVE") + " god mode subscription");
//	        _ship.IsGodMode = godModeSubscription;
//	
//	        // Check premium skin purchase
//	        Purchase cowboyHatPurchase = inventory.GetPurchase(SKU_LARGE_GEM);
//	        bool isPremiumSkin = (cowboyHatPurchase != null && VerifyDeveloperPayload(cowboyHatPurchase.DeveloperPayload));
//	        Debug.Log("User " + (isPremiumSkin ? "HAS" : "HAS NO") + " premium skin");
//	        _ship.IsPremiumSkin = isPremiumSkin;
//	
//	        // Check for delivery of expandable items. If we own some, we should consume everything immediately
//	        Purchase repairKitPurchase = inventory.GetPurchase(SKU_SMALL_GEM);
//	        if (repairKitPurchase != null && VerifyDeveloperPayload(repairKitPurchase.DeveloperPayload))
//	            OpenIAB.consumeProduct(inventory.GetPurchase(SKU_SMALL_GEM));
	    }
	
	    private void OnQueryInventoryFailed(string error)
	    {
	        Debug.Log("Query inventory failed: " + error);
	    }
	
	    private void OnPurchaseSucceded(Purchase purchase)
	    {
	        Debug.Log("Purchase succeded: " + purchase.Sku + "; Payload: " + purchase.DeveloperPayload);
	        if (!VerifyDeveloperPayload(purchase.DeveloperPayload))
	            return;
	        
	        Debug.Log("Purchase JSON: " + purchase.OriginalJson);
	        Debug.Log("Purchase Signature: " + purchase.Signature);
	        
	        // Check what was purchased and update game
	        switch (purchase.Sku)
	        {
	            case SKU_SMALL_GEM:
	                // Consume repair kit
	                OpenIAB.consumeProduct(purchase);
	                break;
	            case SKU_MED_GEM:
	               // _ship.IsGodMode = true;
					OpenIAB.consumeProduct(purchase);
	                break;;
	            case SKU_LARGE_GEM:
	               // _ship.IsPremiumSkin = true;
					OpenIAB.consumeProduct(purchase);
	                break;
				case SKU_XLARGE_GEM:
					OpenIAB.consumeProduct(purchase);
	                break;
				case SKU_MEGA_GEM:
					OpenIAB.consumeProduct(purchase);
	                break;
				case SKU_EPIC_GEM:
					OpenIAB.consumeProduct(purchase);
	                break;
	            default:
	                Debug.LogWarning("Unknown SKU: " + purchase.Sku);
	                break;
	        }

	        // _processingPayment = false;
			PF_Bridge.RaiseCallbackSuccess("IAB Purchase Success", PlayFabAPIMethods.MakePurchase, MessageDisplayStyle.none);
			
			if(Application.platform == RuntimePlatform.Android)
			{
				PF_Bridge.ValidateAndroidPurcahse(purchase.OriginalJson, purchase.Signature);							
			}
			else if( Application.platform == RuntimePlatform.IPhonePlayer)
			{
				// I have chosen to put the reciept in the developer payload field as this was not being used, and seemed like a good place to pass this through
				PF_Bridge.ValidateIosPurchase(purchase.DeveloperPayload);
			}
	    }
	
	    private void OnPurchaseFailed(int errorCode, string error)
	    {
	        Debug.Log("Purchase failed: " + error);
	       // _processingPayment = false;
			PF_Bridge.RaiseCallbackError(error, PlayFabAPIMethods.Generic, MessageDisplayStyle.error);
	    }
	
	    private void OnConsumePurchaseSucceeded(Purchase purchase)
	    {
	        Debug.Log("Consume purchase succeded: " + purchase.ToString());
	        //_processingPayment = false;
			PF_Bridge.RaiseCallbackSuccess("Consume IAB Purchase Success", PlayFabAPIMethods.Generic, MessageDisplayStyle.none);
	    }
	
	    private void OnConsumePurchaseFailed(string error)
	    {
	        Debug.Log("Consume purchase failed: " + error);
	       // _processingPayment = false;
			PF_Bridge.RaiseCallbackError(error, PlayFabAPIMethods.Generic, MessageDisplayStyle.error);
	    }
	
	    private void OnTransactionRestored(string sku)
	    {
	        Debug.Log("Transaction restored: " + sku);
	    }
	
	    private void OnRestoreSucceeded()
	    {
	        Debug.Log("Transactions restored successfully");
	    }
	
	    private void OnRestoreFailed(string error)
	    {
	        Debug.Log("Transaction restore failed: " + error);
	    }
	
	#endregion // Billing

}
