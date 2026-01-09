using MusicWeb.Services.Caching;
using MusicWeb.src.Exceptions;
using MusicWeb.src.Models.Dtos.Artists;
using MusicWeb.src.Models.Entities;
using MusicWeb.src.Repositories;
using MusicWeb.src.Services.Artist.Interfaces;

namespace MusicWeb.src.Services.Artist;

public sealed class ArtistService : IArtistService
{
    private readonly IArtistRepository _repo;
    private readonly IRedisCache _cache;
    private readonly ILogger<ArtistService> _logger;

    private const string AllArtistsKey = "artists:all:v1";
    private static string ArtistByIdKey(int id) => $"artists:{id}:v1";
    private const string AllSongsKey = "songs:all:v1";

    private static readonly TimeSpan AllArtistsTtl = TimeSpan.FromSeconds(30);
    private static readonly TimeSpan ArtistByIdTtl = TimeSpan.FromMinutes(5);

    public ArtistService(IArtistRepository repo, IRedisCache cache, ILogger<ArtistService> logger)
    {
        _repo = repo;
        _cache = cache;
        _logger = logger;
    }

    public async Task<List<ArtistReadDto>> GetAllAsync(CancellationToken ct)
    {
        var cached = await _cache.GetAsync<List<ArtistReadDto>>(AllArtistsKey, ct);
        if (cached is not null) return cached;

        var dtos = await _repo.GetAllWithSongCountAsync(ct);
        await _cache.SetAsync(AllArtistsKey, dtos, AllArtistsTtl, ct);
        return dtos;
    }

    public async Task<ArtistReadDto> GetByIdAsync(int id, CancellationToken ct)
    {
        var key = ArtistByIdKey(id);

        var cached = await _cache.GetAsync<ArtistReadDto>(key, ct);
        if (cached is not null) return cached;

        var dto = await _repo.GetByIdWithSongCountAsync(id, ct)
            ?? throw new NotFoundException("Artist", id);

        await _cache.SetAsync(key, dto, ArtistByIdTtl, ct);
        return dto;
    }

    public async Task<ArtistReadDto> CreateAsync(ArtistCreateDto dto, CancellationToken ct)
    {
        var entity = new MusicWeb.src.Models.Entities.Artist
        {
            Name = dto.Name.Trim()
        };

        await _repo.AddAsync(entity, ct);
        await _repo.SaveChangesAsync(ct);

        await _cache.RemoveAsync(AllArtistsKey, ct);

        var read = await _repo.GetByIdWithSongCountAsync(entity.Id, ct)
                   ?? new ArtistReadDto { Id = entity.Id, Name = entity.Name, SongCount = 0 };

        await _cache.SetAsync(ArtistByIdKey(read.Id), read, ArtistByIdTtl, ct);
        return read;
    }

    public async Task<ArtistReadDto> CreateWithSongsAsync(ArtistWithSongsCreateDto dto, CancellationToken ct)
    {
        var artist = new MusicWeb.src.Models.Entities.Artist
        {
            Name = dto.Name.Trim()
        };

        var songs = dto.Songs.Select(s => new Song
        {
            Title = s.Title.Trim(),
            Text = s.Text.Trim()
        }).ToList();

        var created = await _repo.CreateArtistWithSongsAsync(artist, songs, ct);

        await _cache.RemoveAsync(AllArtistsKey, ct);
        await _cache.RemoveAsync(AllSongsKey, ct);

        var read = new ArtistReadDto
        {
            Id = created.Id,
            Name = created.Name,
            SongCount = created.Songs.Count
        };

        await _cache.SetAsync(ArtistByIdKey(read.Id), read, ArtistByIdTtl, ct);
        return read;
    }

    public async Task UpdateAsync(int id, ArtistUpdateDto dto, CancellationToken ct)
    {
        var ok = await _repo.UpdateNameAsync(id, dto.Name.Trim(), ct);
        if (!ok)
            throw new NotFoundException("Artist", id);

        await _repo.SaveChangesAsync(ct);

        await _cache.RemoveAsync(AllArtistsKey, ct);
        await _cache.RemoveAsync(ArtistByIdKey(id), ct);
    }

    public async Task DeleteAsync(int id, CancellationToken ct)
    {
        var ok = await _repo.DeleteAsync(id, ct);
        if (!ok)
            throw new NotFoundException("Artist", id);

        await _repo.SaveChangesAsync(ct);

        await _cache.RemoveAsync(AllArtistsKey, ct);
        await _cache.RemoveAsync(ArtistByIdKey(id), ct);
    }
}