using MusicWeb.src.Models.Entities;

namespace MusicWeb.src.Repositories.Genres;

public interface IGenreRepository
{
    Task<List<Genre>> GetAllAsync(CancellationToken ct);
    Task<Genre?> GetByIdAsync(int id, CancellationToken ct);

    Task AddAsync(Genre entity, CancellationToken ct);
    Task<bool> UpdateNameAsync(int id, string name, CancellationToken ct);
    Task<bool> DeleteAsync(int id, CancellationToken ct);

    Task<int> SaveChangesAsync(CancellationToken ct);
}