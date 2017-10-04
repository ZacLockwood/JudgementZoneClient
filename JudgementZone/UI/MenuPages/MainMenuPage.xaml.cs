using System;
using System.Threading.Tasks;
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
                // Check if should animate
                var animationEnabled = !MenuLogo.IsAnimating;
                if (animationEnabled)
                {
					MenuLogo.IsAnimating = true;
                }

                // Check if already connected
                if (S_GameConnector.Connector.IsConnected())
                {
                    if (animationEnabled)
                    {
                        Console.WriteLine("ANIMATE SWAP");
                        await MenuLogo.AnimateColorSwapAsync();
                        MenuLogo.IsAnimating = false;
                    }
                }
                else
                {
                    var firstAnimComplete = false;
                    while (!S_GameConnector.Connector.IsConnected())
                    {
                        // Run this at beginning of loop so that connection can not restore during alert,
                        // thereby skipping the callback animation and leaving MenuLogo in a permenant unusable state
                        if (firstAnimComplete)
                        {
							Console.WriteLine("CONNECTION ERROR");
							await DisplayAlert("Connection Error", "Could not connect to server. Press ok to try again.", "OK");
                        }

                        Console.WriteLine("Attempting to Connect...");
                        var connectionTask = S_GameConnector.Connector.StartConnectionAsync();

                        if (animationEnabled && !firstAnimComplete)
                        {
                            Console.WriteLine("ANIMATE UNLOAD");
                            var animTask = MenuLogo.AnimateColorLoadToZeroAsync(0.975, 300);
                            await animTask;
                            firstAnimComplete = true;
                            await connectionTask;
                        }
                        else
                        {
                            await connectionTask;
                        }

                        if (S_GameConnector.Connector.IsConnected())
                        {
                            Console.WriteLine("CONNECTED");
                            Console.WriteLine("ANIMATE RELOAD");
                            await MenuLogo.AnimateColorLoadFromZeroAsync(E_LogoColor.Random, 0.975, 300);
                            MenuLogo.IsAnimating = false;
                        }
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
						if (!MenuLogo.IsAnimating) {
							MenuLogo.IsAnimating = true;
							await MenuLogo.AnimateColorFadeAsync(crossFade: true, pulseScale: 0.985, duration: 200);
							MenuLogo.IsAnimating = false;
						}
						await Navigation.PushAsync(new StartGamePage());
					}
					else
					{
						await DisplayAlert("Connecting to Server", "Please be patient while we connect you to the server.", "OK");
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
						await DisplayAlert("Connecting to Server", "Please be patient while we connect you to the server.", "OK");
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
					if (S_GameConnector.Connector.IsConnected())
					{
						await DisplayAlert("Achievements Not Implemented", "Try again in a later build.", "OK");
					}
					else
					{
						await DisplayAlert("Connecting to Server", "Please be patient while we connect you to the server.", "OK");
					}
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
					if (S_GameConnector.Connector.IsConnected())
					{
						await DisplayAlert("Settings Not Implemented", "Try again in a later build.", "OK");
					}
					else
					{
						await DisplayAlert("Connecting to Server", "Please be patient while we connect you to the server.", "OK");
					}
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
					if (S_GameConnector.Connector.IsConnected())
					{
						await DisplayAlert("About Not Implemented", "Try again in a later build.", "OK");
					}
					else
					{
						await DisplayAlert("Connecting to Server", "Please be patient while we connect you to the server.", "OK");
					}
                    _uiLock = false;
                }
			});
		}

        #endregion

    }
}