using Example.Application;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace Example.Api.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class MessagesController : ControllerBase
    {
        private readonly MessageProcessingService messageService;
        private readonly ILogger<MessagesController> logger;

        public MessagesController(MessageProcessingService messageService, ILogger<MessagesController> logger)
        {
            this.messageService = messageService;
            this.logger = logger;
        }

        [HttpPost("{id}")]
        public void Post(int id)
        {
            messageService.Process(id, "Hello");
            logger.LogInformation("Processed Post for messageId={id}", id);
        }
    }
}
