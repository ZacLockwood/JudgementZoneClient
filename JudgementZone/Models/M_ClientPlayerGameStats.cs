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
		[BsonElement("PlayerId")]
        public string PlayerId { get; set; }

        // LINK TO PARENT
        [BsonElement("GameKey")]
        public string GameKey { get; set; }

		// CONTEXT
		[BsonElement("PercentRedSelections")]
		public float PercentRedSelections { get; set; }

		[BsonElement("PercentYellowSelections")]
		public float PercentYellowSelections { get; set; }

		[BsonElement("PercentGreenSelections")]
		public float PercentGreenSelections { get; set; }

		[BsonElement("PercentBlueSelections")]
		public float PercentBlueSelections { get; set; }

		[BsonElement("PercentRedGuesses")]
		public float PercentRedGuesses { get; set; }

		[BsonElement("PercentYellowGuesses")]
		public float PercentYellowGuesses { get; set; }

		[BsonElement("PercentGreenGuesses")]
		public float PercentGreenGuesses { get; set; }

		[BsonElement("PercentBlueGuesses")]
		public float PercentBlueGuesses { get; set; }

        public M_ClientPlayerGameStats()
        {
        }
    }
}
