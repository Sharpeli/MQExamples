
using Amazon.SimpleNotificationService;
using Amazon.SimpleNotificationService.Model;
using Amazon.SQS;
using Amazon.SQS.Model;
using System.Net;

namespace Consumer2
{
    public class SQSConsumer: IHostedService, IDisposable
    {
        const string QUEUE = "Consumer2";
        const string TOPIC = "localstack-topic-test";

        protected readonly ILogger<SQSConsumer> _logger;

        protected readonly IAmazonSQS _SQSClient;
        protected readonly IAmazonSimpleNotificationService _SNSClient;

        public SQSConsumer(ILogger<SQSConsumer> logger)
        {
            _logger = logger;

            // for localstack, just use test for the secret id and key 
            _SQSClient = new AmazonSQSClient("test", "test", new AmazonSQSConfig
            {
                ServiceURL = "http://aws:4566"
            });
            _SNSClient = new AmazonSimpleNotificationServiceClient("test", "test", new AmazonSimpleNotificationServiceConfig
            {
                ServiceURL = "http://aws:4566"
            });
        }

        public async Task StartAsync(CancellationToken stoppingToken)
        {
            var topicResponse = await _SNSClient.CreateTopicAsync(TOPIC);
            var topicARN = topicResponse.TopicArn;
            var queueResponse = await _SQSClient.CreateQueueAsync(QUEUE); 
            var queueUrl = queueResponse.QueueUrl;

            await _SNSClient.SubscribeQueueAsync(topicARN, _SQSClient, queueUrl);

            Console.WriteLine($"The SQS queue's URL is {queueUrl}");
            Console.WriteLine($"Start Listening {queueUrl}");
            while (true)
            {
                try
                {
                    await ReceiveAndDeleteMessage(queueUrl);
                }
                catch (Exception ex) 
                { 
                    // todo
                }
            }
        }

        public async Task ReceiveAndDeleteMessage(string queueUrl)
        {
            // Receive a single message from the queue.
            var receiveMessageRequest = new ReceiveMessageRequest
            {
                AttributeNames = { "SentTimestamp" },
                //MaxNumberOfMessages = 1,
                MessageAttributeNames = { "All" },
                QueueUrl = queueUrl,
                VisibilityTimeout = 0,
                WaitTimeSeconds = 20,
            };

            var receiveMessageResponse = await _SQSClient.ReceiveMessageAsync(receiveMessageRequest);
            if (receiveMessageResponse.HttpStatusCode == HttpStatusCode.OK)
            {
                var messages = receiveMessageResponse.Messages;

                foreach (var message in messages)
                {
                    Console.WriteLine("Received SQS Message:\n" + message.Body);

                    var deleteMessageRequest = new DeleteMessageRequest
                    {
                        QueueUrl = queueUrl,
                        ReceiptHandle = message.ReceiptHandle,
                    };

                    // Delete the received message from the queue.
                    await _SQSClient.DeleteMessageAsync(deleteMessageRequest);
                }
            }
        }

        public Task StopAsync(CancellationToken stoppingToken)
        {
            return Task.CompletedTask;
        }

        public void Dispose()
        {
            _SQSClient.Dispose();
        }
    }
}
