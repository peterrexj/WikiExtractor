using GeneralInformation.Repository;
using GeneralInformation.Services;
using System;
using System.Collections.Generic;
using System.Text;
using Xamarin.Forms;

namespace GeneralInformation.Exts
{
    public static class ThemeHelper
    {
        public static void SetTheme(OSAppTheme? oSAppTheme = null)
        {
            try
            {
                var appEnv = DependencyService.Get<IAppEnvironment>();
                if (oSAppTheme == null)
                {
                    oSAppTheme = DatabaseService.AppDatabase.PhoneSettingsRepository.GetCurrentTheme();
                }

                if (oSAppTheme == OSAppTheme.Dark)
                {
                    //Application.Current.UserAppTheme = OSAppTheme.Dark;
                    appEnv.SetStatusBarColor(Color.Black, false);
                }
                else
                {
                    //Application.Current.UserAppTheme = OSAppTheme.Light;
                    appEnv.SetStatusBarColor(Color.White, true);
                }
            }
            catch (Exception ex)
            {
            }
        }


    }
}
