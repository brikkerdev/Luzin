using MusicWeb.src.Models.Dto.Auth;

namespace MusicWeb.src.Services.Auth.Interfaces;

public interface IAuthService
{
    Task<AuthResult> RegisterAsync(string username, string password, CancellationToken ct);
    Task<AuthResult> LoginAsync(string username, string password, CancellationToken ct);
    Task<UserInfo> GetCurrentUserAsync(int userId, CancellationToken ct);
}