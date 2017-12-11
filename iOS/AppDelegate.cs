using System;
using System.Collections.Generic;
using System.Linq;

using Foundation;
using ScnViewGestures.Plugin.Forms.iOS.Renderers;
using UIKit;
using System.Threading.Tasks;
using Xamarin.Auth;
using JudgementZone.Services;
using Newtonsoft.Json;
using JudgementZone.UI;
using Microsoft.WindowsAzure.MobileServices;
using Newtonsoft.Json.Linq;
using System.Text;
using JudgementZone.Interfaces;

namespace JudgementZone.iOS
{
    [Register("AppDelegate")]
    public partial class AppDelegate : global::Xamarin.Forms.Platform.iOS.FormsApplicationDelegate, IAuthenticate
    {
        struct FacebookUser
        {
            public string name { get; set; }
            public string id { get; set; }
        }

        public AccountStore AccountStore { get; private set; }

        // 1 = google, 2 = facebook
        private int loginProvider = 2;

        public override bool FinishedLaunching(UIApplication app, NSDictionary options)
        {
            global::Xamarin.Forms.Forms.Init();
            ViewGesturesRenderer.Init();

            App.Init(this);

            LoadApplication(new App());

            return base.FinishedLaunching(app, options);
        }

        //Needed for authentication
        public async Task<bool> Authenticate()
        {
            // Check if a current Azure credentials are available within the key store
            string savedToken = CheckForCurrentCredentials();

            if (savedToken != null) // If there is a saved token then login using that
            {
                var success = await AzureAuthentication(loginProvider, savedToken);

                if (success)// If authenticated, save Azure credentials, notify user, and proceed to main menu
                {
                    // Save the Azure credentials
                    SaveCredentials(
                        "azure_token",
                        S_GameConnector.Connector.client.CurrentUser.MobileServiceAuthenticationToken,
                        S_GameConnector.Connector.client.CurrentUser.UserId);

                    // Label login as successful
                    S_GameConnector.Connector.authenticated = true;

                    // Pull out the user name and id from Facebook
                    FacebookUser fbUser;
                    using (var httpClient = new System.Net.Http.HttpClient())
                    {
                        var response = await httpClient.GetAsync(new Uri("https://graph.facebook.com/me?access_token=" + savedToken));
                        response.EnsureSuccessStatusCode();
                        var content = response.Content.ReadAsStringAsync().Result;

                        fbUser = JsonConvert.DeserializeObject<FacebookUser>(content);// USE THIS TO SAVE THE NAME OF THE USER
                    }

                    // Display success alert notification
                    //var alertController = UIAlertController.Create("Signed in!", string.Format("You are now signed-in as {0}.", fbUser.name), UIAlertControllerStyle.Alert);
                    //alertController.AddAction(UIAlertAction.Create("OK", UIAlertActionStyle.Default, (action) => Console.WriteLine("OK Clicked.")));
                    //UIViewController.PresentViewController(alertController, true, null);

                    //Android.Widget.Toast toast = Android.Widget.Toast.MakeText(
                    //    this,
                    //    string.Format("You are now signed-in as {0}.", fbUser.name),
                    //    Android.Widget.ToastLength.Short);
                    //toast.Show();

                    LoginPage.page.ConnectAndGoToMenu();
                    return true;
                }
                else
                {
                    // Display success toast notification
                    //Android.Widget.Toast toast = Android.Widget.Toast.MakeText(
                    //    this,
                    //    string.Format("Failed to automatically login."),
                    //    Android.Widget.ToastLength.Short);
                    //toast.Show();

                    return ProviderAuthentication(loginProvider);
                }
            }
            else // If there isn't a valid token then prompt user for login
            {
                return ProviderAuthentication(loginProvider);
            }
        }

        // Checks to see if there are current Azure credentials and a Facebook token
        private string CheckForCurrentCredentials()
        {
            string savedToken = string.Empty;
            var accounts = AccountStore.FindAccountsForService(ServerConstants.ACCOUNT_STORE_LABEL);
            if (accounts != null)
            {
                foreach (var acct in accounts)
                {
                    string tokenString;

                    if (acct.Properties.TryGetValue("azure_token", out tokenString))
                    {
                        if (!IsAzureTokenExpired(tokenString))//THIS METHOD DOESN'T WORK WITH NON-AZURE TOKENS
                        {
                            S_GameConnector.Connector.client.CurrentUser = new MobileServiceUser(acct.Username);
                            S_GameConnector.Connector.client.CurrentUser.MobileServiceAuthenticationToken = tokenString;
                        }
                    }

                    if (acct.Properties.TryGetValue("facebook_token", out tokenString))
                    {
                        savedToken = tokenString;
                    }
                }
            }

            return savedToken;
        }

        // Authenticate with a token provider
        private bool ProviderAuthentication(int loginProvider)
        {
            bool didAuthenticate = false;

            OAuth2Authenticator auth = new OAuth2Authenticator(
                clientId: ServerConstants.FACEBOOK_APP_CLIENTID,
                scope: "",
                authorizeUrl: new Uri("https://m.facebook.com/dialog/oauth/"),
                redirectUrl: new Uri("http://www.facebook.com/connect/login_success.html")
                );

            auth.Completed += async (sender, eventArgs) =>
            {
                if (eventArgs.IsAuthenticated)
                {
                    var values = eventArgs.Account.Properties;
                    var accessToken = values["access_token"];

                    // Pull out the user name and id from Facebook
                    FacebookUser fbUser;
                    using (var httpClient = new System.Net.Http.HttpClient())
                    {
                        var response = await httpClient.GetAsync(new Uri("https://graph.facebook.com/me?access_token=" + accessToken));
                        response.EnsureSuccessStatusCode();
                        var content = response.Content.ReadAsStringAsync().Result;

                        fbUser = JsonConvert.DeserializeObject<FacebookUser>(content);// USE THIS TO SAVE THE NAME OF THE USER
                    }

                    // Save the provider credentials
                    SaveCredentials(
                        "facebook_token",
                        accessToken,
                        fbUser.id);

                    didAuthenticate = await AzureAuthentication(loginProvider, accessToken);

                    // If authenticated, save Azure credentials, notify user, and proceed to main menu
                    if (didAuthenticate)
                    {
                        // Save the Azure credentials
                        SaveCredentials(
                            "azure_token",
                            S_GameConnector.Connector.client.CurrentUser.MobileServiceAuthenticationToken,
                            S_GameConnector.Connector.client.CurrentUser.UserId);

                        // Label login as successful
                        S_GameConnector.Connector.authenticated = true;

                        // Display success toast notification
                        //Android.Widget.Toast toast = Android.Widget.Toast.MakeText(
                        //    this,
                        //    string.Format("You are now signed-in as {0}.", fbUser.name),
                        //    Android.Widget.ToastLength.Short);
                        //toast.Show();

                        LoginPage.page.ConnectAndGoToMenu();
                    }
                    else
                    {
                        if (!didAuthenticate)
                        {
                            //// Display success toast notification
                            //Android.Widget.Toast toast = Android.Widget.Toast.MakeText(
                            //    this,
                            //    string.Format("Failed to login."),
                            //    Android.Widget.ToastLength.Short);
                            //toast.Show();
                        }
                    }
                }
                else
                {
                    // The user cancelled
                    didAuthenticate = false;
                }
            };

            //StartActivity(auth.GetUI(this));

            UIApplication.SharedApplication.KeyWindow.RootViewController.PresentViewController(auth.GetUI(), true, null);

            //PresentViewController(auth.GetUI(), true, null);

            return didAuthenticate;
        }

        // Login to the Azure App Service with a provider token
        private async Task<bool> AzureAuthentication(int loginProvider, string accessToken)
        {
            // Initialize the token to send to Azure
            var facebookToken = JObject.FromObject(new
            {
                access_token = accessToken
            });

            try
            {
                // Authenticate with the Azure App Service using a client-managed flow
                await S_GameConnector.Connector.client.LoginAsync(MobileServiceAuthenticationProvider.Facebook, facebookToken);
                return true;
            }
            catch (Exception e)
            {
                var msg = e.Message;
                return false;
            }
        }

        // Save the user name and token of the login
        private bool SaveCredentials(string tokenName, string accessToken, string userName)
        {
            try
            {
                // Save the token
                var account = new Account(userName);
                account.Properties.Add(tokenName, accessToken);
                AccountStore.Save(account, ServerConstants.ACCOUNT_STORE_LABEL);

                return true;
            }
            catch (Exception e)
            {
                var msg = e.Message;
                return false;
            }
        }

        // Checks to see if an Azure auth token has expired
        bool IsAzureTokenExpired(string token)
        {
            // Get just the JWT part of the token (without the signature).
            var jwt = token.Split(new Char[] { '.' })[1];

            // Undo the URL encoding.
            jwt = jwt.Replace('-', '+').Replace('_', '/');
            switch (jwt.Length % 4)
            {
                case 0: break;
                case 2: jwt += "=="; break;
                case 3: jwt += "="; break;
                default:
                    throw new ArgumentException("The token is not a valid Base64 string.");
            }

            // Convert to a JSON String
            var bytes = Convert.FromBase64String(jwt);
            string jsonString = UTF8Encoding.UTF8.GetString(bytes, 0, bytes.Length);

            // Parse as JSON object and get the exp field value,
            // which is the expiration date as a JavaScript primative date.
            JObject jsonObj = JObject.Parse(jsonString);
            var exp = Convert.ToDouble(jsonObj["exp"].ToString());

            // Calculate the expiration by adding the exp value (in seconds) to the
            // base date of 1/1/1970.
            DateTime minTime = new DateTime(1970, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc);
            var expire = minTime.AddSeconds(exp);
            return (expire < DateTime.UtcNow);
        }

        // Needed method for iOS sign-in
        public override bool OpenUrl(UIApplication app, NSUrl url, NSDictionary options)
        {
            return S_GameConnector.Connector.client.ResumeWithURL(url);
        }
    }
}
