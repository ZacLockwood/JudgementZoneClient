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

        public GameLobbyPage(M_Client_GameState gameState)
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
			SetupRealmSubscriptions();
		}

        protected override void OnDisappearing()
		{
			ReleaseRealmSubscriptions();
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
            // If the player is ready to start, make their name bold
            var fontStyle = FontAttributes.None;
            if (newPlayer.IsReadyToStart)
            {
                fontStyle = FontAttributes.Bold;
            }

            var playerLabel = new Label()
            {
                VerticalOptions = LayoutOptions.Start,
                HorizontalOptions = LayoutOptions.CenterAndExpand,
                FontSize = Device.GetNamedSize(NamedSize.Large, typeof(Label)),
                FontAttributes = fontStyle,
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
                    var gameState = gameStateRealm.All<M_Client_GameState>().FirstOrDefault();

                    if (gameState == null)
                    {
                        _uiLock = false;
                        return;
                    };

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

                    await S_GameConnector.Connector.SendGameStartRequest(myPlayer, gameKey);

                    ReleaseRealmSubscriptions();

					await Navigation.PushModalAsync(new GamePage());

                    await Navigation.PopToRootAsync();

                    _uiLock = false;
                }
            });
        }

		#endregion

		#region SignalR Responders

        private void SetupRealmSubscriptions()
		{
            var gameStateRealm = Realm.GetInstance("GameState.Realm");
            var gameState = gameStateRealm.All<M_Client_GameState>().FirstOrDefault();
            RealmGameStateListenerToken = gameState.PlayerList.SubscribeForNotifications((sender, changes, errors) =>
            {
                Device.BeginInvokeOnMainThread(() =>
                {
					DisplayPlayerList(sender.ToList());
                });
            });
		}

        private void ReleaseRealmSubscriptions()
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