using System;
using Xamarin.Forms;

namespace JudgementZone.UI
{
    public partial class AnswerButtonFrame : Frame
    {
        private const double SATURATION_MIN = 0.4;
        private const double SATURATION_MAX = 1.0;
        private const double OPACITY_MIN = 0.35;
        private const double OPACITY_MAX = 1.0;
		private const double SCALE_MIN = 0.985;
        private const double SCALE_STD = 1.0;
		private const double SCALE_MAX = 1.05;

        public int AnswerFrameId { get; set; }
        public bool IsHighlighted { get; set; } = false;

        #region Constructor

        public AnswerButtonFrame(int answerFrameId)
        {
            InitializeComponent();
            AnswerFrameId = answerFrameId;
            SetupGestureRecognizer();
        }

        #endregion

        #region Public Animation Methods

        public void FadeToDisabled(bool animated = true)
        {
            Device.BeginInvokeOnMainThread(() =>
            {
                // Cancel opposing animation
                if (this.AnimationIsRunning("FadeInControls"))
                {
                    this.AbortAnimation("FadeInControls");
                }

                // Don't animate if already animating
                if (this.AnimationIsRunning("FadeOutControls") && animated)
                {
                    return;
                }

                if (animated)
                {
                    var startingOpacity = Opacity;
                    var startingSaturation = BackgroundColor.Saturation;
                    this.Animate("FadeOutControls",
                        (percent) =>
                        {
                            var opacityVal = ConvertScale(OPACITY_MIN, startingOpacity, percent, reverse: true);
                            var satVal = ConvertScale(SATURATION_MIN, startingSaturation, percent, reverse: true);
                            Opacity = opacityVal;
                            BackgroundColor = BackgroundColor.WithSaturation(satVal);
                        },
                        16, 250, Easing.CubicInOut,
                        (double percent, bool canceled) =>
                        {
                            if (canceled)
                                Console.WriteLine("FADE OUT CONTROLS CANCELED");
                            else
                                Console.WriteLine("FADE OUT CONTROLS COMPLETED");
                        });
                }
                else
                {
                    // Instantly set opacity and saturation
                    // Will override and abort gradual fade if animation is running
                    if (this.AnimationIsRunning("FadeOutControls"))
                    {
                        this.AbortAnimation("FadeOutControls");
                    }
                    Opacity = OPACITY_MIN;
                    BackgroundColor = BackgroundColor.WithSaturation(SATURATION_MIN);
                }
            });
        }

        public void FadeToEnabled(bool animated = true)
        {
			Device.BeginInvokeOnMainThread(() =>
			{
				// Cancel opposing animation
				if (this.AnimationIsRunning("FadeOutControls"))
				{
					this.AbortAnimation("FadeOutControls");
				}

				// Don't animate if already animating
				if (this.AnimationIsRunning("FadeInControls") && animated)
				{
					return;
				}

				if (animated)
				{
					var startingOpacity = Opacity;
					var startingSaturation = BackgroundColor.Saturation;
					this.Animate("FadeInControls",
						(percent) =>
						{
                            var opacityVal = ConvertScale(startingOpacity, OPACITY_MAX, percent);
                            var satVal = ConvertScale(startingSaturation, SATURATION_MAX, percent);
							Opacity = opacityVal;
							BackgroundColor = BackgroundColor.WithSaturation(satVal);
						},
						16, 250, Easing.CubicInOut,
						(double percent, bool canceled) =>
						{
							if (canceled)
								Console.WriteLine("FADE IN CONTROLS CANCELED");
							else
								Console.WriteLine("FADE IN CONTROLS COMPLETED");
						});
				}
				else
				{
					// Instantly set opacity and saturation
					// Will override and abort gradual fade if animation is running
					if (this.AnimationIsRunning("FadeInControls"))
					{
						this.AbortAnimation("FadeInControls");
					}
                    Opacity = OPACITY_MAX;
                    BackgroundColor = BackgroundColor.WithSaturation(SATURATION_MAX);
				}
			});
        }

        public void Highlight(bool animated = true, bool setHighlighted = true)
        {
            Device.BeginInvokeOnMainThread(() =>
            {
                // Cancel opposing animation
                if (this.AnimationIsRunning("UnhighlightControl"))
                {
                    this.AbortAnimation("UnhighlightControl");
                }

                // Immediately set to highlighted
                if (setHighlighted)
                {
                    IsHighlighted = true;
                }

                // Don't animate if already animating
                if (this.AnimationIsRunning("HighlightControl") && animated)
                {
                    return;
                }

                if (animated)
                {
                    FadeToEnabled();
                    double startingScale = this.Scale;
                    this.Animate("HighlightControl",
                        (percent) => {
                            if (percent < 0.5)
                            {
                                this.Scale = ConvertScale(startingScale, SCALE_MAX, percent, 0.0, 0.5);
                            }
                            else
                            {
                                this.Scale = ConvertScale(SCALE_STD, SCALE_MAX, percent, 0.5, 1.0, true);
                            }
						},
                        16, 500, Easing.CubicInOut,
						(double percent, bool canceled) =>
						{
                            if (!canceled && setHighlighted)
                            {
    							IsHighlighted = true;
                            }
                            if (canceled && setHighlighted)
                            {
                                IsHighlighted = false;
                            }
						});
                }
                else
                {
                    // Instantly set to enabled
                    // Will override and abort gradual scale if animation is running
                    if (this.AnimationIsRunning("HighlightControl") && animated)
                    {
                        this.AbortAnimation("HightlightControl");
					}

                    FadeToEnabled(false);

                    if (setHighlighted)
                    {
						IsHighlighted = true;
                    }
				}
            });
        }

        public void UnHighlight(bool animated = true, bool setHighlighted = true)
        {
			Device.BeginInvokeOnMainThread(() =>
			{
				// Cancel opposing animation
				if (this.AnimationIsRunning("HighlightControl"))
				{
					this.AbortAnimation("HighlightControl");
				}

                // Immediately set to unhighlighted
                if (setHighlighted)
                {
					IsHighlighted = false;
                }

				// Don't animate if already animating
				if (this.AnimationIsRunning("UnhighlightControl") && animated)
				{
					return;
				}

				if (animated)
				{
                    FadeToDisabled();
					double startingScale = this.Scale;
					this.Animate("UnhighlightControl",
						(percent) =>
                        {
                            this.Scale = ConvertScale(SCALE_MIN, startingScale, percent, reverse: true);
						},
                        16, 500, Easing.CubicInOut,
						(double percent, bool canceled) =>
						{
							if (!canceled && setHighlighted)
							{
                                IsHighlighted = false;
							}
							if (canceled && setHighlighted)
							{
								IsHighlighted = true;
							}
						});
				}
				else
				{
					// Instantly set to disabled
					// Will override and abort gradual scale if animation is running
					if (this.AnimationIsRunning("UnhighlightControl") && animated)
					{
						this.AbortAnimation("UnhightlightControl");
					}

                    FadeToDisabled(false);

                    if (setHighlighted)
                    {
						IsHighlighted = false;
                    }
				}
			});
        }

        #endregion

        #region Private Setup Methods

        private void SetupGestureRecognizer()
        {
            var tap = new TapGestureRecognizer();
            tap.Command = new Command(() =>
            {
                MessagingCenter.Send(this, "AnswerButtonFrameTapped");
            }, () => { return true; });
            GestureRecognizers.Add(tap);
        }

        #endregion

        #region Helper Methods

        private double ConvertScale(double outScaleMin, double outScaleMax, double inPoint, double inScaleMin = 0.0, double inScaleMax = 1.0, bool reverse = false)
        {
            // Error handling returns 0.0 for invalid operations
            if (Math.Abs(outScaleMin - outScaleMax) < 0)
                return 0.0;

            if (Math.Abs(inScaleMin - inScaleMax) < 0)
                return 0.0;

            // Error handling clips output value to requested output range
            if (inPoint >= inScaleMax)
                return reverse ? outScaleMin : outScaleMax;

            if (inPoint <= inScaleMin)
                return reverse ? outScaleMax : outScaleMin;

            // Get scale sizes
            double outScaleSize = outScaleMax - outScaleMin;
            double inScaleSize = inScaleMax - inScaleMin;

            // Get standard 0.0 - 1.0 percentage for inPoint in inScale
            double standardPercent = (inPoint - inScaleMin) / inScaleSize;

            // Reverse the scale if requested
            if (reverse)
            {
				standardPercent = 1.0 - standardPercent;
            }

            // Get the output value placed in the output range
            double outPoint = outScaleMin + (standardPercent * outScaleSize);

            return outPoint;
        }

        #endregion
    }
}
