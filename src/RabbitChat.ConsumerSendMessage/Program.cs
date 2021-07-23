using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using RabbitChat.Application.App.Command;
using RabbitChat.Infra.AmqpAdapters.Consumer;
using RabbitChat.Shared.Config;
using RabbitChat.Shared.Consumer;

namespace RabbitChat.ConsumerSendMessage
{
    public class Program
    {
        public static void Main(string[] args)
        {
            CreateHostBuilder(args).Build().Run();
        }

        public static IHostBuilder CreateHostBuilder(string[] args) =>
            Host.CreateDefaultBuilder(args)
                .ConfigureServices(Configure);

        private static void Configure(HostBuilderContext hostContext, IServiceCollection services)
        {

            var redisConn = "redis:6379,password=Redis2021!";
            services
              .AddSignalR()
              .AddRedis().AddStackExchangeRedis(redisConn);

            services.ResolveDependencies(hostContext);

            services.AddTransient<TaskExecution>();

            services.AddHostedService<SetupWorker>();

            ushort prefetchCount = hostContext.Configuration.GetValue<ushort>("RABBITMQ:PREFETCHCOUNT");

            services.AddQueueWork<TaskExecution, SendMessageCommand>("rabbit_chat_message_queue", prefetchCount, async (svc, data) => await svc.Execute(data));
        }
    }
}
