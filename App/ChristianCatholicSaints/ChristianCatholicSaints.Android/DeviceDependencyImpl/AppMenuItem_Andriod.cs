using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using ChristianCatholicSaints.Droid.DeviceDependencyImpl;
using GeneralInformation.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using WikiExtractor.DbModels;
using Xamarin.Essentials;
using Xamarin.Forms;

[assembly: Dependency(typeof(AppMenuItem_Andriod))]
namespace ChristianCatholicSaints.Droid.DeviceDependencyImpl
{
    public class AppMenuItem_Andriod : IAppMenuItem
    {
        public List<AppMenuItem> AppMenuItems()
        {
            return new List<AppMenuItem>();
        }
    }
}