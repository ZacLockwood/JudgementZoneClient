using System;
using System.Collections.Generic;
using System.Linq;

using Foundation;
using ScnViewGestures.Plugin.Forms.iOS.Renderers;
using UIKit;

using Microsoft.WindowsAzure.MobileServices;
using System.Threading.Tasks;
using JudgementZone.Interfaces;
using JudgementZone.Services;

namespace JudgementZone.iOS
{
    [Register("AppDelegate")]
    public partial class AppDelegate : global::Xamarin.Forms.Platform.iOS.FormsApplicationDelegate, IAuthenticate
    {
        // Define a user and client
        private MobileServiceUser user;
        MobileServiceClient client = new MobileServiceClient(ServerConstants.SERVER_FULL_URL);

        public override bool FinishedLaunching(UIApplication app, NSDictionary options)
        {
            global::Xamarin.Forms.Forms.Init();
            ViewGesturesRenderer.Init();

            App.Init(this);

            LoadApplication(new App());

            return base.FinishedLaunching(app, options);
        }

        public async Task<bool> Authenticate()
        {
            var message = string.Empty;
            var success = false;

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
            }

            UIAlertView avAlert = new UIAlertView("Sign-in result", message, null, "OK", null);
            avAlert.Show();

            return success;
        }

        public override bool OpenUrl(UIApplication app, NSUrl url, NSDictionary options)
        {
            return client.ResumeWithURL(url);
        }
    }
}
