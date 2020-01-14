using System;
using System.Threading;
using System.Threading.Tasks;

using MassTransit;

using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

using MTCommonContract;

namespace MTSyncQueueSender
{
    public class Contract : IContract
    {
        public string Text { get; set; }
    }

    public class Worker : BackgroundService
    {
        private readonly ILogger<Worker> _logger;
        private readonly IBusControl busControl;

        public Worker(ILogger<Worker> logger, IBusControl busControl)
        {
            _logger = logger;
            this.busControl = busControl;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            await busControl.StartAsync(stoppingToken);
            var client = busControl.CreateRequestClient<IContract>();


            while (!stoppingToken.IsCancellationRequested)
            {
                string msg = $"Worker running at: {DateTimeOffset.Now}";

                Response<IContract> response = await client.GetResponse<IContract>(new Contract { Text = $"Sending: {msg}" });

                _logger.LogInformation(response.Message.Text);

                await Task.Delay(5000, stoppingToken);
            }
        }

        public override async Task StopAsync(CancellationToken cancellationToken)
        {
            await busControl.StopAsync(cancellationToken);

            await base.StopAsync(cancellationToken);
        }
    }
}
