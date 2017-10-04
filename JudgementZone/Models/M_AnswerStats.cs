using Realms;
using MongoDB.Bson.Serialization.Attributes;
using MongoDB.Bson;
using System.Collections.Generic;

namespace JudgementZone.Models
{
    public class M_AnswerStats : RealmObject
    {
        [PrimaryKey]
        [BsonElement("QuestionId")]
        public int QuestionId { get; set; }

        [BsonElement("GameRound")]
        public int GameRound { get; set; }

        [BsonElement("FocusedPlayerAnswer")]
        public M_PlayerAnswer FocusedPlayerAnswer { get; set; }

#if __ANDROID__
        [BsonElement("OtherPlayerAnswers")]
        public IList<M_PlayerAnswer> OtherPlayerAnswers { get; set; }
#endif

#if __IOS__
		[BsonElement("OtherPlayerAnswers")]
        public IList<M_PlayerAnswer> OtherPlayerAnswers { get; set; }
#endif

        [BsonElement("GameId")]
		public string GameId { get; set; }

        public M_AnswerStats()
        {
        }
    }
}
