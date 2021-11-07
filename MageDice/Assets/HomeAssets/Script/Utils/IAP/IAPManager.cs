using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Events;
#if IAP
using UnityEngine.Purchasing;
using UnityEngine.Purchasing.Security;
#endif

public class IAPManager : MonoSingleton<IAPManager>
#if IAP
    , IStoreListener
#endif
{

    private UnityAction<bool> buyCallback;
    private string logWhere = string.Empty;
    public void BuyProduct(string key_iap, string where, UnityAction<bool> callback)
    {
        LoadingManager.Instance.ShowLoading(true);
        this.buyCallback = callback;
        this.logWhere = where;

#if UNITY_EDITOR || CHEAT
        this.PurchaseSuccess();
#elif IAP && !CHEAT
         if (!GameUtils.CheckInstallSource())
        {
            this.PurchaseFail();
            return;
        }
        this.BuyProductID(key_iap);
#endif
    }



    private void PurchaseSuccess()
    {
        //MissionDatas.Instance.DoStep(MissionID.BUY_IAP);
        if (this.buyCallback != null)
        {
            this.buyCallback.Invoke(true);
        }
        LoadingManager.Instance.ShowLoading(false);
    }
    private void PurchaseFail()
    {
        if (this.buyCallback != null)
        {
            this.buyCallback.Invoke(false);
        }
        LoadingManager.Instance.ShowLoading(false);
    }

    public string FormatMoneyLocal(string key_iap)
    {
#if IAP
        return this.GetLocalPrice(key_iap);
#endif
        return string.Format("${0}", "...");
    }

#if IAP
    private static IStoreController m_StoreController;          // The Unity Purchasing system.
    private static IExtensionProvider m_StoreExtensionProvider; // The store-specific Purchasing subsystems.
#endif

    public override void Init()
    {
        base.Init();
        GameManager.Instance.AddCallbackNetWork(this.ChangeNetwork);
    }
    private void OnDisable()
    {
#if UNITY_EDITOR
        if (GameManager.isApplicationQuit)
            return;
#endif
        //NetworkHelper.Instance.RemoveCallbackNetwork(this.ChangeNetwork);
    }
    private void ChangeNetwork(bool isNetwork)
    {
        if (!this.IsInitialized() && isNetwork)//chua init ads && network
        {
#if IAP
            this.LoadProduct();
#endif
        }
    }
    private bool IsInitialized()
    {
#if IAP
        // Only say we are initialized if both the Purchasing references are set.
        return m_StoreController != null && m_StoreExtensionProvider != null;
#endif
        return false;
    }

#if IAP
    private void LoadProduct()
    {
        // If we have already connected to Purchasing ...
        if (IsInitialized())
        {
            // ... we are done here.
            return;
        }

        // Create a builder, first passing in a suite of Unity provided stores.
        var builder = ConfigurationBuilder.Instance(StandardPurchasingModule.Instance());

        ///Shop bundle
        ///

        /*List<ShopConfig> configs = ShopConfigs.Instance.cueLites;

        foreach (ShopIAPBaseConfig config in configs)
        {
            builder.AddProduct(config.key_iap, ProductType.Consumable);
        }*/

        // Kick off the remainder of the set-up with an asynchrounous call, passing the configuration 
        // and this class' instance. Expect a response either in OnInitialized or OnInitializeFailed.
        UnityPurchasing.Initialize(this, builder);
        GameManager.Instance.LogMessage("INIT IAP: " + builder.products.Count);
    }
    

    private Product GetProductById(string id)
    {
        try
        {
            return m_StoreController.products.all.ToList<Product>().Find((Product e) => e.definition.id.Equals(id));
        }
        catch (System.Exception ex)
        {
            Debug.Log(ex.Message);
        }
        return null;
    }
    public string GetLocalPrice(string key_iap)
    {
        Product product = this.GetProductById(key_iap);
        if (product != null)
        {
            return product.metadata.localizedPriceString;
        }
        return string.Empty;
    }


    
    public void OnInitialized(IStoreController controller, IExtensionProvider extensions)
    {
        // Purchasing has succeeded initializing. Collect our Purchasing references.
        Debug.Log("OnInitialized: PASS");

        // Overall Purchasing system, configured with products for this application.
        m_StoreController = controller;
        // Store specific subsystem, for accessing device-specific store features.
        m_StoreExtensionProvider = extensions;

        GameManager.Instance.LogMessage("OnInitialized: PASS");
    }

    public void OnInitializeFailed(InitializationFailureReason error)
    {
        // Purchasing set-up has not succeeded. Check error for reason. Consider sharing this reason with the user.
        Debug.Log("OnInitializeFailed InitializationFailureReason:" + error);

        GameManager.Instance.LogMessage("OnInitializeFailed InitializationFailureReason:" + error);
    }

    private void BuyProductID(string productId)
    {
        // If Purchasing has been initialized ...
        if (IsInitialized())
        {
            // ... look up the Product reference with the general product identifier and the Purchasing 
            // system's products collection.
            Product product = m_StoreController.products.WithID(productId);

            // If the look up found a product for this device's store and that product is ready to be sold ... 
            if (product != null && product.availableToPurchase)
            {
                //GameManager.Instance.ShowMessageBox(productId + " tồn tại");
                string msg = string.Format("Purchasing product asychronously: '{0}'", product.definition.id);
                Debug.Log(msg);
                // ... buy the product. Expect a response either through ProcessPurchase or OnPurchaseFailed 
                // asynchronously.
                m_StoreController.InitiatePurchase(product);
                GameManager.Instance.LogMessage(msg);
            }
            // Otherwise ...
            else
            {

                //GameManager.Instance.ShowMessageBox(productId + " không tồn tại");
                // ... report the product look-up failure situation
                string msg = "BuyProductID: FAIL. Not purchasing product, either is not found or is not available for purchase";

                Debug.Log("BuyProductID: FAIL. Not purchasing product, either is not found or is not available for purchase");

                GameManager.Instance.LogMessage(msg);
            }
        }
        // Otherwise ...
        else
        {
            // ... report the fact Purchasing has not succeeded initializing yet. Consider waiting longer or 
            // retrying initiailization.
            //GameManager.Instance.ShowMessageBox(productId + " chưa khởi tạo");
            Debug.Log("BuyProductID FAIL. Not initialized.");
            //GameManager.Instance.LogMessage("BuyProductID FAIL. Not initialized");
        }
    }

    public PurchaseProcessingResult ProcessPurchase(PurchaseEventArgs args)
    {
        Product product = args.purchasedProduct;
        bool validPurchase = true; // Presume valid for platforms with no R.V.

        // Unity IAP's validation logic is only included on these platforms.
#if UNITY_ANDROID || UNITY_IOS || UNITY_STANDALONE_OSX
        // Prepare the validator with the secrets we prepared in the Editor
        // obfuscation window.
        var validator = new CrossPlatformValidator(GooglePlayTangle.Data(),
            AppleTangle.Data(), Application.identifier);

        try {
            // On Google Play, result has a single product ID.
            // On Apple stores, receipts contain multiple products.
            var result = validator.Validate(product.receipt);
            // For informational purposes, we list the receipt(s)
            Debug.Log("Receipt is valid. Contents:");
            foreach (IPurchaseReceipt productReceipt in result) {
                Debug.Log(productReceipt.productID);
                Debug.Log(productReceipt.purchaseDate);
                Debug.Log(productReceipt.transactionID);
            }
        } catch (IAPSecurityException) {
            Debug.Log("Invalid receipt, not unlocking content");
            validPurchase = false;
        }
#endif
        if (validPurchase)
        {
            this.PurchaseSuccess();
            LogManager.LogInApp(product.definition.id, product.transactionID, product.receipt, product.metadata.isoCurrencyCode, product.metadata.localizedPriceString, this.logWhere, RoomDatas.Instance.GetRoomUnlockedMax());
            CosinasDatas.CosinaLogManager.LogInApp(product.definition.id, product.transactionID, product.receipt, product.metadata.isoCurrencyCode, 99, RoomDatas.Instance.GetRoomUnlockedMax(), this.logWhere);
            this.logWhere = String.Empty;
            SoundManager.Instance.Play("purchase_success");
        }
        else
        {
            this.PurchaseFail();
        }
        
        return PurchaseProcessingResult.Complete;
    }

    public void OnPurchaseFailed(Product product, PurchaseFailureReason failureReason)
    {
        this.PurchaseFail();
        //GameManager.Instance.ShowMessageBox("Can't purchase", "There's a problem when purchasing.");
        Debug.Log(string.Format("OnPurchaseFailed: FAIL. Product: '{0}', PurchaseFailureReason: {1}", product.definition.storeSpecificId, failureReason));
        this.logWhere = String.Empty;
        GameManager.Instance.LogMessage(string.Format("OnPurchaseFailed: FAIL. Product: '{0}', PurchaseFailureReason: {1}", product.definition.storeSpecificId, failureReason));
    }
#endif
}
