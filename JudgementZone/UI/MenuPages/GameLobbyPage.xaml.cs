using System;
using System.Collections.Generic;
using Realms;
using JudgementZone.Models;
using JudgementZone.Services;
using Xamarin.Forms;
using System.Linq;

namespace JudgementZone.UI
{
    public partial class GameLobbyPage : ContentPage
    {

        public IDisposable RealmGameStateListenerToken { get; private set; }

        private bool _uiLock;

        #region Constructor

        public GameLobbyPage(M_ClientGameState gameState)
        {
            InitializeComponent();

            if (gameState != null)
            {
                if (!String.IsNullOrWhiteSpace(gameState.GameKey))
				{
                    GameCodeLabel.Text = "Game Key\n" + gameState.GameKey;
				}
                DisplayPlayerList(gameState.PlayerList);
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

		#region UI Helper Methods

		private void DisplayPlayerList(IList<M_Player> playerList)
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

        #region Button Handler

        private void StartGameButtonClicked(object sender, EventArgs e)
        {
            Device.BeginInvokeOnMainThread(async () => {
                if (!_uiLock)
                {
                    _uiLock = true;

                    var gameStateRealm = Realm.GetInstance("GameState.Realm");
                    var gameState = gameStateRealm.All<M_ClientGameState>().FirstOrDefault();

                    if (gameState == null)
                    {
                        _uiLock = false;
                        return;
                    }

                    var gameKey = gameState.GameKey;

					if (String.IsNullOrWhiteSpace(gameKey))
					{
						Console.WriteLine("ERROR WITH GAMEKEY");
                        _uiLock = false;
						return;
					}

                    var myPlayerDataRealm = Realm.GetInstance("MyPlayerData.Realm");
                    var myPlayer = myPlayerDataRealm.All<M_Player>().FirstOrDefault();
					if (myPlayer == null || String.IsNullOrWhiteSpace(myPlayer.PlayerName))
					{
						Console.WriteLine("ERROR WITH PLAYER");
                        _uiLock = false;
						return;
					}

                    if (!S_GameConnector.Connector.IsConnected())
                    {
                        // HACK
                        await DisplayAlert("Connection Error", "You have disconnected from the server.", "OK");
                    }

                    await S_GameConnector.Connector.SendGameStartRequest(gameKey);

					await Navigation.PushModalAsync(new GamePage());

                    _uiLock = false;
                }
            });
        }

		#endregion

		#region SignalR Responders

		private void SetupSignalRSubscriptions()
		{
            var gameStateRealm = Realm.GetInstance("GameState.Realm");
            var gameState = gameStateRealm.All<M_ClientGameState>().FirstOrDefault();
            gameState.PlayerList.SubscribeForNotifications((sender, changes, errors) =>
            {
                Device.BeginInvokeOnMainThread(() =>
                {
					DisplayPlayerList(sender.ToList());
                });
            });
		}

        private void ReleaseSignalRSubscriptions()
		{
            if (RealmGameStateListenerToken != null)
            {
				RealmGameStateListenerToken.Dispose();
                RealmGameStateListenerToken = null;
            }
		}

		#endregion

	}
}