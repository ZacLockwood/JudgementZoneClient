﻿using System;
using Realms;
using System.Collections.Generic;

namespace JudgementZone.Models
{
    public class M_Client_GameState : RealmObject
    {

        // KEY
        [PrimaryKey]
        public string GameKey { get; set; }

        // CLIENT STATE IDENTIFIER
        public int ClientViewCode { get; set; }

        // STATE-INDEPENDENT DATA
        public IList<M_Player> PlayerList { get; }

        // GAME CYCLE METRICS
        public int CurrentQuestionNum { get; set; }

        public int MaxQuestionNum { get; set; }

        public int CurrentRoundNum { get; set; }

        public int MaxRoundNum { get; set; }

        public bool IsNewRound { get; set; }

		// GAME STATE CONTEXT
		public string FocusedPlayerId { get; set; }
		
		public int CurrentQuestionId { get; set; }
		
		public bool CanSubmitAnswer { get; set; }

        public M_Client_QuestionStats QuestionStats { get; set; }

        public IList<M_Client_PlayerGameStats> PlayerGameStatsList { get; }

        #region Constructor

        public M_Client_GameState()
        {
			GameKey = TemporaryKeyGenerator();
        }

        public M_Client_GameState(M_Player firstPlayer)
        {
            GameKey = TemporaryKeyGenerator();

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
