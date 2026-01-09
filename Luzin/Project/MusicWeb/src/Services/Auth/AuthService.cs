using Microsoft.EntityFrameworkCore;
using MusicWeb.Services.Auth;
using MusicWeb.src.Data;
using MusicWeb.src.Exceptions;
using MusicWeb.src.Models.Dto.Auth;
using MusicWeb.src.Models.Entities;
using MusicWeb.src.Models.Enums;
using MusicWeb.src.Services.Auth.Interfaces;

namespace MusicWeb.src.Services.Auth;

public sealed class AuthService : IAuthService
{
    private readonly ApiDbContext _db;
    private readonly IPasswordHasher _hasher;
    private readonly ITokenService _tokens;

    public AuthService(ApiDbContext db, IPasswordHasher hasher, ITokenService tokens)
    {
        _db = db;
        _hasher = hasher;
        _tokens = tokens;
    }

    public async Task<AuthResult> RegisterAsync(string username, string password, CancellationToken ct)
    {
        var trimmedUsername = username.Trim();

        var exists = await _db.Users.AnyAsync(u => u.Username == trimmedUsername, ct);
        if (exists)
            throw new ConflictException("User", "username", trimmedUsername);

        var user = new User
        {
            Username = trimmedUsername,
            PasswordHash = _hasher.Hash(password),
            Role = Role.User,
            CreatedAt = DateTime.UtcNow
        };

        _db.Users.Add(user);
        await _db.SaveChangesAsync(ct);

        return new AuthResult(user.Id, user.Username, user.Role.ToString());
    }

    public async Task<AuthResult> LoginAsync(string username, string password, CancellationToken ct)
    {
        var trimmedUsername = username.Trim();

        var user = await _db.Users.FirstOrDefaultAsync(u => u.Username == trimmedUsername, ct);
        if (user is null || !_hasher.Verify(password, user.PasswordHash))
            throw new UnauthorizedException("Invalid username or password.");

        var token = _tokens.CreateAccessToken(user);

        return new AuthResult(
            user.Id,
            user.Username,
            user.Role.ToString(),
            token,
            _tokens.AccessTokenLifetimeSeconds
        );
    }

    public async Task<UserInfo> GetCurrentUserAsync(int userId, CancellationToken ct)
    {
        var user = await _db.Users.FindAsync(new object[] { userId }, ct)
            ?? throw new NotFoundException("User", userId);

        return new UserInfo(user.Id, user.Username, user.Role.ToString(), user.CreatedAt);
    }
}