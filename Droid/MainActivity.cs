using Android.App;
using Android.Content.PM;
using Android.OS;

using Microsoft.WindowsAzure.MobileServices;
using System.Threading.Tasks;
using JudgementZone.Interfaces;
using System;
using JudgementZone;

namespace JudgementZone.Droid
{
    [Activity(Label = "JudgementZone.Droid", Icon = "@drawable/icon", Theme = "@style/MyTheme", MainLauncher = true, ConfigurationChanges = ConfigChanges.ScreenSize | ConfigChanges.Orientation)]
    public class MainActivity : global::Xamarin.Forms.Platform.Android.FormsAppCompatActivity, IAuthenticate
    {
        // Define a authenticated user.
        private MobileServiceUser user;

        protected override void OnCreate(Bundle bundle)
        {
            TabLayoutResource = Resource.Layout.Tabbar;
            ToolbarResource = Resource.Layout.Toolbar;

            base.OnCreate(bundle);

            global::Xamarin.Forms.Forms.Init(this, bundle);

            LoadApplication(new App());
        }

        public async Task<bool> Authenticate()
        {
            var success = false;
            var message = string.Empty;
            var curClient = new MobileServiceClient();
            try
            {
                // Sign in with Facebook login using a server-managed flow.

                user = await curClient.LoginAsync(this,
                    MobileServiceAuthenticationProvider.Facebook, "{url_scheme_of_your_app}");
                if (user != null)
                {
                    message = string.Format("you are now signed-in as {0}.",
                        user.UserId);
                    success = true;
                }
            }
            catch (Exception ex)
            {
                message = ex.Message;
            }

            // Display the success or failure message.
            AlertDialog.Builder builder = new AlertDialog.Builder(this);
            builder.SetMessage(message);
            builder.SetTitle("Sign-in result");
            builder.Create().Show();

            return success;
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
