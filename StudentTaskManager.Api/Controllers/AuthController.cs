using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using StudentTaskManager.Application.DTOs.Auth;
using StudentTaskManager.Application.Interfaces;

namespace StudentTaskManager.Api.Controllers;

[Route("api/auth")]
public sealed class AuthController : ControllerBase
{
    private readonly IAuthService _auth;

    public AuthController(IAuthService auth) => _auth = auth;

    [HttpPost("register")]
    [AllowAnonymous]
    public async Task<IActionResult> Register([FromBody] RegisterRequest request)
    {
        await _auth.RegisterStudentAsync(request);
        return Ok(new { message = "Student registered successfully." });
    }

    [HttpPost("login")]
    [AllowAnonymous]
    public async Task<ActionResult<AuthResponse>> Login([FromBody] LoginRequest request)
    {
        var result = await _auth.LoginAsync(request);
        return Ok(result);
    }

    [HttpGet("me")]
    [Authorize]
    public IActionResult Me()
    {
        // robust fallback for different claim mappings
        var id = User.FindFirst("sub")?.Value
                 ?? User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;

        var role = User.FindFirst("role")?.Value
                   ?? User.FindFirst(System.Security.Claims.ClaimTypes.Role)?.Value;

        var name = User.FindFirst("name")?.Value
                   ?? User.FindFirst(System.Security.Claims.ClaimTypes.Name)?.Value;

        var email = User.FindFirst("email")?.Value
                    ?? User.FindFirst("http://schemas.xmlsoap.org/ws/2005/05/identity/claims/emailaddress")?.Value;

        return Ok(new
        {
            user = new { id, name, email, role }
        });
    }
}