using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Polly;
using RabbitChat.Data;
using RabbitChat.Infra.AmqpAdapters.Serialization;
using RabbitMQ.Client;
using RabbitMQ.Client.Exceptions;
using System;
using System.Linq;

namespace RabbitChat.ConsumerSendMessage.Config
{
    public static class DependencyInjectionConfig
    {
        public static IServiceCollection ResolveDependencias(this IServiceCollection services, HostBuilderContext hostContext)
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
            var assemblyDomain = AppDomain.CurrentDomain.Load("RabbitChat.Domain");
            var assemblyRepository = AppDomain.CurrentDomain.Load("RabbitChat.Repository");

            var listInterfaces = assemblyDomain.GetTypes()
                                                 .Where(t => t.IsInterface &&
                                                             t.Name.EndsWith("Repository"))
                                                 .ToList();

            var listClasses = assemblyRepository.GetTypes()
                                            .Where(t => t.IsClass &&
                                                        t.Name.EndsWith("Repository"))
                                            .ToList();

            foreach (var repositoryInterface in listInterfaces)
            {
                var serviceClass = listClasses.FirstOrDefault(c => c.Name == repositoryInterface.Name.Substring(1));

                if (serviceClass != null)
                {
                    services.AddTransient(repositoryInterface, serviceClass);
                }
            }
        }

        private static void InjectDependenciesServies(IServiceCollection services)
        {
            var assemblyDomain = AppDomain.CurrentDomain.Load("RabbitChat.Domain");
            var assemblyService = AppDomain.CurrentDomain.Load("RabbitChat.Service");

            var listInterfaces = assemblyDomain.GetTypes()
                                                 .Where(t => t.IsInterface &&
                                                             t.Name.EndsWith("Service"))
                                                 .ToList();

            var listClasses = assemblyService.GetTypes()
                                            .Where(t => t.IsClass &&
                                                        t.Name.EndsWith("Service"))
                                            .ToList();

            foreach (var serviceInterface in listInterfaces)
            {
                var serviceClass = listClasses.FirstOrDefault(c => c.Name == serviceInterface.Name.Substring(1));

                if (serviceClass != null)
                {
                    services.AddTransient(serviceInterface, serviceClass);
                }
            }
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
