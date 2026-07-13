namespace WikiExtractor.Maui.App.Services
{
    // Stub implementation — Plugin.InAppBilling removed for app store submission.
    // Full implementation is preserved in git history; re-enable by restoring the
    // Plugin.InAppBilling package reference in all .csproj files and replacing this file.
    public class NoAdsService : INoAdsService
    {
        public bool IsNoAdsEnabled => false;

        public Task<bool> LoadLocalEntitlementAsync() => Task.FromResult(false);
        public Task<bool> LoadEntitlementAsync() => Task.FromResult(false);
        public Task<NoAdsPurchaseResult> PurchaseNoAdsAsync(string productId) => Task.FromResult(NoAdsPurchaseResult.Failed);
        public Task<bool> RestoreNoAdsAsync(string productId) => Task.FromResult(false);
        public Task<bool> CheckPendingPurchaseAsync() => Task.FromResult(false);
    }
}
