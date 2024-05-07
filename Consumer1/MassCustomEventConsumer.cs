using Events.Contracts;
using MassTransit;

namespace Consumer1
{
    public class MassCustomEventConsumer : IConsumer<CustomEvent>
    {
        private readonly ILogger<MassCustomEventConsumer> _logger;
        public MassCustomEventConsumer(ILogger<MassCustomEventConsumer> logger)
        {
            _logger = logger;
        }

        public Task Consume(ConsumeContext<CustomEvent> context)
        {
            _logger.LogInformation("Consumer1 [*] Message received: {message} ", context.Message.Message);
            return Task.CompletedTask;
        }
    }

    //public class MassCustomEventConsumerfinition :
    //ConsumerDefinition<MassCustomEventConsumer>
    //{
    //    public MassCustomEventConsumerfinition()
    //    {
    //        // override the default endpoint name
    //        EndpointName = "mass-consumer-1";

    //        // limit the number of messages consumed concurrently
    //        // this applies to the consumer only, not the endpoint
    //        ConcurrentMessageLimit = 8;
    //    }

    //    protected override void ConfigureConsumer(IReceiveEndpointConfigurator endpointConfigurator,
    //        IConsumerConfigurator<MassCustomEventConsumer> consumerConfigurator)
    //    {
    //        // configure message retry with millisecond intervals
    //        endpointConfigurator.UseMessageRetry(r => r.Intervals(100, 200, 500, 800, 1000));

    //        // use the outbox to prevent duplicate events from being published
    //        endpointConfigurator.UseInMemoryOutbox();
    //    }
    //}
}
