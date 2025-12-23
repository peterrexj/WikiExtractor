namespace WikiExtractor.Maui.App.Services
{
    /// <summary>
    /// Static helper class for common service access patterns
    /// This provides a more convenient API similar to SharedServices
    /// </summary>
    public static class CustomServices
    {
        /// <summary>
        /// Get Image Service
        /// Usage: var imageService = Services.ImageService;
        /// </summary>
        public static IImageService? ImageService => ServiceLocator.GetService<IImageService>();

        /// <summary>
        /// Get Local Storage Service
        /// Usage: var localStorage = Services.LocalStorage;
        /// </summary>
        public static ILocalStorage? LocalStorage => ServiceLocator.GetService<ILocalStorage>();

        /// <summary>
        /// Get App Information Service
        /// Usage: var appInfo = Services.AppInformation;
        /// </summary>
        public static IAppInformation? AppInformation => ServiceLocator.GetService<IAppInformation>();

        /// <summary>
        /// Get App Menu Item Service
        /// Usage: var menuService = Services.AppMenuItem;
        /// </summary>
        public static IAppMenuItem? AppMenuItem => ServiceLocator.GetService<IAppMenuItem>();

        /// <summary>
        /// Get App Environment Service
        /// Usage: var environment = Services.AppEnvironment;
        /// </summary>
        public static IAppEnvironment? AppEnvironment => ServiceLocator.GetService<IAppEnvironment>();

        /// <summary>
        /// Get Theme Handler Service
        /// Usage: var themeHandler = Services.ThemeHandler;
        /// </summary>
        public static IThemeHandler? ThemeHandler => ServiceLocator.GetService<IThemeHandler>() ?? ServiceLocator.GetServiceFromApp<IThemeHandler>();

        /// <summary>
        /// Generic service getter
        /// Usage: var service = Services.Get<ICustomService>();
        /// </summary>
        public static T? Get<T>() where T : class
        {
            return ServiceLocator.GetService<T>();
        }

        /// <summary>
        /// Generic required service getter
        /// Usage: var service = Services.GetRequired<ICustomService>();
        /// </summary>
        public static T GetRequired<T>() where T : class
        {
            return ServiceLocator.GetRequiredService<T>();
        }
    }
}
