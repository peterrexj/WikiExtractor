using Android.App;
using Android.Content.PM;
using Android.OS;
using Android.Runtime;
using Android.Gms.Ads;
using System;

namespace Maui.Wiki
{
	[Activity(Theme = "@style/Maui.SplashTheme", MainLauncher = true, ConfigurationChanges = ConfigChanges.Orientation | ConfigChanges.ScreenSize)]
	public class MainActivity : MauiAppCompatActivity
	{
		protected override void OnCreate(Bundle savedInstanceState)
		{
			base.OnCreate(savedInstanceState);
			// Initialize Google Mobile Ads SDK manually since the auto-init provider is removed
			MobileAds.Initialize(ApplicationContext);
		}
	}
}
