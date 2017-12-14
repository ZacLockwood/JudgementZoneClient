using Android.App;
using Android.Content.PM;
using Android.OS;

using Microsoft.WindowsAzure.MobileServices;
using System.Threading.Tasks;
using JudgementZone.Interfaces;
using System;
using JudgementZone.Services;

using Xamarin.Auth;
using Newtonsoft.Json.Linq;
using System.Text;
using JudgementZone.UI;
using Newtonsoft.Json;

namespace JudgementZone.Droid
{
    [Activity(Label = "Judgement Zone", Icon = "@drawable/icon", Theme = "@style/MyTheme", MainLauncher = true, ScreenOrientation = ScreenOrientation.Portrait, ConfigurationChanges = ConfigChanges.ScreenSize | ConfigChanges.Orientation)]
    public class MainActivity : global::Xamarin.Forms.Platform.Android.FormsAppCompatActivity, IAuthenticate
    {
        struct FacebookUser
        {
            public string name { get; set; }
            public string id { get; set; }
        }

        // 1 = google, 2 = facebook
        private int loginProvider = 2;

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

        //Needed for authentication
        public async Task<bool> Authenticate()
        {
            // Check if a current Azure credentials are available within the key store
            string providerToken = CheckForCurrentCredentials();

            if (providerToken != null) // If there is a saved token then login using that
            {
                var success = await AzureAuthentication(loginProvider, providerToken);

                if (success)// If authenticated, save Azure credentials, notify user, and proceed to main menu
                {
                    // Save the Azure credentials
                    SaveCredentials(
                        tokenName: "azure",
                        accessToken: S_GameConnector.Connector.client.CurrentUser.MobileServiceAuthenticationToken,
                        userName: S_GameConnector.Connector.client.CurrentUser.UserId);

                    // Label login as successful
                    S_GameConnector.Connector.authenticated = true;

                    // Pull out the user name and id from Facebook
                    FacebookUser fbUser = await GetUserData(loginProvider, providerToken);

                    DisplayLoginNotification(fbUser.name, success);

                    // Save out in shared code the user's first name
                    S_GameConnector.Connector.fbUsername = GetFirstName(fbUser.name);

                    LoginPage.page.ConnectAndGoToMenu();
                    return true;
                }
                else
                {
                    DisplayLoginNotification(string.Empty, success);

                    return ProviderAuthentication(loginProvider);
                }
            }
            else // If there isn't a valid token then prompt user for login
            {
                return ProviderAuthentication(loginProvider);
            }
        }

        // Authenticate with a token provider
        private bool ProviderAuthentication(int loginProvider)
        {
            OAuth2Authenticator auth = null;

            bool didAuthenticate = false;

            auth = new OAuth2Authenticator(
                clientId: ServerConstants.FACEBOOK_APP_CLIENTID,
                scope: "",
                authorizeUrl: new Uri("https://m.facebook.com/dialog/oauth/"),
                redirectUrl: new Uri("https://www.facebook.com/connect/login_success.html")
                );

            StartActivity(auth.GetUI(this));

            auth.Completed += async (sender, eventArgs) =>
            {
                if (eventArgs.IsAuthenticated)
                {
                    var values = eventArgs.Account.Properties;
                    var accessToken = values["access_token"];

                    // Pull out the user name and id from Facebook
                    FacebookUser fbUser;// = GetUserData(loginProvider, accessToken).Result;
                    using (var httpClient = new System.Net.Http.HttpClient())
                    {
                        var response = await httpClient.GetAsync(new Uri("https://graph.facebook.com/me?access_token=" + accessToken));
                        response.EnsureSuccessStatusCode();
                        var content = response.Content.ReadAsStringAsync().Result;

                        fbUser = JsonConvert.DeserializeObject<FacebookUser>(content);
                    }

                    // Save the provider credentials
                    SaveCredentials(
                        tokenName: "facebook_token",
                        accessToken: accessToken,
                        userName: fbUser.id);

                    didAuthenticate = await AzureAuthentication(loginProvider, accessToken);

                    // If authenticated, save Azure credentials, notify user, and proceed to main menu
                    if (didAuthenticate)
                    {
                        // Save the Azure credentials
                        SaveCredentials(
                            tokenName: "azure",
                            accessToken: S_GameConnector.Connector.client.CurrentUser.MobileServiceAuthenticationToken,
                            userName: S_GameConnector.Connector.client.CurrentUser.UserId);

                        // Label login as successful
                        S_GameConnector.Connector.authenticated = true;

                        DisplayLoginNotification(fbUser.name, didAuthenticate);

                        LoginPage.page.ConnectAndGoToMenu();
                    }
                    else
                    {
                        DisplayLoginNotification(string.Empty, didAuthenticate);
                    }
                }
                else
                {
                    // The user cancelled
                    didAuthenticate = false;
                }
            };

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


        #region Credential storage methods

        // Checks to see if there are current Azure credentials and a Facebook token
        private string CheckForCurrentCredentials()
        {
            string providerToken = null;
            try
            {
                var accounts = AccountStore.Create(this).FindAccountsForService(ServerConstants.ACCOUNT_STORE_LABEL);

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
                            providerToken = tokenString;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                var msg = ex.Message;
            }

            return providerToken;
        }

        // Save the user name and token of the login
        private bool SaveCredentials(string tokenName, string accessToken, string userName)
        {
            try
            {
                // Save the token
                var account = new Account(userName);
                account.Properties.Add(tokenName, accessToken);
                AccountStore.Create(this).Save(account, ServerConstants.ACCOUNT_STORE_LABEL);

                return true;
            }
            catch (Exception e)
            {
                var msg = e.Message;
                return false;
            }
        }

        // Checks to see if an Azure auth token has expired
        private bool IsAzureTokenExpired(string token)
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

        #endregion


        #region Helper methods

        // Use the provider api to pull out user data
        private async Task<FacebookUser> GetUserData(int loginProvider, string providerToken)
        {
            using (var httpClient = new System.Net.Http.HttpClient())
            {
                var response = await httpClient.GetAsync(new Uri("https://graph.facebook.com/me?access_token=" + providerToken));
                response.EnsureSuccessStatusCode();
                var content = response.Content.ReadAsStringAsync().Result;

                return JsonConvert.DeserializeObject<FacebookUser>(content);
            }
        }

        // Displays either a success or failure notification to the user
        private void DisplayLoginNotification(string name, bool success)
        {
            if (success)
            {
                // Display success toast notification
                Android.Widget.Toast toast = Android.Widget.Toast.MakeText(
                    this,
                    string.Format("You are now signed in as {0}.", name),
                    Android.Widget.ToastLength.Short);
                toast.Show();
            }
            else
            {
                // Display failure toast notification
                Android.Widget.Toast toast = Android.Widget.Toast.MakeText(
                        this,
                        string.Format("Failed to login."),
                        Android.Widget.ToastLength.Short);
                toast.Show();
            }
        }

        // Pulls out the first name from the full name
        private string GetFirstName(string name)
        {
            string firstName = string.Empty;

            foreach (char c in name)
            {
                if (!c.Equals(' '))
                {
                    firstName += c;
                }
                else
                {
                    break;
                }
            }

            return firstName;
        }

        #endregion
    }
}
