
using Amazon.Runtime.Internal.Endpoints.StandardLibrary;
using Amazon.SimpleNotificationService;
using Amazon.SQS;
using MassTransit;
using MassTransit.AmazonSqsTransport.Configuration;
using System;

namespace Publisher
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // Add services to the container.

            builder.Services.AddControllers();
            // Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
            builder.Services.AddEndpointsApiExplorer();
            builder.Services.AddSwaggerGen();

            // MassTransit for RabbiMQ
            //builder.Services.AddMassTransit(x =>
            //{
            //    x.UsingRabbitMq((context, cfg) =>
            //    {
            //        cfg.Host("rabbitmq", h =>
            //        {
            //            h.Username("user");
            //            h.Password("password");
            //        });
            //        cfg.ConfigureEndpoints(context);
            //    });
            //});

            //builder.Services.AddMassTransit(x =>
            //{
            //    x.UsingActiveMq((context, cfg) =>
            //    {
            //        cfg.Host("activemq", 61616, h =>
            //        {
            //            h.Username("user");
            //            h.Password("password");
            //        });
            //        cfg.ConfigureEndpoints(context);
            //    });
            //});

            builder.Services.AddMassTransit(x =>
            {
                x.UsingAmazonSqs((context, cfg) =>
                {
                    var region = "aws:4566";
                    var accessKey = "test";
                    var secretKey = "test";
                    var hostAddress = new Uri($"amazonsqs://{region}");
                    var hostConfigurator = new AmazonSqsHostConfigurator(hostAddress);
                    hostConfigurator.AccessKey(accessKey);
                    hostConfigurator.SecretKey(secretKey);
                    hostConfigurator.Config(new AmazonSimpleNotificationServiceConfig { ServiceURL = $"http://{region}" });
                    hostConfigurator.Config(new AmazonSQSConfig { ServiceURL = $"http://{region}" });
                    cfg.Host(hostConfigurator.Settings);
                    cfg.ConfigureEndpoints(context);
                });
            });


            var app = builder.Build();

            // Configure the HTTP request pipeline.
            if (app.Environment.IsDevelopment())
            {
                app.UseSwagger();
                app.UseSwaggerUI();
            }

            app.UseHttpsRedirection();

            app.UseAuthorization();


            app.MapControllers();

            app.Run();
        }
    }
}
