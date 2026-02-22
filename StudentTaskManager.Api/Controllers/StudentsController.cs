using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using StudentTaskManager.Application.Interfaces;

namespace StudentTaskManager.Api.Controllers;

[Route("api/students")]
[Authorize(Roles = "Admin")]
public sealed class StudentsController : ControllerBase
{
    private readonly IUserService _users;

    public StudentsController(IUserService users) => _users = users;

    [HttpGet]
    public async Task<IActionResult> GetStudents([FromQuery] int page = 1, [FromQuery] int pageSize = 20)
    {
        var result = await _users.GetStudentsAsync(page, pageSize);
        return Ok(result);
    }
}