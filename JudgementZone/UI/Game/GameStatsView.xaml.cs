using System;
using System.Collections.Generic;
using System.Linq;
using JudgementZone.Interfaces;
using JudgementZone.Models;
using Xamarin.Forms;

namespace JudgementZone.UI
{
    public partial class GameStatsView : ContentView, I_PresentableGameView
    {

        #region Constructor

        public GameStatsView()
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
                    }
                );
            } else if (!this.AnimationIsRunning("FadeIn"))
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

        public void UpdateView(List<M_Client_PlayerGameStats> playerGameStats, List<M_Player> gamePlayers)
        {
            foreach (var pgs in playerGameStats)
            {
                var player = gamePlayers.First(p => p.PlayerId == pgs.PlayerId);

                var playerStatsView = new PlayerGameStatsView() { Margin = new Thickness(0, 10, 0, 0) };
                playerStatsView.UpdateView(player, pgs);

                MainStackLayout.Children.Insert(1, playerStatsView);
            }
        }

        #endregion

        #region Button Handlers

        void NextButtonClicked(object sender, EventArgs e)
        {
            MessagingCenter.Send(this, "EndGameButtonPressed");
        }

        #endregion

    }
}
