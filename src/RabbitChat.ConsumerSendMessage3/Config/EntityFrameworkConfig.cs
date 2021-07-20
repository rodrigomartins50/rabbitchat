using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using RabbitChat.Data;
using System;

namespace RabbitChat.ConsumerSendMessage.Config
{
    public static class EntityFrameworkConfig
    {
        public static IServiceCollection EFConfig(this IServiceCollection services,
                                                  IConfiguration configuration)
        {
            var host = configuration["DBHOST"];
            var port = configuration["DBPORT"];
            var user = configuration["DBUSER"];
            var password = configuration["DBPASSWORD"];
            var database = configuration["DBDATABASE"];

            string connectionString = $"User ID={user};Password={password};Host={host};Port={port};Database={database};";

            services.AddDbContext<RabbitChatContext>(options =>
            {
                options.UseNpgsql(connectionString, options => options.SetPostgresVersion(new Version(9, 6)));
            });

            return services;
        }
    }
}
