using Microsoft.Maui.Graphics;
using System;
using System.Collections.Generic;
using System.Text;
using WikiExtractor.Maui.App.Models.Mix;

namespace WikiExtractor.Maui.App.Services
{
    public interface IAppEnvironment
    {
        void SetStatusBarColor(Color color, bool darkStatusBarTint);

        IStyleModel GetStyle(AppThemes theme);

        bool DisplayAds { get; }
    }
}