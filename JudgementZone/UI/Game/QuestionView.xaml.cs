using System;
using System.Linq;
using JudgementZone.Models;
using JudgementZone.Services;
using Xamarin.Forms;

namespace JudgementZone.UI
{
    public partial class QuestionView : ContentView
    {

        private bool _controlsEnabled = false;
        public bool ControlsEnabled
        {
            get
            {
                return _controlsEnabled;
            }
            set
            {
                _controlsEnabled = value;
            }
        }

        #region Constructor

        public QuestionView()
        {
            InitializeComponent();
            SetupTapResponder();

            BindingContextChanged += (sender, e) =>
            {
                var focusedPlayer = S_LocalGameData.Instance.FocusedPlayer;
                var myPlayer = S_LocalGameData.Instance.MyPlayer;

                if (myPlayer == null)
                {
                    //HACK
                    return;
                }

                if (focusedPlayer.PlayerId == myPlayer.PlayerId)
                {
                    FocusedPlayerLabel.Text = "Your Turn!";
                }
                else
                {
                    var name = focusedPlayer.PlayerName;
                    if (name.ToCharArray().First().ToString().ToUpper() == name.ToCharArray().First().ToString())
                    {
                        FocusedPlayerLabel.Text = focusedPlayer.PlayerName + "\'s Turn";
                    }
                    else
                    {
                        FocusedPlayerLabel.Text = focusedPlayer.PlayerName + "\'s turn";
                    }
                }

                Device.BeginInvokeOnMainThread(() =>
                {
                    if (QuestionLabel.Height > QuestionAbsoluteLayout.Height * 0.1875)
                    {
                        var leftOverSpace = 1.0 - (QuestionLabel.Height / QuestionAbsoluteLayout.Height);
                        var spacing = leftOverSpace * 0.02;
                        AbsoluteLayout.SetLayoutBounds(AnswerButtonsAbsoluteLayout, new Rectangle(0.5, 1.0, 1.0, leftOverSpace - spacing));
                    }
                    else
                    {
                        AbsoluteLayout.SetLayoutFlags(QuestionLabel, AbsoluteLayoutFlags.All);
                        AbsoluteLayout.SetLayoutBounds(QuestionLabel, new Rectangle(0.5, 0.0, 1.0, 0.1875));
                        AbsoluteLayout.SetLayoutBounds(AnswerButtonsAbsoluteLayout, new Rectangle(0.5, 1.0, 1.0, 0.8));
                    }
                });
            };
        }

        #endregion

        #region Public View Management

        public void DisplayQuestionCard(M_QuestionCard newQuestionCard)
        {
            Device.BeginInvokeOnMainThread(() =>
            {
                AbsoluteLayout.SetLayoutFlags(QuestionLabel, AbsoluteLayoutFlags.PositionProportional | AbsoluteLayoutFlags.WidthProportional);
                AbsoluteLayout.SetLayoutBounds(QuestionLabel, new Rectangle(0.5, 0.0, 1.0, -1.0));
                BindingContext = newQuestionCard;
            });
        }

        public void EnableAnswerControls(bool animated = true)
        {
            Device.BeginInvokeOnMainThread(() =>
            {
                ControlsEnabled = true;
                RedAnswerButtonFrame.Highlight(animated, false);
                YellowAnswerButtonFrame.Highlight(animated, false);
                GreenAnswerButtonFrame.Highlight(animated, false);
                BlueAnswerButtonFrame.Highlight(animated, false);
            });
        }

        public void DisableAnswerControls(bool animated = true)
        {
            Device.BeginInvokeOnMainThread(() =>
            {
                ControlsEnabled = false;
                RedAnswerButtonFrame.UnHighlight(animated);
                YellowAnswerButtonFrame.UnHighlight(animated);
                GreenAnswerButtonFrame.UnHighlight(animated);
                BlueAnswerButtonFrame.UnHighlight(animated);
            });
        }

        #endregion

        #region Private Setup Methods

        private void SetupTapResponder()
        {
            MessagingCenter.Subscribe<AnswerButtonFrame>(this, "AnswerButtonFrameTapped", AnswerButtonFrameTapped);
        }

        #endregion

        #region Button Pressed Responder

        public void AnswerButtonFrameTapped(AnswerButtonFrame sender)
        {
            Device.BeginInvokeOnMainThread(() =>
            {
                if (!ControlsEnabled)
                {
                    return;
                }

				if (sender.IsHighlighted)
				{
					DisableAnswerControls();
					sender.UnHighlight();
					var myAnswer = new M_PlayerAnswer();
					myAnswer.PlayerId = S_LocalGameData.Instance.MyPlayer.PlayerId;
					myAnswer.PlayerAnswer = sender.AnswerFrameId;
					var gameKey = S_LocalGameData.Instance.GameKey;
					myAnswer.GameId = gameKey;
					S_GameConnector.Connector.SendAnswerSubmission(myAnswer, gameKey);
				}
				else
				{
					var frameList = new AnswerButtonFrame[]
					{
						RedAnswerButtonFrame,
						YellowAnswerButtonFrame,
						GreenAnswerButtonFrame,
						BlueAnswerButtonFrame
					};
					
					foreach (var frame in frameList)
					{
						if (frame.AnswerFrameId == sender.AnswerFrameId)
						{
							frame.Highlight();
						}
						else
						{
							frame.UnHighlight();
						}
					}
				}
            });
        }

        #endregion
    }
}
