namespace WikiExtractor.Maui.App.Constants
{
    public static class RegisteredFonts
    {
        public static List<string> GetFontFamilies()
        {
            return new List<string>
            {
                "Lato",           // clean humanist sans-serif — default
                "Nunito",         // rounded, friendly sans-serif
                "Quicksand",      // geometric rounded, modern feel
                "Raleway",        // elegant thin geometric sans-serif
                "Merriweather",   // screen-optimized serif — readable
                "SourceSerif4",   // clean Adobe serif — book-like
                "PlayfairDisplay", // high-contrast editorial serif
                "Pacifico",       // handwritten script — fun/unique
            };
        }

        public static string DefaultFontFamily => "Lato";
    }
}
