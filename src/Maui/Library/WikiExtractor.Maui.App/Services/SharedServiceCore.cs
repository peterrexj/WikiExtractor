using System;
using System.IO;
using System.Text.Json;
using System.Threading.Tasks;

namespace WikiExtractor.Maui.App.Services
{
    /// <summary>
    /// Core service for shared functionality across the application.
    /// </summary>
    public static class SharedServiceCore
    {
        /// <summary>
        /// The default theme for the application.
        /// </summary>
        public const AppThemes DefaultAppTheme = AppThemes.Dark;

        /// <summary>
        /// Saves data to a file.
        /// </summary>
        /// <typeparam name="T">The type of data to save.</typeparam>
        /// <param name="data">The data to save.</param>
        /// <returns>A task representing the asynchronous operation.</returns>
        public static async Task SaveData<T>(T data)
        {
            try
            {
                var fileName = GetFileName<T>();
                var filePath = Path.Combine(FileSystem.AppDataDirectory, fileName);
                var json = JsonSerializer.Serialize(data);
                await File.WriteAllTextAsync(filePath, json);
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error saving data: {ex.Message}");
            }
        }

        /// <summary>
        /// Loads data from a file.
        /// </summary>
        /// <typeparam name="T">The type of data to load.</typeparam>
        /// <returns>The loaded data or default if the file does not exist.</returns>
        public static async Task<T?> LoadDataFile<T>() where T : class
        {
            try
            {
                var fileName = GetFileName<T>();
                var filePath = Path.Combine(FileSystem.AppDataDirectory, fileName);

                if (!File.Exists(filePath))
                {
                    return null;
                }

                var json = await File.ReadAllTextAsync(filePath);
                return JsonSerializer.Deserialize<T>(json);
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error loading data: {ex.Message}");
                return null;
            }
        }

        /// <summary>
        /// Gets the file name for a type.
        /// </summary>
        /// <typeparam name="T">The type to get the file name for.</typeparam>
        /// <returns>The file name.</returns>
        private static string GetFileName<T>()
        {
            return $"{typeof(T).Name}.json";
        }
    }
}