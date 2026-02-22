using System.Security.Claims;
using Microsoft.AspNetCore.Mvc;

namespace StudentTaskManager.Api.Controllers;

[ApiController]
public abstract class BaseController : ControllerBase
{
    protected Guid GetUserId()
    {
        var sub = User.FindFirstValue(ClaimTypes.NameIdentifier) ?? User.FindFirstValue(ClaimTypes.NameIdentifier);
        // In our JWT we used JwtRegisteredClaimNames.Sub; map below in Program.cs.
        sub ??= User.FindFirstValue(ClaimTypes.NameIdentifier);
        sub ??= User.FindFirstValue("sub");

        if (sub is null) throw new UnauthorizedAccessException("Missing user id claim.");
        return Guid.Parse(sub);
    }

    protected bool IsAdmin() => User.IsInRole("Admin");
}