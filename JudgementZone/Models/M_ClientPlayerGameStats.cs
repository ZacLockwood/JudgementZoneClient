using System;
using Realms;
using MongoDB.Bson.Serialization.Attributes;
using MongoDB.Bson;

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
		public float PercentRedSelections { get; set; }

		public float PercentYellowSelections { get; set; }

		public float PercentGreenSelections { get; set; }

		public float PercentBlueSelections { get; set; }

		public float PercentRedGuesses { get; set; }

		public float PercentYellowGuesses { get; set; }

		public float PercentGreenGuesses { get; set; }

		public float PercentBlueGuesses { get; set; }

        public M_ClientPlayerGameStats()
        {
        }
    }
}
