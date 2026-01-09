using MusicWeb.src.Models.Dtos.Genres;

namespace MusicWeb.src.Services.Genres.Interfaces;

public interface IGenreService
{
    Task<List<GenreReadDto>> GetAllAsync(CancellationToken ct);
    Task<GenreReadDto> GetByIdAsync(int id, CancellationToken ct);
    Task<GenreReadDto> CreateAsync(GenreCreateDto dto, CancellationToken ct);
    Task UpdateAsync(int id, GenreUpdateDto dto, CancellationToken ct);
    Task DeleteAsync(int id, CancellationToken ct);
}