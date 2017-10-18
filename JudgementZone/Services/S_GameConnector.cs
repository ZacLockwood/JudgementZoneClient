using System;
using System.Threading.Tasks;
using Microsoft.AspNet.SignalR.Client;
using JudgementZone.Interfaces;
using JudgementZone.Models;
using System.Collections.Generic;
using Xamarin.Forms;
using System.Linq;
using Realms;

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

        public async Task SendGameStartRequest(M_Player myPlayer, string gameKey)
        {
			if (DEBUG_SERVER)
				Console.WriteLine("GameServer: Requesting Game Start...");
            await gameHubProxy.Invoke("RequestStartGame", myPlayer, gameKey);
        }

        public async Task SendAnswerSubmission(M_PlayerAnswer myAnswer, string gameKey)
        {
			if (DEBUG_SERVER)
				Console.WriteLine("GameServer: Sending Submission...");
			MessagingCenter.Send(this, "answerSubmitted", myAnswer.PlayerAnswer);
            await gameHubProxy.Invoke("SubmitAnswer", myAnswer, gameKey);
        }

		public async Task SendContinueRequest(string gameKey)
        {
			if (DEBUG_SERVER)
				Console.WriteLine("GameServer: Sending Request to Continue...");
            await gameHubProxy.Invoke("RequestContinueToNextQuestion", gameKey);
        }

        #endregion

        #region Receivers

        private void SetupProxyEventHandlers()
        {
			gameHubProxy.On<string>("DisplayGameKey", (gameKey) =>
			{
				if (DEBUG_SERVER)
                    Console.WriteLine("GameServer: Game Key Received");
                var realm = Realm.GetInstance();
                realm.Write(() =>
                {
                    M_GameState newGame = new M_GameState()
                    {
                        GameId = gameKey
                    };
                    realm.Add(newGame, true);
                });
            });

            gameHubProxy.On<List<M_Player>>("DisplayPlayerList", (remotePlayerList) =>
			{
				if (DEBUG_SERVER)
					Console.WriteLine("GameServer: Player List Received");

				var realm = Realm.GetInstance();
				realm.Write(() =>
				{
                    var gameState = realm.All<M_GameState>().FirstOrDefault();
                    if (gameState == null)
                    {
                        Console.WriteLine("FAIL");
                    }
                    else
                    {
                        gameState.GamePlayers = remotePlayerList;
                    }
				});
			});

            gameHubProxy.On<string, M_QuestionCard>("DisplayQuestion", (focusedPlayerId, focusedQuestion) =>
			{
				if (DEBUG_SERVER)
                    Console.WriteLine("GameServer: Qusetion Received");

                var realm = Realm.GetInstance();
                realm.Write(() =>
                {
					var gameState = realm.All<M_GameState>().FirstOrDefault();
					if (gameState == null)
					{
						Console.WriteLine("FAIL");
					}
					else
					{
                        gameState.FocusedPlayerId = focusedPlayerId;
                        gameState.FocusedQuestionId = focusedQuestion.QuestionId;
                        gameState.GameQuestions.Add(focusedQuestion);
					}
                });
            });

			gameHubProxy.On("EnableAnswerSubmission", () =>
			{
				if (DEBUG_SERVER)
					Console.WriteLine("GameServer: Enable Submission Received");
                MessagingCenter.Send(this, "enableAnswerSubmission");
			});

            gameHubProxy.On<M_AnswerStats>("DisplayQuestionStats", (questionStats) =>
			{
				if (DEBUG_SERVER)
                    Console.WriteLine("GameServer: Stats Received");

				var realm = Realm.GetInstance();
				realm.Write(() =>
				{
					var gameState = realm.All<M_GameState>().FirstOrDefault();
					if (gameState == null)
					{
						Console.WriteLine("FAIL");
					}
					else
					{
                        gameState.GameAnswerStats.Add(questionStats);
					}
				});
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
