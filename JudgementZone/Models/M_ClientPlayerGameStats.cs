using Realms;

namespace JudgementZone.Models
{
    public class M_ClientPlayerGameStats : RealmObject
    {
        // KEY
        [PrimaryKey]
        public string PlayerId { get; set; }

        // LINK TO PARENT
        public string GameKey { get; set; }

		// CONTEXT
		public int NumRedSelections { get; set; }

		public int NumYellowSelections { get; set; }

		public int NumGreenSelections { get; set; }

		public int NumBlueSelections { get; set; }

		public int NumRedGuesses { get; set; }

		public int NumYellowGuesses { get; set; }

		public int NumGreenGuesses { get; set; }

		public int NumBlueGuesses { get; set; }

        public M_ClientPlayerGameStats()
        {
        }
    }
}
