using System.Diagnostics;
using WikiExtractor.Maui.App.Services;

namespace WikiExtractor.Maui.App.Services
{
    public class SecureStorageService : ISecureStorageService
    {
        public async Task<string> GetAsync(string key)
        {
            try
            {
                return await SecureStorage.GetAsync(key);
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Error retrieving from secure storage: {ex.Message}");
                return null;
            }
        }

        public async Task<bool> SetAsync(string key, string value)
        {
            try
            {
                await SecureStorage.SetAsync(key, value);
                return true;
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Error saving to secure storage: {ex.Message}");
                return false;
            }
        }

        public bool Remove(string key)
        {
            try
            {
                SecureStorage.Remove(key);
                return true;
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Error removing from secure storage: {ex.Message}");
                return false;
            }
        }

        public void RemoveAll()
        {
            try
            {
                SecureStorage.RemoveAll();
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Error removing all from secure storage: {ex.Message}");
            }
        }
    }
}