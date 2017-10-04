using System;
using Realms;
using MongoDB.Bson.Serialization.Attributes;
using MongoDB.Bson;

namespace JudgementZone.Models
{
    public class M_PlayerAnswer : RealmObject
    {
        [PrimaryKey]
		[BsonElement("PlayerId")]
        public string PlayerId { get; set; }

		[BsonElement("PlayerAnswer")]
		public int PlayerAnswer { get; set; }

		[BsonElement("GameId")]
		public string GameId { get; set; }

        public M_PlayerAnswer()
        {
        }
    }
}
