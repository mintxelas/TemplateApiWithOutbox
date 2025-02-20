using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Sample.Api.Controllers;

[ApiController]
[Route("[controller]")]
[Authorize]
public class PrivateController: ControllerBase
{
    [HttpGet]
    public IActionResult Get() => Ok("You got in!");
}