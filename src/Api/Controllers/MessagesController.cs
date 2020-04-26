using MediatR;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
using System.Linq;
using System.Threading.Tasks;
using Template.Api.Models;
using Template.Application.CreateMessage;
using Template.Application.ProcessMessage;
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
        private readonly IMediator mediator;
        private readonly ILogger<MessagesController> logger;

        public MessagesController(IMessageRepository repository, IMediator mediator, ILogger<MessagesController> logger)
        {
            this.repository = repository;
            this.mediator = mediator;
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
            var request = new CreateMessageRequest(text);
            var response = await mediator.Send(request);
            if (response is CreateMessageSuccessResponse success)
            {
                logger.LogInformation("Created message with id={id} and text={text}", success.Message.Id, success.Message.Text);
                return Ok(success.Message.Id);
            }

            return BadRequest(response.Description);
        }

        [MapToApiVersion("1.0")]
        [HttpPut("process/{id}")]
        public Task<IActionResult> PutV1([FromRoute] Guid id) => Put(id, "Hello");

        [MapToApiVersion("2.0")]
        [HttpPut("process/{id}")]
        public Task<IActionResult> PutV2([FromRoute] Guid id) => Put(id, "World");

        private async Task<IActionResult> Put(Guid id, string textToMatch)
        {
            var request = new ProcessMessageRequest(id, textToMatch);
            var response = await mediator.Send(request);
            if (response is MessageToProcessNotFoundResponse)
            {
                return NotFound();
            }

            if (response is ErrorProcessingMessageResponse error)
            {
                return BadRequest(error.Description);
            }

            logger.LogInformation("Processed PutV1 for messageId={id}", id);
            return Ok();
        }

        private MessageDto ToDto(Message message)
            => new MessageDto { id = message.Id, text = message.Text };
    }
}
