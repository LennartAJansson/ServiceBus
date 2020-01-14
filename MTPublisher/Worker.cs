using System;
using System.Threading;
using System.Threading.Tasks;

using MassTransit;

using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

using MTCommonContract;

namespace MTPublisher
{
    public class Worker : BackgroundService
    {
        private readonly ILogger<Worker> _logger;
        private readonly IBusControl busControl;
        private readonly IPublishEndpoint publishEndpoint;

        public Worker(ILogger<Worker> logger, IBusControl busControl, IPublishEndpoint publishEndpoint)
        {
            _logger = logger;
            this.busControl = busControl;
            this.publishEndpoint = publishEndpoint;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            await busControl.StartAsync(stoppingToken);

            while (!stoppingToken.IsCancellationRequested)
            {
                string msg = $"Worker running at: {DateTimeOffset.Now}";

                await publishEndpoint.Publish<IContract>(new Contract { Text = $"Sending: {msg}" });

                _logger.LogInformation($"Sent: {msg}");
                await Task.Delay(1000, stoppingToken);
            }
        }

        public override async Task StopAsync(CancellationToken cancellationToken)
        {
            await busControl.StopAsync(cancellationToken);

            await base.StopAsync(cancellationToken);
        }
    }
}
