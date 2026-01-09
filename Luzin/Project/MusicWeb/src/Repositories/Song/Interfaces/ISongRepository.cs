using MusicWeb.src.Models.Dtos.Common;
using MusicWeb.src.Models.Dtos.Songs;
using MusicWeb.src.Models.Entities;

namespace MusicWeb.src.Repositories.Songs;

public interface ISongRepository
{
    Task<List<Song>> GetAllAsync(CancellationToken ct);
    Task<Song?> GetByIdAsync(int id, CancellationToken ct);
    Task AddAsync(Song entity, CancellationToken ct);
    Task<bool> UpdateAsync(int id, Action<Song> apply, CancellationToken ct);
    Task<bool> DeleteAsync(int id, CancellationToken ct);
    Task<int> SaveChangesAsync(CancellationToken ct);
    Task<(List<Song> Items, int Total)> GetPagedAsync(SongFilterQuery query, CancellationToken ct);
}