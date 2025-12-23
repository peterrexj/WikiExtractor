using System;

namespace WikiExtractor.Maui.App.ViewModels
{
    /// <summary>
    /// Interface for view models that support dynamic list height
    /// TODO: Move to proper location when WikiExtractor.Process project is available
    /// </summary>
    public interface IListDynamicHeight
    {
        double ListHeight { get; set; }
    }
}