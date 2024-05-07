
using Apache.NMS;
using Apache.NMS.ActiveMQ;
using Apache.NMS.ActiveMQ.Commands;

namespace Consumer2
{
    public class ActiveMQConsumer : IHostedService, IDisposable
    {
        const string TOPIC_NAME = "test_activemq_topic";
        const string BROKER = "activemq:tcp://activemq:61616";
        const string CONSUMER_ID = "Consumer2";

        protected readonly ILogger<ActiveMQConsumer> _logger;

        protected IConnection _connection;
        protected Apache.NMS.ISession _session;
        protected IMessageConsumer _consumer;

        public ActiveMQConsumer(ILogger<ActiveMQConsumer> logger)
        {
            _logger = logger;

            var connectionFactory = new ConnectionFactory(BROKER);
            _connection = connectionFactory.CreateConnection("user", "password");
            _connection.Start();
            _session = _connection.CreateSession();
            var topic = new ActiveMQTopic(TOPIC_NAME);
            _consumer = _session.CreateDurableConsumer(topic, CONSUMER_ID, "2 > 1", false);
            _consumer.Listener += new MessageListener(OnMessage);
        }

        public void OnMessage(IMessage message)
        {
            ITextMessage textMessage = message as ITextMessage;
            _logger.LogInformation("Received ActiveMQ: {message}", textMessage.Text);

            // logic for process message...
            // manually ack
            message.Acknowledge();
        }
        public void Dispose()
        {
            _consumer.Dispose();
            _session.Dispose();
            _connection.Dispose();
        }

        public Task StartAsync(CancellationToken stoppingToken)
        {

            return Task.CompletedTask;
        }

        public Task StopAsync(CancellationToken stoppingToken)
        {
            return Task.CompletedTask;
        }
    }
}
