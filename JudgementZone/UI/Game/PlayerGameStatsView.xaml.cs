using System;
using System.Collections.Generic;
using JudgementZone.Models;
using Xamarin.Forms;

namespace JudgementZone.UI
{
    public partial class PlayerGameStatsView : ContentView
    {
        public PlayerGameStatsView()
        {
            InitializeComponent();
        }

		public void UpdateView(M_Player player, M_Client_PlayerGameStats pgs)
		{
            OPAnswerStatsAbsoluteLayout.BindingContext = pgs;
            MPAnswerStatsAbsoluteLayout.BindingContext = pgs;
            MyPlayerNameLabel.BindingContext = player;
		}
    }
}
