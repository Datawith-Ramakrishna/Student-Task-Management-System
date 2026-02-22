using StudentTaskManager.Domain.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StudentTaskManager.Application.DTOs.Tasks
{
    public sealed class TaskCreateRequest
    {
        public string Title { get; set; } = string.Empty;
        public string? Description { get; set; }
        public DateTime? DueDateUtc { get; set; }
        public TaskPriority Priority { get; set; } = TaskPriority.Medium;
        public Guid AssignedToUserId { get; set; }
    }
}
