using System;
using JudgementZone.Services;
using Xamarin.Forms;

namespace JudgementZone.UI
{
    public partial class LoginPage : ContentPage
    {
        // Singleton Instance Properties
        private static volatile LoginPage instance;
        private static object syncRoot = new Object();
        public static LoginPage page
        {
            get
            {
                if (instance == null)
                {
                    lock (syncRoot)
                    {
                        if (instance == null)
                        {
                            instance = new LoginPage();
                        }
                    }
                }

                return instance;
            }
        }

        #region Constructor

        public LoginPage()
        {
            InitializeComponent();
        }

        #endregion

        #region View Lifecycle

        protected override void OnAppearing()
        {
            Device.BeginInvokeOnMainThread(async () =>
            {
                if (!MenuLogo.IsAnimating)
                {
                    MenuLogo.IsAnimating = true;
                    await MenuLogo.AnimateColorFadeAsync(crossFade: true, pulseScale: 0.985, duration: 300);
                    MenuLogo.IsAnimating = false;
                }
            });
        }

        #endregion

        #region Button Handlers

        void FacebookLoginClicked(object sender, EventArgs e)
        {
            Device.BeginInvokeOnMainThread(async () =>
            {
                if (App.Authenticator != null && !S_GameConnector.Connector.authenticated)
                {
                    FacebookLogin.Text = "Logging in...";

                    var result = await App.Authenticator.Authenticate();

                    S_GameConnector.Connector.authenticated = result;
                }
            });
        }

        public void ConnectAndGoToMenu()
        {
            Device.BeginInvokeOnMainThread(async () =>
            {
                await S_GameConnector.Connector.StartConnectionAsync();

                if (!MenuLogo.IsAnimating)
                {
                    MenuLogo.IsAnimating = true;
                    await MenuLogo.AnimateColorFadeAsync(crossFade: true, pulseScale: 0.985, duration: 300);
                    MenuLogo.IsAnimating = false;
                }

                await Navigation.PushAsync(new MainMenuPage());
            });
        }

        #endregion

    }
}