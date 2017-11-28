using System;
using System.Threading.Tasks;
using JudgementZone.Interfaces;
using Xamarin.Forms;

namespace JudgementZone.UI
{
	public partial class LoaderView : ContentView, I_PresentableGameView
    {

        public E_LogoColor FirstColor { get; set; } = E_LogoColor.Random;

        public string LoaderMessage { get; set; } = null;

        #region Constructor

        public LoaderView()
        {
            InitializeComponent();
        }

		#endregion

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
				StartContinuousFadeLoaderAsync();
				this.Animate("FadeIn", (percent) =>
				{
					Opacity = startingOpacity + percent * (1.0 - startingOpacity);
				},
					16, 250, Easing.CubicInOut,
					(double percent, bool canceled) =>
					{
						if (!canceled)
						{
							// Gauruntee 1.0 opacity and enable controls on successful completion
							Opacity = 1.0;
							IsEnabled = true;
						}
						else
						{
							// Stop fade loader if canceled
							StopContinuousFadeLoader();
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
							StopContinuousFadeLoader();
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

		#region Public Animation Tasks

		public async Task StartContinuousFadeLoaderAsync()
        {
            LoaderLogo.CurrentColor = FirstColor;
			LoaderLogo.StartContinuousFadeLoader();

			LoaderLabel.Opacity = 0.0;

            if (!String.IsNullOrWhiteSpace(LoaderMessage))
            {
                await Task.Delay(800);
                await LoadMessageFadeIn(LoaderMessage);
            }
        }

        public void StopContinuousFadeLoader()
        {
            LoaderLogo.StopContinuousFadeLoader();
        }

        public async Task SwapLoadMessageAsync(string loadMessage = null)
        {
            await LoadMessageFadeSwap(loadMessage);
        }

        #endregion

        #region Private Animation Tasks

        private async Task LoadMessageFadeIn(string loadMessage = null, uint duration = 1000)
        {
			if (String.IsNullOrWhiteSpace(loadMessage))
			{
                return;
			}

			// Set Up for Animation
            LoaderLabel.Opacity = 0.0;
			LoaderLabel.Text = loadMessage;
            LoaderLabel.TranslationY = LoaderLabel.Height * 1.25;
			
			// Animation
            var translateTask = LoaderLabel.TranslateTo(0, 0, duration, Easing.CubicOut);
            await Task.Delay((int)Math.Floor(duration * 0.1));
            var fadeTask = LoaderLabel.FadeTo(1.0, (uint)Math.Floor(duration * 0.85), Easing.CubicInOut);
			await Task.WhenAll(translateTask, fadeTask);
        }

        private async Task LoadMessageFadeOut(uint duration = 1000)
        {
			// Set Up for Animation
			LoaderLabel.Opacity = 1.0;
            LoaderLabel.TranslationY = 0.0;

			// Animation
			var fadeTask = LoaderLabel.FadeTo(0.0, (uint)Math.Floor(duration * 0.85), Easing.CubicInOut);
            var translateTask = LoaderLabel.TranslateTo(0, LoaderLabel.Height * 1.25, duration, Easing.CubicIn);
			await Task.WhenAll(translateTask, fadeTask);
        }

        private async Task LoadMessageFadeSwap(string loadMessage = null, uint duration = 1000)
        {
            if (loadMessage == null)
            {
                loadMessage = LoaderLabel.Text;
            }

            await LoadMessageFadeOut(duration);
            await LoadMessageFadeIn(loadMessage, duration);
        }

		#endregion

    }
}
