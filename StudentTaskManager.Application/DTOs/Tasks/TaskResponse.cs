using StudentTaskManager.Domain.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;


namespace StudentTaskManager.Application.DTOs.Tasks
{
    public sealed class TaskResponse
    {
        public Guid Id { get; init; }
        public string Title { get; init; } = string.Empty;
        public string? Description { get; init; }

        public DateTime? DueDateUtc { get; init; }
        public TaskPriority Priority { get; init; }
        public TaskItemStatus Status { get; init; }

        public Guid AssignedToUserId { get; init; }
        public string AssignedToName { get; init; } = string.Empty;

        public Guid CreatedByUserId { get; init; }
        public string CreatedByName { get; init; } = string.Empty;

        public DateTime CreatedAtUtc { get; init; }
        public DateTime UpdatedAtUtc { get; init; }
    }
}
