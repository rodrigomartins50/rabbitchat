using RabbitMQ.Client;

namespace RabbitChat.Infra.AmqpAdapters.Consumer
{
    public interface IConsumerFactory
    {
        IBasicConsumer BuildConsumer(IModel model);

    }
}
