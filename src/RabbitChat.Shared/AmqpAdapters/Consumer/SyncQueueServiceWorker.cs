using Microsoft.Extensions.Logging;
using RabbitChat.Shared.AmqpAdapters.Serialization;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System;

namespace RabbitChat.Shared.AmqpAdapters.Consumer
{
    public class SyncQueueServiceWorker<TRequest, TResponse> : QueueServiceWorkerBase
    {

        private readonly IAmqpSerializer serializer;
        private readonly Action<TRequest> fireAndForgetDispatcher;

        private QueueServiceWorkerMode Mode { get; }

        #region Constructors 

        public SyncQueueServiceWorker(ILogger<SyncQueueServiceWorker<TRequest, TResponse>> logger, IConnection connection, IAmqpSerializer serializer, string queueName, ushort prefetchCount, Action<TRequest> fireAndForgetDispatcher)
            : base(logger, connection, queueName, prefetchCount)
        {
            this.serializer = serializer;
            this.Mode = QueueServiceWorkerMode.FireAndForget;
            this.fireAndForgetDispatcher = fireAndForgetDispatcher;
        }

        #endregion

        protected override IBasicConsumer BuildConsumer()
        {
            var consumer = new EventingBasicConsumer(this.Model);

            consumer.Received += this.Receive;

            return consumer;
        }

        private void Receive(object sender, BasicDeliverEventArgs ea)
        {
            TRequest request;

            try
            {
                request = this.serializer.Deserialize<TRequest>(ea);
            }
            catch (Exception exception)
            {
                this.Model.BasicReject(ea.DeliveryTag, false);

                this._logger.LogWarning("Message rejected during desserialization {exception}", exception);

                return;
            }

            try
            {
                if (this.Mode == QueueServiceWorkerMode.FireAndForget)
                {
                    this.fireAndForgetDispatcher(request);
                }

                this.Model.BasicAck(ea.DeliveryTag, false);
            }
            catch (Exception exception)
            {

                this.Model.BasicNack(ea.DeliveryTag, false, false);

                this._logger.LogWarning("Exception on processing message {message} {exception}", this.QueueName, exception);

            }
        }
    }
}
