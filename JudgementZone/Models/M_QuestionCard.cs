using Realms;
using MongoDB.Bson.Serialization.Attributes;
using MongoDB.Bson;
using System;

namespace JudgementZone.Models
{
    public class M_QuestionCard : RealmObject
    {
        [PrimaryKey]
		[BsonElement("QuestionId")]
		public int QuestionId { get; set; }

		[BsonElement("QuestionDeck")]
		public int QuestionDeck { get; set; }

		[BsonElement("GameRound")]
		public int GameRound { get; set; }

		[BsonElement("QuestionText")]
		public string QuestionText { get; set; }

		[BsonElement("RedAnswer")]
		public string RedAnswer { get; set; }

		[BsonElement("YellowAnswer")]
		public string YellowAnswer { get; set; }

		[BsonElement("GreenAnswer")]
		public string GreenAnswer { get; set; }

		[BsonElement("BlueAnswer")]
		public string BlueAnswer { get; set; }

		[BsonElement("DateCreated")]
		public DateTimeOffset DateCreated { get; set; }

        [BsonElement("DateUpdated")]
        public DateTimeOffset DateUpdated { get; set; }

		public M_QuestionCard()
        {
        }
    }
}
