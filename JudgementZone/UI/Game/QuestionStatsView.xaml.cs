using System;
using System.Linq;
using JudgementZone.Models;
using Xamarin.Forms;
using JudgementZone.Services;
using Realms;

namespace JudgementZone.UI
{
    public partial class QuestionStatsView : ContentView
    {
        public QuestionStatsView()
        {
            InitializeComponent();
        }

        public void DisplayStats(M_Client_GameState state)
        {
			NextButton.IsEnabled = false;

			var stats = state.QuestionStats;

            if (stats == null)
            {
                Console.WriteLine("QuestionStatsView: ERROR WITH STATS");
                return;
            }

            RedStatsLabel.Text = stats.RedGuesses + " Guessed Red";
            YellowStatsLabel.Text = stats.YellowGuesses + " Guessed Yellow";
            GreenStatsLabel.Text = stats.GreenGuesses + " Guessed Green";
            BlueStatsLabel.Text = stats.BlueGuesses + " Guessed Blue";

            RedStatsLabel.Opacity = 0.45;
            YellowStatsLabel.Opacity = 0.45;
            GreenStatsLabel.Opacity = 0.45;
            BlueStatsLabel.Opacity = 0.45;

            RedStatsLabel.FontAttributes = FontAttributes.None;
            YellowStatsLabel.FontAttributes = FontAttributes.None;
			GreenStatsLabel.FontAttributes = FontAttributes.None;
            BlueStatsLabel.FontAttributes = FontAttributes.None;

            switch (stats.CorrectAnswerId)
            {
				case 1:
                    RedStatsLabel.Opacity = 1.0;
                    RedStatsLabel.FontAttributes = FontAttributes.Bold;
					break;
				case 2:
                    YellowStatsLabel.Opacity = 1.0;
					YellowStatsLabel.FontAttributes = FontAttributes.Bold;
                    break;
				case 3:
                    GreenStatsLabel.Opacity = 1.0;
					GreenStatsLabel.FontAttributes = FontAttributes.Bold;
					break;
				case 4:
                    BlueStatsLabel.Opacity = 1.0;
					BlueStatsLabel.FontAttributes = FontAttributes.Bold;
					break;
            }

            // HACK
            var myPlayerId = Realm.GetInstance("MyPlayerData.Realm").All<M_Player>().First().PlayerId;
            if (state.FocusedPlayerId == myPlayerId)
            {
                // Settings for focused player
                var countCorrectGuesses = 0;

                switch (stats.CorrectAnswerId)
                {
                    case 1:
                        countCorrectGuesses = stats.RedGuesses;
                        break;
                    case 2:
                        countCorrectGuesses = stats.YellowGuesses;
                        break;
                    case 3:
                        countCorrectGuesses = stats.GreenGuesses;
                        break;
                    case 4:
                        countCorrectGuesses = stats.BlueGuesses;
                        break;

                }


                if (countCorrectGuesses == 0)
                {
                    var closedStrings = new String[] { "Are You a Spy?", "You've Got a Nice Poker Face", "You're Hard to Read", "Are You Hiding Something?" };
                    InfoLabel.Text = closedStrings[new Random().Next(0, closedStrings.Count())];
                }
                else if (countCorrectGuesses == state.PlayerList.Count())
                {
					var openStrings = new String[] { "You are Incredibly Transparent", "People Just Seem to \'Get You\'", "You're an Open Book!", "Was It That Obvious?" };
					InfoLabel.Text = openStrings[new Random().Next(0, openStrings.Count())];
                }
                else if (countCorrectGuesses == 1)
                {
                    var oneStrings = new String[] { "1 Person Guessed Correctly", "At Least Somebody Gets You...", "You Have One Loyal Friend" };
                    InfoLabel.Text = oneStrings[new Random().Next(0, oneStrings.Count())];
                }
                else
                {
                    var someStrings = new String[] { $"{countCorrectGuesses} People Guessed Correctly", $"You Have {countCorrectGuesses} New Friends", $"Guess These {countCorrectGuesses} Are Your Favorite" };
                    InfoLabel.Text = someStrings[new Random().Next(0, someStrings.Count())];
                }

                NextButton.Text = "End Turn";
                NextButton.IsEnabled = true;
            }
            else
            {
                // Settings for other players
                if (stats.IsPlayerCorrect)
                {
                    var winStrings = new String[] { "Correct!", "Nice Job!", "Woohoo!", "Perceptive!", "Killing the Game!" };
                    InfoLabel.Text = winStrings[new Random().Next(0, winStrings.Count())];
                }
                else
                {
                    var loseStrings = new String[] { "Incorrect", "Boo!", "Better Luck Next Time!", "Wrong!", "Nope!" };
                    InfoLabel.Text = loseStrings[new Random().Next(0, loseStrings.Count())];
                }

                NextButton.Text = "Waiting...";
            }
        }

        void NextButtonClicked(object sender, EventArgs e)
        {
            // HACK
            var gameKey = Realm.GetInstance("GameState.Realm").All<M_Client_GameState>().First().GameKey;
            S_GameConnector.Connector.SendContinueRequest(gameKey);
            NextButton.IsEnabled = false;
        }
    }
}
