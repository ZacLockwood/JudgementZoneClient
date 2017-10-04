using System;
using System.Linq;
using JudgementZone.Models;
using Xamarin.Forms;
using JudgementZone.Services;

namespace JudgementZone.UI
{
    public partial class QuestionStatsView : ContentView
    {
        public QuestionStatsView()
        {
            InitializeComponent();
        }

        public void DisplayStats(M_AnswerStats stats)
        {
            NextButton.IsEnabled = false;

            if (stats == null)
            {
                Console.WriteLine("Stats were null");
                //NextButton.IsEnabled = true;
                return;
            }

            if (stats.FocusedPlayerAnswer == null)
            {
                Console.WriteLine("FPA was null");
                //NextButton.IsEnabled = true;
                return;
            }

            if (stats.OtherPlayerAnswers == null)
            {
                Console.WriteLine("OPA were null");
                //NextButton.IsEnabled = true;
                return;
            }

            int RedAnswerCount = stats.OtherPlayerAnswers.Where(pa => pa.PlayerAnswer == 1).Count();
            int YellowAnswerCount = stats.OtherPlayerAnswers.Where(pa => pa.PlayerAnswer == 2).Count();
            int GreenAnswerCount = stats.OtherPlayerAnswers.Where(pa => pa.PlayerAnswer == 3).Count();
            int BlueAnswerCount = stats.OtherPlayerAnswers.Where(pa => pa.PlayerAnswer == 4).Count();

            RedStatsLabel.Text = RedAnswerCount + " Guessed Red";
            YellowStatsLabel.Text = YellowAnswerCount + " Guessed Yellow";
            GreenStatsLabel.Text = GreenAnswerCount + " Guessed Green";
            BlueStatsLabel.Text = BlueAnswerCount + " Guessed Blue";

            RedStatsLabel.Opacity = 0.45;
            YellowStatsLabel.Opacity = 0.45;
            GreenStatsLabel.Opacity = 0.45;
            BlueStatsLabel.Opacity = 0.45;

            RedStatsLabel.FontAttributes = FontAttributes.None;
            YellowStatsLabel.FontAttributes = FontAttributes.None;
			GreenStatsLabel.FontAttributes = FontAttributes.None;
            BlueStatsLabel.FontAttributes = FontAttributes.None;

            switch (stats.FocusedPlayerAnswer.PlayerAnswer)
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

            var myPlayer = S_LocalGameData.Instance.MyPlayer;
			var correctAnswer = stats.FocusedPlayerAnswer.PlayerAnswer;
            if (myPlayer.PlayerId == stats.FocusedPlayerAnswer.PlayerId)
            {
                // Settings for focused player
                var countCorrectGuesses = stats.OtherPlayerAnswers.Where(pa => pa.PlayerAnswer == correctAnswer).Count();

                if (countCorrectGuesses == 0)
                {
                    var closedStrings = new String[] { "Are You a Spy?", "You've Got a Nice Poker Face", "You're Hard to Read", "Are You Hiding Something?" };
                    InfoLabel.Text = closedStrings[new Random().Next(0, closedStrings.Count())];
                }
                else if (countCorrectGuesses == stats.OtherPlayerAnswers.Count())
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
                    InfoLabel.Text = countCorrectGuesses + someStrings[new Random().Next(0, someStrings.Count())];
                }

                NextButton.Text = "End Turn";
                NextButton.IsEnabled = true;
            }
            else
            {
                // Settings for other players
                var myAnswer = stats.OtherPlayerAnswers.Where(pa => pa.PlayerId == myPlayer.PlayerId).FirstOrDefault();
                if (myAnswer == null)
                {
                    throw new Exception("Could not find client player's answer in stats!");
                }

                if (myAnswer.PlayerAnswer == correctAnswer)
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
            S_GameConnector.Connector.SendContinueRequest(S_LocalGameData.Instance.GameKey);
            NextButton.IsEnabled = false;
        }
    }
}
