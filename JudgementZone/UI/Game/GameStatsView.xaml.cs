using System;
using System.Collections.Generic;
using System.Linq;
using JudgementZone.Models;
using Xamarin.Forms;

namespace JudgementZone.UI
{
    public partial class GameStatsView : ContentView
    {
        public GameStatsView()
        {
            InitializeComponent();
        }

        public void UpdateView(List<M_Client_PlayerGameStats> playerGameStats, List<M_Player> gamePlayers)
        {
            foreach (var pgs in playerGameStats)
            {
                var player = gamePlayers.First(p => p.PlayerId == pgs.PlayerId);

                var playerStatsView = new PlayerGameStatsView() { Margin = new Thickness(0,10,0,0) };
                playerStatsView.UpdateView(player, pgs);

                MainStackLayout.Children.Insert(1, playerStatsView);
            }
        }

		void NextButtonClicked(object sender, EventArgs e)
		{
            MessagingCenter.Send(this, "EndGameButtonPressed");
        }
    }
}
