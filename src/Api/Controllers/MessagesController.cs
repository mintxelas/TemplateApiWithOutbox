using System;
using System.Linq;
using System.Threading.Tasks;
using Asp.Versioning;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Sample.Api.Models;
using Sample.Application.CreateMessage;
using Sample.Application.GetAllMessages;
using Sample.Application.GetMessageById;
using Sample.Application.ProcessMessage;
using Sample.Domain;

namespace Sample.Api.Controllers;

[ApiVersion("1.0")]
[ApiVersion("2.0")]
[ApiController]
[Route("[controller]")]
public class MessagesController(IMessageRepository repository, IMediator mediator, ILogger<MessagesController> logger)
    : ControllerBase
{
    private readonly IMessageRepository repository = repository;

    [HttpGet]
    public async Task<ActionResult<MessageDto[]>> GetAll()
    {
        var request = new GetAllMessagesRequest();
        var response = await mediator.Send(request);
        return Ok(response.Messages.Select(ToDto));
    }

    [HttpGet("{id:guid}")]
    public async Task<ActionResult<MessageDto>> Get([FromRoute] Guid id)
    {
        var request = new GetMessageByIdRequest(id);
        var response = await mediator.Send(request);
        return MessageByIdResponse((dynamic)response);
    }

    [HttpPost]
    [Authorize]
    public async Task<ActionResult<Guid>> Post([FromForm] string text)
    {
        var request = new CreateMessageRequest(text);
        var response = await mediator.Send(request);
        return CreateMessageResponse((dynamic)response);
    }

    [MapToApiVersion("1.0")]
    [HttpPut("process/{id:guid}")]
    public Task<IActionResult> PutV1([FromRoute] Guid id) => Put("v1", id, "Hello");

    [MapToApiVersion("2.0")]
    [HttpPut("process/{id:guid}")]
    public Task<IActionResult> PutV2([FromRoute] Guid id) => Put("v2", id, "World");

    private async Task<IActionResult> Put(string version, Guid id, string textToMatch)
    {
        var request = new ProcessMessageRequest(id, textToMatch);
        var response = await mediator.Send(request);
        logger.LogInformation("Processed Put {version} with result '{description}'.", version, response.Description);
        return MessageProcessResponse((dynamic)response);
    }

    private MessageDto ToDto(Message message)
        => new MessageDto { Id = message.Id, Text = message.Text };

    private IActionResult MessageByIdResponse(MessageByIdNotFoundResponse response)
    {
        return NotFound();
    }

    private IActionResult MessageByIdResponse(GetMessageByIdResponse response)
    {
        return Ok(ToDto(response.Message));
    }

    private IActionResult CreateMessageResponse(CreateMessageSuccessResponse success)
    {
        logger.LogInformation("Created message with id={id} and text={text}", success.Message.Id, success.Message.Text);
        return Ok(success.Message.Id);
    }

    private IActionResult CreateMessageResponse(CreateMessageResponse response)
    {
        logger.LogInformation("Error creating message: {description}", response.Description);
        return BadRequest(response.Description);
    }

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
}