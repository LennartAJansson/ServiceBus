using System.Threading.Tasks;

using MassTransit;

using Microsoft.Extensions.Logging;

using MTCommonContract;

namespace MTSyncQueueListener
{
    public class Contract : IContract
    {
        public string Text { get; set; }
    }

    public class MyConsumer : IConsumer<IContract>
    {
        private readonly ILogger<MyConsumer> logger;

        public MyConsumer(ILogger<MyConsumer> logger)
        {
            this.logger = logger;
        }

        public Task Consume(ConsumeContext<IContract> context)
        {
            logger.LogInformation($"Received: {context.Message.Text}");
            context.RespondAsync(new Contract { Text = $"Received: {context.Message.Text}" });
            return Task.CompletedTask;
        }
    }
}
