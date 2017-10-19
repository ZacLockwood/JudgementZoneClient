using Realms;

namespace JudgementZone.Models
{
    public class M_ClientQuestionStats : RealmObject
    {
        
        // KEY
        [PrimaryKey]
		public string GameKey { get; set; }

        // CONTEXT
        public int QuestionId { get; set; }

        public bool IsPlayerCorrect { get; set; }

		public int CorrectAnswerId { get; set; }

		public int NumRedGuesses { get; set; }

		public int NumYellowGuesses { get; set; }

		public int NumGreenGuesses { get; set; }

		public int NumBlueGuesses { get; set; }

        public M_ClientQuestionStats()
        {
        }
    }
}
