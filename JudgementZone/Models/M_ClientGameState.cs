using System;
using Realms;
using MongoDB.Bson.Serialization.Attributes;
using MongoDB.Bson;
using System.Collections.Generic;

namespace JudgementZone.Models
{
    public class M_ClientGameState : RealmObject
    {

        // KEY
        [PrimaryKey]
        [BsonElement("GameKey")]
        public string GameKey { get; set; }

        // CLIENT STATE IDENTIFIER
        [BsonElement("ClientGameStateId")]
        public string ClientGameStateId { get; set; }

        // STATE-INDEPENDENT DATA
		[BsonElement("PlayerList")]
        public IList<M_Player> PlayerList { get; set; }

        // GAME CYCLE METRICS
        [BsonElement("CurrentQuestionNum")]
        public int CurrentQuestionNum { get; set; }

        [BsonElement("MaxQuestionNum")]
        public int MaxQuestionNum { get; set; }

		[BsonElement("CurrentRoundNum")]
        public int CurrentRoundNum { get; set; }

        [BsonElement("MaxRoundNum")]
        public int MaxRoundNum { get; set; }

        [BsonElement("IsNewRound")]
        public bool IsNewRound { get; set; }

		// GAME STATE CONTEXT
		[BsonElement("FocusedPlayerId")]
		public string FocusedPlayerId { get; set; }
		
		[BsonElement("FocusedQuestionId")]
		public int FocusedQuestionId { get; set; }

        [BsonElement("FocusedQuestionStats")]
        public M_ClientQuestionStats FocusedClientQuestionStats { get; set; }

        [BsonElement("GameStats")]
        public M_ClientGameStats ClientGameStats { get; set; }

        #region Constructor

        public M_ClientGameState()
        {
			GameKey = TemporaryKeyGenerator();
        }

        public M_ClientGameState(M_Player firstPlayer)
        {
            GameId = TemporaryIdGenerator();

            PlayerList.Add(firstPlayer);
            FocusedPlayerId = firstPlayer.PlayerId;
        }

        #endregion

        #region Helper Method

        private string TemporaryKeyGenerator()
        {
            var gameKey = "";
            var randomNumGen = new Random();
            for (int i = 0; i < 5; i++)
            {
                gameKey += randomNumGen.Next(0, 10).ToString();
            }
            return gameKey;
        }

        #endregion
    }
}
