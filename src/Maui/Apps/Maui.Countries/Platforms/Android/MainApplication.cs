using Android.App;
using Android.Runtime;
using Maui.Countries.Platforms.Android;
using System;

namespace Maui.Countries
{
#if DEBUG
    [Application(UsesCleartextTraffic = true)]
#else
    [Application]
#endif
    public class MainApplication : MauiApplication
    {
        public MainApplication(IntPtr handle, JniHandleOwnership ownership)
            : base(handle, ownership)
        {
        }

        public override void OnCreate()
        {
            base.OnCreate();
            RegisterActivityLifecycleCallbacks(new CurrentActivityHelper());
        }

        protected override MauiApp CreateMauiApp() =>
            MauiProgram.CreateMauiApp();
    }
}
