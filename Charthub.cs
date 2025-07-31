using Microsoft.AspNetCore.SignalR;

namespace Candidate
{
    public class Charthub: Hub
    {
        public async Task SendData(List<Candidate> candidate)
        {
            await Clients.All.SendAsync("ReceivedData", candidate);
        }
    }
}
