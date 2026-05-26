using Plugin.InAppBilling;
using System.Diagnostics;

namespace WikiExtractor.Maui.App.Services
{
    public class NoAdsService : INoAdsService
    {
        private const string StorageKey = "no_ads_purchased";
        private const string PendingProductKey = "no_ads_pending_product";

        private bool _isNoAdsEnabled;
        public bool IsNoAdsEnabled => _isNoAdsEnabled;

        // Fast path: SecureStorage only — safe to call at startup on the main thread.
        public async Task<bool> LoadLocalEntitlementAsync()
        {
            try
            {
                var stored = await SecureStorage.GetAsync(StorageKey);
                if (stored == "true")
                {
                    _isNoAdsEnabled = true;
                    return true;
                }
                return false;
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"[NoAds] LoadLocalEntitlementAsync error: {ex.Message}");
                return false;
            }
        }

        public async Task<bool> LoadEntitlementAsync()
        {
            try
            {
                var stored = await SecureStorage.GetAsync(StorageKey);
                if (stored == "true")
                {
                    _isNoAdsEnabled = true;
                    return true;
                }

                var productId = await SecureStorage.GetAsync("no_ads_product_id");
                var verified = await VerifyWithStoreAsync(productId ?? string.Empty);
                _isNoAdsEnabled = verified;
                return verified;
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"[NoAds] LoadEntitlementAsync error: {ex.Message}");
                return false;
            }
        }

        public async Task<NoAdsPurchaseResult> PurchaseNoAdsAsync(string productId)
        {
            try
            {
                if (!CrossInAppBilling.IsSupported)
                    return NoAdsPurchaseResult.Failed;

                var billing = CrossInAppBilling.Current;
                var connected = await billing.ConnectAsync();
                if (!connected)
                    return NoAdsPurchaseResult.Failed;

                try
                {
                    var purchase = await billing.PurchaseAsync(productId, ItemType.InAppPurchase);

                    if (purchase == null)
                        return NoAdsPurchaseResult.Cancelled;

                    if (purchase.State == PurchaseState.PaymentPending || purchase.State == PurchaseState.Purchasing)
                    {
                        await SecureStorage.SetAsync(PendingProductKey, productId);
                        return NoAdsPurchaseResult.Cancelled;
                    }

                    if (purchase.State == PurchaseState.Purchased)
                    {
                        var acknowledged = await FinalizePurchaseAsync(billing, purchase);
                        if (acknowledged)
                        {
                            await PersistEntitlementAsync(productId);
                            return NoAdsPurchaseResult.Purchased;
                        }
                        // Acknowledge failed — store as pending so next launch retries
                        await SecureStorage.SetAsync(PendingProductKey, productId);
                        return NoAdsPurchaseResult.Failed;
                    }

                    return NoAdsPurchaseResult.Cancelled;
                }
                catch (InAppBillingPurchaseException ex) when (ex.PurchaseError == PurchaseError.AlreadyOwned)
                {
                    await PersistEntitlementAsync(productId);
                    return NoAdsPurchaseResult.AlreadyOwned;
                }
                finally
                {
                    await billing.DisconnectAsync();
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"[NoAds] PurchaseNoAdsAsync error: {ex.Message}");
                return NoAdsPurchaseResult.Failed;
            }
        }

        public async Task<bool> RestoreNoAdsAsync(string productId)
        {
            try
            {
                // Check if there is a pending purchase to finalize first
                var pendingId = await SecureStorage.GetAsync(PendingProductKey);
                if (!string.IsNullOrEmpty(pendingId))
                    return await CheckPendingPurchaseAsync();

                if (!CrossInAppBilling.IsSupported)
                    return false;

                var billing = CrossInAppBilling.Current;
                var connected = await billing.ConnectAsync();
                if (!connected)
                    return false;

                try
                {
                    var purchases = await billing.GetPurchasesAsync(ItemType.InAppPurchase);
                    var match = purchases?.FirstOrDefault(p =>
                        p.ProductId == productId &&
                        (p.State == PurchaseState.Purchased || p.State == PurchaseState.Restored));

                    if (match == null)
                        return false;

                    if (match.State == PurchaseState.Purchased)
                    {
                        // Finalize any unacknowledged purchase found during restore
                        await FinalizePurchaseAsync(billing, match);
                    }

                    await PersistEntitlementAsync(productId);
                    return true;
                }
                finally
                {
                    await billing.DisconnectAsync();
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"[NoAds] RestoreNoAdsAsync error: {ex.Message}");
                return false;
            }
        }

        // Called at app startup to finalize any purchase that was left pending
        // (e.g. parental approval, bank delay, acknowledgement failure).
        public async Task<bool> CheckPendingPurchaseAsync()
        {
            try
            {
                var pendingId = await SecureStorage.GetAsync(PendingProductKey);
                if (string.IsNullOrEmpty(pendingId))
                    return false;

                if (!CrossInAppBilling.IsSupported)
                    return false;

                var billing = CrossInAppBilling.Current;
                var connected = billing.IsConnected || await billing.ConnectAsync();
                if (!connected)
                    return false;

                try
                {
                    var purchases = await billing.GetPurchasesAsync(ItemType.InAppPurchase);
                    var match = purchases?.FirstOrDefault(p => p.ProductId == pendingId);

                    if (match == null)
                    {
                        await SecureStorage.SetAsync(PendingProductKey, string.Empty);
                        return false;
                    }

                    if (match.State == PurchaseState.PaymentPending)
                        return false;

                    if (match.State == PurchaseState.Purchased)
                    {
                        var acknowledged = await FinalizePurchaseAsync(billing, match);
                        if (acknowledged)
                        {
                            await PersistEntitlementAsync(pendingId);
                            return true;
                        }
                    }

                    return false;
                }
                finally
                {
                    await billing.DisconnectAsync();
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"[NoAds] CheckPendingPurchaseAsync error: {ex.Message}");
                return false;
            }
        }

        // Acknowledges the purchase with Google Play / StoreKit.
        // Without this Google Play auto-refunds unacknowledged purchases after 3 days.
        private static async Task<bool> FinalizePurchaseAsync(IInAppBilling billing, InAppBillingPurchase purchase)
        {
            try
            {
                var results = await billing.FinalizePurchaseAsync(
                    new[] { purchase.PurchaseToken },
                    CancellationToken.None);
                var success = results?.All(r => r.Success) == true;
                if (success)
                    await SecureStorage.SetAsync(PendingProductKey, string.Empty);
                return success;
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"[NoAds] FinalizePurchaseAsync error: {ex.Message}");
                return false;
            }
        }

        private async Task<bool> VerifyWithStoreAsync(string productId)
        {
            try
            {
                if (!CrossInAppBilling.IsSupported)
                    return false;

                var billing = CrossInAppBilling.Current;
                var connected = await billing.ConnectAsync();
                if (!connected)
                    return false;

                try
                {
                    var purchases = await billing.GetPurchasesAsync(ItemType.InAppPurchase);
                    return purchases?.Any(p =>
                        (string.IsNullOrEmpty(productId) || p.ProductId == productId) &&
                        (p.State == PurchaseState.Purchased || p.State == PurchaseState.Restored)) ?? false;
                }
                finally
                {
                    await billing.DisconnectAsync();
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"[NoAds] VerifyWithStoreAsync error: {ex.Message}");
                return false;
            }
        }

        private async Task PersistEntitlementAsync(string? productId = null)
        {
            _isNoAdsEnabled = true;
            try
            {
                await SecureStorage.SetAsync(StorageKey, "true");
                if (!string.IsNullOrEmpty(productId))
                    await SecureStorage.SetAsync("no_ads_product_id", productId);
            }
            catch (Exception ex) { Debug.WriteLine($"[NoAds] PersistEntitlementAsync error: {ex.Message}"); }
        }
    }
}
