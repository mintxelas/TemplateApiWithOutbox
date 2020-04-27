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
            return CreateMessageResponse((dynamic)response);
        }

        [MapToApiVersion("1.0")]
        [HttpPut("process/{id}")]
        public Task<IActionResult> PutV1([FromRoute] Guid id) => Put("v1", id, "Hello");

        [MapToApiVersion("2.0")]
        [HttpPut("process/{id}")]
        public Task<IActionResult> PutV2([FromRoute] Guid id) => Put("v2", id, "World");

        private async Task<IActionResult> Put(string version, Guid id, string textToMatch)
        {
            var request = new ProcessMessageRequest(id, textToMatch);
            var response = await mediator.Send(request);
            logger.LogInformation("Processed Put {version} with result '{description}'.", version, response.Description);
            return MessageProcessResponse((dynamic)response);
        }

        private MessageDto ToDto(Message message)
            => new MessageDto { id = message.Id, text = message.Text };

        private IActionResult MessageProcessResponse(MessageToProcessNotFoundResponse response)
        {
            return NotFound();
        }

        private IActionResult MessageProcessResponse(ErrorProcessingMessageResponse error)
        {
            return BadRequest(error.Description);
        }

        private IActionResult MessageProcessResponse(ProcessMessageResponse response)
        {
            return Ok();
        }

        private IActionResult CreateMessageResponse(CreateMessageSuccessResponse success)
        {
            logger.LogInformation("Created message with id={id} and text={text}", success.Message.Id, success.Message.Text);
            return Ok(success.Message.Id);
        }

        public IActionResult CreateMessageResponse(CreateMessageResponse response)
        {
            logger.LogInformation("Error creating message: {description}", response.Description);
            return BadRequest(response.Description);
        }
    }
}
