
using Microsoft.AspNetCore.Mvc;

namespace WebApiMessages.Samples.Controllers;

[Route("oops")]
public class OopsController : ControllerBase
{
    [HttpGet]
    public void Oops()
    {
        throw new InvalidOperationException("Oops, something wrong.");
    }
}