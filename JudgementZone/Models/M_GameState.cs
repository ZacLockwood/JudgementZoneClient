using System;
using Realms;
using MongoDB.Bson.Serialization.Attributes;
using MongoDB.Bson;
using System.Collections.Generic;

namespace JudgementZone.Models
{
    public class M_GameState : RealmObject
    {
        [PrimaryKey]
        [BsonElement("GameId")]
        public string GameId { get; set; }

        [BsonElement("GameType")]
        public int GameType { get; set; }
		
		[BsonElement("GameRound")]
		public int GameRound { get; set; }
		
		[BsonElement("FocusedPlayerId")]
		public string FocusedPlayerId { get; set; }
		
		[BsonElement("FocusedQuestionId")]
		public int FocusedQuestionId { get; set; }

        [BsonElement("GamePlayers")]
        public IList<M_Player> GamePlayers { get; set; }

        [BsonElement("GameQuestions")]
        public IList<M_QuestionCard> GameQuestions { get; set; }

        [BsonElement("GameAnswerStats")]
        public IList<M_AnswerStats> GameAnswerStats { get; set; }

        public M_GameState()
        {
			GameId = TemporaryIdGenerator();
			GameType = 1;
			GameRound = 1;
        }

        public M_GameState(M_Player firstPlayer)
        {
            GameId = TemporaryIdGenerator();
            GameType = 1;
            GameRound = 1;

            GamePlayers.Add(firstPlayer);
            FocusedPlayerId = firstPlayer.PlayerId;
        }

        private string TemporaryIdGenerator()
        {
            var gameId = "";
            var randomNumGen = new Random();
            for (int i = 0; i < 5; i++)
            {
                gameId += randomNumGen.Next(0, 10).ToString();
            }
            return gameId;
        }
    }
}
