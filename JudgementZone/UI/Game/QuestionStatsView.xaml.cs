﻿using System;
using System.Linq;
using JudgementZone.Models;
using Xamarin.Forms;
using JudgementZone.Services;
using Realms;
using JudgementZone.Interfaces;

namespace JudgementZone.UI
{
    public partial class QuestionStatsView : ContentView, I_PresentableGameView
    {
        public QuestionStatsView()
        {
            InitializeComponent();
        }

		#region View Presentation Management

		public void Present()
		{
			// Cancel FadeOut if running
			if (this.AnimationIsRunning("FadeOut"))
			{
				this.AbortAnimation("FadeOut");
			}

			// Only animate if not already animating
			// And if not currently presented/visible
			if (!this.AnimationIsRunning("FadeIn") && (Opacity < 1.0 || !IsVisible))
			{
				// Gauruntee start from 0 opacity if not visible
				if (!IsVisible)
				{
					Opacity = 0.0;
				}

				// Gauruntee visibility
				IsVisible = true;

				// Animate
				var startingOpacity = Opacity;
				this.Animate("FadeIn", (percent) =>
				{
					Opacity = startingOpacity + percent * (1.0 - startingOpacity);
				},
					16, 250, Easing.CubicInOut,
					(double percent, bool canceled) =>
					{
						if (!canceled)
						{
                            Device.BeginInvokeOnMainThread(() =>
                            {
    							// Gauruntee 1.0 opacity and enable controls on successful completion
    							Opacity = 1.0;
    							IsEnabled = true;
                            });
						}
					}
				);
			}
			else if (!this.AnimationIsRunning("FadeIn"))
			{
				// Guaruntee full opacity/visibility if animation not running
				// Possibly redundant
				Opacity = 1.0;
				IsVisible = true;
			}
		}

		public void Hide()
		{
			// Disable view controls immediately
			IsEnabled = false;

			// Cancel FadeIn if running
			if (this.AnimationIsRunning("FadeIn"))
			{
				this.AbortAnimation("FadeIn");
			}

			// Only animate if not already animating
			// And if currently visible/presented
			if (!this.AnimationIsRunning("FadeOut") && (Opacity > 0.0 && IsVisible))
			{
				var startingOpacity = Opacity;

				this.Animate("FadeOut", (percent) =>
				{
					Opacity = startingOpacity - percent * startingOpacity;
				},
					16, 250, Easing.CubicInOut,
					(double percent, bool canceled) =>
					{
						if (!canceled)
						{
							IsVisible = false;
						}
					});
			}
			else if (!this.AnimationIsRunning("FadeOut"))
			{
				// Guaruntee 0 opacity/no visibility if animation not running
				// Possibly redundant
				Opacity = 0.0;
				IsVisible = false;
			}
		}

		#endregion

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
                AbsoluteLayout.SetLayoutBounds(AnswerTextAbsoluteLayout, new Rectangle(1.0, 1.0, 0.74, leftOverSpace - spacing));
				AbsoluteLayout.SetLayoutBounds(AnswerStatsAbsoluteLayout, new Rectangle(0.0, 1.0, 0.245, leftOverSpace - spacing));
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
