using System;
using System.Linq;
using JudgementZone.Models;
using JudgementZone.Services;
using ScnViewGestures.Plugin.Forms;
using Xamarin.Forms;

namespace JudgementZone.UI
{
    public partial class QuestionView : ViewGestures
    {
        private bool _rejectTouch = true;
        private bool _isTouching = false;

        private bool _controlsEnabled;
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

		public int SelectedAnswerId { get; private set; }

        #region Constructor

        public QuestionView()
        {
            // Setup for ViewGestures
            TouchBegan += OnTouchBegan;
            TouchBegan += OnTouchMoved;
            Drag += OnTouchMoved;
            TouchEnded += OnTouchMoved;
            TouchEnded += OnTouchUp;

            InitializeComponent();

            BindingContextChanged += (sender, e) =>
            {
                Device.BeginInvokeOnMainThread(() =>
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
						FocusedPlayerLabel.Text = "My Turn!";
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

        #region Touch Gesture Responders

        private void OnTouchBegan(object sender, PositionEventArgs args)
        {
            Device.BeginInvokeOnMainThread(() =>
            {
                _isTouching = true;
                _rejectTouch = !ControlsEnabled;
            });
        }

        private void OnTouchMoved(object sender, PositionEventArgs args)
        {
            Device.BeginInvokeOnMainThread(() =>
            {
                _isTouching = true;

                if (ControlsEnabled && !_rejectTouch)
                {
					var Y = args.PositionY - AnswerButtonsAbsoluteLayout.Y - QuestionAbsoluteLayout.Y;
					var totalAnswerAreaHeight = AnswerButtonsAbsoluteLayout.Height;
					
					if (Y <= 0)
					{
						SetAnswerSelection(0);
					}
					else if (Y <= totalAnswerAreaHeight * 0.25)
					{
						SetAnswerSelection(1);
					}
					else if (Y <= totalAnswerAreaHeight * 0.5)
					{
						SetAnswerSelection(2);
					}
					else if (Y <= totalAnswerAreaHeight * 0.75)
					{
						SetAnswerSelection(3);
					}
					else if (Y <= totalAnswerAreaHeight)
					{
						SetAnswerSelection(4);
					}
                    else
                    {
                        SetAnswerSelection(0);
                    }
                }
            });
        }

        private void OnTouchUp(object sender, PositionEventArgs args)
        {
            Device.BeginInvokeOnMainThread(() =>
            {
                if (ControlsEnabled && !_rejectTouch && SelectedAnswerId >= 1 && SelectedAnswerId <= 4)
                {
                    // Disable controls and animate
                    DisableAnswerControls();

                    // Generate answer submission
                    var myAnswer = new M_PlayerAnswer();
                    myAnswer.PlayerId = S_LocalGameData.Instance.MyPlayer.PlayerId;
                    myAnswer.PlayerAnswer = SelectedAnswerId;
                    var gameKey = S_LocalGameData.Instance.GameKey;
                    myAnswer.GameId = gameKey;

                    // Submit answer
                    S_GameConnector.Connector.SendAnswerSubmission(myAnswer, gameKey);

                    // Not using setter because no animation logic required
                    SelectedAnswerId = 0;
                }

                if (ControlsEnabled)
                {
                    _rejectTouch = false;
                }

                _isTouching = false;
            });
        }

        #endregion

        #region Public View Management

        public void DisplayQuestionCard(M_QuestionCard newQuestionCard)
        {
            Device.BeginInvokeOnMainThread(() =>
            {
                // HACK MAYBE?
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
                RedAnswerButtonFrame.Highlight(animated);
                YellowAnswerButtonFrame.Highlight(animated);
                GreenAnswerButtonFrame.Highlight(animated);
                BlueAnswerButtonFrame.Highlight(animated);
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

        public void SetAnswerSelection(int answerId, bool animated = true)
		{
			Device.BeginInvokeOnMainThread(() =>
			{
                SelectedAnswerId = answerId;

				var frameList = new AnswerButtonFrame[]
				{
					RedAnswerButtonFrame,
					YellowAnswerButtonFrame,
					GreenAnswerButtonFrame,
					BlueAnswerButtonFrame
				};

				if (answerId == 0)
				{
					foreach (var frame in frameList)
					{
                        if (!frame.IsHighlighted)
                        {
							frame.Highlight(animated);
                        }
					}
				}
				else
				{
					foreach (var frame in frameList)
					{
						if (frame.AnswerFrameId == answerId)
						{
							if (!frame.IsHighlighted)
							{
								frame.Highlight(animated);
							}
						}
						else
						{
							if (frame.IsHighlighted)
							{
								frame.UnHighlight(animated);
							}
						}
					}
				}
			});
		}

        #endregion

    }
}
