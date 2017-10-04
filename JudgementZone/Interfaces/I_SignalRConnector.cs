using System.Threading.Tasks;
using JudgementZone.Models;

namespace JudgementZone.Interfaces
{
    public interface I_SignalRConnector
    {
        // Connection Lifecycle
        Task StartConnectionAsync();
        void StopConnection();
        bool IsConnected();

        // Send Methods
        Task Send(M_ChatMessage message);

        // Request Methods
        Task SendGetMessagesRequest();
    }
}
