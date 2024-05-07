using Apache.NMS.ActiveMQ.Commands;
using Apache.NMS;
using Microsoft.AspNetCore.Mvc;
using RabbitMQ.Client;
using System.Text;
using Amazon.SimpleNotificationService;
using Amazon.SimpleNotificationService.Model;
using Apache.NMS.Util;

namespace Publisher.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class PublisherController : ControllerBase
    {
        private readonly ILogger<PublisherController> _logger;

        public PublisherController(ILogger<PublisherController> logger)
        {
            _logger = logger;
        }

        [HttpPost("publish/rabbitmq")]
        public void PublishRabbitMQ(string message)
        {
            var factory = new ConnectionFactory { HostName = "rabbitmq", Port = 5672, UserName = "user", Password = "password" };

            using var connection = factory.CreateConnection();
            using var channel = connection.CreateModel();
            var exchangeName = "test_exchange_rabbitmq";
            channel.ExchangeDeclare(exchange: exchangeName, type: ExchangeType.Topic);

            var body = Encoding.UTF8.GetBytes(message);
            var topic = "test_topic_rabbitmq";

            channel.BasicPublish(exchange: exchangeName,
                                 routingKey: topic,
                                 basicProperties: null,
                                 body: body);
        }

        [HttpPost("publish/activemq")]
        public void PublishActiveMQ(string message)
        {
            const string TOPIC_NAME = "test_activemq_topic";
            const string BROKER = "activemq:tcp://activemq:61616";

            var connectionFactory = new Apache.NMS.ActiveMQ.ConnectionFactory(BROKER);
            using var connnection = connectionFactory.CreateConnection("user", "password");
            connnection.Start();
            using var session = connnection.CreateSession();
            var topic = new ActiveMQTopic(TOPIC_NAME);
            using var producer = session.CreateProducer(topic);
            var textMessage = session.CreateTextMessage(message);
            producer.Send(textMessage);
        }

        [HttpPost("publish/sns")]
        public async Task PublishSNS(string message)
        {
            string topic = "localstack-topic-test";
            var client = new AmazonSimpleNotificationServiceClient("test", "test", new AmazonSimpleNotificationServiceConfig
            {
                ServiceURL = "http://aws:4566"
            });

            var topicResponse = await client.CreateTopicAsync(topic);
            var topicARN = topicResponse.TopicArn;
            var request = new PublishRequest
            {
                TopicArn = topicARN,
                Message = message,
            };

            await client.PublishAsync(request);
        }
    }
}
