using System;
using System.Collections.Generic;
using System.Drawing;
using System.Text;

namespace GeneralInformation.Services
{
    public  interface IAppEnvironment
    {
        void SetStatusBarColor(Color color, bool darkStatusBarTint);
    }
}
