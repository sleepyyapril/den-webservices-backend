using Microsoft.AspNetCore.Mvc;

namespace DenWebServices.Backend.Controllers;

[ApiController]
[Route("/denied")]
public class DeniedController : ControllerBase
{
    public IResult Denied()
    {
        return Results.Forbid();
    }
}