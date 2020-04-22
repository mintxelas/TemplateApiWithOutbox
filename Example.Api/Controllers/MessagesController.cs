using Example.Api.Models;
using Example.Application;
using Example.Domain;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
using System.Threading.Tasks;

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
        public async Task<ActionResult<MessageDto>> Get([FromRoute] int id)
        {
            var message = await repository.GetById(id);
            return Ok(ToDto(message));
        }

        [MapToApiVersion("1.0")]
        [HttpPost("{id}")]
        public async Task<IActionResult> Post([FromRoute] int id)
        {
            await messageService.Process(id, "Hello");
            logger.LogInformation("Processed PostV1 for messageId={id}", id);
            return Ok();
        }

        [MapToApiVersion("2.0")]
        [HttpPost("{id}")]
        public async Task<IActionResult> PostV2([FromRoute] int id)
        {
            await messageService.Process(id, "World");
            logger.LogInformation("Processed PostV2 for messageId={id}", id);
            return Ok();
        }

        private MessageDto ToDto(Message message)
            => new MessageDto { id = message.Id, text = message.Text };
    }
}
