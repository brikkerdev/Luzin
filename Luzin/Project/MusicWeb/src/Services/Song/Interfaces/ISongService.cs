using MusicWeb.src.Models.Dtos.Common;
using MusicWeb.src.Models.Dtos.Songs;

namespace MusicWeb.Services.Song.Interfaces;

public interface ISongService
{
    Task<List<SongReadDto>> GetAllAsync(CancellationToken ct);
    Task<SongReadDto> GetByIdAsync(int id, CancellationToken ct);
    Task<SongReadDto> CreateAsync(SongCreateDto dto, CancellationToken ct);
    Task UpdateAsync(int id, SongUpdateDto dto, CancellationToken ct);
    Task DeleteAsync(int id, CancellationToken ct);
    Task<PagedResult<SongReadDto>> GetPagedAsync(SongFilterQuery query, CancellationToken ct);
    Task SetGenresAsync(int songId, List<int> genreIds, CancellationToken ct);
}