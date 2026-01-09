using MusicWeb.src.Models.Dtos.Users;
using MusicWeb.src.Models.Enums;

namespace MusicWeb.src.Services.Users.Interfaces;

public interface IUserService
{
    Task<List<UserReadDto>> GetAllAsync(CancellationToken ct);
    Task<UserReadDto> GetByIdAsync(int id, CancellationToken ct);
    Task<UserReadDto> CreateAsync(UserCreateDto dto, CancellationToken ct);
    Task UpdateAsync(int id, UserUpdateDto dto, CancellationToken ct);
    Task UpdateRoleAsync(int id, Role role, CancellationToken ct);
    Task ChangePasswordAsync(int id, string newPassword, CancellationToken ct);
    Task DeleteAsync(int id, CancellationToken ct);
}