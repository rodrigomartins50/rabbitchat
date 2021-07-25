using RabbitChat.Shared.AmqpAdapters.Serialization;
using RabbitMQ.Client;

namespace RabbitChat.Shared.AmqpAdapters.Rpc
{
    public class SimpleAmqpRpc
    {
        private readonly IModel model;
        private readonly IAmqpSerializer serializer;

        public SimpleAmqpRpc(IModel rabbitmqModel, IAmqpSerializer serializer)
        {
            this.model = rabbitmqModel;
            this.serializer = serializer;
        }

        public void FireAndForget<TRequest>(string exchangeName, string routingKey, TRequest requestModel) => this.Send(exchangeName, routingKey, requestModel);

        protected virtual void Send<TRequest>(string exchangeName, string routingKey, TRequest requestModel)
        {
            var prop = model.CreateBasicProperties();

            model.BasicPublish(exchangeName, routingKey, prop, this.serializer.Serialize(prop, requestModel));
        }
    }
}




