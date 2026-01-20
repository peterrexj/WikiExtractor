namespace WikiExtractor.Maui.App.Services
{
    public interface ISecureStorageService
    {
        Task<string> GetAsync(string key);
        Task<bool> SetAsync(string key, string value);
        bool Remove(string key);
        void RemoveAll();
    }
}