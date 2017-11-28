
using JudgementZone.Models;
using Xamarin.Forms;

namespace JudgementZone.UI.Game
{
    public partial class RoundScreen : ContentView
    {
        public RoundScreen()
        {


            InitializeComponent();
        }

        #region Public View Management

        public void UpdateView(int roundNum)
        {
            
            RoundNumLabel.Text = $"Round {roundNum}";

        }

        #endregion

     }
}