using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using RabbitMQ.Client;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace ConsumerWorkerService
{
    public class SetupWorker : BackgroundService
    {
        private readonly ILogger<SetupWorker> _logger;
        private readonly IModel model;

        public ushort PrefetchCount { get; }
        public string QueueName { get; }

        public SetupWorker(ILogger<SetupWorker> logger, IModel model)
        {
            this._logger = logger;
            this.model = model;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            this.CreateRetryAndUnroutedSchema("customer", new Dictionary<string, string> {
                { "execute_anything", "ExecuteAnything" }
            });

            this.CreateRetryAndUnroutedSchema("calc", new Dictionary<string, string> {
                { "sum", "sum" },
                { "subtract", "subtract" },
                { "multiply", "multiply" },
                { "divide", "divide" }
            });

            this.CreateEventBus("event-hub");

            this.CreateBusConsumer("event-hub", "generic_updater", "*.update", new Dictionary<string, string> { 
                {  "aluno.update", "update" },
                {  "cliente.update", "update" }
            });

            await Task.CompletedTask;
        }





        private void CreateRetryAndUnroutedSchema(string appName, Dictionary<string, string> routes)
        {
            //Unrouted
            //Conjunto de Fila + exchange com o conteúdo que não foi roteado
            this.model.ExchangeDeclare($"{appName}_unrouted_exchange", "fanout", true, false, null);
            this.model.QueueDeclare($"{appName}_unrouted_queue", true, false, false, null);
            this.model.QueueBind($"{appName}_unrouted_queue", $"{appName}_unrouted_exchange", string.Empty, null);

            //Deadletter
            //Fila e exchange que acusam problemas irrecuperáveis que precisam de atenção maunal
            this.model.ExchangeDeclare($"{appName}_deadletter_exchange", "fanout", true, false, null);
            this.model.QueueDeclare($"{appName}_deadletter_queue", true, false, false, null);
            this.model.QueueBind($"{appName}_deadletter_queue", $"{appName}_deadletter_exchange", string.Empty, null);

            //Retry
            //Exchanges e Filas que possuem a demanda de suportar retry
            this.model.ExchangeDeclare($"{appName}_retry_exchange", "fanout", true, false, null);
            this.model.QueueDeclare($"{appName}_retry_queue", true, false, false, new Dictionary<string, object>() {
                { "x-dead-letter-exchange", $"{appName}_deadletter_exchange" },
                { "x-dead-letter-routing-key", "" }
            });
            this.model.QueueBind($"{appName}_retry_queue", $"{appName}_retry_exchange", string.Empty, null);

            this.model.ExchangeDeclare($"{appName}_service", "topic", true, false, new Dictionary<string, object>() {
                 { "alternate-exchange", $"{appName}_unrouted_exchange" }
            });
            foreach (var item in routes)
            {
                string routingKey = item.Key;
                string functionalName = item.Value;
                //EntryPoint
                //Ponto de entrada de processamento

                this.model.QueueDeclare($"{appName}_{functionalName}_queue", true, false, false, new Dictionary<string, object>() {
                    { "x-dead-letter-exchange", $"{appName}_retry_exchange" }
                });
                this.model.QueueBind($"{appName}_{functionalName}_queue", $"{appName}_service", routingKey, null);
            }
        }

        private void CreateEventBus(string hubName)
        {
            //Unrouted
            //Conjunto de Fila + exchange com o conteúdo que não foi roteado
            this.model.ExchangeDeclare($"{hubName}_unrouted-exchange", "fanout", true, false, null);
            this.model.QueueDeclare($"{hubName}_unrouted-queue", true, false, false, null);
            this.model.QueueBind($"{hubName}_unrouted-queue", $"{hubName}_unrouted-exchange", string.Empty, null);


            this.model.ExchangeDeclare(hubName, "topic", true, false, new Dictionary<string, object>() {
                { "alternate-exchange", $"{hubName}_unrouted-exchange" }
            });

        }

        private void CreateBusConsumer(string hubName, string appName, string hubRoutingKey, Dictionary<string, string> routes)
        {
            string exchangeName = $"{hubName}_{appName}-exchange";
            this.model.ExchangeDeclare(exchangeName, "topic", true, false, null);

            foreach (var item in routes)
            {
                var routingKey = item.Key;
                var functionalName = item.Value;

                var queueName = $"{hubName}_{appName}_{functionalName}_queue";

                this.model.QueueDeclare(queueName, true, false, false, new Dictionary<string, object>() {
                    //{ "x-dead-letter-exchange", $"{hubName}_{appName}_retry_exchange" }
                });

                this.model.QueueBind(queueName, exchangeName, routingKey, null);
            }

            this.model.ExchangeBind(exchangeName, hubName, hubRoutingKey, null);

        }

    }
}
