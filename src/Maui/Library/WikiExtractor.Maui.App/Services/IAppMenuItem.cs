using System;
using System.Collections.Generic;
using System.Text;
using WikiExtractor.DbModels;

namespace WikiExtractor.Maui.App.Services
{
    public interface IAppMenuItem
    {
        List<AppMenuItem> AppMenuItems();
    }
}