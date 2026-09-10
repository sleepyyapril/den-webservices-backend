using Microsoft.AspNetCore.Mvc;

namespace DenWebServices.Backend.Controllers;

[ApiController]
[Route("/[controller]")]
public class TestController : ControllerBase
{
    [HttpGet]
    public IResult Test()
    {
        return Results.Ok(new { Message = "Hello World!" });
    }
}