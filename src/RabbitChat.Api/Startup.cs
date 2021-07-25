using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.OpenApi.Models;
using RabbitChat.Application.SignalR;
using RabbitChat.Shared.AmqpAdapters.Rpc;
using RabbitChat.Shared.AmqpAdapters.Serialization;
using RabbitMQ.Client;
using System;

namespace RabbitChat.Api
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            var redisConn = "redis:6379,password=Redis2021!";
            services
              .AddSignalR()
              .AddRedis().AddStackExchangeRedis(redisConn);

            //services.AddSignalR();

            services.AddSingleton(sp =>
            {
                ConnectionFactory factory = new ConnectionFactory();
                this.Configuration.Bind("RABBITMQ", factory);
                return factory;
            });

            services.AddSingleton<IAmqpSerializer, NewtonsoftAmqpSerializer>();

            services.AddSingleton(sp => sp.GetRequiredService<ConnectionFactory>().CreateConnection());

            services.AddScoped(sp => sp.GetRequiredService<IConnection>().CreateModel());

            services.AddScoped(sp => new SimpleAmqpRpc(
                    sp.GetRequiredService<IModel>(),
                    sp.GetRequiredService<IAmqpSerializer>()
                )
            );

            services.AddCors(o => o.AddPolicy("MyPolicy", builder =>
            {
                builder.WithOrigins("http://localhost:4200", "http://localhost:8080")
                       .AllowAnyHeader()
                       .WithMethods("GET", "POST")
                       .AllowCredentials();
            }));


            services.AddAuthorization();

            services.AddControllers();
            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new OpenApiInfo { Title = "RabbitChat.Api", Version = "v1" });
            });
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
                app.UseSwagger();
                app.UseSwaggerUI(c => c.SwaggerEndpoint("/swagger/v1/swagger.json", "RabbitChat.Api v1"));
            }

            //app.UseHttpsRedirection();

            app.UseCors("MyPolicy");

            app.UseRouting();

            app.UseAuthentication();
            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
                endpoints.MapHub<RabbitChatHub>("/rabbitchathub", options =>
                {
                    options.ApplicationMaxBufferSize = 1024000;
                    options.TransportMaxBufferSize = 0;
                });
            });
        }
    }
}
