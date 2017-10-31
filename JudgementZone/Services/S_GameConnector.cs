using System;
using System.Threading.Tasks;
using Microsoft.AspNet.SignalR.Client;
using JudgementZone.Interfaces;
using JudgementZone.Models;
using Realms;
using System.Linq;
using System.Collections.Generic;

namespace JudgementZone.Services
{
    public sealed class S_GameConnector : I_GameConnector
    {
        // Debug Properties
        private const bool DEBUG_SERVER = true;

		// Singleton Instance Properties
        private static volatile S_GameConnector instance;
		private static object syncRoot = new Object();
        public static S_GameConnector Connector
        {
            get
            {
                if (instance == null)
                {
                    lock (syncRoot)
                    {
                        if (instance == null)
                        {
							instance = new S_GameConnector();
                        }
                    }
                }

                return instance;
            }
        }

        // Connection Properties
        private readonly HubConnection hubConnection;
        private readonly IHubProxy gameHubProxy;

        #region Constructor

        private S_GameConnector()
        {
            // Create Hub Connection (Single)
            hubConnection = new HubConnection(ServerConstants.SIGNALR_URL);

            // Create Hub Proxies (Multiple)
            gameHubProxy = hubConnection.CreateHubProxy(ServerConstants.SIGNALR_GAME_HUB_NAME); // GameHub name must match server class name

            // Set Up Hub Proxies to Recieve Data
            SetupProxyEventHandlers();
        }

        #endregion

        #region Connection Lifecycle

        public async Task<bool> StartConnectionAsync()
        {
            // Disallow connection from being started twice from the outside
            // May remove later if bugs emerge from dropped connections
            if (IsConnected())
            {
                return true;
            }

            // Start connection
            try
            {
                hubConnection.Headers.Add("authtoken", ServerConstants.SIGNALR_GAME_HUB_TOKEN);
                await hubConnection.Start();
                return true;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
                return false;
            }
        }

		public void StopConnection()
		{
			hubConnection.Stop();
		}

		public bool IsConnected()
		{
            return hubConnection.State == ConnectionState.Connected;
		}

		#endregion

    	#region Senders

        public async Task SendNewGameRequest(M_Player myPlayer)
        {
            if (DEBUG_SERVER)
                Console.WriteLine("GameServer: Requesting New Game...");
            await gameHubProxy.Invoke("RequestNewGame", myPlayer);
        }

        public async Task SendJoinGameRequest(M_Player myPlayer, string gameKey)
        {
			if (DEBUG_SERVER)
				Console.WriteLine("GameServer: Requesting Game Join...");
            await gameHubProxy.Invoke("RequestJoinGame", myPlayer, gameKey);
        }

        public async Task SendGameStartRequest(string gameKey)
        {
			if (DEBUG_SERVER)
				Console.WriteLine("GameServer: Requesting Game Start...");
            await gameHubProxy.Invoke("RequestStartGame", gameKey);
        }

        public async Task SendAnswerSubmission(int myAnswer, string gameKey)
        {
			if (DEBUG_SERVER)
				Console.WriteLine("GameServer: Sending Submission...");
			
            var myPlayerDataRealm = Realm.GetInstance("MyPlayerData.Realm");
			var myPlayer = myPlayerDataRealm.All<M_Player>().FirstOrDefault();

            await gameHubProxy.Invoke("SubmitAnswer", myPlayer.PlayerId, myAnswer, gameKey);
        }

		public async Task SendContinueRequest(string gameKey)
        {
			if (DEBUG_SERVER)
				Console.WriteLine("GameServer: Sending Request to Continue...");
            await gameHubProxy.Invoke("RequestContinueToNextQuestion", gameKey);
        }

        public async Task SendQuestionSyncRequest()
        {
            var questionDeckRealm = Realm.GetInstance("QuestionDeck.Realm");
            var lastSync = DateTimeOffset.MinValue;

            if (questionDeckRealm.All<M_QuestionCard>().Any())
            {
                lastSync = questionDeckRealm.All<M_QuestionCard>().OrderByDescending(qc => qc.DateModified).First().DateModified.AddSeconds(2);
            }

			if (DEBUG_SERVER)
				Console.WriteLine($"GameServer: Sending Sync Request for Timestamp {lastSync.ToString()}");
            
			await gameHubProxy.Invoke("RequestQuestionListUpdate", lastSync);
        }

        #endregion

        #region Receivers

        private void SetupProxyEventHandlers()
        {

            gameHubProxy.On<M_Client_GameState>("ServerUpdate", (gameState) => {
				if (DEBUG_SERVER)
					Console.WriteLine("GameServer: Game State Received");
                
                var gameStateRealm = Realm.GetInstance("GameState.Realm");

                gameStateRealm.Write(() =>
                {
                    gameStateRealm.Add(gameState, true);
                });
            });

            gameHubProxy.On<List<M_QuestionCard>>("PushQuestionCards", (QuestionList) => {
                if (QuestionList == null || QuestionList.Count() > 0)
                {
					if (DEBUG_SERVER)
						Console.WriteLine($"GameServer: Received {QuestionList.Count()} QuestionCards");
					var questionDeckRealm = Realm.GetInstance("QuestionDeck.Realm");
					questionDeckRealm.Write(() =>
					{
						foreach (var questionCard in QuestionList)
						{
							questionDeckRealm.Add(questionCard, true);
						}
                        if (DEBUG_SERVER)
                            Console.WriteLine($"Realm: Wrote {QuestionList.Count()} QuestionCards to Realm");
                    });
                }
                else
                {
					if (DEBUG_SERVER)
						Console.WriteLine($"GameServer: Received 0 QuestionCards. You're up to date!");
                }
            });

            gameHubProxy.On<string, Exception>("DisplayError", (silly, error) =>
            {
                Console.WriteLine($"GAMESERVER ERRROR: {silly}\nMESSAGE:{error.Message}");

                Console.WriteLine("Continue...");
            });

            //gameHubProxy.On<M_GameStats>("DisplayGameStats", (gameStats) => {});
        }

        #endregion

    }
}
