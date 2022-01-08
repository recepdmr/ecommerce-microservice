using IdentityService.Api.Application.Models;
using IdentityService.Api.Application.Services;
using Microsoft.AspNetCore.Mvc;

namespace IdentityService.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class AuthController : ControllerBase
{
    public AuthController(IIdentityService identityService)
    {
        IdentityService = identityService;
    }

    public IIdentityService IdentityService { get; }

    [HttpPost]
    public async Task<IActionResult> Post([FromBody] LoginRequestModel loginRequestModel)
    {
        var result = await IdentityService.LoginAsync(loginRequestModel);
        return Ok(result);
    }
}
