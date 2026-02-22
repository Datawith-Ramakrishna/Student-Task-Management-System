using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using StudentTaskManager.Domain.Entities;
using StudentTaskManager.Domain.Enums;

namespace StudentTaskManager.Infrastructure.Data;

public static class DbSeeder
{
    public static async Task SeedAdminAsync(AppDbContext db)
    {
        await db.Database.MigrateAsync();

        var hasAdmin = await db.Users.AnyAsync(u => u.Role == UserRole.Admin);
        if (hasAdmin) return;

        var admin = new User
        {
            FullName = "System Admin",
            Email = "admin@local.test",
            Role = UserRole.Admin
        };

        var hasher = new PasswordHasher<User>();
        admin.PasswordHash = hasher.HashPassword(admin, "Admin123!");

        db.Users.Add(admin);
        await db.SaveChangesAsync();
    }
}
