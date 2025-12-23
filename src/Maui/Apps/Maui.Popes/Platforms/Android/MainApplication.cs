using Android.App;
using Android.Runtime;
using System;

namespace Maui.Wiki
{
    [Application]
    public class MainApplication : Android.App.Application
    {
        public MainApplication(IntPtr handle, JniHandleOwnership ownership)
            : base(handle, ownership)
        {
        }

        public override void OnCreate()
        {
            base.OnCreate();
            // Initialize MAUI app
            MauiProgram.CreateMauiApp();
        }
    }
}
