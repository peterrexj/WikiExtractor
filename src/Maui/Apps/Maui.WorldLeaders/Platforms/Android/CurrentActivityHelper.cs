using Android.App;
using Android.OS;
using static Android.App.Application;

namespace Maui.WorldLeaders.Platforms.Android
{
    public class CurrentActivityHelper : Java.Lang.Object, IActivityLifecycleCallbacks
    {
        public static Activity? Activity { get; private set; }

        public void OnActivityCreated(Activity activity, Bundle? savedInstanceState) => Activity = activity;
        public void OnActivityStarted(Activity activity) => Activity = activity;
        public void OnActivityResumed(Activity activity) => Activity = activity;
        public void OnActivityPaused(Activity activity) { }
        public void OnActivityStopped(Activity activity) { }
        public void OnActivitySaveInstanceState(Activity activity, Bundle outState) { }
        public void OnActivityDestroyed(Activity activity) { }
    }
}
