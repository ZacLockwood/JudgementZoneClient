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

        public void DisplayStats(M_Client_QuestionStats stats, M_QuestionCard focusedQuestion, M_Player myPlayer, M_Player focusedPlayer, int questionNum, int maxQuestionNum, int roundNum, int maxRoundNum)
        {
			NextButton.IsEnabled = false;

            if (stats == null)
            {
                Console.WriteLine("QuestionStatsView: ERROR WITH STATS");
                return;
            }

            if (focusedQuestion == null)
			{
				Console.WriteLine("QuestionStatsView: ERROR WITH FOCUSED QUESTION");
				return;
			}

            // Question/Round Indicators
			RoundNumIndicatorLabel.Text = $"Round {roundNum}";
			QuestionNumIndicatorLabel.Text = $"Question {questionNum}";

			// Player Label Text
			if (focusedPlayer.PlayerId == myPlayer.PlayerId)
			{
				FocusedPlayerLabel.Text = "My Turn!";
			}
			else
			{
				if (focusedPlayer.PlayerName.ToCharArray().First().ToString().ToUpper() == focusedPlayer.PlayerName.ToCharArray().First().ToString())
				{
					FocusedPlayerLabel.Text = focusedPlayer.PlayerName + "\'s Turn";
				}
				else
				{
					FocusedPlayerLabel.Text = focusedPlayer.PlayerName + "\'s turn";
				}
			}

			// Set QuestionLabel properties to auto-size height
			AbsoluteLayout.SetLayoutFlags(QuestionLabel, AbsoluteLayoutFlags.PositionProportional | AbsoluteLayoutFlags.WidthProportional);
			AbsoluteLayout.SetLayoutBounds(QuestionLabel, new Rectangle(0.5, 0.0, 1.0, -1.0));

            // Set Bindings for Question, Answers, and Stats
			QuestionLabel.BindingContext = focusedQuestion;
			AnswerTextAbsoluteLayout.BindingContext = focusedQuestion;
			AnswerStatsAbsoluteLayout.BindingContext = stats;

            ForceLayout();

			// Adjust height distribution between QuestionLabel and Answers/StatsAbsoluteLayout if necessary
			if (QuestionLabel.Height > QuestionAbsoluteLayout.Height * 0.1875)
			{
				var leftOverSpace = 1.0 - (QuestionLabel.Height / QuestionAbsoluteLayout.Height);
				var spacing = leftOverSpace * 0.02;
				AbsoluteLayout.SetLayoutBounds(AnswerStatsAbsoluteLayout, new Rectangle(0.0, 1.0, 0.74, leftOverSpace - spacing));
				AbsoluteLayout.SetLayoutBounds(AnswerTextAbsoluteLayout, new Rectangle(1.0, 1.0, 0.275, leftOverSpace - spacing));
			}
			else
			{
				AbsoluteLayout.SetLayoutFlags(QuestionLabel, AbsoluteLayoutFlags.All);
				AbsoluteLayout.SetLayoutBounds(QuestionLabel, new Rectangle(0.5, 0.0, 1.0, 0.1875));
				AbsoluteLayout.SetLayoutBounds(AnswerTextAbsoluteLayout, new Rectangle(0.0, 1.0, 0.74, 0.8));
				AbsoluteLayout.SetLayoutBounds(AnswerStatsAbsoluteLayout, new Rectangle(1.0, 1.0, 0.245, 0.8));
			}

            // Adjust Correct Answer/Stats Highlight
            AnswerStatsFrameRed.Opacity = 0.45;
			AnswerTextFrameRed.Opacity = 0.45;
            AnswerStatsFrameYellow.Opacity = 0.45;
			AnswerTextFrameYellow.Opacity = 0.45;
            AnswerStatsFrameGreen.Opacity = 0.45;
			AnswerTextFrameGreen.Opacity = 0.45;
            AnswerStatsFrameBlue.Opacity = 0.45;
			AnswerTextFrameBlue.Opacity = 0.45;
            switch (stats.CorrectAnswerId)
            {
				case 1:
                    AnswerStatsFrameRed.Opacity = 1.0;
                    AnswerTextFrameRed.Opacity = 1.0;
					break;
				case 2:
					AnswerStatsFrameYellow.Opacity = 1.0;
					AnswerTextFrameYellow.Opacity = 1.0;
                    break;
				case 3:
					AnswerStatsFrameGreen.Opacity = 1.0;
					AnswerTextFrameGreen.Opacity = 1.0;
					break;
				case 4:
					AnswerStatsFrameBlue.Opacity = 1.0;
					AnswerTextFrameBlue.Opacity = 1.0;
					break;
            }

            // Set End Turn button / Info text based on player focus
            if (focusedPlayer.PlayerId == myPlayer.PlayerId)
            {
				NextButton.Text = "End Turn";
				NextButton.IsEnabled = true;

                // Settings for focused player
                /*
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
                }*/
            }
            else
			{

				NextButton.Text = "Waiting...";

                // Settings for other players
                /*
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
                */
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
