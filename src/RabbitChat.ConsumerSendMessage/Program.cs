using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using RabbitChat.ConsumerSendMessage;
using RabbitChat.ConsumerSendMessage.Config;
using RabbitChat.Domain.Entities;
using RabbitChat.Infra.AmqpAdapters.Consumer;

namespace ConsumerWorkerService
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
            services.ResolveDependencias(hostContext);
 
            services.AddHostedService<SetupWorker>();

            ushort prefetchCount = hostContext.Configuration.GetValue<ushort>("RABBITMQ:PREFETCHCOUNT");

            services.AddAsyncQueueWork<Execution, Message>("message", prefetchCount, async  (svc, data) => await svc.Execute(data));

        }


    }
}
