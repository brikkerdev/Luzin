namespace MusicWeb.src.Models.Dtos.Auth;

public class TokenResponseDto
{
    public string AccessToken { get; set; } = string.Empty;
    public int ExpiresInSeconds { get; set; }
    public string Role { get; set; } = string.Empty;
}