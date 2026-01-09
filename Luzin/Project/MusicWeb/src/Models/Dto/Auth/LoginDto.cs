using System.ComponentModel.DataAnnotations;

namespace MusicWeb.src.Models.Dtos.Auth;

public sealed class LoginDto
{
    [Required]
    public string Username { get; set; } = string.Empty;

    [Required]
    public string Password { get; set; } = string.Empty;
}