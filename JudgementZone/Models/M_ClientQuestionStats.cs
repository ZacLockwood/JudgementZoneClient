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
		public string GameKey { get; set; }

        // CONTEXT
        public int QuestionId { get; set; }

        public bool IsPlayerCorrect { get; set; }

		public int CorrectAnswerId { get; set; }

		public float PercentRedGuesses { get; set; }

		public float PercentYellowGuesses { get; set; }

		public float PercentGreenGuesses { get; set; }

		public float PercentBlueGuesses { get; set; }

        public M_ClientQuestionStats()
        {
        }
    }
}
