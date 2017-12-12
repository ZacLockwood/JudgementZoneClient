using Realms;
using Xamarin.Forms;
using JudgementZone.UI;
using JudgementZone.Models;
using System.Linq;
using JudgementZone.Interfaces;

namespace JudgementZone
{
    public partial class App : Application
    {
        public App()
        {
            InitializeComponent();

            MainPage = new NavigationPage(LoginPage.page)
            {
                BarBackgroundColor = Color.Black,
                BarTextColor = Color.White
            };
        }

        protected override void OnStart()
        {
            var gameStateRealm = Realm.GetInstance("GameState.Realm");
            if (gameStateRealm.All<M_Client_GameState>().Any())
            {
                Device.BeginInvokeOnMainThread(async () =>
                {
                    var choice = await MainPage.DisplayAlert("Game in Progess", "Would you like to re-join the game you were just playing?", "Yes!", "Nah.");
                    if (choice)
                    {
                        //REJOIN
                    }
                    else
                    {
                        gameStateRealm.Write(() =>
                        {
                            gameStateRealm.RemoveAll();
                        });
                    }
                });
            }
        }

        protected override void OnSleep()
        {
            // Handle when your app sleeps
        }

        protected override void OnResume()
        {
            // Handle when your app resumes
        }

        #region Helper Methods

        public static IAuthenticate Authenticator { get; private set; }

        public static void Init(IAuthenticate authenticator)
        {
            Authenticator = authenticator;
        }

        #endregion
    }
}