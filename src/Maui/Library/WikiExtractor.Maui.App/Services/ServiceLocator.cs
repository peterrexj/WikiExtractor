using Microsoft.Extensions.DependencyInjection;
using Microsoft.Maui.Controls;

namespace WikiExtractor.Maui.App.Services
{
    /// <summary>
    /// Service locator for dependency injection in MAUI
    /// Provides easy access to registered services throughout the application
    /// Usage: var imageService = ServiceLocator.GetService<IImageService>();
    /// </summary>
    public static class ServiceLocator
    {
        private static IServiceProvider? _serviceProvider;

        /// <summary>
        /// Initialize the service locator with the service provider
        /// This should be called during app startup (typically in MauiProgram.cs)
        /// </summary>
        /// <param name="serviceProvider">The service provider from dependency injection container</param>
        public static void Initialize(IServiceProvider serviceProvider)
        {
            _serviceProvider = serviceProvider;
        }

        /// <summary>
        /// Gets a service of the specified type
        /// </summary>
        /// <typeparam name="T">The type of service to retrieve</typeparam>
        /// <returns>The service instance, or null if not found</returns>
        /// <exception cref="InvalidOperationException">Thrown when ServiceLocator is not initialized</exception>
        public static T? GetService<T>() where T : class
        {
            EnsureInitialized();
            return _serviceProvider!.GetService<T>();
        }

        /// <summary>
        /// Gets a required service of the specified type
        /// </summary>
        /// <typeparam name="T">The type of service to retrieve</typeparam>
        /// <returns>The service instance</returns>
        /// <exception cref="InvalidOperationException">Thrown when ServiceLocator is not initialized or service is not found</exception>
        public static T GetRequiredService<T>() where T : class
        {
            EnsureInitialized();
            return _serviceProvider!.GetRequiredService<T>();
        }

        /// <summary>
        /// Gets a service of the specified type using the current application's service provider
        /// This is a fallback method that tries to get the service provider from the current application
        /// </summary>
        /// <typeparam name="T">The type of service to retrieve</typeparam>
        /// <returns>The service instance, or null if not found</returns>
        public static T? GetServiceFromApp<T>() where T : class
        {
            try
            {
                if (_serviceProvider != null)
                {
                    return _serviceProvider.GetService<T>();
                }

                // Fallback: try to get from current application
                if (Application.Current?.Handler?.MauiContext?.Services != null)
                {
                    return Application.Current.Handler.MauiContext.Services.GetService<T>();
                }

                return null;
            }
            catch
            {
                return null;
            }
        }

        /// <summary>
        /// Gets a required service of the specified type using the current application's service provider
        /// </summary>
        /// <typeparam name="T">The type of service to retrieve</typeparam>
        /// <returns>The service instance</returns>
        /// <exception cref="InvalidOperationException">Thrown when service is not found</exception>
        public static T GetRequiredServiceFromApp<T>() where T : class
        {
            var service = GetServiceFromApp<T>();
            if (service == null)
            {
                throw new InvalidOperationException($"Service of type {typeof(T).Name} is not registered or ServiceLocator is not initialized.");
            }
            return service;
        }

        /// <summary>
        /// Checks if the ServiceLocator has been initialized
        /// </summary>
        public static bool IsInitialized => _serviceProvider != null;

        /// <summary>
        /// Resets the ServiceLocator (useful for testing)
        /// </summary>
        public static void Reset()
        {
            _serviceProvider = null;
        }

        private static void EnsureInitialized()
        {
            if (_serviceProvider == null)
            {
                throw new InvalidOperationException(
                    "ServiceLocator is not initialized. Call ServiceLocator.Initialize(serviceProvider) during app startup.");
            }
        }
    }

    /// <summary>
    /// Extension methods for easier service access
    /// </summary>
    public static class ServiceLocatorExtensions
    {
        /// <summary>
        /// Extension method to get service directly from IServiceProvider
        /// Usage: serviceProvider.GetService<IImageService>();
        /// </summary>
        public static T? GetService<T>(this IServiceProvider serviceProvider) where T : class
        {
            return Microsoft.Extensions.DependencyInjection.ServiceProviderServiceExtensions.GetService<T>(serviceProvider);
        }

        /// <summary>
        /// Extension method to get required service directly from IServiceProvider
        /// Usage: serviceProvider.GetRequiredService<IImageService>();
        /// </summary>
        public static T GetRequiredService<T>(this IServiceProvider serviceProvider) where T : class
        {
            return Microsoft.Extensions.DependencyInjection.ServiceProviderServiceExtensions.GetRequiredService<T>(serviceProvider);
        }
    }
}
