using Microsoft.EntityFrameworkCore;
using MusicWeb.src.Data;
using MusicWeb.src.Exceptions;
using MusicWeb.src.Models.Dtos.Genres;
using MusicWeb.src.Models.Entities;
using MusicWeb.src.Repositories.Genres;
using MusicWeb.src.Services.Genres.Interfaces;

namespace MusicWeb.src.Services.Genres;

public sealed class GenreService : IGenreService
{
    private readonly ApiDbContext _db;
    private readonly IGenreRepository _repo;
    private readonly ILogger<GenreService> _logger;

    public GenreService(ApiDbContext db, IGenreRepository repo, ILogger<GenreService> logger)
    {
        _db = db;
        _repo = repo;
        _logger = logger;
    }

    public async Task<List<GenreReadDto>> GetAllAsync(CancellationToken ct)
    {
        return await _db.Genres.AsNoTracking()
            .OrderBy(g => g.Id)
            .Select(g => new GenreReadDto
            {
                Id = g.Id,
                Name = g.Name,
                SongCount = g.SongGenres.Count()
            })
            .ToListAsync(ct);
    }

    public async Task<GenreReadDto> GetByIdAsync(int id, CancellationToken ct)
    {
        var genre = await _db.Genres.AsNoTracking()
            .Where(g => g.Id == id)
            .Select(g => new GenreReadDto
            {
                Id = g.Id,
                Name = g.Name,
                SongCount = g.SongGenres.Count()
            })
            .FirstOrDefaultAsync(ct);

        return genre ?? throw new NotFoundException("Genre", id);
    }

    public async Task<GenreReadDto> CreateAsync(GenreCreateDto dto, CancellationToken ct)
    {
        var exists = await _db.Genres
            .AnyAsync(g => g.Name.ToLower() == dto.Name.Trim().ToLower(), ct);

        if (exists)
            throw new ConflictException("Genre", "name", dto.Name);

        var entity = new Genre { Name = dto.Name.Trim() };
        await _repo.AddAsync(entity, ct);
        await _repo.SaveChangesAsync(ct);

        return new GenreReadDto { Id = entity.Id, Name = entity.Name, SongCount = 0 };
    }

    public async Task UpdateAsync(int id, GenreUpdateDto dto, CancellationToken ct)
    {
        var duplicate = await _db.Genres
            .AnyAsync(g => g.Id != id && g.Name.ToLower() == dto.Name.Trim().ToLower(), ct);

        if (duplicate)
            throw new ConflictException("Genre", "name", dto.Name);

        var ok = await _repo.UpdateNameAsync(id, dto.Name.Trim(), ct);
        if (!ok)
            throw new NotFoundException("Genre", id);

        await _repo.SaveChangesAsync(ct);
    }

    public async Task DeleteAsync(int id, CancellationToken ct)
    {
        var ok = await _repo.DeleteAsync(id, ct);
        if (!ok)
            throw new NotFoundException("Genre", id);

        await _repo.SaveChangesAsync(ct);
    }
}