using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using StudentTaskManager.Application.DTOs.Auth;
using StudentTaskManager.Application.Interfaces;
using StudentTaskManager.Domain.Entities;
using StudentTaskManager.Domain.Enums;
using StudentTaskManager.Infrastructure.Auth;
using StudentTaskManager.Infrastructure.Data;

namespace StudentTaskManager.Infrastructure.Services;

public sealed class AuthService : IAuthService
{
    private readonly AppDbContext _db;
    private readonly IJwtTokenGenerator _jwt;
    private readonly IPasswordHasher<User> _hasher;

    public AuthService(AppDbContext db, IJwtTokenGenerator jwt, IPasswordHasher<User> hasher)
    {
        _db = db;
        _jwt = jwt;
        _hasher = hasher;
    }

    public async Task RegisterStudentAsync(RegisterRequest request)
    {
        var email = request.Email.Trim().ToLowerInvariant();

        var exists = await _db.Users.AnyAsync(u => u.Email.ToLower() == email);
        if (exists) throw new InvalidOperationException("Email is already registered.");

        var user = new User
        {
            FullName = request.FullName.Trim(),
            Email = email,
            Role = UserRole.Student
        };

        user.PasswordHash = _hasher.HashPassword(user, request.Password);

        _db.Users.Add(user);
        await _db.SaveChangesAsync();
    }

    public async Task<AuthResponse> LoginAsync(LoginRequest request)
    {
        var email = request.Email.Trim().ToLowerInvariant();

        var user = await _db.Users.FirstOrDefaultAsync(u => u.Email.ToLower() == email);
        if (user is null) throw new UnauthorizedAccessException("Invalid credentials.");

        var result = _hasher.VerifyHashedPassword(user, user.PasswordHash, request.Password);
        if (result == PasswordVerificationResult.Failed)
            throw new UnauthorizedAccessException("Invalid credentials.");

        var token = _jwt.GenerateToken(user);
        return new AuthResponse { AccessToken = token };
    }
}