using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Polly;
using RabbitMQ.Client;
using RabbitMQ.Client.Exceptions;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace RabbitChat.Shared.AmqpAdapters.Consumer
{
    public abstract class QueueServiceWorkerBase : BackgroundService
    {
        protected readonly ILogger _logger;
        protected readonly IConnection connection;
        protected IModel Model{ get; private set; }

        public ushort PrefetchCount { get; }
        public string QueueName { get; }

        #region Constructors 

        protected QueueServiceWorkerBase(ILogger logger, IConnection connection, string queueName, ushort prefetchCount)
        {
            this._logger = logger;
            this.connection = connection;
            this.QueueName = queueName;
            this.PrefetchCount = prefetchCount;
        }


        #endregion

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            this.Model = this.BuildModel();

            IBasicConsumer consumer = this.BuildConsumer();

            this.WaitQueueCreation();

            string consumerTag = consumer.Model.BasicConsume(
                             queue: this.QueueName,
                             autoAck: false,
                             consumer: consumer);

            while (!stoppingToken.IsCancellationRequested)
            {
                this._logger.LogInformation("Worker running at: {time}", DateTimeOffset.Now);
                await Task.Delay(1000, stoppingToken);
            }

            this.Model.BasicCancelNoWait(consumerTag);
        }

        protected virtual void WaitQueueCreation()
        {
            Policy
            .Handle<OperationInterruptedException>()
                .WaitAndRetry(5, retryAttempt =>
                {
                    this._logger.LogWarning("Queue {QueueName} not found... retrying", this.QueueName);
                    return TimeSpan.FromSeconds(Math.Pow(2, retryAttempt));
                })
                .Execute(() =>
                {
                    using var testModel = this.BuildModel();

                    testModel.QueueDeclarePassive(this.QueueName);
                });
        }

        protected virtual IModel BuildModel()
        {
            IModel model = this.connection.CreateModel();

            model.BasicQos(0, this.PrefetchCount, false);

            return model;
        }

        protected abstract IBasicConsumer BuildConsumer();

    }
}
