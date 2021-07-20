using RabbitChat.Infra.AmqpAdapters.Serialization;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System;
using System.Collections.Concurrent;
using System.Threading.Tasks;

namespace RabbitChat.Infra.AmqpAdapters.Rpc
{
    public class SimpleAmqpRpc
    {
        private readonly IModel model;
        private readonly IAmqpSerializer serializer;
        private readonly TimeSpan defaultTimeout;

        public SimpleAmqpRpc(IModel rabbitmqModel, IAmqpSerializer serializer, TimeSpan defaultTimeout)
        {
            this.model = rabbitmqModel;
            this.serializer = serializer;
            this.defaultTimeout = defaultTimeout;
        }

        public void FireAndForget<TRequest>(string exchangeName, string routingKey, TRequest requestModel) => this.Send(exchangeName, routingKey, requestModel, null);

        public async Task<TResponse> SendAndReceiveAsync<TRequest, TResponse>(string exchangeName, string routingKey, TRequest requestModel, TimeSpan? timeout = null)
        {

            QueueDeclareOk queue = model.QueueDeclare(
                queue: string.Empty,
                durable: false,
                exclusive: true,
                autoDelete: true,
                arguments: null);

            Task sendTask = Task.Run(() => { this.Send(exchangeName, routingKey, requestModel, queue.QueueName); });

            TResponse responseModel = default;

            using var localQueue = new BlockingCollection<TResponse>();

            Task receiveTask = Task.Run(() => { responseModel = Receive(queue, localQueue, timeout ?? this.defaultTimeout); });

            await Task.WhenAll(sendTask, receiveTask);

            return responseModel;
        }

        protected virtual void Send<TRequest>(string exchangeName, string routingKey, TRequest requestModel, string callbackQueueName = null)
        {
            var prop = model.CreateBasicProperties();

            if (!string.IsNullOrWhiteSpace(callbackQueueName))
            {
                prop.ReplyTo = callbackQueueName;
            }

            model.BasicPublish(exchangeName, routingKey, prop, this.serializer.Serialize(prop, requestModel));
        }

        protected virtual TResponse Receive<TResponse>(QueueDeclareOk queue, BlockingCollection<TResponse> localQueue, TimeSpan receiveTimeout)
        {
            var consumer = new EventingBasicConsumer(model);

            consumer.Received += (sender, ea) =>
            {
                localQueue.Add(this.serializer.Deserialize<TResponse>(ea));
                localQueue.CompleteAdding();
            };

            var consumerTag = model.BasicConsume(queue.QueueName, true, consumer);

            TResponse responseModel;

            try
            {
                if (!localQueue.TryTake(out responseModel, receiveTimeout))
                    throw new TimeoutException($"Timeout on AMQP RPC call.");
            }
            finally
            {
                model.BasicCancelNoWait(consumerTag);
            }

            return responseModel;
        }

    }
}




