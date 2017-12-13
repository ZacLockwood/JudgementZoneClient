
using JudgementZone.Models;
using Xamarin.Forms;

namespace JudgementZone.UI
{
    public partial class RoundScreen : ContentView
    {
        int roundNum;

        public RoundScreen()
        {
            InitializeComponent();
        }

        public RoundScreen(int rn)
        {
            roundNum = rn;
            UpdateView();
            InitializeComponent();
        }

        #region Public View Management

        public void UpdateView()
        {
            
            RoundNumLabel.Text = $"Round {roundNum}";

        }

        #endregion

     }
}