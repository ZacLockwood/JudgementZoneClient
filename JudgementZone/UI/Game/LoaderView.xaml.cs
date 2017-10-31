using System;
using System.Threading.Tasks;
using Xamarin.Forms;

namespace JudgementZone.UI
{
	public partial class LoaderView : ContentView
    {

        #region Constructor

        public LoaderView()
        {
            InitializeComponent();
        }

        #endregion

        #region Public Animation Tasks

        public async Task StartContinuousFadeLoaderAsync(string loadMessage = null, E_LogoColor firstColor = E_LogoColor.Random)
        {
            LoaderLogo.CurrentColor = firstColor;
			LoaderLogo.StartContinuousFadeLoader();

			LoaderLabel.Opacity = 0.0;

            if (!String.IsNullOrWhiteSpace(loadMessage))
            {
                await Task.Delay(800);
                await LoadMessageFadeIn(loadMessage);
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
