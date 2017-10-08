using System;
using System.Threading;
using System.Threading.Tasks;
using Xamarin.Forms;

namespace JudgementZone.UI
{
  //  public enum E_LogoColor
  //  {
  //      Red = 1,
  //      Yellow,
		//Green,
    //    Blue,
    //    Random,
    //    Current
    //}

    public partial class JZLoaderLogoView : ContentView
    {

        public bool IsAnimating;
        public String Light = "Light";

        private E_LogoColor _lastColor;
        private E_LogoColor _currentColor;
        public E_LogoColor CurrentColor
        {
            get
            {
                return _currentColor;
            }
            set
            {
                if (value == E_LogoColor.Current)
                {
					return;
                }
                
                _lastColor = _currentColor;

                if (value == E_LogoColor.Random)
                {
                    var random = new Random().Next(1, 5);
                    _currentColor = (E_LogoColor)random;
                }
                else
                {
					_currentColor = value;
                }

                ColoredLogo.Source = GetImageSourceForColor(_currentColor);
            }
        }

        private CancellationTokenSource _continuousFadeLoaderCancelationTokenSource = new CancellationTokenSource();

        #region Constructors

        public JZLoaderLogoView()
        {
            InitializeComponent();
            CurrentColor = E_LogoColor.Random;
        }

        public JZLoaderLogoView(E_LogoColor startupColor = E_LogoColor.Random)
        {
			InitializeComponent();

            CurrentColor = startupColor;
        }

        #endregion

        #region View Lifecycle

        protected override SizeRequest OnMeasure(double widthConstraint, double heightConstraint)
        {
            return LogoBackground.Measure(widthConstraint, heightConstraint);
        }

		#endregion

		#region Sychronous Animation Starters

        public void AnimateColorSwap(bool pickRandomColor = true, double pulseScale = 0.98, uint duration = 400, Easing easing = null)
        {
			Device.BeginInvokeOnMainThread(async () =>
            {
                if (!IsAnimating)
                {
					IsAnimating = true;
					await AnimateColorSwapAsync(pickRandomColor, pulseScale, duration, easing);
					IsAnimating = false;
                }
            });
        }

        public void AnimateColorFade(bool crossFade = false, bool pickRandomColor = true, double pulseScale = 0.975, uint duration = 500, Easing easing = null)
        {
			Device.BeginInvokeOnMainThread(async () =>
			{
                if (!IsAnimating)
                {
					IsAnimating = true;
					await AnimateColorFadeAsync(crossFade, pickRandomColor, pulseScale, duration, easing);
					IsAnimating = false;
                }
			});
        }

        public void StartContinuousFadeLoader(bool crossFade = true, bool pickRandomColor = true, double pulseScale = 0.9, uint duration = 1200, Easing easing = null)
		{
			Device.BeginInvokeOnMainThread(async () =>
			{
				if (!IsAnimating)
				{
					_continuousFadeLoaderCancelationTokenSource.Dispose();
					_continuousFadeLoaderCancelationTokenSource = new CancellationTokenSource();
					IsAnimating = true;
					while (!_continuousFadeLoaderCancelationTokenSource.IsCancellationRequested)
					{
                        await AnimateColorFadeAsync(crossFade, pickRandomColor, pulseScale, duration, Easing.SinInOut);
					}
					IsAnimating = false;
				}
			});
		}

		public void StopContinuousFadeLoader()
		{
			_continuousFadeLoaderCancelationTokenSource.Cancel();
		}

        #endregion

		#region Async Animation Tasks

        public async Task AnimateColorLoadToZeroAsync(double scaleTo = 0.98, uint duration = 200, Easing easing = null)
        {
			if (easing == null)
				easing = Easing.CubicInOut;

            // Animation Prep
            ResetAnimationPropertiesToDefaults();

			// Animation
            var unloadTask = ColoredLogo.TranslateTo(0, ColoredLogo.Height, duration, easing);
			if (Math.Abs(scaleTo - 1.0) > 0.005)
			{
                var shrinkTask = LogoAbsoluteLayout.ScaleTo(scaleTo, duration, easing);
                await Task.WhenAll(unloadTask, shrinkTask);
			}
			else
			{
				await unloadTask;
			}
        }

        public async Task AnimateColorLoadFromZeroAsync(E_LogoColor changeToColor = E_LogoColor.Random, double scaleFrom = 0.98, uint duration = 200, Easing easing = null)
        {
			if (easing == null)
                easing = Easing.CubicInOut;

            // Color Swap
            if (changeToColor != E_LogoColor.Current && changeToColor != E_LogoColor.Random)
            {
                CurrentColor = changeToColor;
            }
            else if (changeToColor == E_LogoColor.Random)
            {
                CurrentColor = NextLogoColor(true);
            }

            // Animation
			var loadTask = ColoredLogo.TranslateTo(0, 0, duration, easing);
			if (Math.Abs(scaleFrom - 1.0) > 0.005)
			{
				var growTask = LogoAbsoluteLayout.ScaleTo(1.0, duration, easing);
				await Task.WhenAll(loadTask, growTask);
			}
			else
			{
				await loadTask;
			}
        }
		
        public async Task AnimateColorSwapAsync(bool pickRandomColor = true, double pulseScale = 0.98, uint duration = 400, Easing easing = null)
		{
			if (easing == null)
				easing = Easing.SinInOut;

			// Animation Prep
			ResetAnimationPropertiesToDefaults();
			uint eachAnimDuration = (uint)Math.Floor(duration * 0.5);

			// Animation Part 1
			var unloadTask = ColoredLogo.TranslateTo(0, ColoredLogo.Height, eachAnimDuration, easing);
            if (Math.Abs(pulseScale - 1.0) > 0.005)
            {
				var shrinkTask = LogoAbsoluteLayout.ScaleTo(pulseScale, eachAnimDuration, easing);
				await Task.WhenAll(unloadTask, shrinkTask);
            }
            else
            {
                await unloadTask;
            }

			// Switch Colors
			CurrentColor = NextLogoColor(pickRandomColor);

			// Animation Part 2
			var reloadTask = ColoredLogo.TranslateTo(0, 0, eachAnimDuration, easing);
            if (Math.Abs(pulseScale - 1.0) > 0.005)
            {
				var growTask = LogoAbsoluteLayout.ScaleTo(1.0, eachAnimDuration, easing);
				await Task.WhenAll(reloadTask, growTask);
            }
            else
            {
                await reloadTask;
            }
		}

		public async Task AnimateColorFadeAsync(bool crossFade = false, bool pickRandomColor = true, double pulseScale = 0.975, uint duration = 500, Easing easing = null)
		{
			if (easing == null)
				easing = Easing.SinInOut;
			
			// Animation Prep
			ResetAnimationPropertiesToDefaults();
			uint eachAnimDuration = (uint)Math.Floor(duration * 0.5);
			
			if (!crossFade)
			{
				// Animation Part 1
				var fadeOutTask = ColoredLogo.FadeTo(0.0, eachAnimDuration, easing);
                if (Math.Abs(pulseScale - 1.0) > 0.005)
                {
					var shrinkTask = LogoAbsoluteLayout.ScaleTo(pulseScale, eachAnimDuration, easing);
                    await Task.WhenAll(fadeOutTask, shrinkTask);
                }
                else
                {
                    await fadeOutTask;
                }
				
				// Switch Colors
				CurrentColor = NextLogoColor(pickRandomColor);
				
				// Animation Part 2
				var fadeInTask = ColoredLogo.FadeTo(1.0, eachAnimDuration, easing);
                if (Math.Abs(pulseScale - 1.0) > 0.005)
                {
                    var growTask = LogoAbsoluteLayout.ScaleTo(1.0, eachAnimDuration, easing);
                    await Task.WhenAll(fadeInTask, growTask);
                }
                else
                {
                    await fadeInTask;
                }
			}
			else
			{
				// Animation Prep
				E_LogoColor nextColor = NextLogoColor(pickRandomColor);
                ColoredLogoStandIn.Source = GetImageSourceForColor(nextColor);
				ColoredLogoStandIn.IsVisible = true;

                if (Math.Abs(pulseScale - 1.0) > 0.005)
                {
					// Create Animation Task for Logo Pulse Part 1
					var pulseTask1 = LogoAbsoluteLayout.ScaleTo(pulseScale, eachAnimDuration, easing);

					// Create Animation Tasks for Cross-Fade
					await Task.Delay((int)Math.Floor(eachAnimDuration * 0.85));
					var crossFadeTask1 = ColoredLogo.FadeTo(0.0, (uint)Math.Floor(eachAnimDuration * 1.05), easing);
					var crossFadeTask2 = ColoredLogoStandIn.FadeTo(1.0, (uint)Math.Floor(eachAnimDuration * 1.15), easing);
					
					// Create Animation Task for Logo Pulse Part 2
					await pulseTask1;
					var pulseTask2 = LogoAbsoluteLayout.ScaleTo(1.0, eachAnimDuration, easing);
					
					// Await All Anim Tasks Completion
					await crossFadeTask1;
					CurrentColor = nextColor;
					await Task.WhenAll(crossFadeTask1, crossFadeTask2, pulseTask1, pulseTask2);
                }
                else
                {
					// Create Animation Tasks for Cross-Fade
                    var crossFadeTask1 = ColoredLogo.FadeTo(0.0, (uint)Math.Floor(duration * 0.9), easing);
                    var crossFadeTask2 = ColoredLogoStandIn.FadeTo(1.0, duration, easing);

					// Await All Anim Tasks Completion
					await crossFadeTask1;
					CurrentColor = nextColor;
					await Task.WhenAll(crossFadeTask1, crossFadeTask2);
                }

                // Animation Callback
                // Switch ColorGradient references
                var reference = ColoredLogo;
                ColoredLogo = ColoredLogoStandIn;
                ColoredLogoStandIn = reference;

                // Make new StandIn invisible
                ColoredLogoStandIn.IsVisible = false;
            }
		}

		#endregion

		#region Helper Methods

		private void ResetAnimationPropertiesToDefaults()
		{
			LogoAbsoluteLayout.Scale = 1.0;
			ColoredLogo.TranslationY = 0.0;
            ColoredLogo.Opacity = 1.0;
            ColoredLogo.IsVisible = true;
			ColoredLogoStandIn.IsVisible = false;
            ColoredLogoStandIn.Opacity = 0.0;
            ColoredLogoStandIn.TranslationY = 0.0;
		}

        private string GetImageSourceForColor(E_LogoColor logoColor)
        {
			// Should not be called with E_LogoColor.Random or E_LogoColor.Current
			switch (logoColor)
			{
				case E_LogoColor.Red:
                    return $"LogoRawScript{Light}Red.png";
				case E_LogoColor.Yellow:
                    return $"LogoRawScript{Light}Yellow.png";
				case E_LogoColor.Green:
                    return $"LogoRawScript{Light}Green.png";
				case E_LogoColor.Blue:
                    return $"LogoRawScript{Light}Blue.png";
                default:
                    return "";
			}
        }

        private E_LogoColor NextLogoColor(bool pickSmartRandom = true)
        {
            if (pickSmartRandom)
            {
				// Get random number generator and output rndColor
				var rnd = new Random();
				var rndColor = CurrentColor;
				
				// Rolling logic ALWAYS changes rndColor to appropriate value
				for (var roll = 1; roll <= 2; roll++)
				{
					// Calculate random shift from current color
					var colorShifter = rnd.Next(1, 4);
					var newColorCode = (int)CurrentColor + colorShifter;
					if (newColorCode > 4)
					{
						newColorCode -= 4;
					}

					rndColor = (E_LogoColor)newColorCode;

                    // Skip re-roll if didn't get last color
                    if (newColorCode != (int)_lastColor)
                    {
                        roll = 2;
					}
				}
				
                return rndColor;
            }
            else
            {
				var newColorCode = (int)CurrentColor + 1;
				if (newColorCode > 4)
				{
					newColorCode -= 4;
				}
				return (E_LogoColor)newColorCode;
            }
        }

        #endregion
    }
}
