using System;
using JudgementZone.Models;
using JudgementZone.Services;
using Xamarin.Forms;

namespace JudgementZone.UI
{
    public partial class JoinGamePage : ContentPage
    {

        #region Constructor

        public JoinGamePage()
        {
            InitializeComponent();

            var myPlayer = S_LocalGameData.Instance.MyPlayer;
            if (myPlayer != null && !String.IsNullOrWhiteSpace(myPlayer.PlayerName))
            {
                UsernameEntryField.Text = myPlayer.PlayerName;
            }

            var gameKey = S_LocalGameData.Instance.GameKey;
            if (!String.IsNullOrWhiteSpace(gameKey))
            {
                GameKeyEntryField.Text = gameKey;
            }

            if (!S_GameConnector.Connector.IsConnected())
            {
                JoinGameButton.Text = "Connecting...";
                Device.BeginInvokeOnMainThread(async () =>
                {
                    Console.WriteLine("STARTED CONNECTING...");
                    await S_GameConnector.Connector.StartConnectionAsync();

                    if (S_GameConnector.Connector.IsConnected())
                    {
                        JoinGameButton.Text = "Join Game";
                        Console.WriteLine("DONE CONNECTING!");
                    }
                    else
                    {
                        JoinGameButton.Text = "CONNECTION ERROR";
                        Console.WriteLine("CONNECTION ERROR");
                    }
                });
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

        void JoinGameButtonClicked(object sender, EventArgs e)
        {
			if (String.IsNullOrWhiteSpace(GameKeyEntryField.Text))
			{
				Device.BeginInvokeOnMainThread(async () =>
				{
					await DisplayAlert("Invalid Game Key", "Please enter a game key for an existing game. If you don't have a game key, go create a new game!", "OK");
				});
				return;
			}

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

                // Get and store gameKey
                var gameKey = GameKeyEntryField.Text;
                S_LocalGameData.Instance.GameKey = gameKey;

                S_GameConnector.Connector.SendJoinGameRequest(myPlayer, gameKey);
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
            MessagingCenter.Unsubscribe<S_GameConnector>(this, "gameKeyReceived");
        }

		#endregion

	}
}
