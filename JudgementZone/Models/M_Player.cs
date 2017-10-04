using System;
using Realms;
using MongoDB.Bson.Serialization.Attributes;
using MongoDB.Bson;

namespace JudgementZone.Models
{
    public class M_Player : RealmObject
    {
        [PrimaryKey]
		[BsonElement("PlayerId")]
        public string PlayerId { get; set; } = Guid.NewGuid().ToString();

		[BsonElement("PlayerName")]
		public string PlayerName { get; set; }

        public M_Player()
        {
        }
    }
}
