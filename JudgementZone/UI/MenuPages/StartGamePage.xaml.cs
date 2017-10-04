using System;
using JudgementZone.Models;
using JudgementZone.Services;
using Xamarin.Forms;

namespace JudgementZone.UI
{
    public partial class StartGamePage : ContentPage
    {

        #region Constructor

        public StartGamePage()
        {
            InitializeComponent();

            var myPlayer = S_LocalGameData.Instance.MyPlayer;
            if (myPlayer != null && !String.IsNullOrWhiteSpace(myPlayer.PlayerName))
            {
                UsernameEntryField.Text = myPlayer.PlayerName;
            }

            if (!S_GameConnector.Connector.IsConnected())
            {
                CreateNewGameButton.Text = "Connecting...";
                Device.BeginInvokeOnMainThread(async () =>
                {
                    Console.WriteLine("STARTED CONNECTING...");
                    await S_GameConnector.Connector.StartConnectionAsync();

                    if (S_GameConnector.Connector.IsConnected())
                    {
                        CreateNewGameButton.Text = "Create New Game";
                        Console.WriteLine("DONE CONNECTING!");
                    }
                    else
                    {
                        CreateNewGameButton.Text = "CONNECTION ERROR";
                        Console.WriteLine("CONNECTION ERROR");
                    }
                });
            }
            else
            {
                CreateNewGameButton.Text = "Create New Game";
            }
        }

		#endregion

		#region View Lifecycle

		protected override void OnAppearing()
		{
			SetupSignalRSubscriptions();
        }

		protected override void OnDisappearing()
		{
			ReleaseSignalRSubscriptions();
		}

		#endregion

		#region Button Handlers

		private void CreateNewGameButtonClicked(object sender, EventArgs e)
        {
            if (String.IsNullOrWhiteSpace(UsernameEntryField.Text))
            {
                Device.BeginInvokeOnMainThread(async () =>
                {
                    await DisplayAlert("Invalid Username", "Please enter a non-empty username in the text box. Any amount of text will do!", "OK");
                });
                return;
            }

            if (S_GameConnector.Connector.IsConnected())
            {
                // Get and store player
                var myPlayer = S_LocalGameData.Instance.MyPlayer;
                if (myPlayer == null || String.IsNullOrWhiteSpace(myPlayer.PlayerId))
                {
                    myPlayer = new M_Player()
                    {
                        PlayerName = UsernameEntryField.Text
                    };
                    S_LocalGameData.Instance.MyPlayer = myPlayer;
                }
                else
                {
                    myPlayer.PlayerName = UsernameEntryField.Text;
                }

                S_GameConnector.Connector.SendNewGameRequest(myPlayer);
            }
        }

        #endregion

        #region SignalR Responders

        private void SetupSignalRSubscriptions()
        {
            MessagingCenter.Subscribe(this, "gameKeyReceived", (S_GameConnector sender) =>
            {
                Device.BeginInvokeOnMainThread(async () => {
					await Navigation.PushAsync(new GameLobbyPage());
                });
            });
        }

        private void ReleaseSignalRSubscriptions()
        {
            MessagingCenter.Unsubscribe<S_GameConnector, string>(this, "gameKeyReceived");
        }

        #endregion
    }
}