using System;
using Xamarin.Forms;

namespace JudgementZone.UI
{
    public partial class AnswerStatsFrame : Frame
    {
		private const uint HIGHLIGHT_ANIM_LENGTH = 215;
        private const uint UNHIGHLIGHT_ANIM_LENGTH = 215;
        private const double SATURATION_MIN = 0.5;
        private const double SATURATION_MAX = 1.0;
        private const double OPACITY_MIN = 0.475;
        private const double OPACITY_MAX = 1.0;
        private const double SCALE_MIN = 0.985;
        private const double SCALE_STD = 1.0;
        private const double SCALE_MAX = 1.03;

        public int AnswerStatId { get; set; }
        public bool IsHighlighted { get; set; }

        #region Constructor

        public AnswerStatsFrame()
        {
            InitializeComponent();
            AnswerStatsId = answerStatsId;
        }

        #endregion

        #region Public Animation Methods

        public void Highlight(bool animated = true)
        {
            Device.BeginInvokeOnMainThread(() =>
            {
                // Cancel opposing animation
                if (this.AnimationIsRunning("UnhighlightControl"))
                {
                    this.AbortAnimation("UnhighlightControl");
                }

                // Immediately set to highlighted
                IsHighlighted = true;

                // Don't animate if already animating
                if (this.AnimationIsRunning("HighlightControl") && animated)
                {
                    return;
                }

                if (animated)
                {
                    double startingScale = this.Scale;
					double startingOpacity = Opacity;
					double startingSaturation = BackgroundColor.Saturation;
                    bool reachedMiddle = false;
                    this.Animate("HighlightControl",
                        (percent) => {

                            if (percent <= 0.4)
                            {
    							double opacityVal = ConvertScale(startingOpacity, OPACITY_MAX, percent, 0.0, 0.4);
    							double satVal = ConvertScale(startingSaturation, SATURATION_MAX, percent, 0.0, 0.4);
    							Opacity = opacityVal;
    							BackgroundColor = BackgroundColor.WithSaturation(satVal);
                            }
                            else if (!reachedMiddle)
                            {
                                reachedMiddle = true;
                                Opacity = OPACITY_MAX;
                                BackgroundColor = BackgroundColor.WithSaturation(SATURATION_MAX);
                            }
    
                            if (percent < 0.68)
                            {
                                this.Scale = ConvertScale(startingScale, SCALE_MAX, percent, 0.0, 0.68);
                            }
                            else
                            {
                                this.Scale = ConvertScale(SCALE_STD, SCALE_MAX, percent, 0.68, 1.0, true);
                            }
						},
                        16, HIGHLIGHT_ANIM_LENGTH, Easing.SinInOut,
						(double percent, bool canceled) =>
						{
                            if (!canceled)
                            {
                                this.Scale = SCALE_STD;
								Opacity = OPACITY_MAX;
								BackgroundColor = BackgroundColor.WithSaturation(SATURATION_MAX);
    							IsHighlighted = true;
                            }
                            if (canceled)
                            {
                                IsHighlighted = false;
                            }
						});
                }
                else
                {
                    // Instantly set to highlighted
                    // Will override and abort gradual scale if animation is running
                    if (this.AnimationIsRunning("HighlightControl") && animated)
                    {
                        this.AbortAnimation("HightlightControl");
					}

					IsHighlighted = true;

                    Opacity = OPACITY_MAX;
                    BackgroundColor = BackgroundColor.WithSaturation(SATURATION_MAX);
				}
            });
        }

        public void UnHighlight(bool animated = true)
        {
			Device.BeginInvokeOnMainThread(() =>
			{
				// Cancel opposing animation
				if (this.AnimationIsRunning("HighlightControl"))
				{
					this.AbortAnimation("HighlightControl");
				}

                // Immediately set to unhighlighted
				IsHighlighted = false;
                

				// Don't animate if already animating
				if (this.AnimationIsRunning("UnhighlightControl") && animated)
				{
					return;
				}

				if (animated)
				{
					double startingOpacity = Opacity;
					double startingSaturation = BackgroundColor.Saturation;
					double startingScale = this.Scale;
                    bool reachedMiddle = false;
					this.Animate("UnhighlightControl",
						(percent) =>
                        {
                            //HACK REPLACE WITH MATHEMATICAL EASINGS
                            if (percent <= 0.4)
                            {
    							double opacityVal = ConvertScale(OPACITY_MIN, startingOpacity, percent, 0.0, 0.4, true);
    							double satVal = ConvertScale(SATURATION_MIN, startingSaturation, percent, 0.0, 0.4, true);
    							Opacity = opacityVal;
    							BackgroundColor = BackgroundColor.WithSaturation(satVal);
                            }
                            else if (!reachedMiddle)
                            {
                                reachedMiddle = true;
								Opacity = OPACITY_MIN;
								BackgroundColor = BackgroundColor.WithSaturation(SATURATION_MIN);
                            }
                            
    						this.Scale = ConvertScale(SCALE_MIN, startingScale, percent, reverse: true);
						},
                        16, UNHIGHLIGHT_ANIM_LENGTH, Easing.SinInOut,
						(double percent, bool canceled) =>
						{
							if (!canceled)
							{
								this.Scale = SCALE_MIN;
								Opacity = OPACITY_MIN;
								BackgroundColor = BackgroundColor.WithSaturation(SATURATION_MIN);
                                IsHighlighted = false;
							}
							if (canceled)
							{
								IsHighlighted = true;
							}
						});
				}
				else
				{
					// Instantly set to unhighighted
					// Will override and abort gradual scale if animation is running
					if (this.AnimationIsRunning("UnhighlightControl") && animated)
					{
						this.AbortAnimation("UnhightlightControl");
					}

					IsHighlighted = false;

                    Opacity = OPACITY_MIN;
                    BackgroundColor = BackgroundColor.WithSaturation(SATURATION_MIN);
				}
			});
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
