using MusicWeb.src.Models.Enums;

namespace MusicWeb.src.Models.Dtos.Users;

public record UserReadDto(int Id, string Username, string Role, DateTime CreatedAt);

public record UserCreateDto(string Username, string Password, Role Role = Role.User);

public record UserUpdateDto(string? Username, Role? Role);

public record UserChangePasswordDto(string NewPassword);

public record UserRoleUpdateDto(Role Role);