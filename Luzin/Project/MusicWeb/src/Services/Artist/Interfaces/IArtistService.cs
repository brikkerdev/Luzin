using MusicWeb.src.Models.Dtos.Artists;

namespace MusicWeb.src.Services.Artist.Interfaces;

public interface IArtistService
{
    Task<List<ArtistReadDto>> GetAllAsync(CancellationToken ct);
    Task<ArtistReadDto> GetByIdAsync(int id, CancellationToken ct);
    Task<ArtistReadDto> CreateAsync(ArtistCreateDto dto, CancellationToken ct);
    Task<ArtistReadDto> CreateWithSongsAsync(ArtistWithSongsCreateDto dto, CancellationToken ct);
    Task UpdateAsync(int id, ArtistUpdateDto dto, CancellationToken ct);
    Task DeleteAsync(int id, CancellationToken ct);
}