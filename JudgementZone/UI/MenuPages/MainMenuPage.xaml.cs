using System;
using JudgementZone.Services;
using Xamarin.Forms;

namespace JudgementZone.UI
{
    public partial class MainMenuPage : ContentPage
    {

        private bool _uiLock;

        #region Constructor

        public MainMenuPage()
        {
            InitializeComponent();
        }

        #endregion

        #region View Lifecycle

        protected override void OnAppearing()
        {
            Device.BeginInvokeOnMainThread(async () =>
            {
                if (S_GameConnector.Connector.IsConnected())
                {
                    await S_GameConnector.Connector.SendQuestionSyncRequest();
                    await MenuLogo.AnimateColorLoadFromZeroAsync(E_LogoColor.Random, 0.975, 300);
                    MenuLogo.IsAnimating = false;
                }

                // Check if should animate
                var animationEnabled = !MenuLogo.IsAnimating;
                if (animationEnabled)
                {
                    MenuLogo.IsAnimating = true;
                }

                // Check if already connected
                if (S_GameConnector.Connector.IsConnected() || S_GameConnector.Connector.authenticated)
                {
                    if (animationEnabled)
                    {
                        await MenuLogo.AnimateColorSwapAsync();
                        MenuLogo.IsAnimating = false;
                    }
                }
                else
                {
                    var firstAnimComplete = false;
                    ////HACK
                    while (!S_GameConnector.Connector.IsConnected() || !S_GameConnector.Connector.authenticated)
                    {
                        // Run this at beginning of loop so that connection can not restore during alert,
                        // thereby skipping the callback animation and leaving MenuLogo in a permenant unusable state
                        if (firstAnimComplete)
                        {
                            Console.WriteLine("CONNECTION ERROR");
                            await DisplayAlert("Connection Error", "Could not connect to server. Press ok to try again.", "OK");
                        }

                        if (animationEnabled && !firstAnimComplete)
                        {
                            var animTask = MenuLogo.AnimateColorLoadToZeroAsync(0.975, 300);
                            await animTask;
                            firstAnimComplete = true;
                        }

                        //if (S_GameConnector.Connector.IsConnected())
                        //{
                        //    await S_GameConnector.Connector.SendQuestionSyncRequest();
                        //    await MenuLogo.AnimateColorLoadFromZeroAsync(E_LogoColor.Random, 0.975, 300);
                        //    MenuLogo.IsAnimating = false;
                        //}
                    }
                }
            });
        }

        #endregion

        #region Button Handlers

        void CreateGameButtonClicked(object sender, EventArgs e)
        {
            Device.BeginInvokeOnMainThread(async () =>
            {
                if (!_uiLock)
                {
                    _uiLock = true;
                    if (S_GameConnector.Connector.IsConnected())
                    {
                        if (!MenuLogo.IsAnimating)
                        {
                            MenuLogo.IsAnimating = true;
                            await MenuLogo.AnimateColorFadeAsync(crossFade: true, pulseScale: 0.985, duration: 200);
                            MenuLogo.IsAnimating = false;
                        }
                        await Navigation.PushAsync(new StartGamePage());
                    }
                    else
                    {
                        if (S_GameConnector.Connector.authenticated)
                        {
                            await DisplayAlert("Authenticated", "Successfully authenticated but couldn't connect to the server.", "OK");
                        }
                    }
                    _uiLock = false;
                }
            });
        }

        void JoinGameButtonClicked(object sender, EventArgs e)
        {
            Device.BeginInvokeOnMainThread(async () =>
            {
                if (!_uiLock)
                {
                    _uiLock = true;
                    if (S_GameConnector.Connector.IsConnected())
                    {
                        if (!MenuLogo.IsAnimating)
                        {
                            MenuLogo.IsAnimating = true;
                            await MenuLogo.AnimateColorFadeAsync(crossFade: true, pulseScale: 0.985, duration: 200);
                            MenuLogo.IsAnimating = false;
                        }
                        await Navigation.PushAsync(new JoinGamePage());
                    }
                    else
                    {
                        if (S_GameConnector.Connector.authenticated)
                        {
                            await DisplayAlert("Authenticated", "Successfully authenticated but couldn't connect to the server.", "OK");
                        }
                    }
                    _uiLock = false;
                }
            });
        }

        void AchievementsButtonClicked(object sender, EventArgs e)
        {
            Device.BeginInvokeOnMainThread(async () =>
            {
                if (!_uiLock)
                {
                    _uiLock = true;
                    await DisplayAlert("Achievements Not Implemented", "Try again in a later build.", "OK");
                    _uiLock = false;
                }
            });
        }

        void SettingsButtonClicked(object sender, EventArgs e)
        {
            Device.BeginInvokeOnMainThread(async () =>
            {
                if (!_uiLock)
                {
                    _uiLock = true;
                    await DisplayAlert("Settings Not Implemented", "Try again in a later build.", "OK");
                    _uiLock = false;
                }
            });
        }

        void AboutButtonClicked(object sender, EventArgs e)
        {
            Device.BeginInvokeOnMainThread(async () =>
            {
                if (!_uiLock)
                {
                    _uiLock = true;
                    await DisplayAlert("About Not Implemented", "Try again in a later build.", "OK");
                    _uiLock = false;
                }
            });
        }

        #endregion

    }
}