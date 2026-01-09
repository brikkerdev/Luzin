using Microsoft.EntityFrameworkCore;
using MusicWeb.src.Data;
using MusicWeb.src.Models.Entities;

namespace MusicWeb.src.Repositories.Genres;

public sealed class GenreRepository : IGenreRepository
{
    private readonly ApiDbContext _db;

    public GenreRepository(ApiDbContext db) => _db = db;

    public Task<List<Genre>> GetAllAsync(CancellationToken ct) =>
        _db.Genres.AsNoTracking()
            .OrderBy(g => g.Id)
            .ToListAsync(ct);

    public Task<Genre?> GetByIdAsync(int id, CancellationToken ct) =>
        _db.Genres.AsNoTracking()
            .FirstOrDefaultAsync(g => g.Id == id, ct);

    public Task AddAsync(Genre entity, CancellationToken ct) =>
        _db.Genres.AddAsync(entity, ct).AsTask();

    public async Task<bool> UpdateNameAsync(int id, string name, CancellationToken ct)
    {
        var entity = await _db.Genres.FirstOrDefaultAsync(g => g.Id == id, ct);
        if (entity is null) return false;
        entity.Name = name;
        return true;
    }

    public async Task<bool> DeleteAsync(int id, CancellationToken ct)
    {
        var entity = await _db.Genres.FirstOrDefaultAsync(g => g.Id == id, ct);
        if (entity is null) return false;
        _db.Genres.Remove(entity);
        return true;
    }

    public Task<int> SaveChangesAsync(CancellationToken ct) => _db.SaveChangesAsync(ct);
}