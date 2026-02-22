using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using StudentTaskManager.Domain.Entities;

namespace StudentTaskManager.Infrastructure.Auth;

public interface IJwtTokenGenerator
{
    string GenerateToken(User user);
}
