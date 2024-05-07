using RabbitMQ.Client.Events;
using RabbitMQ.Client;
using System.Text;

namespace Consumer1
{
    public class RabbitMQConsumer: IHostedService, IDisposable
    {
        protected readonly ILogger<RabbitMQConsumer> _logger;
        protected readonly IModel Channel;
        protected readonly IConnection Connection;

        public RabbitMQConsumer(ILogger<RabbitMQConsumer> logger)
        {
            _logger = logger;

            var factory = new ConnectionFactory { HostName = "rabbitmq", Port = 5672, UserName = "user", Password = "password", DispatchConsumersAsync = true };
            Connection = factory.CreateConnection();
            Channel = Connection.CreateModel();
            var exchangeName = "test_exchange_rabbitmq";
            var topic = "test_topic_rabbitmq";
            Channel.ExchangeDeclare(exchange: exchangeName, type: ExchangeType.Topic);

            var queueName = "consumer1";
            Channel.QueueDeclare(queue: queueName,
                                     durable: false,
                                     exclusive: false,
                                     autoDelete: false,
                                     arguments: null);


            var consumer = new AsyncEventingBasicConsumer(Channel);
            consumer.Received += OnEventReceived;
            Channel.BasicConsume(queue: queueName,
                                 autoAck: false,
                                 consumer: consumer);


            _logger.LogInformation("Started RabbitMQ basic Consume");

            Channel.QueueBind(queue: queueName,
                             exchange: exchangeName,
                             routingKey: topic);
        }

        protected virtual Task OnEventReceived(object sender, BasicDeliverEventArgs @event)
        {
            byte[] body = @event.Body.ToArray();
            var message = Encoding.UTF8.GetString(body);
            _logger.LogInformation("Received RabbitMQ: {message}", message);

            // logic for process message...
            // manually ack
            Channel.BasicAck(@event.DeliveryTag, multiple: false);

            return Task.CompletedTask;
        }

        public Task StartAsync(CancellationToken stoppingToken)
        {
            return Task.CompletedTask;
        }

        public Task StopAsync(CancellationToken stoppingToken)
        {
            return Task.CompletedTask;
        }
        public void Dispose()
        {
            // todo: should close the connection
            _logger.LogInformation("disposed");
        }
    }
}
