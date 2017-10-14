using Android.App;
using Android.Content.PM;
using Android.OS;

namespace JudgementZone.Droid
{
    [Activity(Label = "JudgementZone.Droid", Icon = "@drawable/icon", Theme = "@style/MyTheme", MainLauncher = true, ConfigurationChanges = ConfigChanges.ScreenSize | ConfigChanges.Orientation)]
    public class MainActivity : global::Xamarin.Forms.Platform.Android.FormsAppCompatActivity
    {
        protected override void OnCreate(Bundle bundle)
        {
            TabLayoutResource = Resource.Layout.Tabbar;
            ToolbarResource = Resource.Layout.Toolbar;

            base.OnCreate(bundle);

            global::Xamarin.Forms.Forms.Init(this, bundle);

            LoadApplication(new App());
        }

        //public int GetStatusBarHeight()
        //{
        //    int height;

        //    int idStatusBarHeight = Resources.GetIdentifier("status_bar_height", "dimen", "android");
        //    if (idStatusBarHeight > 0)
        //    {
        //        height = Resources.GetDimensionPixelSize(idStatusBarHeight);
        //    }
        //    else
        //    {
        //        height = 0;
        //    }

        //    return height;
        //}
    }
}
