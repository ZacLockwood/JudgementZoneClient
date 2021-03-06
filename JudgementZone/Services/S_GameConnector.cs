﻿using System;
using System.Threading.Tasks;
using Microsoft.AspNet.SignalR.Client;
using JudgementZone.Interfaces;
using JudgementZone.Models;
using System.Collections.Generic;
using Xamarin.Forms;
using System.Linq;

namespace JudgementZone.Services
{
    public sealed class S_GameConnector : I_GameConnector
    {
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
            await gameHubProxy.Invoke("RequestNewGame", myPlayer);
        }

        public async Task SendJoinGameRequest(M_Player myPlayer, string gameKey)
        {
            await gameHubProxy.Invoke("RequestJoinGame", myPlayer, gameKey);
        }

        public async Task SendGameStartRequest(M_Player myPlayer, string gameKey)
        {
            await gameHubProxy.Invoke("RequestStartGame", myPlayer, gameKey);
        }

        public async Task SendAnswerSubmission(M_PlayerAnswer myAnswer, string gameKey)
        {
            Console.WriteLine("SENDING SUBMISSION");
			MessagingCenter.Send(this, "answerSubmitted", myAnswer.PlayerAnswer);
            await gameHubProxy.Invoke("SubmitAnswer", myAnswer, gameKey);
        }

		public async Task SendContinueRequest(string gameKey)
        {
            await gameHubProxy.Invoke("RequestContinueToNextQuestion", gameKey);
        }

        #endregion

        #region Receivers

        private void SetupProxyEventHandlers()
        {
			gameHubProxy.On<string>("DisplayGameKey", (gameKey) =>
			{
                S_LocalGameData.Instance.GameKey = gameKey;
                MessagingCenter.Send(this, "gameKeyReceived");
            });

            gameHubProxy.On<List<M_Player>>("DisplayPlayerList", (remotePlayerList) =>
			{
                // Add/Update/Remove PlayersInGame List
                var localGamePlayerList = S_LocalGameData.Instance.PlayersInGame;
                for (var i = localGamePlayerList.Count() - 1; i >= 0; i--)
                {
                    M_Player localPlayer = localGamePlayerList[i];
                    var remotePlayer = remotePlayerList.FirstOrDefault(p => p.PlayerId == localPlayer.PlayerName);
                    if (remotePlayer == null)
                    {
                        S_LocalGameData.Instance.PlayersInGame.Remove(localPlayer);
                    }
                    else
                    {
                        localPlayer.PlayerName = remotePlayer.PlayerName;
                    }
                }
                foreach (M_Player remotePlayer in remotePlayerList.Where(rp => !localGamePlayerList.Any(lp => lp.PlayerId == rp.PlayerId)))
                {
                    localGamePlayerList.Add(remotePlayer);
                }

                // Add/Update AllPlayers list [does not delete]
                var localAllPlayerList = S_LocalGameData.Instance.AllPlayers;
                foreach (M_Player remotePlayer in remotePlayerList)
                {
                    var localPlayer = localAllPlayerList.FirstOrDefault(lp => lp.PlayerId == remotePlayer.PlayerId);
                    if (localPlayer == null)
                    {
						localAllPlayerList.Add(remotePlayer);
                    }
                    else
                    {
                        localPlayer.PlayerName = remotePlayer.PlayerName;
                    }
                }

                MessagingCenter.Send(this, "playerListReceived");
			});

            gameHubProxy.On<string, M_QuestionCard>("DisplayQuestion", (focusedPlayerId, focusedQuestion) =>
			{
				Console.WriteLine($"FPID: {focusedPlayerId}\nFQ: {focusedQuestion.QuestionText}");
                var fp = S_LocalGameData.Instance.PlayersInGame.Where(p => p.PlayerId == focusedPlayerId).FirstOrDefault();
                S_LocalGameData.Instance.FocusedPlayer = fp;
                S_LocalGameData.Instance.FocusedQuestion = focusedQuestion;
                MessagingCenter.Send(this, "questionReceived");
            });

			gameHubProxy.On("EnableAnswerSubmission", () =>
			{
                MessagingCenter.Send(this, "enableAnswerSubmission");
			});

            gameHubProxy.On<M_AnswerStats>("DisplayQuestionStats", (questionStats) =>
			{
                Console.WriteLine("STATS RECEIVED");
                MessagingCenter.Send(this, "questionStatsReceived", questionStats);
			});

            gameHubProxy.On<string, Exception>("DisplayError", (silly, error) =>
            {
                Console.WriteLine($"ERRROR: {silly}\nMESSAGE:{error.Message}");

                Console.WriteLine("Continue...");
            });

            //gameHubProxy.On<M_GameStats>("DisplayGameStats", (gameStats) => {});
        }

        #endregion

    }
}
