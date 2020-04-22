using Example.Application;
using Example.Model;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace Example.Api.Controllers
{
    [ApiVersion("1.0")]
    [ApiVersion("2.0")]
    [ApiController]
    [Route("[controller]")]
    public class MessagesController : ControllerBase
    {
        private readonly IMessageRepository repository;
        private readonly MessageProcessingService messageService;
        private readonly ILogger<MessagesController> logger;

        public MessagesController(IMessageRepository repository, MessageProcessingService messageService, ILogger<MessagesController> logger)
        {
            this.repository = repository;
            this.messageService = messageService;
            this.logger = logger;
        }

        [HttpGet("{id}")]
        public ActionResult<Message> Get([FromRoute] int id)
        {
            return Ok(repository.GetById(id));
        }

        [MapToApiVersion("1.0")]
        [HttpPost("{id}")]
        public IActionResult Post([FromRoute] int id)
        {
            messageService.Process(id, "Hello");
            logger.LogInformation("Processed Post for messageId={id}", id);
            return Ok();
        }

        [MapToApiVersion("2.0")]
        [HttpPost("{id}")]
        public IActionResult PostV2([FromRoute] int id)
        {
            messageService.Process(id, "World");
            logger.LogInformation("Processed PostV2 for messageId={id}", id);
            return Ok();
        }


    }
}
