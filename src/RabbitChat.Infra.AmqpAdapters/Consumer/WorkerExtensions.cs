using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using RabbitChat.Infra.AmqpAdapters.Serialization;
using RabbitMQ.Client;
using System;
using System.Threading.Tasks;

namespace RabbitChat.Infra.AmqpAdapters.Consumer
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

        /// <summary>
        /// Create a new QueueServiceWorker to bind a queue with an function
        /// </summary>
        /// <typeparam name="TService">Service Type will be used to determine which service will be used to connect on queue</typeparam>
        /// <typeparam name="TRequest">Type of message sent by publisher to consumer. Must be exactly same Type that functionToExecute parameter requests.</typeparam>
        /// <typeparam name="TResponse">Type of returned message sent by consumer to publisher. Must be exactly same Type that functionToExecute returns.</typeparam>
        /// <param name="services">Dependency Injection Service Collection</param>
        /// <param name="queueName">Name of queue</param>
        /// <param name="functionToExecute">Function to execute when any message are consumed from queue</param>
        public static void AddQueueWork<TService, TRequest, TResponse>(this IServiceCollection services, string queueName, ushort prefetchCount, Func<TService, TRequest, TResponse> functionToExecute)
        {
            services.AddHostedService(sp =>
                    new SyncQueueServiceWorker<TRequest, TResponse>(
                        sp.GetService<ILogger<SyncQueueServiceWorker<TRequest, TResponse>>>(),
                        sp.GetRequiredService<IConnection>(),
                        sp.GetRequiredService<IAmqpSerializer>(),
                        queueName,
                        prefetchCount,
                        it => { return functionToExecute(sp.GetService<TService>(), it); }
                    )
                );
        }



        /// <summary>
        /// Create a new QueueServiceWorker to bind a queue with an function
        /// </summary>
        /// <typeparam name="TService">Service Type will be used to determine which service will be used to connect on queue</typeparam>
        /// <typeparam name="TRequest">Type of message sent by publisher to consumer. Must be exactly same Type that functionToExecute parameter requests.</typeparam>
        /// <typeparam name="TResponse">Type of returned message sent by consumer to publisher. Must be exactly same Type that functionToExecute returns.</typeparam>
        /// <param name="services">Dependency Injection Service Collection</param>
        /// <param name="queueName">Name of queue</param>
        /// <param name="functionToExecute">Function to execute when any message are consumed from queue</param>
        public static void AddAsyncQueueWork<TService, TRequest, TResponse>(this IServiceCollection services, string queueName, ushort prefetchCount, Func<TService, TRequest, Task<TResponse>> functionToExecute)
        {
            services.AddHostedService(sp =>
                    new AsyncQueueServiceWorker<TRequest, TResponse>(
                        sp.GetService<ILogger<AsyncQueueServiceWorker<TRequest, TResponse>>>(),
                        sp.GetRequiredService<IConnection>(),
                        sp.GetRequiredService<IAmqpSerializer>(),
                        queueName,
                        prefetchCount,
                        async it => { 
                            return await functionToExecute(sp.GetService<TService>(), it); 
                        }
                    )
                );
        }

        /// <summary>
        /// Create a new QueueServiceWorker to bind a queue with an function
        /// </summary>
        /// <typeparam name="TService">Service Type will be used to determine which service will be used to connect on queue</typeparam>
        /// <typeparam name="TRequest">Type of message sent by publisher to consumer. Must be exactly same Type that functionToExecute parameter requests.</typeparam>
        /// <typeparam name="TResponse">Type of returned message sent by consumer to publisher. Must be exactly same Type that functionToExecute returns.</typeparam>
        /// <param name="services">Dependency Injection Service Collection</param>
        /// <param name="queueName">Name of queue</param>
        /// <param name="functionToExecute">Function to execute when any message are consumed from queue</param>
        public static void AddAsyncQueueWork<TService, TRequest>(this IServiceCollection services, string queueName, ushort prefetchCount, Func<TService, TRequest, Task> functionToExecute)
        {
            services.AddHostedService(sp =>
                    new AsyncQueueServiceWorker<TRequest, Task>(
                        sp.GetService<ILogger<AsyncQueueServiceWorker<TRequest, Task>>>(),
                        sp.GetRequiredService<IConnection>(),
                        sp.GetRequiredService<IAmqpSerializer>(),
                        queueName,
                        prefetchCount,
                        async it => {
                            await functionToExecute(sp.GetService<TService>(), it);
                        }
                    )
                );
        }
    }
}
