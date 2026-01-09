using System.ComponentModel.DataAnnotations;

namespace MusicWeb.src.Models.Dtos.Auth;

public sealed class RegisterDto
{
    [Required, MaxLength(100)]
    public string Username { get; set; } = string.Empty;

    [Required, MinLength(6), MaxLength(200)]
    public string Password { get; set; } = string.Empty;
}