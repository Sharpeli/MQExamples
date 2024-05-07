using Microsoft.AspNetCore.Mvc;
using System.Text;
using MassTransit;
using Events.Contracts;

namespace Publisher.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class MassTransitPublisherController : ControllerBase
    {
        private readonly ILogger<MassTransitPublisherController> _logger;
        private readonly IPublishEndpoint _publishEndpoint;

        public MassTransitPublisherController(ILogger<MassTransitPublisherController> logger,
            IPublishEndpoint publishEndpoint)
        {
            _logger = logger; 
            _publishEndpoint = publishEndpoint;
        }

        [HttpPost("publish")]
        public async Task Publish(string message)
        {
            await _publishEndpoint.Publish<CustomEvent>(new
            {
                message = message
            });
        }
    }
}
