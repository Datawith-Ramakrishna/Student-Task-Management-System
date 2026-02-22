using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using StudentTaskManager.Application.DTOs.Tasks;
using StudentTaskManager.Application.Interfaces;
using StudentTaskManager.Domain.Enums;
using System.Security.Claims;

namespace StudentTaskManager.Api.Controllers;

[Route("api/tasks")]
public sealed class TasksController : ControllerBase
{
    private readonly ITaskService _tasks;

    public TasksController(ITaskService tasks) => _tasks = tasks;

    private Guid CurrentUserId()
    {
        var id = User.FindFirstValue(ClaimTypes.NameIdentifier) ?? User.FindFirstValue("sub");
        if (id is null) throw new UnauthorizedAccessException("Missing user id.");
        return Guid.Parse(id);
    }

    private bool IsAdmin() => User.IsInRole("Admin");

    // ADMIN: Create task & assign
    [HttpPost]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> Create([FromBody] TaskCreateRequest request)
    {
        var created = await _tasks.CreateAsync(CurrentUserId(), request);
        return CreatedAtAction(nameof(GetById), new { id = created.Id }, created);
    }

    // ADMIN: Update any task
    [HttpPut("{id:guid}")]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> Update([FromRoute] Guid id, [FromBody] TaskUpdateRequest request)
    {
        var updated = await _tasks.UpdateAsync(CurrentUserId(), id, request);
        return Ok(updated);
    }

    // ADMIN: Soft delete
    [HttpDelete("{id:guid}")]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> Delete([FromRoute] Guid id)
    {
        await _tasks.SoftDeleteAsync(CurrentUserId(), id);
        return NoContent();
    }

    // ADMIN: List all tasks with filters
    [HttpGet]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> GetAll(
        [FromQuery] TaskItemStatus? status,
        [FromQuery] TaskPriority? priority,
        [FromQuery] Guid? assignedToUserId,
        [FromQuery] DateTime? dueBeforeUtc,
        [FromQuery] int page = 1,
        [FromQuery] int pageSize = 20)
    {
        var result = await _tasks.GetAllTasksAsync(status, priority, assignedToUserId, dueBeforeUtc, page, pageSize);
        return Ok(result);
    }

    // STUDENT: My tasks
    [HttpGet("my")]
    [Authorize]
    public async Task<IActionResult> GetMy(
        [FromQuery] TaskItemStatus? status,
        [FromQuery] TaskPriority? priority,
        [FromQuery] DateTime? dueBeforeUtc,
        [FromQuery] int page = 1,
        [FromQuery] int pageSize = 20)
    {
        var result = await _tasks.GetMyTasksAsync(CurrentUserId(), status, priority, dueBeforeUtc, page, pageSize);
        return Ok(result);
    }

    // BOTH: Get task by id (admin OR assigned student)
    [HttpGet("{id:guid}")]
    [Authorize]
    public async Task<IActionResult> GetById([FromRoute] Guid id)
    {
        var result = await _tasks.GetByIdAsync(CurrentUserId(), IsAdmin(), id);
        return Ok(result);
    }

    // STUDENT: Update my task status
    [HttpPatch("{id:guid}/status")]
    [Authorize(Roles = "Student,Admin")]
    public async Task<IActionResult> UpdateStatus([FromRoute] Guid id, [FromBody] TaskStatusUpdateRequest request)
    {
        if (IsAdmin())
        {
            // Admin should use PUT in this design; keeping this strict helps “business rules”
            throw new InvalidOperationException("Admin should update status via PUT /api/tasks/{id}.");
        }

        var result = await _tasks.UpdateMyTaskStatusAsync(CurrentUserId(), id, request.Status);
        return Ok(result);
    }
}