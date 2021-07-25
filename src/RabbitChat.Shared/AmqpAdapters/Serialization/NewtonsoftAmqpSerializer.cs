using Newtonsoft.Json;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System.Text;

namespace RabbitChat.Shared.AmqpAdapters.Serialization
{
    public class NewtonsoftAmqpSerializer : IAmqpSerializer
    {
        public TResponse Deserialize<TResponse>(BasicDeliverEventArgs eventArgs)
        {
            string message = Encoding.UTF8.GetString(eventArgs.Body.ToArray());
            return JsonConvert.DeserializeObject<TResponse>(message);
        }

        public byte[] Serialize<T>(IBasicProperties basicProperties, T objectToSerialize) => Encoding.UTF8.GetBytes(JsonConvert.SerializeObject(objectToSerialize));
    }
}
