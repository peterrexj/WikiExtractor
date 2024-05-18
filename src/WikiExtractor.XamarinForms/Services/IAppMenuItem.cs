using System;
using System.Collections.Generic;
using System.Text;
using WikiExtractor.DbModels;

namespace GeneralInformation.Services
{
    public interface IAppMenuItem
    {
        List<AppMenuItem> AppMenuItems();
    }
}
