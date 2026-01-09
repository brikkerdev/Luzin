using MusicWeb.src.Models.Dtos.Songs;
using MusicWeb.src.Models.Entities;

namespace MusicWeb.src.Mapping;

public static class SongMapping
{
    public static SongReadDto ToReadDto(this Song s) => new()
    {
        Id = s.Id,
        Title = s.Title,
        Text = s.Text,
        ArtistId = s.ArtistId,
        ArtistName = s.Artist?.Name ?? string.Empty,
        Genres = s.SongGenres.Select(song => song.GenreId).ToList(),
    };

    public static Song ToEntity(this SongCreateDto dto) => new()
    {
        Title = dto.Title.Trim(),
        Text = dto.Text.Trim(),
        ArtistId = dto.ArtistId
    };

    public static void Apply(this SongUpdateDto dto, Song entity)
    {
        entity.Title = dto.Title.Trim();
        entity.Text = dto.Text.Trim();
        entity.ArtistId = dto.ArtistId;
    }
}