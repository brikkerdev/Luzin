using Microsoft.EntityFrameworkCore;
using MusicWeb.Services.Auth;
using MusicWeb.src.Data;
using MusicWeb.src.Exceptions;
using MusicWeb.src.Models.Dtos.Users;
using MusicWeb.src.Models.Entities;
using MusicWeb.src.Models.Enums;
using MusicWeb.src.Services.Users.Interfaces;

namespace MusicWeb.src.Services.Users;

public sealed class UserService : IUserService
{
    private readonly ApiDbContext _db;
    private readonly IPasswordHasher _hasher;

    public UserService(ApiDbContext db, IPasswordHasher hasher)
    {
        _db = db;
        _hasher = hasher;
    }

    public async Task<List<UserReadDto>> GetAllAsync(CancellationToken ct)
    {
        return await _db.Users
            .AsNoTracking()
            .Select(u => new UserReadDto(u.Id, u.Username, u.Role.ToString(), u.CreatedAt))
            .ToListAsync(ct);
    }

    public async Task<UserReadDto> GetByIdAsync(int id, CancellationToken ct)
    {
        var user = await _db.Users.FindAsync(new object[] { id }, ct)
            ?? throw new NotFoundException("User", id);

        return new UserReadDto(user.Id, user.Username, user.Role.ToString(), user.CreatedAt);
    }

    public async Task<UserReadDto> CreateAsync(UserCreateDto dto, CancellationToken ct)
    {
        var trimmedUsername = dto.Username.Trim();

        var exists = await _db.Users.AnyAsync(u => u.Username == trimmedUsername, ct);
        if (exists)
            throw new ConflictException("User", "username", dto.Username);

        var user = new User
        {
            Username = trimmedUsername,
            PasswordHash = _hasher.Hash(dto.Password),
            Role = dto.Role,
            CreatedAt = DateTime.UtcNow
        };

        _db.Users.Add(user);
        await _db.SaveChangesAsync(ct);

        return new UserReadDto(user.Id, user.Username, user.Role.ToString(), user.CreatedAt);
    }

    public async Task UpdateAsync(int id, UserUpdateDto dto, CancellationToken ct)
    {
        var user = await _db.Users.FindAsync(new object[] { id }, ct)
            ?? throw new NotFoundException("User", id);

        if (!string.IsNullOrWhiteSpace(dto.Username))
        {
            var trimmedUsername = dto.Username.Trim();
            var exists = await _db.Users.AnyAsync(u => u.Username == trimmedUsername && u.Id != id, ct);
            if (exists)
                throw new ConflictException("User", "username", dto.Username);

            user.Username = trimmedUsername;
        }

        if (dto.Role.HasValue)
            user.Role = dto.Role.Value;

        user.UpdatedAt = DateTime.UtcNow;
        await _db.SaveChangesAsync(ct);
    }

    public async Task UpdateRoleAsync(int id, Role role, CancellationToken ct)
    {
        var user = await _db.Users.FindAsync(new object[] { id }, ct)
            ?? throw new NotFoundException("User", id);

        user.Role = role;
        user.UpdatedAt = DateTime.UtcNow;
        await _db.SaveChangesAsync(ct);
    }

    public async Task ChangePasswordAsync(int id, string newPassword, CancellationToken ct)
    {
        var user = await _db.Users.FindAsync(new object[] { id }, ct)
            ?? throw new NotFoundException("User", id);

        user.PasswordHash = _hasher.Hash(newPassword);
        user.UpdatedAt = DateTime.UtcNow;
        await _db.SaveChangesAsync(ct);
    }

    public async Task DeleteAsync(int id, CancellationToken ct)
    {
        var user = await _db.Users.FindAsync(new object[] { id }, ct)
            ?? throw new NotFoundException("User", id);

        _db.Users.Remove(user);
        await _db.SaveChangesAsync(ct);
    }
}