using MusicWeb.src.Models.Dtos.Artists;
using MusicWeb.src.Models.Entities;

namespace MusicWeb.src.Repositories;

public interface IArtistRepository
{
    Task<List<Artist>> GetAllAsync(CancellationToken ct);
    Task<Artist?> GetByIdAsync(int id, CancellationToken ct);
    Task<List<ArtistReadDto>> GetAllWithSongCountAsync(CancellationToken ct);
    Task<ArtistReadDto?> GetByIdWithSongCountAsync(int id, CancellationToken ct);
    Task AddAsync(Artist entity, CancellationToken ct);
    Task<bool> UpdateNameAsync(int id, string name, CancellationToken ct);
    Task<bool> DeleteAsync(int id, CancellationToken ct);
    Task<int> SaveChangesAsync(CancellationToken ct);
    Task<Artist> CreateArtistWithSongsAsync(Artist artist, List<Song> songs, CancellationToken ct);
}