using MusicWeb.src.Models.Dtos.Artists;
using MusicWeb.src.Models.Entities;

namespace MusicWeb.src.Mapping
{
    public static class ArtistMapping
    {
        public static ArtistReadDto ToReadDto(this Artist a) =>
            new ArtistReadDto
            {
                Id = a.Id,
                Name = a.Name,
                SongCount = a.Songs?.Count ?? 0
            };

        public static Artist ToEntity(this ArtistCreateDto dto) =>
            new Artist
            {
                Name = dto.Name.Trim()
            };
    }
}