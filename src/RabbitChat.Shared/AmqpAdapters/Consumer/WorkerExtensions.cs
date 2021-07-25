using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using RabbitChat.Shared.AmqpAdapters.Serialization;
using RabbitMQ.Client;
using System;

namespace RabbitChat.Shared.AmqpAdapters.Consumer
{
    public static class WorkerExtensions
    {
        /// <summary>
        /// Create a new QueueServiceWorker to bind a queue with an action
        /// </summary>
        /// <typeparam name="TService">Service Type will be used to determine which service will be used to connect on queue</typeparam>
        /// <typeparam name="TRequest">Type of message sent by publisher to consumer. Must be exactly same Type that actionToExecute parameter requests.</typeparam>
        /// <param name="services">Dependency Injection Service Collection</param>
        /// <param name="queueName">Name of queue</param>
        /// <param name="actionToExecute">Action to execute when any message are consumed from queue</param>
        public static void AddQueueWork<TService, TRequest>(this IServiceCollection services, string queueName, ushort prefetchCount, Action<TService, TRequest> actionToExecute)
        {
            services.AddHostedService(sp =>
                    new SyncQueueServiceWorker<TRequest, object>(
                        sp.GetService<ILogger<SyncQueueServiceWorker<TRequest, object>>>(),
                        sp.GetService<IConnection>(),
                        sp.GetRequiredService<IAmqpSerializer>(),
                        queueName,
                        prefetchCount,
                        it => actionToExecute(sp.GetService<TService>(), it)
                    )
                );
        }
    }
}
