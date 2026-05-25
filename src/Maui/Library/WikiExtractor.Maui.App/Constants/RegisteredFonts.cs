namespace WikiExtractor.Maui.App.Constants
{
    /// <summary>
    /// Contains the list of font families registered in the application.
    /// </summary>
    public static class RegisteredFonts
    {
        /// <summary>
        /// Gets the list of all registered font families available in the application.
        /// This should match the fonts configured in MauiProgram.ConfigureFonts().
        /// </summary>
        public static List<string> GetFontFamilies()
        {
            return new List<string>
            {
                "Calibri",
                "Lato",
                "Nunito",
                "Pacifico",
                "Raleway"
            };
        }
    }
}
