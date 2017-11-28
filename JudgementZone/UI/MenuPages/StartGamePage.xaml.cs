using System;
using Realms;
using JudgementZone.Models;
using JudgementZone.Services;
using Xamarin.Forms;
using System.Linq;

namespace JudgementZone.UI
{
    public partial class StartGamePage : ContentPage
    {

        private IDisposable RealmGameStateListenerToken;

        #region Constructor

        public StartGamePage()
        {
            InitializeComponent();

            var myPlayerDataRealm = Realm.GetInstance("MyPlayerData.Realm");

            var myPlayer = myPlayerDataRealm.All<M_Player>().FirstOrDefault();
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
			SetupRealmSubscriptions();
        }

		protected override void OnDisappearing()
		{
			ReleaseRealmSubscriptions();
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

                S_GameConnector.Connector.SendNewGameRequest(myPlayer);
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
            Device.BeginInvokeOnMainThread(() =>
            {
				if (RealmGameStateListenerToken != null)
				{
					RealmGameStateListenerToken.Dispose();
					RealmGameStateListenerToken = null;
				}
            });
        }

        #endregion
    }
}