using Realms;
using MongoDB.Bson.Serialization.Attributes;
using MongoDB.Bson;
using System.Collections.Generic;

namespace JudgementZone.Models
{
    public class M_ClientQuestionStats : RealmObject
    {
        
        // KEY
        [PrimaryKey]
		[BsonElement("GameKey")]
		public string GameKey { get; set; }

        // CONTEXT
        [BsonElement("QuestionId")]
        public int QuestionId { get; set; }

        [BsonElement("IsPlayerCorrect")]
        public bool IsPlayerCorrect { get; set; }

		[BsonElement("CorrectAnswerId")]
		public int CorrectAnswerId { get; set; }

		[BsonElement("PercentRedGuesses")]
		public float PercentRedGuesses { get; set; }

		[BsonElement("PercentYellowGuesses")]
		public float PercentYellowGuesses { get; set; }

		[BsonElement("PercentGreenGuesses")]
		public float PercentGreenGuesses { get; set; }

		[BsonElement("PercentBlueGuesses")]
		public float PercentBlueGuesses { get; set; }

        public M_ClientQuestionStats()
        {
        }
    }
}
