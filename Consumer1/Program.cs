using Amazon.SimpleNotificationService;
using Amazon.SQS;
using MassTransit;
using MassTransit.AmazonSqsTransport.Configuration;
using System;
using System.Reflection;

namespace Consumer1
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

            //builder.Services.AddHostedService<RabbitMQConsumer>();
            //builder.Services.AddHostedService<ActiveMQConsumer>();
            //builder.Services.AddHostedService<SQSConsumer>();

            // MassTransit RabbitMQ Example
            //builder.Services.AddMassTransit(m =>
            //{
            //    m.AddConsumer<MassCustomEventConsumer>();
            //    //m.AddConsumer<MassCustomEventConsumer>().Endpoint(e =>
            //    //{
            //    //    // set if each service instance should have its own endpoint for the consumer
            //    //    // so that messages fan out to each instance.
            //    //    e.InstanceId = "MASSConsumer1";
            //    //});
            //    m.AddConfigureEndpointsCallback((name, cfg) =>
            //    {
            //        cfg.UseMessageRetry(r => r.Immediate(2));
            //    });
            //    m.UsingRabbitMq((ctx, cfg) =>
            //    {
            //        cfg.Host("rabbitmq", c =>
            //        {
            //            c.Username("user");
            //            c.Password("password");
            //        });
            //        //cfg.ConfigureEndpoints(ctx);
            //        cfg.ReceiveEndpoint("mass-consumer-1", e =>
            //        {
            //            e.ConfigureConsumer<MassCustomEventConsumer>(ctx);
            //        });
            //    });
            //});

            //builder.Services.AddMassTransit(m =>
            //{
            //    m.AddConsumer<MassCustomEventConsumer>();
            //    //m.AddConsumer<MassCustomEventConsumer>().Endpoint(e =>
            //    //{
            //    //    // set if each service instance should have its own endpoint for the consumer
            //    //    // so that messages fan out to each instance.
            //    //    e.InstanceId = "MASSConsumer1";
            //    //});
            //    m.AddConfigureEndpointsCallback((name, cfg) =>
            //    {
            //        cfg.UseMessageRetry(r => r.Immediate(2));
            //    });
            //    m.UsingActiveMq((ctx, cfg) =>
            //    {
            //        cfg.Host("activemq", 61616, c =>
            //        {
            //            c.Username("user");
            //            c.Password("password");
            //        });
            //        //cfg.ConfigureEndpoints(ctx);
            //        cfg.ReceiveEndpoint("mass-consumer-1", e =>
            //        {
            //            e.ConfigureConsumer<MassCustomEventConsumer>(ctx);
            //        });
            //    });
            //});

            builder.Services.AddMassTransit(m =>
            {
                m.AddConsumer<MassCustomEventConsumer>();
                //m.AddConsumer<MassCustomEventConsumer>().Endpoint(e =>
                //{
                //    // set if each service instance should have its own endpoint for the consumer
                //    // so that messages fan out to each instance.
                //    e.InstanceId = "MASSConsumer1";
                //});
                m.AddConfigureEndpointsCallback((name, cfg) =>
                {
                    cfg.UseMessageRetry(r => r.Immediate(2));
                });
                m.UsingAmazonSqs((ctx, cfg) =>
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
                    //cfg.ConfigureEndpoints(ctx);
                    cfg.ReceiveEndpoint("mass-consumer-1", e =>
                    {
                        e.ConfigureConsumer<MassCustomEventConsumer>(ctx);
                    });
                });
            });

            var app = builder.Build();

            // Configure the HTTP request pipeline.
            if (app.Environment.IsDevelopment())
            {
                app.UseSwagger();
                app.UseSwaggerUI();
            }

            app.UseAuthorization();


            app.MapControllers();

            app.Run();
        }
    }
}
