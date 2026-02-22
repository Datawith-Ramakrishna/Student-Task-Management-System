using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using StudentTaskManager.Domain.Enums;

namespace StudentTaskManager.Application.DTOs.Tasks;

public sealed class TaskStatusUpdateRequest
{
    public TaskItemStatus Status { get; set; }
}