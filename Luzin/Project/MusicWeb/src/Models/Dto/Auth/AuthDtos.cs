namespace MusicWeb.src.Models.Dto.Auth
{
    public record AuthResult(int Id, string Username, string Role, string? AccessToken = null, int? ExpiresInSeconds = null);
    public record UserInfo(int Id, string Username, string Role, DateTime CreatedAt);
}
