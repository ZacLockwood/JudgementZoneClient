using Realms;

namespace JudgementZone.Models
{
    public class M_Client_PlayerGameStats : RealmObject
    {
        // KEY
        [PrimaryKey]
        public string PlayerId { get; set; }

        // LINK TO PARENT
        public string GameKey { get; set; }

		// CONTEXT
		public int PlayerSelectionsRed { get; set; }

		public int PlayerSelectionsYellow { get; set; }

		public int PlayerSelectionsGreen { get; set; }

		public int PlayerSelectionsBlue { get; set; }

		public int OtherSelectionsRed { get; set; }

		public int OtherSelectionsYellow { get; set; }

		public int OtherSelectionsGreen { get; set; }

		public int OtherSelectionsBlue { get; set; }

        public M_Client_PlayerGameStats()
        {
        }
    }
}
