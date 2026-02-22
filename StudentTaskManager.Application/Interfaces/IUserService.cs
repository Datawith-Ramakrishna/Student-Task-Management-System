using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using StudentTaskManager.Application.Common;
using StudentTaskManager.Application.DTOs.Users;

namespace StudentTaskManager.Application.Interfaces;

public interface IUserService
{
    Task<PagedResult<UserResponse>> GetStudentsAsync(int page, int pageSize);
}
