using MusicWeb.src.Models.Dtos.Genres;
using MusicWeb.src.Models.Entities;

namespace MusicWeb.src.Mapping;

public static class GenreMapping
{
    public static GenreReadDto ToReadDto(this Genre g, int songCount) => new()
    {
        Id = g.Id,
        Name = g.Name,
        SongCount = songCount
    };

    public static Genre ToEntity(this GenreCreateDto dto) => new()
    {
        Name = dto.Name.Trim()
    };
}