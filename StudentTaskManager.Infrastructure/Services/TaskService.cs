using Microsoft.EntityFrameworkCore;
using StudentTaskManager.Application.Common;
using StudentTaskManager.Application.DTOs.Tasks;
using StudentTaskManager.Application.Interfaces;
using StudentTaskManager.Domain.Entities;
using StudentTaskManager.Domain.Enums;
using StudentTaskManager.Infrastructure.Data;
using System;

namespace StudentTaskManager.Application.Services;

public sealed class TaskService : ITaskService
{
    private readonly AppDbContext _db;

    public TaskService(AppDbContext db) => _db = db;

    public async Task<TaskResponse> CreateAsync(Guid adminUserId, TaskCreateRequest request)
    {
        // ensure assigned user exists
        var assigned = await _db.Users.FirstOrDefaultAsync(u => u.Id == request.AssignedToUserId);
        if (assigned is null) throw new InvalidOperationException("Assigned student not found.");

        var task = new TaskItem
        {
            Title = request.Title.Trim(),
            Description = request.Description?.Trim(),
            DueDateUtc = request.DueDateUtc,
            Priority = request.Priority,
            Status = TaskItemStatus.Todo,
            AssignedToUserId = request.AssignedToUserId,
            CreatedByUserId = adminUserId,
            CreatedAtUtc = DateTime.UtcNow,
            UpdatedAtUtc = DateTime.UtcNow
        };

        _db.Tasks.Add(task);
        await _db.SaveChangesAsync();

        return await MapTaskAsync(task.Id);
    }

    public async Task<TaskResponse> UpdateAsync(Guid adminUserId, Guid taskId, TaskUpdateRequest request)
    {
        var task = await _db.Tasks.FirstOrDefaultAsync(t => t.Id == taskId);
        if (task is null) throw new KeyNotFoundException("Task not found.");

        var assigned = await _db.Users.FirstOrDefaultAsync(u => u.Id == request.AssignedToUserId);
        if (assigned is null) throw new InvalidOperationException("Assigned student not found.");

        task.Title = request.Title.Trim();
        task.Description = request.Description?.Trim();
        task.DueDateUtc = request.DueDateUtc;
        task.Priority = request.Priority;
        task.Status = request.Status;
        task.AssignedToUserId = request.AssignedToUserId;
        task.UpdatedAtUtc = DateTime.UtcNow;

        await _db.SaveChangesAsync();
        return await MapTaskAsync(task.Id);
    }

    public async Task SoftDeleteAsync(Guid adminUserId, Guid taskId)
    {
        var task = await _db.Tasks.FirstOrDefaultAsync(t => t.Id == taskId);
        if (task is null) throw new KeyNotFoundException("Task not found.");

        task.IsDeleted = true;
        task.UpdatedAtUtc = DateTime.UtcNow;
        await _db.SaveChangesAsync();
    }

    public async Task<TaskResponse> GetByIdAsync(Guid requesterUserId, bool isAdmin, Guid taskId)
    {
        var query = _db.Tasks.AsNoTracking()
            .Where(t => t.Id == taskId)
            .Include(t => t.AssignedToUser)
            .Include(t => t.CreatedByUser);

        var task = await query.FirstOrDefaultAsync();
        if (task is null) throw new KeyNotFoundException("Task not found.");

        if (!isAdmin && task.AssignedToUserId != requesterUserId)
            throw new UnauthorizedAccessException("You do not have access to this task.");

        return Map(task);
    }

    public async Task<PagedResult<TaskResponse>> GetMyTasksAsync(
        Guid studentUserId,
        TaskItemStatus? status,
        TaskPriority? priority,
        DateTime? dueBeforeUtc,
        int page,
        int pageSize)
    {
        page = page <= 0 ? 1 : page;
        pageSize = pageSize <= 0 ? 20 : Math.Min(pageSize, 100);

        var query = _db.Tasks
            .AsNoTracking()
            .Include(t => t.AssignedToUser)
            .Include(t => t.CreatedByUser)
            .Where(t => t.AssignedToUserId == studentUserId);

        if (status is not null) query = query.Where(t => t.Status == status);
        if (priority is not null) query = query.Where(t => t.Priority == priority);
        if (dueBeforeUtc is not null) query = query.Where(t => t.DueDateUtc != null && t.DueDateUtc < dueBeforeUtc);

        query = query.OrderBy(t => t.DueDateUtc ?? DateTime.MaxValue).ThenByDescending(t => t.Priority);

        var total = await query.CountAsync();
        var items = await query.Skip((page - 1) * pageSize).Take(pageSize).ToListAsync();

        return new PagedResult<TaskResponse>
        {
            Items = items.Select(Map).ToList(),
            Page = page,
            PageSize = pageSize,
            TotalCount = total
        };
    }

    public async Task<PagedResult<TaskResponse>> GetAllTasksAsync(
        TaskItemStatus? status,
        TaskPriority? priority,
        Guid? assignedToUserId,
        DateTime? dueBeforeUtc,
        int page,
        int pageSize)
    {
        page = page <= 0 ? 1 : page;
        pageSize = pageSize <= 0 ? 20 : Math.Min(pageSize, 100);

        var query = _db.Tasks
            .AsNoTracking()
            .Include(t => t.AssignedToUser)
            .Include(t => t.CreatedByUser)
            .AsQueryable();

        if (status is not null) query = query.Where(t => t.Status == status);
        if (priority is not null) query = query.Where(t => t.Priority == priority);
        if (assignedToUserId is not null) query = query.Where(t => t.AssignedToUserId == assignedToUserId);
        if (dueBeforeUtc is not null) query = query.Where(t => t.DueDateUtc != null && t.DueDateUtc < dueBeforeUtc);

        query = query.OrderBy(t => t.DueDateUtc ?? DateTime.MaxValue).ThenByDescending(t => t.Priority);

        var total = await query.CountAsync();
        var items = await query.Skip((page - 1) * pageSize).Take(pageSize).ToListAsync();

        return new PagedResult<TaskResponse>
        {
            Items = items.Select(Map).ToList(),
            Page = page,
            PageSize = pageSize,
            TotalCount = total
        };
    }

    public async Task<TaskResponse> UpdateMyTaskStatusAsync(Guid studentUserId, Guid taskId, TaskItemStatus newStatus)
    {
        var task = await _db.Tasks.FirstOrDefaultAsync(t => t.Id == taskId);
        if (task is null) throw new KeyNotFoundException("Task not found.");

        if (task.AssignedToUserId != studentUserId)
            throw new UnauthorizedAccessException("You can only update your own tasks.");

        task.Status = newStatus;
        task.UpdatedAtUtc = DateTime.UtcNow;

        await _db.SaveChangesAsync();
        return await MapTaskAsync(task.Id);
    }

    private async Task<TaskResponse> MapTaskAsync(Guid taskId)
    {
        var task = await _db.Tasks
            .AsNoTracking()
            .Include(t => t.AssignedToUser)
            .Include(t => t.CreatedByUser)
            .FirstAsync(t => t.Id == taskId);

        return Map(task);
    }

    private static TaskResponse Map(TaskItem t) => new()
    {
        Id = t.Id,
        Title = t.Title,
        Description = t.Description,
        DueDateUtc = t.DueDateUtc,
        Priority = t.Priority,
        Status = t.Status,
        AssignedToUserId = t.AssignedToUserId,
        AssignedToName = t.AssignedToUser?.FullName ?? "",
        CreatedByUserId = t.CreatedByUserId,
        CreatedByName = t.CreatedByUser?.FullName ?? "",
        CreatedAtUtc = t.CreatedAtUtc,
        UpdatedAtUtc = t.UpdatedAtUtc
    };
}