using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Polly;
using RabbitChat.Data;
using RabbitChat.Data.Repositories;
using RabbitChat.Domain.Repositories;
using RabbitChat.Domain.Services;
using RabbitChat.Infra.AmqpAdapters.Serialization;
using RabbitChat.Service;
using RabbitMQ.Client;
using RabbitMQ.Client.Exceptions;
using System;
using System.Linq;

namespace RabbitChat.ConsumerSendMessage2.Config
{
    public static class DependencyInjectionConfig
    {
        public static IServiceCollection ResolveDependencies(this IServiceCollection services, HostBuilderContext hostContext)
        {
            InjectEntityFramework(services, hostContext);

            InjectDependenciesRepositories(services);
            InjectDependenciesServies(services);

            InjectDependencyRabbitMQ(services, hostContext);

            return services;
        }

        private static void InjectEntityFramework(IServiceCollection services, HostBuilderContext hostContext)
        {
            var host = hostContext.Configuration["DBHOST"];
            var port = hostContext.Configuration["DBPORT"];
            var user = hostContext.Configuration["DBUSER"];
            var password = hostContext.Configuration["DBPASSWORD"];
            var database = hostContext.Configuration["DBDATABASE"];

            string connectionString = $"User ID={user};Password={password};Host={host};Port={port};Database={database};";

            services.AddDbContext<RabbitChatContext>(options =>
            {
                options.UseNpgsql(connectionString, options => options.SetPostgresVersion(new Version(9, 6)));
            });

            services.AddScoped<DbContext, RabbitChatContext>();
        }

        private static void InjectDependenciesRepositories(IServiceCollection services)
        {
            services.AddTransient<IUserRepository, UserRepository>();
            services.AddTransient<IMessageRepository, MessageRepository>();
        }

        private static void InjectDependenciesServies(IServiceCollection services)
        {
            services.AddTransient<IUserService, UserService>();
            services.AddTransient<IMessageService, MessageService>();
        }

        private static void InjectDependencyRabbitMQ(IServiceCollection services, HostBuilderContext hostContext)
        {
            services.AddSingleton(sp =>
            {
                ConnectionFactory factory = new ConnectionFactory();
                hostContext.Configuration.Bind("RABBITMQ", factory);
                return factory;
            });

            services.AddSingleton(sp => Policy
                    .Handle<BrokerUnreachableException>()
                    .WaitAndRetry(2, retryAttempt =>
                    {
                        Console.WriteLine("Tentando...");
                        return TimeSpan.FromSeconds(Math.Pow(2, retryAttempt));
                    })
                    .Execute(() => sp.GetRequiredService<ConnectionFactory>().CreateConnection())
            );

            services.AddTransient(sp => sp.GetRequiredService<IConnection>().CreateModel());

            services.AddSingleton<IAmqpSerializer, NewtonsoftAmqpSerializer>();
        }

    }
}
