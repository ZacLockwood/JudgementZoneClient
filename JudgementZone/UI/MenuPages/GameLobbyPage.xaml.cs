using System;
using System.Collections.Generic;
using JudgementZone.Models;
using JudgementZone.Services;
using Xamarin.Forms;

namespace JudgementZone.UI
{
    public partial class GameLobbyPage : ContentPage
    {

        private bool _uiLock;

        #region Constructor

        public GameLobbyPage()
        {
            InitializeComponent();

            var gameKey = S_LocalGameData.Instance.GameKey;
            if (!String.IsNullOrWhiteSpace(gameKey))
            {
                GameCodeLabel.Text = "Game Key\n" + gameKey;
            }

            var allPlayers = S_LocalGameData.Instance.PlayersInGame;
            DisplayPlayerList(allPlayers);
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

		#region UI Helper Methods

		private void DisplayPlayerList(List<M_Player> playerList)
		{
            PlayerStackLayout.Children.Clear();
            foreach (M_Player player in playerList)
            {
                AddPlayerToLobbyView(player);
            }
		}

        private void AddPlayerToLobbyView(M_Player newPlayer)
        {
            var playerLabel = new Label()
            {
                VerticalOptions = LayoutOptions.Start,
                HorizontalOptions = LayoutOptions.CenterAndExpand,
                FontSize = Device.GetNamedSize(NamedSize.Large, typeof(Label)),
                FontAttributes = FontAttributes.Bold,
                HorizontalTextAlignment = TextAlignment.Center,
                VerticalTextAlignment = TextAlignment.Center,
                Text = newPlayer.PlayerName,
                TextColor = Color.White,
            };

            PlayerStackLayout.Children.Add(playerLabel);
        }

        #endregion

        #region Button Handlers

        private void StartGameButtonClicked(object sender, EventArgs e)
        {
            Device.BeginInvokeOnMainThread(async () => {
                if (!_uiLock)
                {
                    _uiLock = true;

					var gameKey = S_LocalGameData.Instance.GameKey;
					if (String.IsNullOrWhiteSpace(gameKey))
					{
						Console.WriteLine("ERROR WITH GAMEKEY");
						return;
					}
					
					var myPlayer = S_LocalGameData.Instance.MyPlayer;
					if (myPlayer == null || String.IsNullOrWhiteSpace(myPlayer.PlayerName))
					{
						Console.WriteLine("ERROR WITH PLAYER");
						return;
					}

                    if (!S_GameConnector.Connector.IsConnected())
                    {
                        // HACK
                        await DisplayAlert("Connection Error", "You have disconnected from the server.", "OK");
                    }

                    await S_GameConnector.Connector.SendGameStartRequest(myPlayer, gameKey);

					await Navigation.PushModalAsync(new GamePage());

                    _uiLock = false;
                }

            });
        }

		#endregion

		#region SignalR Responders

		private void SetupSignalRSubscriptions()
		{
            MessagingCenter.Subscribe(this, "playerListReceived", (S_GameConnector sender) =>
			{
				Device.BeginInvokeOnMainThread(() => {
                    var playerList = S_LocalGameData.Instance.PlayersInGame;
                    DisplayPlayerList(playerList);
				});
			});
			Device.BeginInvokeOnMainThread(() => {
				var playerList = S_LocalGameData.Instance.PlayersInGame;
                if (playerList != null)
    				DisplayPlayerList(playerList);
			});
		}

        private void ReleaseSignalRSubscriptions()
		{
			MessagingCenter.Unsubscribe<S_GameConnector>(this, "playerListReceived");
		}

		#endregion

	}
}
