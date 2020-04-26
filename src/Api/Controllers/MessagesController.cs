using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Template.Api.Models;
using Template.Application;
using Template.Domain;

namespace Template.Api.Controllers
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

        [HttpGet]
        public async Task<ActionResult<MessageDto>> GetAll()
        {
            var messages = await repository.GetAll();
            if (messages is null)
                return NotFound();
            return Ok(messages.Select(ToDto));
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<MessageDto>> Get([FromRoute] Guid id)
        {
            var message = await repository.GetById(id);
            if (message is null)
                return NotFound();
            return Ok(ToDto(message));
        }

        [HttpPost]
        public async Task<ActionResult<Guid>> Post([FromForm] string text)
        {
            var message = await messageService.Create(text);
            logger.LogInformation("Created message with id={id} and text={text}", message.Id, message.Text);
            return Ok(ToDto(message));
        }

        [MapToApiVersion("1.0")]
        [HttpPut("process/{id}")]
        public async Task<IActionResult> PutV1([FromRoute] Guid id)
        {
            await messageService.Process(id, "Hello");
            logger.LogInformation("Processed PutV1 for messageId={id}", id);
            return Ok();
        }

        [MapToApiVersion("2.0")]
        [HttpPut("process/{id}")]
        public async Task<IActionResult> PutV2([FromRoute] Guid id)
        {
            await messageService.Process(id, "World");
            logger.LogInformation("Processed PutV2 for messageId={id}", id);
            return Ok();
        }

        private MessageDto ToDto(Message message)
            => new MessageDto { id = message.Id, text = message.Text };
    }
}
