using System;
using System.Collections.Generic;
using System.Linq;

using Foundation;
using ScnViewGestures.Plugin.Forms.iOS.Renderers;
using UIKit;

//Needed for authentication
using Microsoft.WindowsAzure.MobileServices;
using System.Threading.Tasks;
using JudgementZone.Interfaces;
using JudgementZone.Services;

namespace JudgementZone.iOS
{
    [Register("AppDelegate")]
    public partial class AppDelegate : global::Xamarin.Forms.Platform.iOS.FormsApplicationDelegate, IAuthenticate //Needed for authentication
    {
        //Needed for authentication: Defines a user and client
        MobileServiceClient client = new MobileServiceClient(ServerConstants.SERVER_FULL_URL);

        public override bool FinishedLaunching(UIApplication app, NSDictionary options)
        {
            global::Xamarin.Forms.Forms.Init();
            ViewGesturesRenderer.Init();

            //Needed for authentication
            App.Init(this);

            LoadApplication(new App());

            return base.FinishedLaunching(app, options);
        }

        //Needed for authentication
        public async Task<MobileServiceUser> Authenticate()
        {
            var message = string.Empty;
            var success = false;

            MobileServiceUser user = new MobileServiceUser("none");

            try
            {
                while (!success)
                {

                    // Sign in with login using a server-managed flow.
                    user = await client.LoginAsync(UIApplication.SharedApplication.KeyWindow.RootViewController,
                        MobileServiceAuthenticationProvider.Google, ServerConstants.SERVER_URI_SCHEME);

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
                return user;
            }

            UIAlertView avAlert = new UIAlertView("Sign-in result", message, null, "OK", null);
            avAlert.Show();

            return user;
        }

        //Needed for authentication
        public override bool OpenUrl(UIApplication app, NSUrl url, NSDictionary options)
        {
            return client.ResumeWithURL(url);
        }
    }
}
