namespace WikiExtractor.Maui.App.Services
{
    public interface INoAdsService
    {
        bool IsNoAdsEnabled { get; }
        Task<bool> LoadLocalEntitlementAsync();
        Task<bool> LoadEntitlementAsync();
        Task<NoAdsPurchaseResult> PurchaseNoAdsAsync(string productId);
        Task<bool> RestoreNoAdsAsync(string productId);
        Task<bool> CheckPendingPurchaseAsync();
    }

    public enum NoAdsPurchaseResult
    {
        Purchased,
        AlreadyOwned,
        Cancelled,
        Failed
    }
}
