using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using RabbitMQ.Client;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace RabbitChat.Shared.Consumer
{
    public class SetupWorker : BackgroundService
    {
        private readonly ILogger<SetupWorker> _logger;
        private readonly IModel model;

        public SetupWorker(ILogger<SetupWorker> logger, IModel model)
        {
            this._logger = logger;
            this.model = model;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            this.CreateRetryAndUnroutedSchema("rabbit_chat", new Dictionary<string, string> {
                { "message", "message"},
                { "load_messages", "load_messages"}
            });

            await Task.CompletedTask;
        }

        private void CreateRetryAndUnroutedSchema(string appName, Dictionary<string, string> routes)
        {
            this.model.ExchangeDeclare($"{appName}_service", "topic", true, false);

            foreach (var item in routes)
            {
                string routingKey = item.Key;
                string functionalName = item.Value;

                this.model.QueueDeclare($"{appName}_{functionalName}_queue", true, false, false);
                this.model.QueueBind($"{appName}_{functionalName}_queue", $"{appName}_service", routingKey, null);
            }
        }
    }
}
