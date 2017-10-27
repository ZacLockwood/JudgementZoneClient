using Realms;

namespace JudgementZone.Models
{
    public class M_Client_QuestionStats : RealmObject
    {
        
        // KEY
        [PrimaryKey]
		public string GameKey { get; set; }

        // CONTEXT
        public int QuestionId { get; set; }

        public bool IsPlayerCorrect { get; set; }

		public int CorrectAnswerId { get; set; }

		public int RedGuesses { get; set; }

		public int YellowGuesses { get; set; }

		public int GreenGuesses { get; set; }

		public int BlueGuesses { get; set; }

        public M_Client_QuestionStats()
        {
        }
    }
}
