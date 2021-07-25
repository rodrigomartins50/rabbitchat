﻿using RabbitMQ.Client;
using RabbitMQ.Client.Events;

namespace RabbitChat.Shared.AmqpAdapters.Serialization
{
    public interface IAmqpSerializer
    {
        TResponse Deserialize<TResponse>(BasicDeliverEventArgs  eventArgs);

        byte[] Serialize<T>(IBasicProperties  basicProperties, T objectToSerialize);
    }
}
