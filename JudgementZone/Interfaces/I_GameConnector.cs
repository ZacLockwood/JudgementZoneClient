using System.Threading.Tasks;
using JudgementZone.Models;

namespace JudgementZone.Interfaces
{
    public interface I_GameConnector
    {
        // Connection Lifecycle
        Task<bool> StartConnectionAsync();
        void StopConnection();
        bool IsConnected();

        // Request Methods
        Task SendNewGameRequest(M_Player myPlayer);
        Task SendJoinGameRequest(M_Player myPlayer, string gameKey);
        Task SendGameStartRequest(M_Player myPlayer, string gameKey);
        Task SendAnswerSubmission(M_PlayerAnswer myAnswer, string gameKey);
        Task SendContinueRequest(string gameKey);

		// Display Methods - Invoked by Server - Implemented in the SetupProxyEventHandlers
		// DisplayGameKey(string gameKey);
		// DisplayPlayerList(List<M_Player> playerList);
		// DisplayQuestion(M_Player focusedPlayer, M_QuestionCard focusedQuestion);
		// EnableAnswerSubmission();
		// DisplayHandAnswerStats(M_AnswerStats handStats);
		// DisplayGameStats(M_GameStats gameStats);

	}
}
