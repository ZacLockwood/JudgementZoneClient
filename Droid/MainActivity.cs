using Android.App;
using Android.Content.PM;
using Android.OS;

//Needed for user authentication
using Microsoft.WindowsAzure.MobileServices;
using System.Threading.Tasks;
using JudgementZone.Interfaces;
using System;
using JudgementZone.Services;

namespace JudgementZone.Droid
{
    [Activity(Label = "JudgementZone.Droid", Icon = "@drawable/icon", Theme = "@style/MyTheme", MainLauncher = true, ConfigurationChanges = ConfigChanges.ScreenSize | ConfigChanges.Orientation)]
    public class MainActivity : global::Xamarin.Forms.Platform.Android.FormsAppCompatActivity, IAuthenticate //Needed for authentication
    {
        //Needed for authentication: Defines a user and client
        private MobileServiceUser user;
        MobileServiceClient client = new MobileServiceClient(ServerConstants.SERVER_FULL_URL);

        protected override void OnCreate(Bundle bundle)
        {
            TabLayoutResource = Resource.Layout.Tabbar;
            ToolbarResource = Resource.Layout.Toolbar;

            base.OnCreate(bundle);

            global::Xamarin.Forms.Forms.Init(this, bundle);

            //Needed for authentication: Initialize the authenticator before loading the app
            App.Init((IAuthenticate)this);

            LoadApplication(new App());
        }

        //Needed for authentication
        public async Task<bool> Authenticate()
        {
            var message = string.Empty;
            var success = false;

            try
            {
                while (!success)
                {
                    // Sign in with login using a server-managed flow
                    user = await client.LoginAsync(this, MobileServiceAuthenticationProvider.Google, ServerConstants.SERVER_URI_SCHEME);

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
