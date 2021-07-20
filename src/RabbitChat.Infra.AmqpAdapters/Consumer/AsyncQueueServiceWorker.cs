using Microsoft.Extensions.Logging;
using RabbitChat.Infra.AmqpAdapters.Serialization;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System;
using System.Threading.Tasks;

namespace RabbitChat.Infra.AmqpAdapters.Consumer
{


    public class AsyncQueueServiceWorker<TRequest, TResponse> : QueueServiceWorkerBase
    {

        private readonly IAmqpSerializer serializer;
        private readonly Func<TRequest, Task> fireAndForgetDispatcher;
        private readonly Func<TRequest, Task<TResponse>> rpcDispatcher;

        private QueueServiceWorkerMode Mode { get; }

        #region Constructors 


        public AsyncQueueServiceWorker(ILogger logger, IConnection connection, IAmqpSerializer serializer, string queueName, ushort prefetchCount, Func<TRequest, Task<TResponse>> rpcDispatcher)
            : base(logger, connection, queueName, prefetchCount)
        {
            this.serializer = serializer;
            this.Mode = QueueServiceWorkerMode.RPC;
            this.rpcDispatcher = rpcDispatcher;
        }

        public AsyncQueueServiceWorker(ILogger logger, IConnection connection, IAmqpSerializer serializer, string queueName, ushort prefetchCount, Func<TRequest, Task> fireAndForgetDispatcher)
            : base(logger, connection, queueName, prefetchCount)
        {
            this.serializer = serializer;
            this.Mode = QueueServiceWorkerMode.FireAndForget;
            this.fireAndForgetDispatcher = fireAndForgetDispatcher;
        }

        #endregion


        protected override IBasicConsumer BuildConsumer()
        {
            var consumer = new AsyncEventingBasicConsumer(this.Model);

            consumer.Received += this.Receive;

            return consumer;
        }

        private async Task Receive(object sender, BasicDeliverEventArgs ea)
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
                    await this.fireAndForgetDispatcher(request);
                }
                else if (this.Mode == QueueServiceWorkerMode.RPC)
                {
                    if (ea.BasicProperties.ReplyTo == null)
                    {
                        this.Model.BasicReject(ea.DeliveryTag, false);

                        this._logger.LogWarning("Message rejected because doesn't sent a ReplyTo header to delivery feedback of RPC");

                        return;
                    }

                    TResponse response = await this.rpcDispatcher(request);

                    var props = this.Model.CreateBasicProperties();

                    this.Model.BasicPublish(string.Empty, ea.BasicProperties.ReplyTo, props, this.serializer.Serialize(props, response));

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
