using StudentTaskManager.Application.Common;
using StudentTaskManager.Application.DTOs.Tasks;
using StudentTaskManager.Domain.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StudentTaskManager.Application.Interfaces
{
    public interface ITaskService
    {
        Task<TaskResponse> CreateAsync(Guid adminUserId, TaskCreateRequest request);
        Task<TaskResponse> UpdateAsync(Guid adminUserId, Guid taskId, TaskUpdateRequest request);
        Task SoftDeleteAsync(Guid adminUserId, Guid taskId);

        Task<TaskResponse> GetByIdAsync(Guid requesterUserId, bool isAdmin, Guid taskId);

        Task<PagedResult<TaskResponse>> GetMyTasksAsync(
            Guid studentUserId,
            TaskItemStatus? status,
            TaskPriority? priority,
            DateTime? dueBeforeUtc,
            int page,
            int pageSize);

        Task<PagedResult<TaskResponse>> GetAllTasksAsync(
            TaskItemStatus? status,
            TaskPriority? priority,
            Guid? assignedToUserId,
            DateTime? dueBeforeUtc,
            int page,
            int pageSize);

        Task<TaskResponse> UpdateMyTaskStatusAsync(Guid studentUserId, Guid taskId, TaskItemStatus newStatus);
    }
}
