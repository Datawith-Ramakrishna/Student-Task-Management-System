using Microsoft.EntityFrameworkCore;
using StudentTaskManager.Application.Common;
using StudentTaskManager.Application.DTOs.Users;
using StudentTaskManager.Application.Interfaces;
using StudentTaskManager.Domain.Enums;
using StudentTaskManager.Infrastructure.Data;

namespace StudentTaskManager.Infrastructure.Services;

public sealed class UserService : IUserService
{
    private readonly AppDbContext _db;
    public UserService(AppDbContext db) => _db = db;

    public async Task<PagedResult<UserResponse>> GetStudentsAsync(int page, int pageSize)
    {
        page = page <= 0 ? 1 : page;
        pageSize = pageSize <= 0 ? 20 : Math.Min(pageSize, 100);

        var query = _db.Users.AsNoTracking()
            .Where(u => u.Role == UserRole.Student)
            .OrderBy(u => u.FullName);

        var total = await query.CountAsync();

        var items = await query
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .Select(u => new UserResponse
            {
                Id = u.Id,
                FullName = u.FullName,
                Email = u.Email,
                Role = u.Role,
                CreatedAtUtc = u.CreatedAtUtc
            })
            .ToListAsync();

        return new PagedResult<UserResponse>
        {
            Items = items,
            Page = page,
            PageSize = pageSize,
            TotalCount = total
        };
    }
}