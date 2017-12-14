using System;
using Realms;
using System.Linq;
using JudgementZone.Models;
using JudgementZone.Services;
using Xamarin.Forms;

namespace JudgementZone.UI
{
    public partial class JoinGamePage : ContentPage
    {

        private IDisposable RealmGameStateListenerToken;

        #region Constructor

        public JoinGamePage()
        {
            InitializeComponent();

            // Default to the name from Facebook
            UsernameEntryField.Text = S_GameConnector.Connector.fbUsername;

            var myPlayer = Realm.GetInstance("MyPlayerData.Realm").All<M_Player>().FirstOrDefault();
            if (myPlayer != null && !String.IsNullOrWhiteSpace(myPlayer.PlayerName))
            {
                UsernameEntryField.Text = myPlayer.PlayerName;
            }

            var gameState = Realm.GetInstance("GameState.Realm").All<M_Client_GameState>().FirstOrDefault();
            if (gameState != null && !String.IsNullOrWhiteSpace(gameState.GameKey))
            {
                GameKeyEntryField.Text = gameState.GameKey;
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
            SetupRealmSubscriptions();
        }

        protected override void OnDisappearing()
        {
            ReleaseRealmSubscriptions();
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
                var playerRealm = Realm.GetInstance("MyPlayerData.Realm");
                var myPlayer = playerRealm.All<M_Player>().FirstOrDefault();
                if (myPlayer == null || String.IsNullOrWhiteSpace(myPlayer.PlayerId))
                {
                    myPlayer = new M_Player()
                    {
                        PlayerName = UsernameEntryField.Text
                    };
                    playerRealm.Write(() =>
                    {
                        playerRealm.Add(myPlayer, true);
                    });
                }
                else
                {
                    playerRealm.Write(() =>
                    {
                        myPlayer.PlayerName = UsernameEntryField.Text;
                    });
                }

                // Get and store gameKey
                var gameKey = GameKeyEntryField.Text;

                S_GameConnector.Connector.SendJoinGameRequest(myPlayer, gameKey);
            }
        }

		#endregion

		#region SignalR Responders

		private void SetupRealmSubscriptions()
		{
			var gameStateRealm = Realm.GetInstance("GameState.Realm");
            RealmGameStateListenerToken = gameStateRealm.All<M_Client_GameState>().SubscribeForNotifications((sender, changes, errors) =>
            {
                var gameState = sender.FirstOrDefault();
                if (gameState == null)
                {
                    return;
                }

                switch(gameState.ClientViewCode)
                {
                    case 1:
                        // WAITING FOR GAME START
                        Device.BeginInvokeOnMainThread(async () => {
                            ReleaseRealmSubscriptions();
                            await Navigation.PushAsync(new GameLobbyPage(gameState));
                        });
                        break;
                    case 2:
                        // DISPLAY QUESTION
                        break;
                    case 3:
                        // DISPLAY QUESTION STATS
                        break;
                    case 4:
                        // DISPLAY GAME STATS
                        break;
                }
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
