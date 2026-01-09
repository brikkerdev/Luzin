using MusicWeb.src.Models.Entities;

namespace MusicWeb.Services.Auth;

public interface ITokenService
{
    string CreateAccessToken(User user);
    int AccessTokenLifetimeSeconds { get; }
}