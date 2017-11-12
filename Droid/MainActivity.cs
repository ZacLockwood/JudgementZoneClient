using Android.App;
using Android.Content.PM;
using Android.OS;

//Needed for user authentication
using Microsoft.WindowsAzure.MobileServices;
using System.Threading.Tasks;
using JudgementZone.Interfaces;
using System;
using JudgementZone.Services;

//Needed for user authentication
using System.Net.Http;
using Xamarin.Forms;

namespace JudgementZone.Droid
{
    [Activity(Label = "JudgementZone.Droid", Icon = "@drawable/icon", Theme = "@style/MyTheme", MainLauncher = true, ConfigurationChanges = ConfigChanges.ScreenSize | ConfigChanges.Orientation)]
    public class MainActivity : global::Xamarin.Forms.Platform.Android.FormsAppCompatActivity, IAuthenticate
    {
        // Define an authenticated user.
        private MobileServiceUser user;

        protected override void OnCreate(Bundle bundle)
        {
            TabLayoutResource = Resource.Layout.Tabbar;
            ToolbarResource = Resource.Layout.Toolbar;

            base.OnCreate(bundle);

            global::Xamarin.Forms.Forms.Init(this, bundle);

            // Initialize the authenticator before loading the app.
            App.Init((IAuthenticate)this);

            LoadApplication(new App());
        }

        public async Task<bool> Authenticate()
        {
            var message = string.Empty;
            var success = false;

            try
            {

                MobileServiceClient client = new MobileServiceClient(ServerConstants.SIGNALR_URL);

                while (!success)
                {

                    // Sign in with login using a server-managed flow.
                    user = await client.LoginAsync(this, MobileServiceAuthenticationProvider.Google, "judgementzonedev");

                    if (user != null)
                    {
                        message = string.Format("you are now signed-in as {0}.", user.UserId);
                            success = true;
                    }
                }
            }

            catch (Exception ex)
            {
                message = ex.Message;
            }


            return success;
        }
    }
}
