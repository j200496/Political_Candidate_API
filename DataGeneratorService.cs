
using Microsoft.AspNetCore.SignalR;

namespace Candidate
{
    public class DataGeneratorService(IHubContext<Charthub> hub) : BackgroundService
    {
        private readonly IHubContext<Charthub> hub = hub;

        protected override Task ExecuteAsync(CancellationToken stoppingToken)
        {
            throw new NotImplementedException();
        }
    }
}
