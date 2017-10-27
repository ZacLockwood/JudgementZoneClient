using System;
using System.Linq;
using JudgementZone.Models;
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

                    // Submit answer
                    MessagingCenter.Send(this, "AnswerSelected", SelectedAnswerId);

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

        public void UpdateView(M_QuestionCard newQuestionCard, M_Player myPlayer, M_Player focusedPlayer, int questionNum, int maxQuestionNum, int roundNum, int maxRoundNum)
        {
            Device.BeginInvokeOnMainThread(() =>
            {
                // ADD ERROR HANDLING!!!
                // if question null
                // if MP null
                // if FP null

                // Update Indicator Labels
				//RoundNumIndicatorLabel.Text = $"Round {roundNum} / {maxRoundNum}";
                //QuestionNumIndicatorLabel.Text = $"Question {questionNum} / {maxQuestionNum}";
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

                // Set Question/Answer Display Text
                BindingContext = newQuestionCard;

                // Adjust height distribution between QuestionLabel and AnswerButtonsAbsoluteLayout if necessary
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
