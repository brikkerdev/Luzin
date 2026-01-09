using Microsoft.EntityFrameworkCore;
using MusicWeb.Observability;
using MusicWeb.Services.Caching;
using MusicWeb.Services.Song.Interfaces;
using MusicWeb.src.Data;
using MusicWeb.src.Exceptions;
using MusicWeb.src.Mapping;
using MusicWeb.src.Models.Dtos.Common;
using MusicWeb.src.Models.Dtos.Songs;
using MusicWeb.src.Models.Entities;
using MusicWeb.src.Repositories.Songs;
using System.Diagnostics;

namespace MusicWeb.Services.Songs;

public sealed class SongService : ISongService
{
    private readonly ApiDbContext _db;
    private readonly ISongRepository _repo;
    private readonly IRedisCache _cache;
    private readonly ILogger<SongService> _logger;

    private const string AllSongsKey = "songs:all:v1";
    private static string SongByIdKey(int id) => $"songs:{id}:v1";
    private const string AllArtistsKey = "artists:all:v1";
    private static string ArtistByIdKey(int id) => $"artists:{id}:v1";
    private const string AllGenresKey = "genres:all:v1";
    private static string GenreByIdKey(int id) => $"genres:{id}:v1";

    private static readonly TimeSpan AllSongsTtl = TimeSpan.FromSeconds(30);
    private static readonly TimeSpan SongByIdTtl = TimeSpan.FromMinutes(5);

    public SongService(ApiDbContext db, ISongRepository repo, IRedisCache cache, ILogger<SongService> logger)
    {
        _db = db;
        _repo = repo;
        _cache = cache;
        _logger = logger;
    }

    public async Task<List<SongReadDto>> GetAllAsync(CancellationToken ct)
    {
        var sw = Stopwatch.StartNew();
        try
        {
            var cached = await _cache.GetAsync<List<SongReadDto>>(AllSongsKey, ct);
            if (cached is not null)
            {
                SongMetrics.CacheHits.Add(1, new KeyValuePair<string, object?>("operation", "get_all"));
                return cached;
            }

            SongMetrics.CacheMisses.Add(1, new KeyValuePair<string, object?>("operation", "get_all"));

            var dbSw = Stopwatch.StartNew();
            var songs = await _repo.GetAllAsync(ct);
            dbSw.Stop();
            SongMetrics.DbDurationMs.Record(dbSw.Elapsed.TotalMilliseconds, new KeyValuePair<string, object?>("operation", "get_all"));

            var dtos = songs.Select(s => s.ToReadDto()).ToList();
            await _cache.SetAsync(AllSongsKey, dtos, AllSongsTtl, ct);
            return dtos;
        }
        finally
        {
            sw.Stop();
            SongMetrics.Requests.Add(1, new KeyValuePair<string, object?>("operation", "get_all"));
            SongMetrics.DurationMs.Record(sw.Elapsed.TotalMilliseconds, new KeyValuePair<string, object?>("operation", "get_all"));
        }
    }

    public async Task<SongReadDto> GetByIdAsync(int id, CancellationToken ct)
    {
        var sw = Stopwatch.StartNew();
        try
        {
            var key = SongByIdKey(id);

            var cached = await _cache.GetAsync<SongReadDto>(key, ct);
            if (cached is not null)
            {
                SongMetrics.CacheHits.Add(1, new KeyValuePair<string, object?>("operation", "get_by_id"));
                return cached;
            }

            SongMetrics.CacheMisses.Add(1, new KeyValuePair<string, object?>("operation", "get_by_id"));

            var dbSw = Stopwatch.StartNew();
            var entity = await _repo.GetByIdAsync(id, ct);
            dbSw.Stop();
            SongMetrics.DbDurationMs.Record(dbSw.Elapsed.TotalMilliseconds, new KeyValuePair<string, object?>("operation", "get_by_id"));

            if (entity is null)
                throw new NotFoundException("Song", id);

            var dto = entity.ToReadDto();
            await _cache.SetAsync(key, dto, SongByIdTtl, ct);
            return dto;
        }
        finally
        {
            sw.Stop();
            SongMetrics.Requests.Add(1, new KeyValuePair<string, object?>("operation", "get_by_id"));
            SongMetrics.DurationMs.Record(sw.Elapsed.TotalMilliseconds, new KeyValuePair<string, object?>("operation", "get_by_id"));
        }
    }

    public async Task<SongReadDto> CreateAsync(SongCreateDto dto, CancellationToken ct)
    {
        var sw = Stopwatch.StartNew();
        try
        {
            var artistExists = await _db.Artists.AnyAsync(a => a.Id == dto.ArtistId, ct);
            if (!artistExists)
                throw new BadRequestException($"Artist with id '{dto.ArtistId}' does not exist.");

            var entity = dto.ToEntity();
            await _repo.AddAsync(entity, ct);

            var dbSw = Stopwatch.StartNew();
            await _repo.SaveChangesAsync(ct);
            dbSw.Stop();
            SongMetrics.DbDurationMs.Record(dbSw.Elapsed.TotalMilliseconds, new KeyValuePair<string, object?>("operation", "create"));

            await InvalidateSongCache(entity.Id, entity.ArtistId, ct);

            var created = await _repo.GetByIdAsync(entity.Id, ct) ?? entity;
            var read = created.ToReadDto();
            await _cache.SetAsync(SongByIdKey(read.Id), read, SongByIdTtl, ct);
            return read;
        }
        finally
        {
            sw.Stop();
            SongMetrics.Requests.Add(1, new KeyValuePair<string, object?>("operation", "create"));
            SongMetrics.DurationMs.Record(sw.Elapsed.TotalMilliseconds, new KeyValuePair<string, object?>("operation", "create"));
        }
    }

    public async Task UpdateAsync(int id, SongUpdateDto dto, CancellationToken ct)
    {
        var sw = Stopwatch.StartNew();
        try
        {
            var before = await _repo.GetByIdAsync(id, ct)
                ?? throw new NotFoundException("Song", id);

            var artistExists = await _db.Artists.AnyAsync(a => a.Id == dto.ArtistId, ct);
            if (!artistExists)
                throw new BadRequestException($"Artist with id '{dto.ArtistId}' does not exist.");

            var oldArtistId = before.ArtistId;

            var dbSw = Stopwatch.StartNew();
            var ok = await _repo.UpdateAsync(id, entity => dto.Apply(entity), ct);
            if (!ok)
                throw new NotFoundException("Song", id);

            await _repo.SaveChangesAsync(ct);
            dbSw.Stop();
            SongMetrics.DbDurationMs.Record(dbSw.Elapsed.TotalMilliseconds, new KeyValuePair<string, object?>("operation", "update"));

            await InvalidateSongCache(id, oldArtistId, dto.ArtistId, ct);
        }
        finally
        {
            sw.Stop();
            SongMetrics.Requests.Add(1, new KeyValuePair<string, object?>("operation", "update"));
            SongMetrics.DurationMs.Record(sw.Elapsed.TotalMilliseconds, new KeyValuePair<string, object?>("operation", "update"));
        }
    }

    public async Task DeleteAsync(int id, CancellationToken ct)
    {
        var sw = Stopwatch.StartNew();
        try
        {
            var existing = await _repo.GetByIdAsync(id, ct)
                ?? throw new NotFoundException("Song", id);

            var artistId = existing.ArtistId;

            var dbSw = Stopwatch.StartNew();
            var ok = await _repo.DeleteAsync(id, ct);
            if (!ok)
                throw new NotFoundException("Song", id);

            await _repo.SaveChangesAsync(ct);
            dbSw.Stop();
            SongMetrics.DbDurationMs.Record(dbSw.Elapsed.TotalMilliseconds, new KeyValuePair<string, object?>("operation", "delete"));

            await InvalidateSongCache(id, artistId, ct);
        }
        finally
        {
            sw.Stop();
            SongMetrics.Requests.Add(1, new KeyValuePair<string, object?>("operation", "delete"));
            SongMetrics.DurationMs.Record(sw.Elapsed.TotalMilliseconds, new KeyValuePair<string, object?>("operation", "delete"));
        }
    }

    public async Task<PagedResult<SongReadDto>> GetPagedAsync(SongFilterQuery query, CancellationToken ct)
    {
        var sw = Stopwatch.StartNew();
        try
        {
            var cacheKey = BuildPagedCacheKey(query);

            var cached = await _cache.GetAsync<PagedResult<SongReadDto>>(cacheKey, ct);
            if (cached is not null)
            {
                SongMetrics.CacheHits.Add(1, new KeyValuePair<string, object?>("operation", "get_paged"));
                return cached;
            }

            SongMetrics.CacheMisses.Add(1, new KeyValuePair<string, object?>("operation", "get_paged"));

            var dbSw = Stopwatch.StartNew();
            var (items, total) = await _repo.GetPagedAsync(query, ct);
            dbSw.Stop();
            SongMetrics.DbDurationMs.Record(dbSw.Elapsed.TotalMilliseconds, new KeyValuePair<string, object?>("operation", "get_paged"));

            var result = new PagedResult<SongReadDto>
            {
                Items = items.Select(s => s.ToReadDto()).ToList(),
                Total = total,
                Page = query.Page,
                PageSize = query.PageSize
            };

            await _cache.SetAsync(cacheKey, result, TimeSpan.FromSeconds(15), ct);
            return result;
        }
        finally
        {
            sw.Stop();
            SongMetrics.Requests.Add(1, new KeyValuePair<string, object?>("operation", "get_paged"));
            SongMetrics.DurationMs.Record(sw.Elapsed.TotalMilliseconds, new KeyValuePair<string, object?>("operation", "get_paged"));
        }
    }

    public async Task SetGenresAsync(int songId, List<int> genreIds, CancellationToken ct)
    {
        var sw = Stopwatch.StartNew();
        try
        {
            var ids = (genreIds ?? new List<int>()).Distinct().Where(x => x > 0).ToList();

            var song = await _db.Songs
                .Include(s => s.SongGenres)
                .FirstOrDefaultAsync(s => s.Id == songId, ct)
                ?? throw new NotFoundException("Song", songId);

            if (ids.Count > 0)
            {
                var existingCount = await _db.Genres.CountAsync(g => ids.Contains(g.Id), ct);
                if (existingCount != ids.Count)
                    throw new BadRequestException("One or more genreIds do not exist.");
            }

            song.SongGenres.Clear();
            foreach (var gid in ids)
            {
                song.SongGenres.Add(new SongGenre { SongId = songId, GenreId = gid });
            }

            await _db.SaveChangesAsync(ct);

            await _cache.RemoveAsync(AllSongsKey, ct);
            await _cache.RemoveAsync(SongByIdKey(songId), ct);
            await _cache.RemoveAsync(AllGenresKey, ct);
            foreach (var gid in ids)
                await _cache.RemoveAsync(GenreByIdKey(gid), ct);
        }
        finally
        {
            sw.Stop();
            SongMetrics.Requests.Add(1, new KeyValuePair<string, object?>("operation", "set_genres"));
            SongMetrics.DurationMs.Record(sw.Elapsed.TotalMilliseconds, new KeyValuePair<string, object?>("operation", "set_genres"));
        }
    }

    private async Task InvalidateSongCache(int songId, int artistId, CancellationToken ct)
    {
        await _cache.RemoveAsync(AllSongsKey, ct);
        await _cache.RemoveAsync(SongByIdKey(songId), ct);
        await _cache.RemoveAsync(AllArtistsKey, ct);
        await _cache.RemoveAsync(ArtistByIdKey(artistId), ct);
    }

    private async Task InvalidateSongCache(int songId, int oldArtistId, int newArtistId, CancellationToken ct)
    {
        await InvalidateSongCache(songId, oldArtistId, ct);
        if (oldArtistId != newArtistId)
            await _cache.RemoveAsync(ArtistByIdKey(newArtistId), ct);
    }

    private static string BuildPagedCacheKey(SongFilterQuery query)
    {
        var parts = new List<string> { "songs:paged", $"p{query.Page}", $"ps{query.PageSize}" };
        if (!string.IsNullOrWhiteSpace(query.Search)) parts.Add($"s{query.Search.Trim().ToLower().GetHashCode()}");
        if (query.ArtistId.HasValue) parts.Add($"a{query.ArtistId.Value}");
        if (query.GenreId.HasValue) parts.Add($"g{query.GenreId.Value}");
        if (!string.IsNullOrWhiteSpace(query.SortBy)) parts.Add($"sb{query.SortBy.ToLower()}");
        if (query.IsDescending) parts.Add("desc");
        return string.Join(":", parts);
    }
}