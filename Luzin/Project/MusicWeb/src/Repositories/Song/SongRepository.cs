using Microsoft.EntityFrameworkCore;
using MusicWeb.src.Data;
using MusicWeb.src.Models.Dtos.Songs;
using MusicWeb.src.Models.Entities;

namespace MusicWeb.src.Repositories.Songs;

public sealed class SongRepository : ISongRepository
{
    private readonly ApiDbContext _db;

    public SongRepository(ApiDbContext db)
    {
        _db = db;
    }

    public async Task<List<Song>> GetAllAsync(CancellationToken ct)
    {
        return await _db.Songs
            .AsNoTracking()
            .Include(s => s.Artist)
            .Include(s => s.SongGenres)
                .ThenInclude(sg => sg.Genre)
            .ToListAsync(ct);
    }

    public async Task<(List<Song> Items, int Total)> GetPagedAsync(SongFilterQuery query, CancellationToken ct)
    {
        var baseQuery = _db.Songs
            .AsNoTracking()
            .Include(s => s.Artist)
            .Include(s => s.SongGenres)
                .ThenInclude(sg => sg.Genre)
            .AsQueryable();

        baseQuery = ApplyFilters(baseQuery, query);

        var total = await baseQuery.CountAsync(ct);

        baseQuery = ApplySorting(baseQuery, query);

        var items = await baseQuery
            .Skip(query.Skip)
            .Take(query.PageSize)
            .ToListAsync(ct);

        return (items, total);
    }

    private static IQueryable<Song> ApplyFilters(IQueryable<Song> query, SongFilterQuery filter)
    {
        if (!string.IsNullOrWhiteSpace(filter.Search))
        {
            var searchLower = filter.Search.Trim().ToLower();
            query = query.Where(s =>
                s.Title.ToLower().Contains(searchLower) ||
                s.Text.ToLower().Contains(searchLower) ||
                (s.Artist != null && s.Artist.Name.ToLower().Contains(searchLower)));
        }

        if (filter.ArtistId.HasValue && filter.ArtistId.Value > 0)
        {
            query = query.Where(s => s.ArtistId == filter.ArtistId.Value);
        }

        if (filter.GenreId.HasValue && filter.GenreId.Value > 0)
        {
            query = query.Where(s => s.SongGenres.Any(sg => sg.GenreId == filter.GenreId.Value));
        }

        return query;
    }

    private static IQueryable<Song> ApplySorting(IQueryable<Song> query, SongFilterQuery filter)
    {
        var sortBy = filter.SortBy?.ToLower() ?? "id";
        var descending = filter.IsDescending;

        return sortBy switch
        {
            "title" => descending
                ? query.OrderByDescending(s => s.Title)
                : query.OrderBy(s => s.Title),

            "artist" => descending
                ? query.OrderByDescending(s => s.Artist != null ? s.Artist.Name : "")
                : query.OrderBy(s => s.Artist != null ? s.Artist.Name : ""),

            _ => descending
                ? query.OrderByDescending(s => s.Id)
                : query.OrderBy(s => s.Id)
        };
    }

    public async Task<Song?> GetByIdAsync(int id, CancellationToken ct)
    {
        return await _db.Songs
            .AsNoTracking()
            .Include(s => s.Artist)
            .Include(s => s.SongGenres)
                .ThenInclude(sg => sg.Genre)
            .FirstOrDefaultAsync(s => s.Id == id, ct);
    }

    public Task AddAsync(Song entity, CancellationToken ct) =>
        _db.Songs.AddAsync(entity, ct).AsTask();

    public async Task<bool> UpdateAsync(int id, Action<Song> apply, CancellationToken ct)
    {
        var entity = await _db.Songs.FirstOrDefaultAsync(s => s.Id == id, ct);
        if (entity is null) return false;

        apply(entity);
        return true;
    }

    public async Task<bool> DeleteAsync(int id, CancellationToken ct)
    {
        var entity = await _db.Songs.FirstOrDefaultAsync(s => s.Id == id, ct);
        if (entity is null) return false;

        _db.Songs.Remove(entity);
        return true;
    }

    public Task<int> SaveChangesAsync(CancellationToken ct) =>
        _db.SaveChangesAsync(ct);
}